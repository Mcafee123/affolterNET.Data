﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using affolterNET.Data.DtoHelper.CodeGen;

namespace affolterNET.Data.DtoHelper.Database
{
    public class TablesLoader
    {
        private readonly GeneratorCfg cfg;

        private readonly TextWriter tw;

        public TablesLoader(GeneratorCfg cfg, TextWriter? tw = null)
        {
            if (string.IsNullOrWhiteSpace(cfg.ConnString))
            {
                throw new ArgumentException("please provide a connectionstring");
            }

            this.cfg = cfg;
            if (tw == null)
            {
                tw = Console.Out;
            }

            this.tw = tw;
        }

        public TablesResultat LoadTables()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var builder = new DbConnectionStringBuilder { ConnectionString = cfg.ConnString };
            builder["Password"] = "[xxxxx]";
            tw.WriteLine("// This file was automatically generated by the Dapper.SimpleCRUD T4 Template");
            tw.WriteLine("// Do not make changes directly to this file - edit the template instead");
            tw.WriteLine("// ");
            tw.WriteLine("// The following connection settings were used to generate this file");
            tw.WriteLine("// ");
            tw.WriteLine("//     Connection String:      `{0}`", ZapPassword(builder.ConnectionString!));
            tw.WriteLine("//     Include Views:          `{0}`", cfg.IncludeViews);
            tw.WriteLine(string.Empty);

            try
            {
                using var conn = new SqlConnection(cfg.ConnString);
                conn.Open();

                // Assume SQL Server
                var reader = new SqlServerSchemaReader(tw);

                var result = reader.ReadSchema(conn, cfg);

                // Remove unrequired tables/views
                for (var i = result.Count - 1; i >= 0; i--)
                {
                    if (cfg.IsTableExcluded(result[i]))
                    {
                        result.RemoveAt(i);
                    }
                }

                conn.Close();

                var rxClean =
                    new Regex(
                        "^(Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
                foreach (var t in result)
                {
                    t.ClassName = cfg.ClassPrefix + t.ClassName + cfg.ClassSuffix;
                    foreach (var c in t.AllColumns)
                    {
                        if (c.PropertyName == null)
                        {
                            throw new InvalidOperationException($"{nameof(c.PropertyName)} was empty");
                        }

                        c.PropertyName = rxClean.Replace(c.PropertyName, "_$1");

                        // Make sure property name doesn't clash with class name
                        if (c.PropertyName == t.ClassName)
                        {
                            c.PropertyName = "_" + c.PropertyName;
                        }
                    }
                }

                var schemas = result
                    .Select(t => t.Schema)
                    .Distinct()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!);
 
                return new TablesResultat(schemas) { Tables = result };
            }
            catch (Exception x)
            {
                var error = x.Message.Replace("\r\n", "\n").Replace("\n", " ");

                tw.WriteLine(string.Empty);
                tw.WriteLine(
                    "// -----------------------------------------------------------------------------------------");
                tw.WriteLine("// Failed to read database schema - {0}", error);
                tw.WriteLine(
                    "// -----------------------------------------------------------------------------------------");
                tw.WriteLine(string.Empty);
                return new TablesResultat { Error = error, Ex = x };
            }
        }

        private static string ZapPassword(string connectionString)
        {
            {
                var rx = new Regex(
                    "Password=.*;",
                    RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                return rx.Replace(connectionString, "Password=******;");
            }
        }
    }
}