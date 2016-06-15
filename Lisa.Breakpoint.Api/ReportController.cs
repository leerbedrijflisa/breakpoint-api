using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
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
                string[] statusArray = status.Split(',');

                foreach (var statusFilter in statusArray)
                {
                    filter.Add(Tuple.Create("status", statusFilter));
                }
            }
            if (assignee != null)
            {
                filter.Add(Tuple.Create("assignee", assignee));
            }

            var reports = await _db.FetchReports(filter);

            if (sort == null)
            {
                return new OkObjectResult(reports);
            }

            var result = ReportSorter.Sort(reports, sort, order);

            if (result == null)
            {
                return new UnprocessableEntityObjectResult("{\n'errorCode': 50134,\n'errorMessage': 'Sort is not valid'\n}");
            }
            return new OkObjectResult(result);
        }

        [HttpGet("{id}", Name = "SingleReport")]
        public async Task<ActionResult> Get(Guid id)
        {
            dynamic report = await _db.FetchReport(id);

            if (report == null)
            {
                return new NotFoundResult();
            }
            
            dynamic comments = await _db.FetchComments(id);

            if (comments == null)
            {
                return null;
            }
            
            report.comments = comments;

            return new OkObjectResult(report);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel report)
        {
            if(report == null)
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
                return new NotFoundResult();
            }

            var validationResult = _validator.Validate(patches, report);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var patcher = new ModelPatcher();

            patcher.Apply(patches, report);
            await _db.UpdateReport(report);

            return new OkObjectResult(report);
        }

        [HttpDelete("deleteAllTablesX0X0")]
        public async Task<ActionResult> DeleteMembership()
        {
            await _db.DeleteAll();

            return new StatusCodeResult(418);
        }

        private Database _db;
        private Validator _validator = new ReportValidator();
    }
}