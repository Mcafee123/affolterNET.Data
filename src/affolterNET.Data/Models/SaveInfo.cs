namespace affolterNET.Data.Models
{
    public class SaveInfo
    {
        public string? Schema { get; set; }
        public string? Table { get; set; }
        public string? Id { get; set; }
        public string? Action { get; set; }
        public object? Dto { get; set; }
        
        public override string ToString()
        {
            return $"Schema: {Schema}; Table: {Table}; Id: {Id}; Action: {Action}; Dto: {Dto}";
        }
    }
}