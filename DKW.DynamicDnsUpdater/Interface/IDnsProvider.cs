using DKW.DynamicDnsUpdater.Configuration;

namespace DKW.DynamicDnsUpdater.Interface;

public interface IDnsProvider
{
	DnsProviderType Type { get; }

	String UpdateDns(String domainName, String newIPaddress);
	ChangeStatusType CheckUpdateStatus(String domainName);
}
