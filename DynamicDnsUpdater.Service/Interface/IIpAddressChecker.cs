namespace DynamicDnsUpdater.Service.Interface
{
	public interface IIpAddressChecker
    {       
          string GetCurrentIpAddress(string IpProviderURL, IClient client);     
    }
}
