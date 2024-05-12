namespace MultiTenantApps.Models
{
    public class LoginModel
    {
        public string TenantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
