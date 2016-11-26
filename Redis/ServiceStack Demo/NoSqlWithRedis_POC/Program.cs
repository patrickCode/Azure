using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NoSqlWithRedis_POC;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Text;

namespace NoSqlWithRedis_POC
{

    internal class Program
    {
        public static string loginName = "Anonymous";

        private static void Main(string[] args)
        {
            NativeClientDemo();
            RedisClientDemo();
            //RedisClientTypedDemo();
            //TransactionDemo();

            Console.ReadLine();
            
            
            //var listener = new chatThreadCtrl.ThreadController();
            //var chatThreadListener = new chatThreadCtrl(listener);
            //var chatThread = new Thread(chatThreadListener.listen);
            //chatThread.Start();
            //var normalThread = new Thread(GetData);
            //normalThread.Start();

            //
            //Console.WriteLine("Welcome to Console Chat Application\nPlease enter a login alias to continue");
            //loginName = Console.ReadLine();
            //using (var client = new RedisClient())
            //{
            //    client.PublishMessage("chatChannel", loginName + " has joined the channel");
            //}
            //var listener = new ThreadController();
            //var chatThreadController_listener = new ChatThreadController(listener);
            //var chatThreadController_sender = new ChatThreadController(listener);

            //var listenerThread = new Thread(chatThreadController_listener.listen);
            //var senderThread = new Thread(chatThreadController_listener.send);

            //listenerThread.Start();
            //senderThread.Start();
            //
            
            
            //Thread chatThread = new Thread();
            //SubscribeDemo();
            //KeysDemo();
            //SetDemo();

            Console.ReadKey();
        }


        public static void GetData()
        {
            while (true)
            {
                var data = Console.ReadLine();
                if (data.Equals("exit")) break;
            }
        }

        private static void SetDemo()
        {
            using (
                var client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                const string key = "urn:Set:Customer";
                client.AddRangeToSet(key, new List<string> { "abc", "cde", "def" });
                client.AddRangeToSet(key, new List<string> { "abc", "cdsae", "def" });
                client.AddItemToSet(key, "One");
            }

            using (
                var client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                const string key = "urn:Set:Customer";
                var items = client.GetAllItemsFromSet(key);
                foreach (var item in items) Console.WriteLine(item);
            }

            using (
                var client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                Console.ReadKey();
                const string key = "urn:Set:Customer";
                var items = client.GetAllItemsFromSet(key);
                foreach (var item in items) client.RemoveItemFromSet(key, item);
            }

            using (
                var client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                const string key = "urn:Set:Customer";
                client.AddRangeToSet(key, new List<string> { "abcasd", "cadsdsae", "de12f" });
                var items = client.GetAllItemsFromSet(key);
                foreach (var item in items) Console.WriteLine(item);
            }
        }

        private static void KeysDemo()
        {
            using (
                IRedisNativeClient client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                var keys = client.Keys("*a*");
                var keyList = keys.Select(key => Encoding.UTF8.GetString(key)).ToList();
                keyList.ForEach(Console.WriteLine);
            }
        }

        private static void NativeClientDemo()
        {
            using (
                IRedisNativeClient client = new RedisClient())
            {
                client.Set("urn:messages:1", Encoding.UTF8.GetBytes("Message 1"));
            }

            using (IRedisNativeClient client = new RedisClient())
            {
                var result = Encoding.UTF8.GetString(client.Get("urn:messages:1"));
                Console.WriteLine("Message {0}", result);
            }
        }

        private static void RedisClientDemo()
        {
            using (
                IRedisClient client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                var customerList = client.Lists["urn:customernames"];
                customerList.Clear();
                customerList.Add("Pratik");
                customerList.Add("Varun");
                customerList.Add("Pradeep");
                customerList.Add("Manish");
                //Redis client has all the operations that can be done on a C# list, like - enqueue, dequeue, pop, push, etc
            }

            using (
                IRedisClient client =
                    new RedisClient(
                        "Qg7EG3uqYBDZBLMPdjkWblKNRhjCb7e9Lo60RJbTHd8=@customer-C.redis.cache.windows.net?ssl=true"))
            {
                var customerNames = client.Lists["urn:customernames"];
                foreach (var customerName in customerNames)
                {
                    Console.WriteLine("Customer: {0}", customerName);
                }
            }
        }

