using DKW.DynamicDnsUpdater.Helpers;
using DKW.DynamicDnsUpdater.Interface;
using System.Collections.Generic;


namespace DKW.DynamicDnsUpdater.Providers
{
	public class JsonIpAddressChecker : IIpAddressChecker
	{
		/// <summary>
		/// Get Current IP address 
		/// </summary>
		/// <param name="ipProviderURL"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public string GetCurrentIpAddress(string ipProviderURL, IClient client)
		{
			// Pass the parser as function to the client
			DelegateParser handler = Parse;
			return client.GetContent(ipProviderURL, handler);

		}

		/// <summary>
		/// Parse the JsonIP in JSON format to IP
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private string Parse(string jsonString)
		{
			string ipString = null;

			// format: {"ip":"x.x.x.x","about":"/about","Pro!":"http://getjsonip.com"}

			var jsonSerializer = new JavaScriptSerializer();
			Dictionary<string, string> data = jsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
			ipString = data["ip"];

			// Validate if this is a valid IPV4 address
			if (IpHelper.IpAddressV4Validator(ipString))
				return ipString;
			else
				return null;
		}

	}
}
