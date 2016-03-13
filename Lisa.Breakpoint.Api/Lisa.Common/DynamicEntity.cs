using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Lisa.Common.TableStorage
{
    public class DynamicEntity : DynamicObject, ITableEntity
    {
        public string ETag { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> tableData, OperationContext operationContext)
        {
            _tableData = tableData;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return _tableData;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_tableData.ContainsKey(binder.Name))
            {
                var value = _tableData[binder.Name];
                result = value.PropertyAsObject;
                return true;
            }

            if (FindProperty(binder.Name) != null)
            {
                return base.TryGetMember(binder, out result);
            }

            result = null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = FindProperty(binder.Name);
            if (property != null)
            {
                property.SetValue(this, value);
            }
            else
            {
                _tableData[binder.Name] = EntityProperty.CreateEntityPropertyFromObject(value);
            }

            return true;
        }

        private PropertyInfo FindProperty(string name)
        {
            var type = GetType();
            return type.GetProperty(name);
        }

        private IDictionary<string, EntityProperty> _tableData = new Dictionary<string, EntityProperty>();
    }
}