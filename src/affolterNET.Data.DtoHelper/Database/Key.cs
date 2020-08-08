namespace affolterNET.Data.DtoHelper.Database
{
    public class Key
    {
        public string Name { get; set; }

        public string PropertyName { get; set; }

        public string ReferencedTableColumnName { get; set; }

        public string ReferencedTableName { get; set; }

        public string ReferencingTableColumnName { get; set; }

        public string ReferencingTableName { get; set; }
    }
}