namespace DynamicDnsUpdater.Service.Interface
{
	public interface IIpAddressChecker
    {
        System.String GetCurrentIpAddress(System.String IpProviderURL, IClient client);     
    }
}
