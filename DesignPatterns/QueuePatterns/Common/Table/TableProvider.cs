using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Table
{
    public class TableProvider<T> where T: TableEntity
    {
        private readonly CloudTable _table;
        public TableProvider(string accountName, string accountKey, string tableName)
        {
            var storageCredentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
            _table.CreateIfNotExists();
        }

        public T Get(string partitionKey, string rowKey)
        {
            var query = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = _table.Execute(query).Result;
            return result as T;
        }

        public void Add(T entity)
        {
            var insertOperation = TableOperation.Insert(entity);
            _table.Execute(insertOperation);
        }

        public void Update(T entity)
        {
            var updateOperation = TableOperation.InsertOrReplace(entity);
            _table.Execute(updateOperation);
        }
    }
}