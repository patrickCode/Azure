using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ServiceStack.Redis;

namespace Chat_Console
{
    class Program
    {
        public static string LoginName;
        public const string ChannelName = "globalChatChannel";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Console Chat Application\nPlease enter a login alias to continue");
            LoginName = Console.ReadLine();
            using (var client = new RedisClient())
            {
                client.PublishMessage(ChannelName, LoginName + " has joined the channel");
            }
            var listener = new ThreadController();
            var chatThreadControllerListener = new ChatThreadController(listener);
            var chatThreadControllerSender = new ChatThreadController(listener);

            var listenerThread = new Thread(chatThreadControllerListener.Listen);
            var senderThread = new Thread(chatThreadControllerListener.Send);

            listenerThread.Start();
            senderThread.Start();

        }
    }

    public class ChatThreadController
    {
        private readonly ThreadController _chatThread;

        public ChatThreadController(ThreadController thread)
        {
            _chatThread = thread;
        }

        public void Listen()
        {
            _chatThread.ListenToMessages();
        }

        public void Send()
        {
            while (true)
            {
                _chatThread.SendMessage();
            }
        }
    }

    public class ThreadController
    {
        public void ListenToMessages()
        {
            using (IRedisClient client = new RedisClient())
            {
                var sub = client.CreateSubscription();
                sub.OnMessage = PrintMessage;
                sub.SubscribeToChannels(Program.ChannelName);
            }
        }

        private static void PrintMessage(string channel, string message)
        {
            if (message.Substring(0, Program.LoginName.Length).Contains(Program.LoginName)) return;
            Console.WriteLine(message);

        }

        public void SendMessage()
        {
            using (var client = new RedisClient())
            {
                var rawMessage = Console.ReadLine();
                if (rawMessage != null && rawMessage.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                {
                    client.PublishMessage(Program.ChannelName, Program.LoginName + " has left the chat");
                    Environment.Exit(1);
                }
                client.PublishMessage(Program.ChannelName, Program.LoginName + ": " + rawMessage);
            }
        }
    }
}
