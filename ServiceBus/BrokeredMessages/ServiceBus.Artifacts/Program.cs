using System;
using System.Configuration;

namespace ServiceBusManagementConsole
{
    class Program
    {   
        private static string ServiceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

        static void Main(string[] args)
        {
            ManagementHelper helper = new ManagementHelper(ServiceBusConnectionString);

            bool done = false;
            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(">");
                string commandLine = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                string[] commands = commandLine.Split(' ');

                try
                {
                    if (commands.Length > 0)
                    {
                        switch (commands[0])
                        {
                            case "createqueue":
                            case "cq":
                                if (commands.Length > 1)
                                {
                                    helper.CreateQueue(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue path not specified.");
                                }
                                break;
                            case "listqueues":
                            case "lq":
                                helper.ListQueues();
                                break;
                            case "getqueue":
                            case "gq":
                                if (commands.Length > 1)
                                {
                                    helper.GetQueue(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue path not specified.");
                                }
                                break;
                            case "deletequeue":
                            case "dq":
                                if (commands.Length > 1)
                                {
                                    helper.DeleteQueue(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue path not specified.");
                                }
                                break;
                            case "createtopic":
                            case "ct":
                                if (commands.Length > 1)
                                {
                                    helper.CreateTopic(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue path not specified.");
                                }
                                break;
                            case "createsubscription":
                            case "cs":
                                if (commands.Length > 2)
                                {
                                    helper.CreateSubscription(commands[1], commands[2]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue path not specified.");
                                }
                                break;
                            case "listtopics":
                            case "lt":
                                helper.ListTopicsAndSubscriptions();
                                break;
                            case "exit":
                                done = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                }

            } while (!done);
        }
    }
}
