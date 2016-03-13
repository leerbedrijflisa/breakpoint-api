using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/reports/")]
    public class ReportController
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var reports = await _db.FetchReports();
            return new HttpOkObjectResult(reports);
        }

        private Database _db = new Database();
    }
}