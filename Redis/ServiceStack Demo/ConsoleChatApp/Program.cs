using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace ConsoleChatApp
{
    class Program
    {
        public static string loginName;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Console Chat Application\nPlease enter a login alias to continue");
            loginName = Console.ReadLine();
            var listener = new ThreadController();
            var chatThreadController_listener = new ChatThreadController(listener);
            var chatThreadController_sender = new ChatThreadController(listener);

            var listenerThread = new Thread(chatThreadController_listener.listen);
            var senderThread = new Thread(chatThreadController_listener.send);

            listenerThread.Start();
            senderThread.Start();

        }
    }

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
            chatThread.SendMessage();
        }
    }

    public class ThreadController
    {
        public void listenToMessages()
        {
            using (IRedisClient client = new RedisClient())
            {
                var sub = client.CreateSubscription();
                sub.OnMessage = (c, m) => Console.WriteLine("Message from {0} channel: {1}", c, m);
                sub.SubscribeToChannels("chatChannel");
            }
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
