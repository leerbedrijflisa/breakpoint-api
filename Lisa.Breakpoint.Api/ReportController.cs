using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        public async Task<ActionResult> GetReports([FromQuery] string project, [FromQuery] string status)
        {
            List<Tuple<string, string>> filter = new List<Tuple<string, string>>();

            if (project != null)
            {
                filter.Add(Tuple.Create("Project", project));
            }
            if (status != null)
            {
                filter.Add(Tuple.Create("Status", status));
            }

            var reports = await _db.FetchReports(filter);

            return new HttpOkObjectResult(reports);
        }

        private Database _db;
    }
}