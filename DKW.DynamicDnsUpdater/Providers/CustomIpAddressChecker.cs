using DKW.DynamicDnsUpdater.Helpers;
using DKW.DynamicDnsUpdater.Interface;

namespace DKW.DynamicDnsUpdater.Providers
{
	public class CustomIpAddressChecker : IIpAddressChecker
	{
		/*
        
        // Using CustomIpChecker: On your favourite ISP or Amazon AWS EC2, create a simple traditional non-MVC ASPX page with 
        // the following content and you will be able to get the client IP without parsing other 3rd party IpCheckers
         
        <%@ Page Language="C#" %>
        <%
        // In case your server is behind proxy
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        // If not, then use the traditional remote_addr
        if (ip == null) 
           ip = Request.ServerVariables["REMOTE_ADDR"];
        Response.Write(ip);
        %>
         
        */

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

			// No parsing needed, pure IP address return without any HTML markup
			var ipString = client.GetContent(ipProviderURL, handler);

			if (IpHelper.IpAddressV4Validator(ipString))
				return ipString;
			else
				return null;
		}


		/// <summary>
		/// Return whatever input = output 
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private String Parse(String input)
		{
			return input;
		}
	}

}
