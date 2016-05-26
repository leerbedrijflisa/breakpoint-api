using Microsoft.AspNetCore.Mvc;

namespace Lisa.Breakpoint.Api
{
    [Route("/test/")]
    public class TestController
    {
        [HttpGet]
        public ActionResult Get()
        {
            return new OkObjectResult("pong");
        }
    }
}
