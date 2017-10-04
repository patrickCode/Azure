using System;
using System.Configuration;
using Microsoft.Azure.KeyVault;

namespace Table.CSE
{
    class Program
    {
        private static TableProvider<ItemEntity> _tableProvider;
        static void Main(string[] args)
        {
            var accountName = ConfigurationManager.AppSettings["AccountName"];
            var accountKey = ConfigurationManager.AppSettings["AccountKey"];
            var itemTable = ConfigurationManager.AppSettings["ItemTableName"];
            var encryptionKeyId = ConfigurationManager.AppSettings["EncryptionKey"];
            var keyVaultUri = ConfigurationManager.AppSettings["KeyVaultUri"];
            var clientId = ConfigurationManager.AppSettings["KeyVaultClientId"];
            var keyVaultCertificateThumbprint = ConfigurationManager.AppSettings["KeyVaultClientCertificateThumbprint"];

            var authService = new AadAuthService(clientId, keyVaultCertificateThumbprint);
            var keyVaultProvider = new KeyVaultProvider(keyVaultUri, authService);

            var cloudResolver = new GlobalKeyResolver(keyVaultProvider);

            //One-Time job. Should be done by a script
            cloudResolver.AddKey(encryptionKeyId, Guid.NewGuid().ToString()).Wait();

            var kid = keyVaultProvider.GetSecretIdAsync(encryptionKeyId).Result;


            _tableProvider = new TableProvider<ItemEntity>(accountName, accountKey, itemTable, kid, cloudResolver);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*************************************SECURE ITEM MANAGEMENT*************************************");

            var shouldContinue = true;
            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("MENU");
                Console.WriteLine("1. Insert Item");
                Console.WriteLine("2. Get Item By ID");
                Console.WriteLine("3. List all Items by Category");
                Console.WriteLine("Enter any other number to exit");
                Console.Write("> ");

                var option = int.Parse(Console.ReadLine());
                shouldContinue = Operate(option);

            } while (shouldContinue);
        }

        private static bool Operate(int option)
        {
            switch (option)
            {
                case 1:
                    InserItem();
                    break;
                case 2:
                    GetItem();
                    break;
                case 3:
                    GetItemsByCategory();
                    break;
                default: return false;
            }
            return true;
        }

        private static void InserItem()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(">> Create New Line Item");
            Console.WriteLine(">> Item Category");
            Console.Write(">>> ");
            var itemCategory = Console.ReadLine();
            Console.WriteLine(">> Item ID");
            Console.Write(">>> ");
            var itemId = Console.ReadLine();
            Console.WriteLine(">> Item Name");
            Console.Write(">>> ");
            var itemName = Console.ReadLine();
            Console.WriteLine(">> Description");
            Console.Write(">>> ");
            var description = Console.ReadLine();
            Console.WriteLine(">> SKU");
            Console.Write(">>> ");
            var sku = Console.ReadLine();
            Console.WriteLine(">> User Name");
            Console.Write(">>> ");
            var userName = Console.ReadLine();
            Console.WriteLine(">> User Contact");
            Console.Write(">>> ");
            var userContact = Console.ReadLine();

            Console.WriteLine("Uploading Item ...");
            var item = new ItemEntity(itemId: itemId,
                category: itemCategory,
                name: itemName,
                sku: sku,
                description: description,
                userName: userName,
                phoneNumber: userContact);

            _tableProvider.Insert(item);
            Console.WriteLine("Item Uploaded");
            Console.WriteLine("--------------------------------------------------------------------------------------");
        }

        private static void GetItem()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(">> Enter Item Details");
            Console.WriteLine(">> Item Category");
            Console.Write(">>> ");
            var itemCategory = Console.ReadLine();
            Console.WriteLine(">> Item ID");
            Console.Write(">>> ");
            var itemId = Console.ReadLine();

            Console.WriteLine("Downloading Item ...");
            var item = _tableProvider.Get(itemCategory, itemId);
            PrintItem(item);
        }

        private static void GetItemsByCategory()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(">> Enter Category");
            Console.Write(">>> ");
            var category = Console.ReadLine();


            Console.WriteLine("Downloading Items ...");
            var items = _tableProvider.Query("PartitionKey", category, "eq");
            items.ForEach(PrintItem);
        }

        private static void PrintItem(ItemEntity item)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(">> Line Item Details");
            Console.WriteLine($">>> ID - {item.RowKey}");
            Console.WriteLine($">>> Name - {item.Name}");
            Console.WriteLine($">>> Category - {item.PartitionKey}");
            Console.WriteLine($">>> SKU - {item.SKU}");
            Console.WriteLine($">>> Description - {item.Description}");
            Console.WriteLine($">>> Purchased By - {item.BoughtBy}");
            Console.WriteLine($">>> Contact - {item.BuyerPhoneNumber}");
            Console.WriteLine("--------------------------------------------------------------------------------------");
        }
    }
}