using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000062 RID: 98
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddPrefabComponentToAgentBone : GameNetworkMessage
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00006B2B File Offset: 0x00004D2B
		// (set) Token: 0x06000373 RID: 883 RVA: 0x00006B33 File Offset: 0x00004D33
		public Agent Agent { get; private set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00006B3C File Offset: 0x00004D3C
		// (set) Token: 0x06000375 RID: 885 RVA: 0x00006B44 File Offset: 0x00004D44
		public string PrefabName { get; private set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00006B4D File Offset: 0x00004D4D
		// (set) Token: 0x06000377 RID: 887 RVA: 0x00006B55 File Offset: 0x00004D55
		public sbyte BoneIndex { get; private set; }

		// Token: 0x06000378 RID: 888 RVA: 0x00006B5E File Offset: 0x00004D5E
		public AddPrefabComponentToAgentBone(Agent agent, string prefabName, sbyte boneIndex)
		{
			this.Agent = agent;
			this.PrefabName = prefabName;
			this.BoneIndex = boneIndex;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00006B7B File Offset: 0x00004D7B
		public AddPrefabComponentToAgentBone()
		{
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00006B84 File Offset: 0x00004D84
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.PrefabName = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00006BC2 File Offset: 0x00004DC2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteStringToPacket(this.PrefabName);
			GameNetworkMessage.WriteIntToPacket((int)this.BoneIndex, CompressionMission.BoneIndexCompressionInfo);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00006BEA File Offset: 0x00004DEA
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00006BF4 File Offset: 0x00004DF4
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Add prefab component: ",
				this.PrefabName,
				" on bone with index: ",
				this.BoneIndex,
				" on agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
