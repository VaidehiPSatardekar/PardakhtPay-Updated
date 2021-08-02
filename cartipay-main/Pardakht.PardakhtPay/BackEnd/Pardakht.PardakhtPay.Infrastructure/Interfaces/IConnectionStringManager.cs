namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface IConnectionStringManager
    {
        string Database { get; }
        string MainConnectionString { get; }
        string Password { get; }
        string Server { get; }
        string User { get; }
    }
}
