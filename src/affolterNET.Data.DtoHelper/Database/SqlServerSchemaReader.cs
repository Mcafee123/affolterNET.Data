using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using affolterNET.Data.DtoHelper.CodeGen;

namespace affolterNET.Data.DtoHelper.Database
{
    public class SqlServerSchemaReader
    {
        private const string TableSql = @"SELECT *
		FROM  INFORMATION_SCHEMA.TABLES
		WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'
        ORDER BY TABLE_SCHEMA, TABLE_NAME";

        private const string OuterKeysSql = @"SELECT 
			FK = OBJECT_NAME(pt.constraint_object_id),
            Referenced_schema = OBJECT_SCHEMA_NAME(pt.referenced_object_id),
			Referenced_tbl = OBJECT_NAME(pt.referenced_object_id),
			Referencing_col = pc.name, 
			Referenced_col = rc.name
		FROM sys.foreign_key_columns AS pt
		INNER JOIN sys.columns AS pc
		ON pt.parent_object_id = pc.[object_id]
		AND pt.parent_column_id = pc.column_id
		INNER JOIN sys.columns AS rc
		ON pt.referenced_column_id = rc.column_id
		AND pt.referenced_object_id = rc.[object_id]
		WHERE pt.parent_object_id = OBJECT_ID(@tableName)
        Order By Referenced_tbl;";

        private const string InnerKeysSql = @"SELECT 
			[Schema] = OBJECT_SCHEMA_NAME(pt.parent_object_id),
			Referencing_tbl = OBJECT_NAME(pt.parent_object_id),
			FK = OBJECT_NAME(pt.constraint_object_id),
			Referencing_col = pc.name, 
			Referenced_col = rc.name
		FROM sys.foreign_key_columns AS pt
		INNER JOIN sys.columns AS pc
		ON pt.parent_object_id = pc.[object_id]
		AND pt.parent_column_id = pc.column_id
		INNER JOIN sys.columns AS rc
		ON pt.referenced_column_id = rc.column_id
		AND pt.referenced_object_id = rc.[object_id]
		WHERE pt.referenced_object_id = OBJECT_ID(@tableName);";

        private static readonly PluralizationServiceWrapper PluralizationService = new PluralizationServiceWrapper();

        // SchemaReader.ReadSchema
        private readonly TextWriter tw;

        private SqlConnection? connection;

        public SqlServerSchemaReader(TextWriter tw)
        {
            this.tw = tw;
        }

        public Tables ReadSchema(SqlConnection cn, GeneratorCfg props)
        {
            var result = new Tables();

            connection = cn;

            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandText = TableSql
            };

            // pull the tables in a reader
            using (cmd)
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var tbl = new Table(props)
                        {
                            FullName = rdr["TABLE_SCHEMA"] + "_" + rdr["TABLE_NAME"],
                            Name = rdr["TABLE_NAME"].ToString()!,
                            Schema = rdr["TABLE_SCHEMA"].ToString()!,
                            IsView = string.Compare(
                                         rdr["TABLE_TYPE"].ToString(),
                                         "View",
                                         StringComparison.OrdinalIgnoreCase) == 0
                        };
                        tbl.CleanName = tbl.Name;
                        if (tbl.CleanName.StartsWith("tbl_"))
                        {
                            tbl.CleanName = tbl.CleanName.Replace("tbl_", string.Empty);
                        }

                        if (tbl.CleanName.StartsWith("tbl"))
                        {
                            tbl.CleanName = tbl.CleanName.Replace("tbl", string.Empty);
                        }

                        tbl.CleanName = tbl.CleanName.Replace("_", string.Empty);
                        tbl.ClassName = Singularize(RemoveTablePrefixes(tbl.CleanName));
                        tbl.ObjectName = RenameSchema(tbl, props.RenameTableSchemas);
                        result.Add(tbl);
                    }
                }
            }

            foreach (var tbl in result)
            {
                tbl.LoadColumns(cn);

                // Mark the primary key
                tbl.MarkPrimaryKeys(cn);

                try
                {
                    LoadOuterKeys(tbl);
                    LoadInnerKeys(tbl);
                }
                catch (Exception x)
                {
                    var error = x.Message.Replace("\r\n", "\n").Replace("\n", " ");
                    tw.WriteLine(string.Empty);
                    tw.WriteLine(
                        "// -----------------------------------------------------------------------------------------");
                    tw.WriteLine("// Failed to get relationships for `{0}` - {1}", tbl.Name, error);
                    tw.WriteLine(
                        "// -----------------------------------------------------------------------------------------");
                    tw.WriteLine(string.Empty);
                }
            }

            return result;
        }

        private void FixPropertyNames(List<Key> result)
        {
            foreach (var key in result)
            {
                if (result.Count(k => k.PropertyName == key.PropertyName) > 1)
                {
                    key.PropertyName = key.Name;
                }
            }
        }

        private void LoadInnerKeys(Table tbl)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = InnerKeysSql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = tbl.Schema + "." + tbl.Name;
                cmd.Parameters.Add(p);

                var result = new List<Key>();
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var key = new Key
                        {
                            Name = rdr["FK"].ToString(),
                            PropertyName = rdr["Referencing_tbl"].ToString()!
                                    .Replace("_", string.Empty),
                            ReferencingTableName = rdr["Schema"] + "_" + rdr["Referencing_tbl"],
                            ReferencedTableColumnName = rdr["Referenced_col"].ToString(),
                            ReferencingTableColumnName = rdr["Referencing_col"].ToString()
                        };
                        result.Add(key);
                    }
                }

                FixPropertyNames(result);

                // add to table object
                foreach (var key in result)
                {
                    tbl.InnerKeys.Add(key);
                }
            }
        }

        private void LoadOuterKeys(Table tbl)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = OuterKeysSql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = tbl.Schema + "." + tbl.Name;
                cmd.Parameters.Add(p);

                var result = new List<Key>();
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var key = new Key
                        {
                            Name = rdr["FK"].ToString(),
                            PropertyName = rdr["Referenced_tbl"].ToString()!
                                .Replace("_", string.Empty),
                            ReferencedTableName =
                                rdr["Referenced_schema"] + "_" + rdr["Referenced_tbl"],
                            ReferencedTableColumnName = rdr["Referenced_col"].ToString(),
                            ReferencingTableColumnName = rdr["Referencing_col"].ToString()
                        };
                        result.Add(key);
                    }
                }

                FixPropertyNames(result);

                // add to table object
                foreach (var key in result)
                {
                    tbl.OuterKeys.Add(key);
                }
            }
        }

        private static string RemoveTablePrefixes(string word)
        {
            var cleanword = word;
            if (cleanword.StartsWith("tbl_"))
            {
                cleanword = cleanword.Replace("tbl_", string.Empty);
            }

            if (cleanword.StartsWith("tbl"))
            {
                cleanword = cleanword.Replace("tbl", string.Empty);
            }

            cleanword = cleanword.Replace("_", string.Empty);
            return cleanword;
        }

        private string RenameSchema(Table tbl, Dictionary<string, string> schemaRenames)
        {
            var rename = schemaRenames.Where(s => s.Key == tbl.Schema).Select(s => s.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(rename))
            {
                rename = tbl.Schema;
            }

            return $"{rename}_{tbl.Name}";
        }

        private static string Singularize(string word)
        {
            var singularword = PluralizationService.Singularize(word);
            return singularword;
        }
    }
}