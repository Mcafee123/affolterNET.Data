using System.Collections.Generic;
using System.Linq;
using affolterNET.Data.Models.Filters;

namespace affolterNET.Data.Models
{
    public class SearchParams
    {
        public RootFilter RootFilter { get; set; } = new RootFilter();

        public List<OrderBy> SortOrder { get; set; } = new List<OrderBy>();

        public long StartRow { get; set; } = 0;

        public long EndRow { get; set; } = 10;

        public OverviewLayout GridLayout { get; set; } = new OverviewLayout();

        public string SortString()
        {
            return string.Join(", ", SortOrder.Select(ob => ob.ToString()));
        }

        public string Paging(string attributeName)
        {
            // the DenseRank to return should be < "EndRow" for returning.
            // Because we want to check if it's the last row, we will handle that in LoadInstitutionenQuery
            return $"where {attributeName} >= {StartRow + 1} and {attributeName} <= {EndRow + 1}";
        }
        
        public List<SqlAttribute> GetAttributes()
        {
            var attributes = RootFilter.GetAttributes();
            attributes.AddRange(GridLayout.Columns);
            attributes.AddRange(SortOrder.Where(so => so.WasSet).Select(so => so.Attribute!));
            return attributes;
        }

        public bool FieldsValid()
        {
            // FilterList
            return RootFilter.IsValid();
        }

        public override string ToString()
        {
            return RootFilter.ToString();
        }

        public string GetSearchSql(string viewName, string firstViewColumn)
        {
            var fields = string.Join("\n, ", GridLayout.Columns);
            if (!string.IsNullOrWhiteSpace(fields))
            {
                fields = $", {fields}";
            }
            
            return $@"
                select
                ResultRowNum
                , {firstViewColumn}
                {fields}
                from (
                    select
                    InstitutionId as InstId
                    , count(InstitutionId) as InstCount
                    , min(RowNum) as MinRowNum
                    , row_number() over (order by min(RowNum)) as ResultRowNum
                    from (
                        select InstitutionId
                        , row_number() over (order by {SortString()}) as RowNum
                        from {viewName}
                        {RootFilter}
                    ) search
                    group by InstitutionId
                ) dist
                join {viewName}
                    on dist.InstId = InstitutionId
                {Paging("ResultRowNum")}
                order by ResultRowNum
            ";
        }
    }
}