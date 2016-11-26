using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableProvider
{
    public class CustomerTableIndex
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public CustomerTableIndex()
        {
            PartitionKey = String.Empty;
            RowKey = String.Empty;
        }

        public CustomerTableIndex(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public CustomerTableIndex(ITableEntity customer)
        {
            PartitionKey = customer.PartitionKey;
            RowKey = customer.RowKey;
        }

        public static List<CustomerTableIndex> CreateTableIndexes(List<CustomerEntity> customers)
        {
            var tableIndexes = new List<CustomerTableIndex>();
            customers.ForEach(customer => tableIndexes.Add(new CustomerTableIndex(customer)));
            return tableIndexes;
        }

        public bool IsSameAs(CustomerTableIndex customerTableIndex)
        {
            return (this.PartitionKey == customerTableIndex.PartitionKey && this.RowKey == customerTableIndex.RowKey);
        }
    }
}
