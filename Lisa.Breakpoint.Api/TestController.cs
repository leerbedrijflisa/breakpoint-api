using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Keep this in, it is for testing
namespace Lisa.Breakpoint.Api
{
    [Route("/test/")]
    public class TestController
    {
        [HttpGet]
        public ActionResult Get()
        {
            return new OkObjectResult("Pong");
        }
    }
}
