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
            string userName = "supertheo";
            string userRole = null;
            string deletedName = null;
            string deletedRole = null;
            string project = null;
            int managerCount = 0;

            dynamic memberships = await _db.FetchAllMemberships();
            if (memberships == null)
            {
                return new NotFoundResult();
            }

            foreach (var membership in memberships)
            {
                if (membership.id == id)
                {
                    deletedName = membership.userName;
                    deletedRole = membership.role;
                    project = membership.project;
                }
            }

            if (deletedRole == null)
            {
                return new NotFoundResult();
            }

            foreach (var membership in memberships)
            {
                if (membership.userName == userName)
                {
                    userRole = membership.role;
                }
            }

            if (!((userRole == "manager") ||
                (userRole == "developer" && (deletedRole == "tester" || userName == deletedName)) ||
                (userRole == "tester" && userName == deletedName)))
            {
                return new StatusCodeResult(401);
            }

            if (deletedRole == "manager")
            {
                foreach (var membership in memberships)
                {
                    if (membership.project == project && membership.role == "manager")
                    {
                        managerCount += 1;
                    }
                }

                if (managerCount == 1)
                {
                    return new StatusCodeResult(401);
                }
            }

            await _db.DeleteMembership(id);
            
            return new StatusCodeResult(204);
        }

        private Database _db;
        private Validator _validator = new MemberShipValidator();
    }
}