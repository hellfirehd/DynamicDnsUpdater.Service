namespace DKW.DynamicDnsUpdater.Interface
{
	public interface IIpAddressChecker
	{
        String GetCurrentIpAddress(String IpProviderURL, IClient client);
	}
}
