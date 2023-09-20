using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000095 RID: 149
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationChannelParameter : GameNetworkMessage
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x0000B339 File Offset: 0x00009539
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x0000B341 File Offset: 0x00009541
		public MissionObject MissionObject { get; private set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x0000B34A File Offset: 0x0000954A
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x0000B352 File Offset: 0x00009552
		public int ChannelNo { get; private set; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x0000B35B File Offset: 0x0000955B
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x0000B363 File Offset: 0x00009563
		public float Parameter { get; private set; }

		// Token: 0x06000610 RID: 1552 RVA: 0x0000B36C File Offset: 0x0000956C
		public SetMissionObjectAnimationChannelParameter(MissionObject missionObject, int channelNo, float parameter)
		{
			this.MissionObject = missionObject;
			this.ChannelNo = channelNo;
			this.Parameter = parameter;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0000B389 File Offset: 0x00009589
		public SetMissionObjectAnimationChannelParameter()
		{
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x0000B394 File Offset: 0x00009594
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (flag)
			{
				this.ChannelNo = (flag2 ? 1 : 0);
			}
			this.Parameter = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0000B3DB File Offset: 0x000095DB
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
			GameNetworkMessage.WriteFloatToPacket(this.Parameter, CompressionBasic.AnimationProgressCompressionInfo);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x0000B406 File Offset: 0x00009606
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0000B410 File Offset: 0x00009610
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set animation parameter: ",
				this.Parameter,
				" on channel: ",
				this.ChannelNo,
				" of MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
