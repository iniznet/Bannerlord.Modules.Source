using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009A RID: 154
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrameOverTime : GameNetworkMessage
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0000B83E File Offset: 0x00009A3E
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x0000B846 File Offset: 0x00009A46
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0000B84F File Offset: 0x00009A4F
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x0000B857 File Offset: 0x00009A57
		public MatrixFrame Frame { get; private set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0000B860 File Offset: 0x00009A60
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x0000B868 File Offset: 0x00009A68
		public float Duration { get; private set; }

		// Token: 0x06000644 RID: 1604 RVA: 0x0000B871 File Offset: 0x00009A71
		public SetMissionObjectFrameOverTime(MissionObject missionObject, ref MatrixFrame frame, float duration)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
			this.Duration = duration;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0000B893 File Offset: 0x00009A93
		public SetMissionObjectFrameOverTime()
		{
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x0000B89C File Offset: 0x00009A9C
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0000B8D8 File Offset: 0x00009AD8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
			GameNetworkMessage.WriteFloatToPacket(this.Duration, CompressionMission.FlagCapturePointDurationCompressionInfo);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0000B900 File Offset: 0x00009B00
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0000B908 File Offset: 0x00009B08
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Move-to-frame on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" over a period of ",
				this.Duration,
				" seconds."
			});
		}
	}
}
