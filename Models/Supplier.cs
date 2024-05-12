using System.Net;

namespace MultiTenantApps.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public AddressModel Address { get; set; }
    }
}
