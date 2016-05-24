using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
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
                return new NotFoundResult();
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

            dynamic comment = await _db.FetchComment(id);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            var validationResult = _validator.Validate(patches, comment);

            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            for (int i = 0; i < patches.Length; i++)
            {
                if (patches[i].Field == "deleted")
                {
                    int newIndex = patches.Length;

                    Array.Resize(ref patches, patches.Length + 1);
                    patches[newIndex] = new Patch();

                    patches[newIndex].Action = "replace";
                    patches[newIndex].Field = "deletionDate";

                    if (Convert.ToBoolean(patches[i].Value))
                    {
                        patches[newIndex].Value = DateTime.UtcNow;
                    }
                    else
                    {
                        patches[newIndex].Value = "";
                    }
                }
            }

            var patcher = new ModelPatcher();

            patcher.Apply(patches, comment);
            await _db.UpdateComment(comment);
            
            return new OkObjectResult(comment);
        }

        private Database _db;
        private Validator _validator = new CommentValidator();
    }
}