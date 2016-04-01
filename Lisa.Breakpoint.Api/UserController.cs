using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/users/")]
    public class UserController
    {
        public UserController (Database database)
        {
            _db = database;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var filter = new List<Tuple<string, string>>();
            var reports = await _db.FetchReports(filter);
            var users = new List<string>();
            foreach(dynamic report in reports)
            {
                if (!users.Contains(report.Assignee))
                {
                    users.Add(report.Assignee);
                }
            }

            return new HttpOkObjectResult(users);
        }

        private Database _db;
    }
}   