using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009F RID: 159
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVertexAnimationProgress : GameNetworkMessage
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x0000C01A File Offset: 0x0000A21A
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x0000C022 File Offset: 0x0000A222
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x0000C02B File Offset: 0x0000A22B
		// (set) Token: 0x0600067D RID: 1661 RVA: 0x0000C033 File Offset: 0x0000A233
		public float Progress { get; private set; }

		// Token: 0x0600067E RID: 1662 RVA: 0x0000C03C File Offset: 0x0000A23C
		public SetMissionObjectVertexAnimationProgress(MissionObject missionObject, float progress)
		{
			this.MissionObject = missionObject;
			this.Progress = progress;
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0000C052 File Offset: 0x0000A252
		public SetMissionObjectVertexAnimationProgress()
		{
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0000C05C File Offset: 0x0000A25C
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0000C08B File Offset: 0x0000A28B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.AnimationProgressCompressionInfo);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0000C0B0 File Offset: 0x0000A2B0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set progress of Vertex Animation on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" to: ",
				this.Progress
			});
		}
	}
}
