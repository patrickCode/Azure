namespace Json.Sender
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public override string ToString()
        {
            return string.Format("ID > {0}\nType - {1}\nDescription - {2}\nCustomer ID > {3}", Id, Type, Description, CustomerId);
        }
    }
}
