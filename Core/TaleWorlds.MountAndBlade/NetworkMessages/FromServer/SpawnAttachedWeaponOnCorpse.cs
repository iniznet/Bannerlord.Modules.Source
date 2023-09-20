using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000B1 RID: 177
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnCorpse : GameNetworkMessage
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0000D2D7 File Offset: 0x0000B4D7
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x0000D2DF File Offset: 0x0000B4DF
		public Agent Agent { get; private set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0000D2E8 File Offset: 0x0000B4E8
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x0000D2F0 File Offset: 0x0000B4F0
		public int AttachedIndex { get; private set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600073E RID: 1854 RVA: 0x0000D2F9 File Offset: 0x0000B4F9
		// (set) Token: 0x0600073F RID: 1855 RVA: 0x0000D301 File Offset: 0x0000B501
		public int ForcedIndex { get; private set; }

		// Token: 0x06000740 RID: 1856 RVA: 0x0000D30A File Offset: 0x0000B50A
		public SpawnAttachedWeaponOnCorpse(Agent agent, int attachedIndex, int forcedIndex)
		{
			this.Agent = agent;
			this.AttachedIndex = attachedIndex;
			this.ForcedIndex = forcedIndex;
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x0000D327 File Offset: 0x0000B527
		public SpawnAttachedWeaponOnCorpse()
		{
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0000D330 File Offset: 0x0000B530
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.AttachedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x0000D372 File Offset: 0x0000B572
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.AttachedIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x0000D39F File Offset: 0x0000B59F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0000D3A3 File Offset: 0x0000B5A3
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"SpawnAttachedWeaponOnCorpse with index: ",
				this.Agent.Index,
				", and with ID: ",
				this.ForcedIndex
			});
		}
	}
}
