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
            entity.username = model.username;
            entity.comment = model.comment;

            dynamic metadata = model.GetMetadata();
            if (metadata == null)
            {
                entity.id = Guid.NewGuid();
                entity.datetime = DateTime.UtcNow;
                entity.deleted = false;
                entity.deletionDate = " ";
            }
            else
            {
                entity.id = model.id;
                entity.datetime = model.datetime;
                entity.deleted = model.deleted;
                entity.PartitionKey = metadata.PartitionKey;
                entity.RowKey = metadata.RowKey;
                entity.deletionDate = model.deletionDate;
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
            model.username = entity.username;
            model.comment = entity.comment;
            model.datetime = entity.datetime;
            model.deleted = entity.deleted;
            model.deletionDate = entity.deletionDate;

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