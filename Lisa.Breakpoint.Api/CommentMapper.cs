using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lisa.Breakpoint.Api
{
    public class CommentMapper
    {
        public static ITableEntity ToEntity(dynamic model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            dynamic entity = new DynamicEntity();
            entity.userName = model.userName;
            entity.comment = model.comment;

            dynamic metadata = model.GetMetadata();
            if (metadata == null)
            {
                entity.id = Guid.NewGuid();
                entity.created = DateTime.UtcNow;
                entity.status = "open";
                entity.statusChanged = "";
            }
            else
            {
                entity.id = model.id;
                entity.created = model.created;
                entity.status = model.status;
                entity.PartitionKey = metadata.PartitionKey;
                entity.RowKey = metadata.RowKey;
                entity.statusChanged = model.statusChanged;
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
            model.userName = entity.userName;
            model.comment = entity.comment;
            model.created = entity.created;
            model.status = entity.status;
            model.statusChanged = entity.statusChanged;

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