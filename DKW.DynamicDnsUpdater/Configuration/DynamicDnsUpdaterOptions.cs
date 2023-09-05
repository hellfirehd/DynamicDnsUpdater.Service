namespace DKW.DynamicDnsUpdater.Configuration;

public class DynamicDnsUpdaterOptions
{
	public Int32 ClientTimeoutInMinutes { get; set; } = 1;
	public Int32 UpdateIntervalInMinutes { get; set; } = 5;
	public Int32 MonitorStatusInMinutes { get; set; } = 60;
	public Int32 ForceUpdateInDays { get; set; } = 30;
	public Boolean EnablePasswordEncryption { get; set; }
	public IReadOnlyCollection<Domain> Domains { get; set; } = Array.Empty<Domain>();
	public IReadOnlyCollection<Provider> Providers { get; set; } = Array.Empty<Provider>();
	public IReadOnlyCollection<IpChecker> IpCheckers { get; set; } = Array.Empty<IpChecker>();
}

public class Domain
{
	public String DomainName { get; set; } = String.Empty;
	public DnsProviderType ProviderType { get; set; }
	public String HostedZoneId { get; set; } = String.Empty;
	public String AccessID { get; set; } = String.Empty;
	public String SecretKey { get; set; } = String.Empty;
	public Int32 MinimalUpdateIntervalInMinutes { get; set; }
	public String LastIpAddress { get; set; } = String.Empty;
	public DateTime? LastUpdatedDateTime { get; set; }
	public String ChangeStatusID { get; set; } = String.Empty;
	public ICollection<String> HistoricalIPAddress { get; set; } = new List<String>();
	public UpdateReasonType LastUpdatedReason { get; set; }
}

public class Provider
{
	public DnsProviderType ProviderType { get; set; }
	public String ProviderUrl { get; set; } = String.Empty;
	public String ApiKey { get; set; } = String.Empty;
}

public class IpChecker
{
	public IpCheckerType IpCheckerType { get; set; }
	public ClientType ClientType { get; set; }
	public String IpCheckerUrl { get; set; } = String.Empty;
}

public enum DnsProviderType
{
	AMAZON_ROUTE_53
}

public enum IpCheckerType
{
	DYN_DNS,
	JSON_IP,
	CUSTOM
}

public enum UpdateReasonType
{
	FORCED,
	CHANGED
}

public enum ClientType
{
	WEB_HTTP
}
