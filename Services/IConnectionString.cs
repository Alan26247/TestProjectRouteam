namespace BillingService.Services
{
    public interface IConnectionString
    {
        /// <summary>
        /// получить строку подключения
        /// </summary>
        string ConnectionString { get; }
    }
}