        private static void RedisClientTypedDemo()
        {
            string lastId = "";
            using (IRedisClient client = new RedisClient())
            {
                var customerClient = client.As<Customer>();
                var customer = new Customer()
                {
                    Id = "Pratik_Bhattacharya", //Gets the unique number from RedisClient, Redis keeps track of the ID
                    Address = "207 Golden Sands 305",
                    Name = "Pratik Bhattacharya",
                    Orders =
                        new List<Order>
                        {
                            new Order
                            {
                                OrderNumber = "ABC123"
                            },
                            new Order
                            {
                                OrderNumber = "DEF123"
                            }
                        }
                };

                var storedCustomer = customerClient.Store(customer); //Saving the customer
                lastId = storedCustomer.Id;
            }

            using (IRedisClient client = new RedisClient())
            {
                var customerClient = client.As<Customer>();
                var customer = customerClient.GetById(lastId);

                Console.WriteLine("Customer Details {0} {1} {2} {3} {4}", customer.Name, customer.Id.ToString(),
                    customer.Address, customer.Orders[0].OrderNumber, customer.Orders[1].OrderNumber);
            }
        }

        private static void TransactionDemo()
        {
            using (IRedisClient client = new RedisClient())
            {
                var transaction = client.CreateTransaction();
                transaction.QueueCommand(c => c.Set("Key:1", 100));
                transaction.QueueCommand(c => c.Increment("Key:1", 1));
                transaction.Commit();
                var result = client.Get<int>("Key:1");
                Console.WriteLine("Result: {0}", result);
            }
        }

        private static void PublishDemo()
        {
            using (IRedisClient client = new RedisClient())
            {
                client.PublishMessage("chatChannel", "Hi");
            }
        }

        private static void SubscribeDemo()
        {
            using (IRedisClient client = new RedisClient())
            {
                var sub = client.CreateSubscription();
                sub.OnMessage = (c, m) => Console.WriteLine("Message from {0} channel: {1}", c, m);
                sub.SubscribeToChannels("chatChannel");
            }
        }
    }


    //public class chatThreadCtrl
    //{
    //    private ThreadController _ctrlThread;

    //    public chatThreadCtrl(ThreadController ctrl)
    //    {
    //        _ctrlThread = ctrl;
    //    }

    //    public void listen()
    //    {
    //        _ctrlThread.SubscribeDemo();
    //    }

        public class ChatThreadController
        {
            private ThreadController chatThread;

            public ChatThreadController(ThreadController thread)
            {
                chatThread = thread;
            }

            public void listen()
            {
                chatThread.listenToMessages();
            }

            public void send()
            {
                while (true) { 
                chatThread.SendMessage();
                }
            }
        }

        public class ThreadController
        {
            public void listenToMessages()
            {
                using (IRedisClient client = new RedisClient())
                {
                    var sub = client.CreateSubscription();
                    sub.OnMessage = printMessage;
                    sub.SubscribeToChannels("chatChannel");
                }
            }

            private void printMessage(string channel, string message)
            {
                if (message.Substring(0, Program.loginName.Length).Contains(Program.loginName)) return;
                Console.WriteLine(message);

            }

            public void SendMessage()
            {
                using (var client = new RedisClient())
                {
                    var rawMessage = Console.ReadLine();
                    //if (rawMessage.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    client.PublishMessage("chatChannel", Program.loginName + ": " + rawMessage);
                }
            }
        }

    }

    //public class ThreadController
    //{
    //    public void SubscribeDemo()
    //        {
    //            using (IRedisClient client = new RedisClient())
    //            {
    //                var sub = client.CreateSubscription();
    //                sub.OnMessage = (c, m) => Console.WriteLine("Message from {0} channel: {1}", c, m);
    //                sub.SubscribeToChannels("chatChannel");
    //            }
    //        }
    //}
//}