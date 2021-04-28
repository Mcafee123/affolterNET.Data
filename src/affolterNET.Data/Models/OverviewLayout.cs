using System.Collections.Generic;

namespace affolterNET.Data.Models
{
    public class OverviewLayout
    {
        public List<OverviewColumn> Columns { get; set; } = new();

        public string? TypeName { get; set; }

        public string? Name { get; set; }
        
    }
}