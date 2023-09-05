namespace DynamicDnsUpdater.Service.Interface
{
	public interface IClient
    {
        System.String GetContent(System.String IpProviderUrl, DelegateParser parser);
    }
}
