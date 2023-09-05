namespace DKW.DynamicDnsUpdater.Interface
{
	public interface IIpAddressChecker
	{
		string GetCurrentIpAddress(string IpProviderURL, IClient client);
	}
}
