using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Table.CSE
{
    public class ItemEntity: TableEntity
    {
        public ItemEntity() { }
        //Parition Key - Category
        //Row Key - GUID
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }

        [EncryptProperty]
        public string BoughtBy { get; set; }
        [EncryptProperty]
        public string BuyerPhoneNumber { get; set; }
        public ItemEntity(string name, string category, string itemId, string sku, string description, string userName, string phoneNumber) 
        {
            PartitionKey = category;
            RowKey = itemId;
            Name = name;
            SKU = sku;
            Description = description;
            BoughtBy = userName;
            BuyerPhoneNumber = phoneNumber;
        }
    }
}