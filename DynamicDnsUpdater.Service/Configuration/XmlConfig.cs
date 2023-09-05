using System;
using System.Xml.Serialization;

namespace DynamicDnsUpdater.Service.Configuration
{
	/// <summary>
	/// Xml Serialization on mapping to the XmlConfig.xml
	/// </summary>

	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class XmlConfig
    {

        [System.Xml.Serialization.XmlArrayItemAttribute("Domain", IsNullable = false)]
        public Domain[] Domains { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("Provider", IsNullable = false)]
        public Provider[] Providers { get; set; }

        [System.Xml.Serialization.XmlArrayItemAttribute("IpChecker", IsNullable = false)]
        public IpChecker[] IpCheckers { get; set; }

    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Provider
    {
        public DynamicDnsUpdater.Service.Meta.Enum.DnsProviderType ProviderType { get; set; }
        public string ProviderUrl { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class IpChecker
    {
        public DynamicDnsUpdater.Service.Meta.Enum.IpCheckerType IpCheckerType { get; set; }
        public DynamicDnsUpdater.Service.Meta.Enum.ClientType ClientType { get; set; }
        public string IpCheckerUrl { get; set; }
    }
}

