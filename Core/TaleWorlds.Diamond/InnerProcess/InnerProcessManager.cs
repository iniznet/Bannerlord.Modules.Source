using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	public class InnerProcessManager
	{
		public InnerProcessManager()
		{
			this._activeServers = new Dictionary<int, IInnerProcessServer>();
			this._connectionRequests = new List<InnerProcessConnectionRequest>();
		}

		internal void Activate(IInnerProcessServer server, int port)
		{
			this._activeServers.Add(port, server);
		}

		internal void RequestConnection(IInnerProcessClient client, int port)
		{
			this._connectionRequests.Add(new InnerProcessConnectionRequest(client, port));
		}

		public void Update()
		{
			for (int i = 0; i < this._connectionRequests.Count; i++)
			{
				InnerProcessConnectionRequest innerProcessConnectionRequest = this._connectionRequests[i];
				IInnerProcessClient client = innerProcessConnectionRequest.Client;
				int port = innerProcessConnectionRequest.Port;
				IInnerProcessServer innerProcessServer;
				if (this._activeServers.TryGetValue(port, out innerProcessServer))
				{
					this._connectionRequests.RemoveAt(i);
					i--;
					InnerProcessServerSession innerProcessServerSession = innerProcessServer.AddNewConnection(client);
					client.HandleConnected(innerProcessServerSession);
					innerProcessServerSession.HandleConnected(client);
				}
			}
			foreach (IInnerProcessServer innerProcessServer2 in this._activeServers.Values)
			{
				innerProcessServer2.Update();
			}
		}

		private Dictionary<int, IInnerProcessServer> _activeServers;

		private List<InnerProcessConnectionRequest> _connectionRequests;
	}
}
