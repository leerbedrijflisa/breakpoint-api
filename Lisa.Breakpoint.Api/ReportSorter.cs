using Lisa.Common.WebApi;
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
            if (splittedSort[1] != null) {
                foreach (string sortableField in sortableFields)
                {
                    if (splittedSort[0] == sortableField)
                    {
                        var orderByAscending = reports.AsQueryable().OrderBy(r => r[splittedSort[0]]);
                        var orderByDescending = reports.AsQueryable().OrderByDescending(r => r[splittedSort[0]]);
                        var splittedOrder0 = splittedOrder[0].ToLower();
                        var splittedOrder1 = splittedOrder[1].ToLower();

                        if (splittedOrder0 == "asc" && splittedOrder1 == "asc")
                        {
                            return orderByAscending.ThenBy(r => r[splittedSort[1]]).ToList();
                        }
                        else if (splittedOrder0 == "asc" && splittedOrder1 == "desc")
                        {
                            return orderByAscending.ThenByDescending(r => r[splittedSort[1]]).ToList();
                        }
                        else if (splittedOrder0 == "desc" && splittedOrder1 == "asc")
                        {
                            return orderByDescending.ThenBy(r => r[splittedSort[1]]).ToList();
                        }
                        else if (splittedOrder0 == "desc" && splittedOrder1 == "desc")
                        {
                            return orderByDescending.ThenByDescending(r => r[splittedSort[1]]).ToList();
                        }
                        else
                        {
                            return reports.ToList();
                        }
                    }
                }
            }
            else
            {
                foreach (string sortableField in sortableFields)
                {
                    if (splittedSort[0] == sortableField)
                    {
                        if (splittedOrder[0].ToLower() == "asc")
                        {
                            return reports.AsQueryable().OrderBy(r => r[splittedSort[0]]).ToList();
                        }
                        else if (splittedOrder[0].ToLower() == "desc")
                        {
                            return reports.AsQueryable().OrderByDescending(r => r[splittedSort[0]]).ToList();
                        }
                        else
                        {
                            return reports.ToList();
                        }
                    }
                }
            }

            return reports.ToList();
        }

        public static string[] sortableFields = new string[6] {
            "id",
            "title",
            "project",
            "status",
            "reported",
            "assignee"
        };
    }
}