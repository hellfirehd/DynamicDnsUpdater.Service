namespace DKW.DynamicDnsUpdater.Interface
{
	public interface IClient
	{
		string GetContent(string IpProviderUrl, DelegateParser parser);
	}
}
