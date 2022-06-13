using System;
using System.Linq;
using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class SaveByIdGenerator
    {
        private readonly Table tbl;

        public SaveByIdGenerator(Table tbl)
        {
            this.tbl = tbl;
        }
        
        public void Generate(Action<MemberDeclarationSyntax> add)
        {
            var pk = tbl.AllColumns.FirstOrDefault(t => t.IsPK);
            if (pk == null)
            {
                return;
            }

            var sg = new StringGenerator(
                $@"
                public string GetSaveByIdCommand(bool select = false, params string[] excludedColumns)
                {{
                    return 
                        @$""
                        declare @rowcnt int
                        if exists (select {pk.Name} from {tbl.Schema}.{tbl.Name} where {pk.Name} = @{pk.Name})
                            begin
                                {{GetUpdateCommand(excludedColumns)}}; set @rowcnt = (select @@rowcount);
                                select '{tbl.Schema}' as [Schema], '{tbl.Name}' as [Table], convert(nvarchar(50), @{pk.Name}) as [Id], case when @rowcnt = 0 then '{{Constants.NoAction}}' else '{{Constants.Updated}}' end as [Action];
                            end
                        else
                            begin
                                {{GetInsertCommand({(tbl.GetPrimaryKeyColumn()?.IsAutoIncrement == true ? "true" : "false")}, excludedColumns)}}; set @rowcnt = (select @@rowcount);
                                select '{tbl.Schema}' as [Schema], '{tbl.Name}' as [Table], convert(nvarchar(50), @{pk.Name}) as [Id], case when @rowcnt = 0 then '{{Constants.NoAction}}' else '{{Constants.Inserted}}' end as [Action];
                            end
                        {{(select ? GetSelectCommand(1, excludedColumns) : string.Empty)}}"";
                }}
            ");
            sg.Generate(add);
        }
    }
}