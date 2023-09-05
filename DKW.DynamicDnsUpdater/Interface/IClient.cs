namespace DKW.DynamicDnsUpdater.Interface
{
	public interface IClient
	{
        String GetContent(String IpProviderUrl, DelegateParser parser);
	}
}
