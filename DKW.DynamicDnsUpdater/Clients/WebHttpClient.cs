using DKW.DynamicDnsUpdater.Configuration;
using DKW.DynamicDnsUpdater.Interface;
using Microsoft.Extensions.Options;
using System.Net;

namespace DKW.DynamicDnsUpdater.Clients
{

	/// <summary>
	/// Web Http Client with Delegate Parser
	/// </summary>
	public class WebHttpClient : IClient
	{
		private readonly DynamicDnsUpdaterOptions _options;

		public WebHttpClient(IOptions<DynamicDnsUpdaterOptions> options)
		{
			_options = options.Value;
		}

		/// <summary>
		/// Get the content of the Html as string
		/// </summary>
		/// <param name="IpProviderUrl"></param>
		/// <returns></returns>
		public string? GetContent(string IpProviderUrl, DelegateParser parser)
		{
			string? content = null;
			int timeoutInMilliSeconds = _options.ClientTimeoutInMinutes * 60 * 1000;

			// Use IDisposable webclient to get the page of content of existing IP
			using (TimeoutWebClient client = new TimeoutWebClient(timeoutInMilliSeconds))
			{
				content = client.DownloadString(IpProviderUrl);
			}

			return content != null ? parser(content) : null;
		}

		/// <summary>
		/// To support timeout value, alternatively you can use HttpWebRequest and Stream
		/// </summary>
		public class TimeoutWebClient : WebClient
		{
			public int Timeout { get; set; }

			public TimeoutWebClient(int timeout)
			{
				Timeout = timeout;
			}

			protected override WebRequest GetWebRequest(Uri address)
			{
				WebRequest request = base.GetWebRequest(address);
				request.Timeout = Timeout;
				return request;
			}
		}


	}
}
