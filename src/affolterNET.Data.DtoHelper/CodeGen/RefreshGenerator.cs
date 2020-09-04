﻿using System;
using System.Linq;
using System.Text;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class RefreshGenerator
    {
        private readonly Table tbl;

        public RefreshGenerator(Table tbl)
        {
            this.tbl = tbl;
        }

        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var columnsBuilder = new StringBuilder();
            foreach (var c in tbl.Columns)
            {
                if (c.IsPK)
                {
                    continue;
                }

                columnsBuilder.Append($"this.{c.Name} = loaded.{c.Name};");
            }
            var columns = columnsBuilder.ToString();

            var sgRefresh = new StringGenerator(
                $@"
            public {tbl.ObjectName} GetFromDb(IDbConnection conn, IDbTransaction trsact) {{
                return conn.QueryFirstOrDefault<{tbl.ObjectName}>(this.GetSelectCommand(1), this, trsact);
            }}

            public void Reload(IDbConnection conn, IDbTransaction trsact) {{
                var loaded = this.GetFromDb(conn, trsact);
                {columns}
            }}");
            sgRefresh.Generate(add);
        }
    }
}