using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableProvider
{
    public class AzureTable
    {
        private string _azureAccountStorage;
        private string _azureAccountKey;
        private string _tableName;
        private CloudTable _azureTable;

        public AzureTable()
        {
            _azureAccountStorage = "portalvhds5svgdf6dy0njb";
            _azureAccountKey = "L5/PN2ERn+A8xoqf6COEy0hHxIWlvp7Co6rXFemMYGoSJ15BdEEvnibKHI5KR9KLNmZKm5YRlQuSSl+3u5LXgw==";
            _tableName = "HardwareCustomerTable";
            InitializeTable();
        }

        public AzureTable(string account, string key, string table)
        {
            _azureAccountStorage = account;
            _azureAccountKey = key;
            _tableName = table;
            InitializeTable();
        }

        private void InitializeTable()
        {
            var tableCredentials = new StorageCredentials(_azureAccountStorage, _azureAccountKey);
            var account = new CloudStorageAccount(tableCredentials, useHttps: true);
            var tableClient = account.CreateCloudTableClient();
            var azureTable = tableClient.GetTableReference(_tableName);
            azureTable.CreateIfNotExists();
            _azureTable = azureTable;
        }

        public bool AddCustomer(CustomerEntity newCustomer)
        {
            try
            {
                var addOperation = TableOperation.InsertOrReplace(newCustomer);
                _azureTable.Execute(addOperation);
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public bool AddCustomers(List<CustomerEntity> customers)
        {
            try
            {
                customers.ForEach(customer => AddCustomer(customer));
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public CustomerEntity RetrieveCustomerByIndex(string partitionKey, string rowKey)
        {
            if (partitionKey == null || rowKey == null) return null;
            try
            {
                var query = TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey);
                var result = _azureTable.Execute(query);
                if (result == null) return null;
                return (CustomerEntity)result.Result;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public CustomerEntity RetrieveCustomerByIndex(CustomerTableIndex customerTableIndex)
        {
            try
            {
                return RetrieveCustomerByIndex(customerTableIndex.PartitionKey, customerTableIndex.RowKey);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public List<CustomerEntity> RetriveCustomersByTableIndexList(List<CustomerTableIndex> tableIndexes)
        {
            try
            {
                var customerList = new List<CustomerEntity>();
                tableIndexes.ForEach(tableIndex => customerList.Add(RetrieveCustomerByIndex(tableIndex)));
                return customerList;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public List<CustomerEntity> All()
        {
            TableQuery<CustomerEntity> tableQuery = new TableQuery<CustomerEntity>();
            TableContinuationToken continuationToken = null;

            var fullList = new List<CustomerEntity>();
            do
            {
                TableQuerySegment<CustomerEntity> result = _azureTable.ExecuteQuerySegmented(tableQuery, continuationToken);
                continuationToken = result.ContinuationToken;
                var customerList = result as IList<CustomerEntity> ?? result.ToList();
                fullList.AddRange(customerList);
            } while (continuationToken != null);
            return fullList;
        }

        public List<CustomerEntity> RetrieveCustomersByProperty(string propertyName, string propertyValue)
        {
            if (propertyName.Equals("Name"))
                return RetrieveCustomerByName(propertyValue);
            try
            {
                var query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue));
                var result = _azureTable.ExecuteQuery(query);
                var customerList = result as IList<CustomerEntity> ?? result.ToList();
                if (!customerList.Any()) return null;
                return customerList.ToList();
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public List<CustomerEntity> RetrieveCustomerByName(string name)
        {
            var Name = name.Split(' ');

            try
            {
                if (Name.Length == 1)
                    return RetrieveCustomersByProperty("FirstName", name);
                var query =
                    new TableQuery<CustomerEntity>().
                        Where(
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("FirstName", QueryComparisons.Equal, Name[0]),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("LastName", QueryComparisons.Equal,
                                    Name[Name.Length - 1]))
                        );
                var result = _azureTable.ExecuteQuery(query);
                var customerList = result as IList<CustomerEntity> ?? result.ToList();
                return !customerList.Any() ? null : customerList.ToList();
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public List<CustomerEntity> RetrieveCustomersByAllName(string name)
        {
            var Name = name.Split(' ');
            
            var customers = new List<CustomerEntity>();
            try
            {
                if (Name.Length == 1) return RetrieveCustomerWithStartsWithPattern("FirstName", name);

                var startsWithPattern = Name[0];
                var length = startsWithPattern.Length - 1;
                var lastChar = startsWithPattern[length];
                var nextLastChar = (char)(lastChar + 1);
                var startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;

                var firstNameCondition = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("FirstName",
                        QueryComparisons.GreaterThanOrEqual,
                        startsWithPattern),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("FirstName",
                        QueryComparisons.LessThan,
                        startsWithEndPattern));
                
                startsWithPattern = Name[1];
                length = startsWithPattern.Length - 1;
                lastChar = startsWithPattern[length];
                nextLastChar = (char)(lastChar + 1);
                startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;
                var lastNameCondition = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("LastName",
                        QueryComparisons.GreaterThanOrEqual,
                        startsWithPattern),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("LastName",
                        QueryComparisons.LessThan,
                        startsWithEndPattern));

                var query = new TableQuery<CustomerEntity>().Where(TableQuery.CombineFilters(firstNameCondition, TableOperators.And, lastNameCondition));

                var result = _azureTable.ExecuteQuery(query);
                var customerList = result as IList<CustomerEntity> ?? result.ToList();
                return !customerList.Any() ? null : customerList.ToList();

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }


        public List<CustomerEntity> RetrieveCustomerWithStartsWithPattern(string property, string startsWithPattern)
        {
            try
            {
                if (property.Equals("Name"))
                    return RetrieveCustomersByAllName(startsWithPattern);
                var length = startsWithPattern.Length - 1;
                var lastChar = startsWithPattern[length];

                var nextLastChar = (char)(lastChar + 1);

                var startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;

                var query =
                    new TableQuery<CustomerEntity>().
                        Where(TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(property,
                                QueryComparisons.GreaterThanOrEqual,
                                startsWithPattern),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition(property,
                                QueryComparisons.LessThan,
                                startsWithEndPattern)
                            ));

                var result = _azureTable.ExecuteQuery(query);

                var customerList = result as IList<CustomerEntity> ?? result.ToList();
                return !customerList.Any() ? null : customerList.ToList();
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }
    }
}
