using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableProvider
{   
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey) { }

        public CustomerEntity() {
        }

        public string IndividualID { get; set; }
        public string GeographyID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ParentLanguage { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string SecondaryEmailAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string ExtendedPostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryDialCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string Tick { get; set; }
    }
}
