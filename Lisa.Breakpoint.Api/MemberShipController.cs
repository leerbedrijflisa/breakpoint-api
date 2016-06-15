using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/memberships/")]
    public class MembershipController : Controller
    {
        public MembershipController(Database database)
        {
            _db = database;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string project, [FromQuery] string role, [FromQuery] string userName)
        {
            List<Tuple<string, string>> filter = new List<Tuple<string, string>>();

            if (project != null)
            {
                filter.Add(Tuple.Create("project", project));
            }
            if (role != null)
            {
                filter.Add(Tuple.Create("role", role));
            }
            if (userName != null)
            {
                filter.Add(Tuple.Create("userName", userName));
            }

            dynamic memberships = await _db.FetchMemberships(filter);

            if (memberships == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(memberships);
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
                var error = MembershipValidator.MembershipError(membership);

                return new UnprocessableEntityObjectResult(error);
            }

            dynamic result = await _db.SaveMembership(membership);

            string location = Url.RouteUrl("SingleReport", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMembership([FromQuery] string userName, Guid id)
        {
            dynamic memberships = await _db.FetchMemberships();
            if (memberships == null)
            {
                return new NotFoundResult();
            }

            string deletedName = null;
            string deletedRole = null;
            string project = null;

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

            string userRole = null;

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
                int managerCount = 0;

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
        private Validator _validator = new MembershipValidator();
    }
}