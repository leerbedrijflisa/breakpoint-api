using Lisa.Common.WebApi;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/memberships/")]
    public class MemberShipsController : Controller
    {
        public MemberShipsController(Database database)
        {
            _db = database;
        }

        [HttpGet("{projectName}", Name = "SingleMembership")]
        public async Task<ActionResult> Get(string projectName)
        {
            dynamic membership = await _db.FetchMemberships(projectName);

            if (membership == null)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(membership);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel membership)
        {
            if (membership == null)
            {
                return new BadRequestResult();
            }

            var henk = await _db.CheckMembership(membership);

            if (henk == null)
            {
                return new UnprocessableEntityObjectResult(membership);
            }

            var validationResult = _validator.Validate(membership);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            dynamic result = await _db.SaveMembership(membership);

            string location = Url.RouteUrl("SingleReport", new { id = result.id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        private Database _db;
        private Validator _validator = new MemberShipsValidator();
    }
}