using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/projects/")]
    public class ProjectController : Controller
    {
        public ProjectController(Database database)
        {
            _db = database;
        }

        [HttpGet("{id}", Name = "SingleProject")]
        public ActionResult GetSingle()
        {
            return new OkResult();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel project)
        {
            if (project == null)
            {
                return new BadRequestResult();
            }

            var validationResult = _validator.Validate(project);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            dynamic result = await _db.saveProject(project);

            string location = Url.RouteUrl("SingleProject", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        private ProjectValidator _validator = new ProjectValidator();
        private Database _db;
    }
}