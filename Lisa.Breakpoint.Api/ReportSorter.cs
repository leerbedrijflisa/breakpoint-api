using Lisa.Common.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Breakpoint.Api
{
    public static class ReportSorter
    {
        public static List<DynamicModel> Sort(IEnumerable<DynamicModel> reports, string sort, string order)
        {
            if (sort.ToLower() == "project")
            {
                if (order.ToLower() == "asc")
                {
                    return reports.AsQueryable().OrderBy(r => r[sort]).ToList();
                }
            }
            return null;
        }
    }
}