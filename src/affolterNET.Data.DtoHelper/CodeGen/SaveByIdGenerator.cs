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
                public string GetSaveByIdCommand()
                {{
                    return 
                        @$""if exists (select {pk.Name} from {tbl.Schema}.{tbl.Name} where {pk.Name} = @{pk.Name})
                            {{GetUpdateCommand()}}
                        else
                            {{GetInsertCommand()}}
                        "";
                }}
            ");
            sg.Generate(add);
        }
    }
}