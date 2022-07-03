namespace affolterNET.Data.DtoHelper.CodeGen;

public class ListContentsCfg
{
    public string SchemaName { get; }
    public string TableName { get; }
    public string IdAttribute { get; }
    public string NameAttribute { get; }
    public string ClassName { get; }

    public ListContentsCfg(string tableName, string idAttribute, string nameAttribute,
        string className): this("dbo", tableName, idAttribute, nameAttribute, className)
    {}
    
    public ListContentsCfg(string schemaName, string tableName, string idAttribute, string nameAttribute,
        string className)
    {
        SchemaName = schemaName;
        TableName = tableName;
        IdAttribute = idAttribute;
        NameAttribute = nameAttribute;
        ClassName = className;
    }
}