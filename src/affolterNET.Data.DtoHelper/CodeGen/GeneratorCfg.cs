﻿using System;
using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class GeneratorCfg
    {
        private readonly List<string> excludeColumns = new List<string>();

        private readonly List<string> excludeSchemas = new List<string>();

        private readonly List<string> excludeTablePrefixes = new List<string>();

        private readonly List<string> excludeTables = new List<string>();

        public GeneratorCfg()
        {
            IncludeViews = true;
        }

        public bool AddUpdateDateToInsert { get; private set; }

        public bool AddUpdateUserToInsert { get; private set; }

        public List<string> BaseTypes { get; } = new List<string>();

        public List<string> ViewBaseTypes { get; } = new List<string>();

        public string ClassPrefix { get; } = string.Empty;

        public string ClassSuffix { get; } = string.Empty;

        public List<string> Comments { get; } = new List<string>();

        public string? ConnString { get; private set; }

        public string[] ExcludeSchemas => excludeSchemas.ToArray();

        public string[] ExcludeTablePrefixes => excludeTablePrefixes.ToArray();

        public string[] ExcludeTables => excludeTables.ToArray();

        public bool IncludeViews { get; }

        public Func<string?, bool> InsertDateFunc { get; private set; } = s => false;

        public Func<string?, bool> InsertUserFunc { get; private set; } = s => false;

        public Func<string?, bool> IsActiveFunc { get; set; } = s => false;

        public string? Namespace { get; private set; }

        public Dictionary<string, string> RenameTableSchemas { get; } = new Dictionary<string, string>();

        public string? TargetFile { get; private set; }

        public Func<string?, bool> UpdateDateFunc { get; private set; } = s => false;

        public Func<string?, bool> UpdateUserFunc { get; private set; } = s => false;

        public List<string> Usings { get; } = new List<string>();

        public Func<string?, bool> VersionFunc { get; private set; } = s => false;

        public GeneratorCfg WithSchemaExclusion(string schemaName)
        {
            excludeSchemas.Add(schemaName);
            return this;
        }

        public GeneratorCfg WithTablePrefixExclusion(string tablePrefix)
        {
            excludeTablePrefixes.Add(tablePrefix);
            return this;
        }

        public GeneratorCfg WithColumnExclusion(string columnPrefix)
        {
            excludeColumns.Add(columnPrefix);
            return this;
        }

        public GeneratorCfg WithSchemaRename(string schemaName, string desiredObjectPrefix)
        {
            RenameTableSchemas.Add(schemaName, desiredObjectPrefix);
            return this;
        }

        public GeneratorCfg WithUsing(string u)
        {
            Usings.Add(u);
            return this;
        }

        public GeneratorCfg WithComment(string c)
        {
            Comments.Add(c);
            return this;
        }

        public GeneratorCfg WithBaseType(string bt)
        {
            BaseTypes.Add(bt);
            return this;
        }

        public GeneratorCfg WithBaseViewType(string viewtype)
        {
            ViewBaseTypes.Add(viewtype);
            return this;
        }

        public bool IsTableExcluded(Table tbl)
        {
            if (string.IsNullOrWhiteSpace(tbl.Name))
            {
                throw new InvalidOperationException($"{nameof(tbl.Name)} was empty");
            }
            if (string.IsNullOrWhiteSpace(tbl.Schema))
            {
                throw new InvalidOperationException($"{nameof(tbl.Schema)} was empty");
            }
            if (excludeSchemas.IndexOf(tbl.Schema) > -1)
            {
                tbl.Ignore = true;
                tbl.DebugText = $"Excluded by Schema: {tbl.Schema}";
                return true;
            }

            if (excludeTablePrefixes.Any(tp => tbl.Name.StartsWith(tp)))
            {
                tbl.Ignore = true;
                tbl.DebugText =
                    $"Excluded by Table Prefix: {excludeTablePrefixes.Single(tp => tbl.Name.StartsWith(tp))}";
                return true;
            }

            if (excludeTables.Any(t => t == tbl.FullName))
            {
                tbl.Ignore = true;
                tbl.DebugText = $"Excluded by Table Name: {excludeTables.Single(t => t == tbl.FullName)}";
                return true;
            }

            return false;
        }

        public bool IsColumnExcluded(string colNamePrefix)
        {
            if (excludeColumns.Any(c => c.StartsWith(colNamePrefix)))
            {
                return true;
            }

            return false;
        }

        public GeneratorCfg WithConn(string conn)
        {
            ConnString = conn;
            return this;
        }

        public GeneratorCfg WithTargetFile(string targetFile)
        {
            TargetFile = targetFile;
            return this;
        }

        public GeneratorCfg WithTableNameExclusion(string tableName)
        {
            excludeTables.Add(tableName);
            return this;
        }

        public GeneratorCfg WithNamespace(string ns)
        {
            Namespace = ns;
            return this;
        }

        public GeneratorCfg WithInsertUser(Func<string?, bool> func)
        {
            InsertUserFunc = func;
            return this;
        }

        public GeneratorCfg WithInsertDate(Func<string?, bool> func)
        {
            InsertDateFunc = func;
            return this;
        }

        public GeneratorCfg WithUpdateUser(Func<string?, bool> func, bool addToInsertCommand = false)
        {
            UpdateUserFunc = func;
            AddUpdateUserToInsert = addToInsertCommand;
            return this;
        }

        public GeneratorCfg WithUpdateDate(Func<string?, bool> func, bool addToInsertCommand = false)
        {
            UpdateDateFunc = func;
            AddUpdateDateToInsert = addToInsertCommand;
            return this;
        }

        public GeneratorCfg WithIsActive(Func<string?, bool> func)
        {
            IsActiveFunc = func;
            return this;
        }

        public GeneratorCfg WithVersion(Func<string?, bool> func)
        {
            VersionFunc = func;
            return this;
        }
    }
}