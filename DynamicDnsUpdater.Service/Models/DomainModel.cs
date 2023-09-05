using DynamicDnsUpdater.Service.Interface;
using System;

namespace DynamicDnsUpdater.Service.Models
{


	/// <summary>
	/// Model mapping of Domain for XmlConfig
	/// </summary>
	public class DomainModel
    {
        public String DomainName { get; set; }
        public Meta.Enum.DnsProviderType DnsProviderType { get; set; }
        public IDnsProvider DnsProvider { get; set; }
        public String ProviderUrl { get; set; }     // Linked in XmlConfig and now it is flatten as one of the properties
        public String HostedZoneId { get; set; }
        public String AccessID { get; set; }
        public String SecretKey { get; set; }
        public String MinimalUpdateIntervalInMinutes { get; set; }
        public String LastIpAddress { get; set; }    // CURRENT ip address
        public DateTime LastUpdatedDateTime { get; set; }
        public String ChangeStatusID { get; set; }     // Id for tracking update status 
        public String HistoricalIpAddress { get; set; }  // PREVIOUS ip address
        public Meta.Enum.UpdateReasonType LastUpdatedReason { get; set; }
    }

}
