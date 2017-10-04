using Microsoft.Azure.KeyVault.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Table.CSE
{
    public class TableProvider<TElement> where TElement: TableEntity, new()
    {
        private readonly CloudTable _table;
        private readonly TableRequestOptions _requestOption;
        private readonly TableRequestOptions _retreiveOption;

        public TableProvider(string accountName, string accountKey, string tableName, string keyId, IKeyResolver keyResolver)
        {
            var storageCredentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
            _table.CreateIfNotExists();

            
            _requestOption = new TableRequestOptions()
            {
                EncryptionPolicy = new TableEncryptionPolicy(keyResolver.ResolveKeyAsync(keyId, CancellationToken.None).Result, null)
            };
            _retreiveOption = new TableRequestOptions()
            {
                EncryptionPolicy = new TableEncryptionPolicy(null, keyResolver)
            };
        }

        public void Insert(TElement element)
        {
            var operation = TableOperation.Insert(element);
            _table.Execute(operation, _requestOption);
        }

        public TElement Get(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve<TElement>(partitionKey, rowKey);
            var result = _table.Execute(operation, _retreiveOption);
            return (TElement)result.Result;
        }

        public List<TElement> Query(string propertyName, string propertyValue, string operatorStr)
        {
            var query = GenerateFilter(propertyName, propertyValue, operatorStr);
            var tableQuery = new TableQuery<TElement>().Where(query);
            return _table.ExecuteQuery(tableQuery, _retreiveOption).ToList();
        }

        public string GenerateFilter(string propertyName, object propertyValue, string operatorString)
        {
            var propertyType = propertyValue.GetType();

            if (propertyType == typeof(string))
            {
                return TableQuery.GenerateFilterCondition(propertyName, operatorString, propertyValue as string);
            }

            if (propertyType == typeof(DateTime))
            {
                return TableQuery.GenerateFilterConditionForDate(propertyName, operatorString, (DateTime)propertyValue);
            }

            if (propertyType == typeof(bool))
            {
                return TableQuery.GenerateFilterConditionForBool(propertyName, operatorString, (bool)propertyValue);
            }

            if (propertyType == typeof(double))
            {
                return TableQuery.GenerateFilterConditionForDouble(propertyName, operatorString, (double)propertyValue);
            }

            if (propertyType == typeof(int))
            {
                return TableQuery.GenerateFilterConditionForInt(propertyName, operatorString, (int)propertyValue);
            }

            if (propertyType == typeof(long))
            {
                return TableQuery.GenerateFilterConditionForLong(propertyName, operatorString, (long)propertyValue);
            }

            if (propertyType == typeof(Guid))
            {
                return TableQuery.GenerateFilterConditionForGuid(propertyName, operatorString, (Guid)propertyValue);
            }

            return propertyType == typeof(byte[]) ?
                   TableQuery.GenerateFilterConditionForBinary(propertyName, operatorString, (byte[])propertyValue) :
                   TableQuery.GenerateFilterCondition(propertyName, operatorString, propertyValue.ToString());
        }
    }
}