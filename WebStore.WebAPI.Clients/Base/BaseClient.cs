namespace WebStore.WebAPI.Clients.Base
{
    public abstract class BaseClient
    {
        protected HttpClient? Http { get; }
        protected string Address { get; }
        protected BaseClient(HttpClient client, string address) => (Http, Address) = (client, address);
    }
}
