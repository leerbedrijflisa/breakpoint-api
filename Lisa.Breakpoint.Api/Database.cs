using Lisa.Common.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    public class Database
    {
        public Database()
        {
            CreateDummyData();
        }

        public async Task<IEnumerable<DynamicModel>> FetchReports()
        {
            return _reports;
        }

        private void CreateDummyData()
        {
            dynamic report = new DynamicModel();
            report.Title = "The app crashes when you don't do anything.";
            report.Project = "Idle App";
            _reports.Add(report);

            report = new DynamicModel();
            report.Title = "The app doesn't do anything when it crashes.";
            report.Project = "Crash App";
            _reports.Add(report);

        }

        private List<DynamicModel> _reports = new List<DynamicModel>();
    }
}