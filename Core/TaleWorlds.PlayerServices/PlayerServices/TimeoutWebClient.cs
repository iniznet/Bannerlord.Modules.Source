using System;
using System.Net;

namespace TaleWorlds.PlayerServices
{
	public class TimeoutWebClient : WebClient
	{
		public TimeoutWebClient()
		{
			this.Timeout = 15000;
		}

		public TimeoutWebClient(int timeout)
		{
			this.Timeout = timeout;
		}

		public int Timeout { get; set; }

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			webRequest.Timeout = this.Timeout;
			return webRequest;
		}
	}
}
