using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string status)
        {
            List<Tuple<string, string>> filter = new List<Tuple<string, string>>();

            if (status != null)
            {
                filter.Add(Tuple.Create("status", status));
            }

            var projects = await _db.FetchProjects(filter);
            return new OkObjectResult(projects);
        }

        [HttpGet("{id}", Name = "SingleProject")]
        public async Task<ActionResult> GetSingle(Guid id)
        {
            dynamic project = await _db.FetchProject(id);

            if (project == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(project);
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

            dynamic result = await _db.SaveProject(project);

            string location = Url.RouteUrl("SingleProject", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch([FromBody] Patch[] patches, Guid id)
        {
            if (patches == null)
            {
                return new BadRequestResult();
            }

            var project = await _db.FetchProject(id);
            if (project == null)
            {
                return new NotFoundResult();
            }

            var validationResult = _validator.Validate(patches, project);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var patcher = new ModelPatcher();
            patcher.Apply(patches, project);

            await _db.UpdateProject(project);

            return new OkObjectResult(project);
        }

        private ProjectValidator _validator = new ProjectValidator();
        private Database _db;
    }
}