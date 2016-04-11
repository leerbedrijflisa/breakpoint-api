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
        public async Task<ActionResult> Get([FromQuery] string project)
        {
            var filter = new List<Tuple<string, string>>();
            var users = new List<string>();
            if (project != null)
            {
                filter.Add(Tuple.Create("Project", project));
            }

            var reports = await _db.FetchReports(filter);

            foreach (dynamic report in reports)
            {
                if (report.Assignee != null)
                {
                    if (!users.Contains(report.Assignee))
                    {
                        users.Add(report.Assignee);
                    }
                }
            }
            if (users.Count == 0)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(users);
        }

        private Database _db;
    }
}