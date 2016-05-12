using Lisa.Common.WebApi;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/reports/")]
    public class ReportController : Controller
    {
        public ReportController(Database database)
        {
            _db = database;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string sort, [FromQuery] string order, [FromQuery] string project, [FromQuery] string status, [FromQuery] string assignee)
        {
            List<Tuple<string, string>> filter = new List<Tuple<string, string>>();

            if (project != null)
            {
                filter.Add(Tuple.Create("project", project));
            }
            if (status != null)
            {
                filter.Add(Tuple.Create("status", status));
            }
            if (assignee != null)
            {
                filter.Add(Tuple.Create("assignee", assignee));
            }

            var reports = await _db.FetchReports(filter);

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

        [HttpGet("{id}", Name = "SingleReport")]
        public async Task<ActionResult> Get(Guid id)
        {
            dynamic report = await _db.FetchReport(id);

            if (report == null)
            {
                return new HttpNotFoundResult();
            }

            dynamic comment = await _db.FetchComments(id);
            report.comments = comment;

            return new HttpOkObjectResult(report);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel report)
        {
            if (report == null)
            {
                return new BadRequestResult();
            }

            var validationResult = _validator.Validate(report);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            dynamic result = await _db.SaveReport(report);

            string location = Url.RouteUrl("SingleReport", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch([FromBody] Patch[] patches, Guid id)
        {
            if (patches == null)
            {
                return new BadRequestResult();
            }

            var report = await _db.FetchReport(id);
            if (report == null)
            {
                return new HttpNotFoundResult();
            }

            var validationResult = _validator.Validate(patches, report);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var patcher = new ModelPatcher();

            patcher.Apply(patches, report);
            await _db.UpdateReport(report);

            return new HttpOkObjectResult(report);
        }

        private Database _db;
        private Validator _validator = new ReportValidator();
    }
}