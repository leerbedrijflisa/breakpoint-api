using Lisa.Common.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lisa.Breakpoint.Api
{
    public static class ReportSorter
    {
        public static List<DynamicModel> Sort(IEnumerable<DynamicModel> reports, string sort, string order)
        {
            string[] splittedSort = sort.Split(',');
            string[] splittedOrder = order.Split(',');
            IQueryable<string> queryableData = splittedSort.AsQueryable<string>();
            ParameterExpression pe = Expression.Parameter(typeof(string), "splittedSort");

            Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
            Expression right = Expression.Constant("splittedSort[1]");
            Expression e1 = Expression.Equal(left, right);

            left = Expression.Property(pe, typeof(string).GetProperty("Length"));
            right = Expression.Constant(16, typeof(int));
            Expression e2 = Expression.GreaterThan(left, right);

            Expression predicateBody = Expression.OrElse(e1, e2);
            MethodCallExpression whereCallExpression = Expression.Call(
    typeof(Queryable),
    "Where",
    new Type[] { queryableData.ElementType },
    queryableData.Expression,
    Expression.Lambda<Func<string, bool>>(predicateBody, new ParameterExpression[] { pe }));

            MethodCallExpression orderByCallExpression = Expression.Call(
    typeof(Queryable),
    "OrderBy",
    new Type[] { queryableData.ElementType, queryableData.ElementType },
    whereCallExpression,
    Expression.Lambda<Func<string, string>>(pe, new ParameterExpression[] { pe }));

            IQueryable<string> results = queryableData.Provider.CreateQuery<string>(orderByCallExpression);
            foreach (string result in results)
                Console.WriteLine(result);
            //foreach (string sortableField in sortableFields)
            //{
            //    if (splittedSort[1] == null)
            //    {
            //        if (sort == sortableField)
            //        {
            //            if (order != null)
            //            {
            //                if (order.ToLower() == "desc")
            //                {
            //                    return reports.AsQueryable().OrderByDescending(r => r[sort]).ToList();
            //                }
            //                if (order.ToLower() == "asc")
            //                {
            //                    return reports.AsQueryable().OrderBy(r => r[sort]).ToList();
            //                }
            //            }
            //            else
            //            {
            //                return reports.AsQueryable().OrderBy(r => r[sort]).ToList();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        int i = 0;
            //        var k = reports.AsQueryable();
            //        foreach (var item in splittedSort)
            //        {
            //            if (i == 0)
            //            {                           
            //                if (splittedOrder[i] == "desc")
            //                {
            //                    k = k.OrderByDescending(r => r[item]);
            //                }
            //                else
            //                {
            //                    k = k.OrderBy(r => r[item]);
            //                }
            //                i++;
            //            }
            //            else
            //            {
            //                if (splittedOrder[i] == "desc")
            //                {
            //                    k = k.ThenByDescending(r => r[item]);
            //                }
            //                else
            //                {
            //                    k = k.ThenBy(r => r[item]);
            //                }

            //                i++;
            //            }
            //            var m = k.ToList();
            //        }                  
            //    }
            //}

            return reports.ToList();
        }

        public static string[] sortableFields = new string[] {
            "id",
            "title",
            "project",
            "status",
            "reported",
            "assignee",
            "datetime"
        };
    }
}