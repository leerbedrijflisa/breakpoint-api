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

            entity.Id = Guid.NewGuid();
            entity.Name = model.name;
            entity.CreatedBy = model.createdBy;
            if (model.status != null)
            {
                entity.Status = model.status;
            }
            else
            {
                entity.Status = "open";
            }
            entity.Created = DateTime.UtcNow;

            if (model.platform != null)
            {
                string platformList = "";

                foreach (string platform in model.platforms)
                {
                    platformList = platformList + platform + ",";
                }
                platformList = platformList.Remove(platformList.Length - 1);

                entity.Platform = platformList;
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

            model.id = entity.Id;
            model.name = entity.Name;
            model.status = entity.Status;
            model.createdBy = entity.CreatedBy;
            if (entity.Platform != null)
            {
                model.platform = entity.Platform.Split(',');
            }
            else
            {
                model.platform = "";
            }
            model.created = entity.Created;

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