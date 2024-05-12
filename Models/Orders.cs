namespace MultiTenantApps.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public string OrderDate { get; set; }
        public string RequiredDate { get; set; }
        public string ShippedDate { get; set; }
        public int ShipVia { get; set; }
        public float Freight { get; set; }
        public string ShipName { get; set; }
        public ShipAddress ShipAddress { get; set; }
        public List<OrderDetail> Details { get; set; }

    }
}
