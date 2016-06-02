using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/memberships/")]
    public class MemberShipController : Controller
    {
        public MemberShipController(Database database)
        {
            _db = database;
        }

        [HttpGet("{projectName}", Name = "SingleMembership")]
        public async Task<ActionResult> Get(string projectName)
        {
            projectName = projectName.Replace("%20", "");
            dynamic membership = await _db.FetchMemberships(projectName);

            if (membership == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(membership);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel membership)
        {
            if (membership == null)
            {
                return new BadRequestResult();
            }

            var validationResult = _validator.Validate(membership);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var membershipCheck = await _db.CheckMembership(membership);
            if (membershipCheck == null)
            {
                var error = MemberShipValidator.MembershipError(membership);

                return new UnprocessableEntityObjectResult(error);
            }

            dynamic result = await _db.SaveMembership(membership);

            string location = Url.RouteUrl("SingleReport", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMembership(Guid id)
        {
            var membership = await _db.DeleteMembership(id);

            if (!membership)
            {
                return new NotFoundResult();
            }
            return new StatusCodeResult(204);
        }

        private Database _db;
        private Validator _validator = new MemberShipValidator();
    }
}