using Lisa.Common.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Breakpoint.Api
{
    public static class ReportSorter
    {
        public static List<DynamicModel> Sort(IEnumerable<DynamicModel> reports, string sort, string order)
        {
            string[] splittedSort = sort.Split(',');
            string[] splittedOrder = order.Split(',');
            foreach (string sortableField in sortableFields)
            {
                if (splittedSort[0] == sortableField)
                {
                    if (splittedOrder[0].ToLower() == "asc")
                    {
                        return reports.AsQueryable().OrderBy(r => r[sort]).ToList();
                    }
                    else if (splittedOrder[0].ToLower() == "desc")
                    {
                        return reports.AsQueryable().OrderByDescending(r => r[sort]).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public static string[] sortableFields = new string[4] {
            "id",
            "title",
            "project",
            "reported"
        };
    }
}