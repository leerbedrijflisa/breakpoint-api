using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.WindowsAzure.Storage.Table;
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

            if (model.assignee != null)
            {
                if (!model.assignee.Contains("userName") && !model.assignee.Contains("group"))
                {
                    entity.assignee =  model.assignee;
                }
                else if (model.assignee.userName != null)
                {
                    entity.assignee = "userName: " + model.assignee.userName;
                }
                else if (model.assignee.group != null)
                {
                    entity.assignee = "group: " + model.assignee.group;
                }
            }
            
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

            if (entity.assignee != null)
            {
                model.assignee = entity.assignee;
            }
            else
            {
                model.assignee = "";
            }

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