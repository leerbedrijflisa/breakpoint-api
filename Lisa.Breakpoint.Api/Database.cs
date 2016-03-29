using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.Extensions.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    public class Database
    {
        public Database(IOptions<TableStorageSettings> settings)
        {
            _settings = settings.Value;
        }
        
        public async Task<IEnumerable<DynamicModel>> FetchReports(List<Tuple<string, string>> filter)
        {
            CloudTable table = await Connect();
            var query = new TableQuery<DynamicEntity>();

            if (filter.Count == 1)
            {
                var filterCondition = TableQuery.GenerateFilterCondition(filter[0].Item1, QueryComparisons.Equal, filter[0].Item2);

                query = query.Where(filterCondition);
            }
            else if (filter.Count > 1)
            {
                List<string> filterConditions = new List<string>();

                for (int i = 0; i < filter.Count; i++)
                {
                    if(i == 0)
                    {
                        filterConditions.Add(TableQuery.GenerateFilterCondition(filter[i].Item1, QueryComparisons.Equal, filter[i].Item2));
                    }
                    else
                    {
                        filterConditions.Add(TableOperators.And);
                        filterConditions.Add(TableQuery.GenerateFilterCondition(filter[i].Item1, QueryComparisons.Equal, filter[i].Item2));
                    }
                }

                string numbers = "({0}) {1} ({2})";
                var allFilterConditions = string.Format(numbers, filterConditions.ToArray());

                query = query.Where(allFilterConditions);
            }
            
            var reports = await table.ExecuteQuerySegmentedAsync(query, null);
            var results = reports.Select(r => ReportMapper.ToModel(r));

            return results;
        }

        public async Task<DynamicModel> FetchReport(Guid id)
        {
            CloudTable table = await Connect();

            var query = new TableQuery<DynamicEntity>().Where(TableQuery.GenerateFilterConditionForGuid("Id", QueryComparisons.Equal, id));
            var report = await table.ExecuteQuerySegmentedAsync(query, null);
            var result = report.Select(r => ReportMapper.ToModel(r)).SingleOrDefault();

            return result;
        }

        public async Task<DynamicModel> SaveReport(dynamic report)
        {
            CloudTable table = await Connect();

            dynamic reportEntity = ReportMapper.ToEntity(report);

            reportEntity.PartitionKey = reportEntity.Project;
            reportEntity.RowKey = reportEntity.Id.ToString();

            var action = TableOperation.Insert(reportEntity);
            await table.ExecuteAsync(action);
            var result = ReportMapper.ToModel(reportEntity);

            return result;
        }

        private async Task<CloudTable> Connect()
        {
            var account = CloudStorageAccount.Parse(_settings.ConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Reports");
            await table.CreateIfNotExistsAsync();

            return table;
        }

        private TableStorageSettings _settings;
    }
}