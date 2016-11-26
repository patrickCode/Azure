namespace Filter
{
    public class ServiceRequest
    {
        public int Id;
        public string City;
        public string Country;
        public int Amount;
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Id, City, Country, Amount);
        }
    }
}