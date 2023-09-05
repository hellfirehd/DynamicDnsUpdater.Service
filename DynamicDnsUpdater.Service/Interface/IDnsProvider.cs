namespace DynamicDnsUpdater.Service.Interface
{
	public interface IDnsProvider
    {
        System.String UpdateDns(System.String accessID, System.String secretKey, System.String providerUrl, System.String domainName, System.String hostZoneId, System.String newIPaddress);
        Meta.Enum.ChangeStatusType CheckUpdateStatus(System.String accessID, System.String secretKey, System.String providerUrl, System.String id);
    }
}
