using System;

namespace TaleWorlds.Diamond.InnerProcess
{
	internal class InnerProcessConnectionRequest
	{
		public IInnerProcessClient Client { get; private set; }

		public int Port { get; private set; }

		public InnerProcessConnectionRequest(IInnerProcessClient client, int port)
		{
			this.Client = client;
			this.Port = port;
		}
	}
}
