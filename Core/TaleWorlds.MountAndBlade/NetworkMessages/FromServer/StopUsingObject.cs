using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B7 RID: 183
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StopUsingObject : GameNetworkMessage
	{
		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000792 RID: 1938 RVA: 0x0000DBE2 File Offset: 0x0000BDE2
		// (set) Token: 0x06000793 RID: 1939 RVA: 0x0000DBEA File Offset: 0x0000BDEA
		public Agent Agent { get; private set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000794 RID: 1940 RVA: 0x0000DBF3 File Offset: 0x0000BDF3
		// (set) Token: 0x06000795 RID: 1941 RVA: 0x0000DBFB File Offset: 0x0000BDFB
		public bool IsSuccessful { get; private set; }

		// Token: 0x06000796 RID: 1942 RVA: 0x0000DC04 File Offset: 0x0000BE04
		public StopUsingObject(Agent agent, bool isSuccessful)
		{
			this.Agent = agent;
			this.IsSuccessful = isSuccessful;
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x0000DC1A File Offset: 0x0000BE1A
		public StopUsingObject()
		{
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0000DC24 File Offset: 0x0000BE24
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsSuccessful = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0000DC4F File Offset: 0x0000BE4F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteBoolToPacket(this.IsSuccessful);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0000DC67 File Offset: 0x0000BE67
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x0000DC6F File Offset: 0x0000BE6F
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Stop using Object on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
