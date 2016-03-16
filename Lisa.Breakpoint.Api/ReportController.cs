using Lisa.Common.WebApi;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/reports/")]
    public class ReportController
    {
        public ReportController(Database database)
        {
            _db = database;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string sort, [FromQuery] string order)
        {
            var reports = await _db.FetchReports();
            if (sort == null || order == null)
            {
                return new HttpOkObjectResult(reports);
            }

            var result = ReportSorter.Sort(reports, sort, order);

            if (result == null)
            {
                return new UnprocessableEntityObjectResult("{\n'errorCode': 50134,\n'errorMessage': 'Sort is not valid'\n}");
            }
            return new HttpOkObjectResult(result);
        }

        //[HttpGet]
        //public async Task<ActionResult> Get([FromQuery]string sort, [FromQuery]string order)
        //{
        //    var reports = await _db.FetchReports();
        //    return new HttpOkObjectResult(sort);
        //}

        private Database _db;
    }
}