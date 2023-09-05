using DKW.DynamicDnsUpdater.Interface;

namespace DKW.DynamicDnsUpdater.Models
{

	/// <summary>
	/// Model mapping of IpChecker for XmlConfig
	/// </summary>
	public class IpCheckerModel
	{
		public Meta.Enum.IpCheckerType IpCheckerType { get; set; }
		public Meta.Enum.ClientType ClientType { get; set; }
		public String IpCheckerUrl { get; set; }
		public IClient Client { get; set; }
		public IIpAddressChecker IpAddressChecker { get; set; }
	}



}
