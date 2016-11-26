using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableProvider;
using Redis = CacheProvider;

namespace CacheInvalidator
{
    class Program
    {
        private static Redis.CacheProvider _cacheProvider;
        static void Main(string[] args)
        {
            var storageTable = new AzureTable();
            _cacheProvider = new Redis.CacheProvider("sampleredis.redis.cache.windows.net:6380,password=tCsQIbc7xV4UU1lPVdVlD0dG6I1ASbvfiZsQ3vJTO1Q=,ssl=True,abortConnect=False");
            var data = storageTable.All();
            data.ForEach(AddOrUpdateCache);

            Console.WriteLine("Press any key to exit ...");
            Console.ReadLine();
        }

        private static string GetRedisKey(CustomerEntity customer)
        {
            return string.Format("usp_cust_{0}", customer.IndividualID);
        }

        private static void AddOrUpdateCache(CustomerEntity customer)
        {
            var key = GetRedisKey(customer);
            var policy = new Redis.RedisCachePolicy();
            policy.ContentType = typeof(CustomerEntity);

            if (!_cacheProvider.Exists(key))
            {
                customer.Tick = customer.Timestamp.Ticks.ToString();
                _cacheProvider.Set(key, customer, policy);
            }
            else
            {
                CustomerEntity cachedCustomer = _cacheProvider.Get(key, policy);
                if (!cachedCustomer.Tick.Equals(customer.Timestamp.Ticks.ToString()))
                {
                    _cacheProvider.Remove(key);
                    customer.Tick = customer.Timestamp.Ticks.ToString();
                    _cacheProvider.Set(key, customer, policy);
                }
            }
        }
    }
}
