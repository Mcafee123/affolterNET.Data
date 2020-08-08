using System.Collections.Generic;
using System.Linq;

namespace affolterNET.Data.DtoHelper.Database
{
    public class Tables : List<Table>
    {
        public Table this[string fullTableName] => GetTable(fullTableName);

        public Table GetTable(string fullTableName)
        {
            var tbl = this.SingleOrDefault(x => string.Compare(x.FullName, fullTableName, true) == 0);
            if (tbl == null)
            {
                throw new KeyNotFoundException("could not find " + fullTableName);
            }

            return tbl;
        }
    }
}