using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x02000049 RID: 73
	public class InnerProcessManager
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x0000529E File Offset: 0x0000349E
		public InnerProcessManager()
		{
			this._activeServers = new Dictionary<int, IInnerProcessServer>();
			this._connectionRequests = new List<InnerProcessConnectionRequest>();
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x000052BC File Offset: 0x000034BC
		internal void Activate(IInnerProcessServer server, int port)
		{
			this._activeServers.Add(port, server);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000052CB File Offset: 0x000034CB
		internal void RequestConnection(IInnerProcessClient client, int port)
		{
			this._connectionRequests.Add(new InnerProcessConnectionRequest(client, port));
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000052E0 File Offset: 0x000034E0
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

		// Token: 0x04000098 RID: 152
		private Dictionary<int, IInnerProcessServer> _activeServers;

		// Token: 0x04000099 RID: 153
		private List<InnerProcessConnectionRequest> _connectionRequests;
	}
}
