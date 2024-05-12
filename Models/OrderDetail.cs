namespace MultiTenantApps.Models
{
    public class OrderDetail
    {
        public int ProductId { get; set; }
        public float UnitPrice { get; set; }
        public float Quantity { get; set; }
        public float Discount { get; set; }
    }
}
