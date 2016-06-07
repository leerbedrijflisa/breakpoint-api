using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using System;

namespace Lisa.Breakpoint.Api
{
    public class ProjectMapper
    {
        public static DynamicEntity ToEntity(dynamic model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            dynamic entity = new DynamicEntity();

            entity.name = model.name;
            entity.createdBy = model.createdBy;

            if (model.platform != null)
            {
                if (model.platform.GetType() == typeof(Array))
                {
                    string platformList = "";

                    foreach (string platform in model.platforms)
                    {
                        platformList = platformList + platform + ",";
                    }
                    platformList = platformList.Remove(platformList.Length - 1);

                    entity.platform = platformList;
                }
            }
            else
            {
                entity.platform = "";
            }

            dynamic metadata = model.GetMetadata();
            if (metadata == null)
            {
                entity.id = Guid.NewGuid();
                entity.created = DateTime.UtcNow;
                entity.status = "open";
            }
            else
            {
                entity.id = model.id;
                entity.created = model.created;
                entity.PartitionKey = metadata.PartitionKey;
                entity.RowKey = metadata.RowKey;
                entity.status = model.status;
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
            model.name = entity.name;
            model.status = entity.status;
            model.createdBy = entity.createdBy;
            if (entity.platform != null)
            {
                model.platform = entity.platform.Split(',');
            }
            else
            {
                model.platform = "";
            }
            model.created = entity.created;

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