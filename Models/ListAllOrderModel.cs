namespace MultiTenantApps.Models
{
    public class ListAllOrderModel
    {
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string CustomerName { get; set; }
        public float Total { get; set; }

    }
}
