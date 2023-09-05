namespace DynamicDnsUpdater.Service.Interface
{
	public interface IDnsProvider
    {
        string UpdateDns(string accessID, string secretKey, string providerUrl, string domainName, string hostZoneId, string newIPaddress);
        Meta.Enum.ChangeStatusType CheckUpdateStatus(string accessID, string secretKey, string providerUrl, string id);
    }
}
