namespace DynamicDnsUpdater.Service.Interface
{
	public interface IClient
    {
        string GetContent(string IpProviderUrl, DelegateParser parser);
    }
}
