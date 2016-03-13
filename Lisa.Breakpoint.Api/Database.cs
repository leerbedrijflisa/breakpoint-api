using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    public class Database
    {
        public async Task<IEnumerable<DynamicModel>> FetchReports()
        {
            CloudTable table = await Connect();

            var query = new TableQuery<DynamicEntity>();
            var reports = await table.ExecuteQuerySegmentedAsync(query, null);

            var results = reports.Select(r => ReportMapper.ToModel(r));
            return results;
        }

        private static async Task<CloudTable> Connect()
        {
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Reports");
            await table.CreateIfNotExistsAsync();

            return table;
        }

        private List<DynamicModel> _reports = new List<DynamicModel>();
    }
}