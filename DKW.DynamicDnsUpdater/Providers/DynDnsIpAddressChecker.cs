using DKW.DynamicDnsUpdater.Helpers;
using DKW.DynamicDnsUpdater.Interface;
using System.Xml;

namespace DKW.DynamicDnsUpdater.Providers
{
	public class DynDnsIpAddressChecker : IIpAddressChecker
	{
		/// <summary>
		/// Get Current IP address 
		/// </summary>
		/// <param name="ipProviderURL"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public String GetCurrentIpAddress(String ipProviderURL, IClient client)
		{
			// Pass the parser as function to the client
			DelegateParser handler = Parse;
			return client.GetContent(ipProviderURL, handler);

		}

		/// <summary>
		/// Parse the DynDns using Xpath
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private String Parse(String html)
		{
            String ipString = null;

			// Load the HTML into XmlDoc
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(html);

			// Parse the DynDns HTML document
			// <html><head><title>Current IP Check</title></head><body>Current IP Address: xxx.xxx.xxx.xxx</body></html>

			XmlElement root = xmlDocument.DocumentElement;
			XmlNode node = root.SelectSingleNode("/html/body");

			// Parse using simple substring
			if (node != null)
				ipString = node.InnerXml.Substring(node.InnerXml.IndexOf(':') + 1).Trim();

			// Validate if this is a valid IPV4 address
			if (IpHelper.IpAddressV4Validator(ipString))
				return ipString;
			else
				return null;
		}


	}

}
