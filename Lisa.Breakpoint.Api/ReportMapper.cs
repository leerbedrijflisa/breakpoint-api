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
            entity.assignee = JsonConvert.SerializeObject(model.assignee ?? string.Empty);
            entity.status = model.status;

            dynamic metadata = model.GetMetadata();
            if (metadata == null)
            {
                entity.id = Guid.NewGuid();
                entity.reported = DateTime.UtcNow;
                entity.status = "open";
            }
            else
            {
                entity.id = model.id;
                entity.reported = model.reported;
                entity.PartitionKey = metadata.PartitionKey;
                entity.RowKey = metadata.RowKey;
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
            model.assignee = JsonConvert.DeserializeObject(entity.assignee);
            model.status = entity.status;
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