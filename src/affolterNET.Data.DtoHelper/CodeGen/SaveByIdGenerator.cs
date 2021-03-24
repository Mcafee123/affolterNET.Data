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
                public string GetSaveByIdCommand(bool select = true)
                {{
                    return 
                        @$""
                        if exists (select {pk.Name} from {tbl.Schema}.{tbl.Name} where {pk.Name} = @{pk.Name})
                            begin
                                {{GetUpdateCommand()}};
                                {{(select ? string.Empty : ""select 'updated'"")}}
                            end
                        else
                            begin
                                {{GetInsertCommand({(tbl.GetPrimaryKeyColumn()?.IsAutoIncrement == true ? "true" : "false")})}}
                                {{(select ? string.Empty : ""select 'inserted'"")}}
                            end
                        {{(select ? GetSelectCommand() : string.Empty)}}"";
                }}
            ");
            sg.Generate(add);
        }
    }
}