using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace AzureRedisCache_Console
{
    class Program
    {
        private static ConnectionMultiplexer _connection;
        private static IDatabase cacheDatabase;

        private const string AzureRedisConnectionString =
            "sampleredis.redis.cache.windows.net, password=tCsQIbc7xV4UU1lPVdVlD0dG6I1ASbvfiZsQ3vJTO1Q=";
        static void Main(string[] args)
        {
            _connection =
                ConnectionMultiplexer.Connect(AzureRedisConnectionString);
            BasicUsage_Demo();
            Console.ReadKey();
            Keys();
            //Migrate_Demo();
            _connection.Dispose();

        }

        private static void BasicUsage_Demo()
        {
            cacheDatabase = _connection.GetDatabase();
            if (!cacheDatabase.KeyExists("PratikBhattacharya")) cacheDatabase.StringSet("PratikBhattacharya", "7207294857");
            if (!cacheDatabase.KeyExists("VarunPyasi")) cacheDatabase.StringSet("VarunPyasi", "1234567890");
            Console.WriteLine("Data Written to Redis Cache\nPress enter to continue...");
            Console.ReadKey();
            var phone1 = cacheDatabase.StringGet("PratikBhattacharya");
            var phone2 = cacheDatabase.StringGet("VarunPyasi");
            Console.WriteLine("Data Received. Press enter to view...");
            Console.ReadKey();
            Console.WriteLine(phone1+"\t"+phone2);
        }

        private static void Migrate_Demo()
        {
            ConnectionMultiplexer newConnect = ConnectionMultiplexer.Connect("ecorediscustomer.redis.cache.windows.net, password=POf13l28POjLgUdvzyaFDBgfi4o24kdaX+c135Ziof8=");
            var newDatabase = newConnect.GetDatabase();
            
            RedisKey key = new RedisKey();
            key = "key1";

            cacheDatabase = _connection.GetDatabase();

            var b = cacheDatabase.KeyExists(key);

            b = newDatabase.KeyExists(key);
            var endPoint = _connection.GetEndPoints(true).Single();

            newDatabase.KeyMigrate(key, endPoint);
            b = newDatabase.KeyExists(key);
        }

        private static void Keys()
        {
            var server = _connection.GetServer(_connection.GetEndPoints().FirstOrDefault());
            var keys = server.Keys();
            cacheDatabase = _connection.GetDatabase();
            foreach (var key in keys) 
            {
                if (key == "PratikBhattacharya")
                {
                    ConnectionMultiplexer newConnect = ConnectionMultiplexer.Connect("ecorediscustomer.redis.cache.windows.net, password=POf13l28POjLgUdvzyaFDBgfi4o24kdaX+c135Ziof8=");
                    var newDatabase = newConnect.GetDatabase();
                    var b = newDatabase.KeyExists("PratikBhattacharya");
                    newDatabase.StringSet("PratikBhattacharya", cacheDatabase.StringGet("PratikBhattacharya"));
                    b = newDatabase.KeyExists("PratikBhattacharya");
                }
                Console.WriteLine(key);
            }
        }

        private static void Hash(){
        //{
        //    HashEntry kv = new HashEntry("Pratik", "Value1");
        //    cacheDatabase = _connection.GetDatabase();
        //    cacheDatabase.HashSet("Hash1", new HashEntry[]);
        }

    }
}
