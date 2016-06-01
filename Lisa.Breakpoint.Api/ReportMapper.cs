using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace Lisa.Breakpoint.Api
{
    public class ReportMapper
    {
        public static ITableEntity ToEntity(dynamic model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            dynamic entity = new DynamicEntity();
            entity.title = model.title;
            entity.project = model.project;
            entity.description = model.description;
            string platfromString = "";
            foreach (string platform in model.platform)
            {
                platfromString = platfromString + platform + ",";
            }
            platfromString = platfromString.Remove(platfromString.Length - 1);
            entity.platform = platfromString;
            entity.assignee = JsonConvert.SerializeObject(model.assignee ?? string.Empty);
            entity.status = model.status;
            entity.priority = model.priority;            

            dynamic metadata = model.GetMetadata();
            if (metadata == null)
            {
                entity.id = Guid.NewGuid();
                entity.reported = DateTime.UtcNow;
                entity.status = "open";
                entity.solvedCommit = "";
            }
            else
            {
                entity.id = model.id;
                entity.reported = model.reported;
                entity.PartitionKey = metadata.PartitionKey;
                entity.RowKey = metadata.RowKey;
                entity.solvedCommit = model.solvedCommit;
            }

            return entity;
        }

        public static DynamicModel ToModel(dynamic entity)
        {
            if (entity == null)
            {
                return null;
            }

            dynamic model = new DynamicModel();
            model.id = entity.id;
            model.title = entity.title;
            model.project = entity.project;
            model.description = entity.description;
            if (entity.platform != null)
            {
                model.platform = entity.platform.Split(',');
            }
            else
            {
                model.platform = "";
            }
            try
            {
                model.assignee = JsonConvert.DeserializeObject(entity.assignee);
            }
            catch(Exception E)
            {
                model.assignee = "";
                Console.WriteLine(E.StackTrace);
            }

            model.assignee = JsonConvert.DeserializeObject(entity.assignee);
            model.status = entity.status;
            model.priority = entity.priority;
            model.solvedCommit = entity.solvedCommit;
            model.reported = entity.reported;

            var metadata = new
            {
                PartitionKey = entity.PartitionKey,
                RowKey = entity.RowKey
            };
            model.SetMetadata(metadata);

            return model;
        }
    }
}