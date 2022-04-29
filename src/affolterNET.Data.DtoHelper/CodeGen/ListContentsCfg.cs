namespace affolterNET.Data.DtoHelper.CodeGen;

public class ListContentsCfg
{
    public string TableName { get; }
    public string IdAttribute { get; }
    public string NameAttribute { get; }
    public string ClassName { get; }

    public ListContentsCfg(string tableName, string idAttribute, string nameAttribute, string? className = null)
    {
        TableName = tableName;
        IdAttribute = idAttribute;
        NameAttribute = nameAttribute;
        ClassName = className ?? $"List{TableName}";
    }
}