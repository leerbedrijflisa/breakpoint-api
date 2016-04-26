using Lisa.Common.WebApi;
using Microsoft.AspNet.Mvc;
using System;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/comments/")]
    public class CommentController : Controller
    {
        public CommentController(Database database)
        {
            _db = database;
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> Post([FromBody] DynamicModel comment, Guid id)
        {
            if (comment == null)
            {
                return new BadRequestResult();
            }

            var report = await _db.FetchReport(id);
            if (report == null)
            {
                return new HttpNotFoundResult();
            }

            var validationResult = _validator.Validate(comment);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            dynamic result = await _db.SaveComment(comment, id);

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
        private Validator _validator = new CommentValidator();
    }
}