using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/releases/")]
    public class ReleaseController : Controller
    {
        public ReleaseController(Database database)
        {
            _db = database;
        }

        [HttpPost("{project}")]
        public async Task<ActionResult> Post([FromBody] DynamicModel release, string project)
        {
            if (release == null)
            {
                return new BadRequestResult();
            }
            var validationResult = _validator.Validate(release);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            dynamic result = await _db.SaveRelease(release);
            return null;
        }
        private Database _db;
        private Validator _validator = new ReleaseValidator();
    }
}