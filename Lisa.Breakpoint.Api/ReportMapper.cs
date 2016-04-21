using Lisa.Common.TableStorage;
using Lisa.Common.WebApi;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            entity.assignee = model.assignee;
            entity.status = model.status;

            if (model.comment != null)
            {
                var commentList = new List<string>();
                if (model.comment.GetType().Name == "String")
                {
                    commentList.Add(model.comment);

                    entity.comment = JsonConvert.SerializeObject(commentList);
                }
                else
                {
                    entity.comment = JsonConvert.SerializeObject(model.comment);
                }
            }
            else
            {
                var commentList = new List<string>();
                model.comment = JsonConvert.SerializeObject(commentList);
                entity.comment = model.comment;
            }
           

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
            model.status = entity.status;
            model.comment = JsonConvert.DeserializeObject(entity.comment);
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