namespace BillingService.Models
{
    public class UserModel
    {
        public UserWithCoinCounts[] users { get; set; }
        public Pagination pagination { get; set; }
    }
}
