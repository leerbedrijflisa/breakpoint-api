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
            if (model.platform != null)
            {
                foreach (string platform in model.platform)
                {
                    platfromString = platfromString + platform + ",";
                }
                platfromString = platfromString.Remove(platfromString.Length - 1);
            }    
                   
            entity.platform = platfromString;

            if (model.assignee.ToString() != null) {
                entity.assignee = JsonConvert.SerializeObject(model.assignee);
            }
            else
            {
                entity.assignee = model.assignee;
            }

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
                entity.status = model.status;
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

            if (entity.assignee != null)
            {
                model.assignee = JsonConvert.DeserializeObject(entity.assignee);
            }
            else
            {
                model.assignee = "";
            }

            model.status = entity.status;

            if (entity.priority != null)
            {
                model.priority = entity.priority;
            }
            else
            {
                model.priority = "";
            }
            
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