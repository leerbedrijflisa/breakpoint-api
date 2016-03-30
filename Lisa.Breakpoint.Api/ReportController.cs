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
        public async Task<ActionResult> Get([FromQuery] string sort, [FromQuery] string order, [FromQuery] string project)
        {
            List<Tuple<string, string>> filter = new List<Tuple<string, string>>();

            if (project != null)
            {
                filter.Add(Tuple.Create("Project", project));
            }

            var reports = await _db.FetchReports(filter);

            if (sort == null || order == null)
            {
                return new HttpOkObjectResult(reports);
            }

            var result = ReportSorter.Sort(reports, sort, order);

            if (result[0] == "order")
            {
                return new UnprocessableEntityObjectResult(new Error()
                {
                    Code = 51154,
                    Message = "The order field was not valid, values most be either 'asc' or 'desc'.",
                    Values = new { Field = "order" }
                });
            }

            if (result[0] == "sort")
            {
                return new UnprocessableEntityObjectResult(new Error()
                {
                    Code = 51155,
                    Message = "The sort field was not valid. Valid values listed below",
                    Values = new { Field = "sort", Values = ReportSorter.sortableFields}
                })
            }

            return new HttpOkObjectResult(result);
        }

        [HttpGet("{id}", Name = "SingleReport")]
        public async Task<ActionResult> Get(Guid id)
        {
            object report = await _db.FetchReport(id);

            if (report == null)
            {
                return new HttpNotFoundResult();
            }

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
        private Database _db;
        private Validator _validator = new ReportValidator();
    }
}