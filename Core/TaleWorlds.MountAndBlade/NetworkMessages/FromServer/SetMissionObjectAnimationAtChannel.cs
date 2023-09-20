using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000094 RID: 148
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationAtChannel : GameNetworkMessage
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x0000B14E File Offset: 0x0000934E
		// (set) Token: 0x060005FD RID: 1533 RVA: 0x0000B156 File Offset: 0x00009356
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x0000B15F File Offset: 0x0000935F
		// (set) Token: 0x060005FF RID: 1535 RVA: 0x0000B167 File Offset: 0x00009367
		public int ChannelNo { get; private set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x0000B170 File Offset: 0x00009370
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x0000B178 File Offset: 0x00009378
		public int AnimationIndex { get; private set; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x0000B181 File Offset: 0x00009381
		// (set) Token: 0x06000603 RID: 1539 RVA: 0x0000B189 File Offset: 0x00009389
		public float AnimationSpeed { get; private set; }

		// Token: 0x06000604 RID: 1540 RVA: 0x0000B192 File Offset: 0x00009392
		public SetMissionObjectAnimationAtChannel(MissionObject missionObject, int channelNo, int animationIndex, float animationSpeed)
		{
			this.MissionObject = missionObject;
			this.ChannelNo = channelNo;
			this.AnimationIndex = animationIndex;
			this.AnimationSpeed = animationSpeed;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0000B1B7 File Offset: 0x000093B7
		public SetMissionObjectAnimationAtChannel()
		{
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0000B1C0 File Offset: 0x000093C0
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.ChannelNo = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? 1 : 0);
			this.AnimationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref flag);
			this.AnimationSpeed = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref flag) : 1f);
			return flag;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0000B228 File Offset: 0x00009428
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
			GameNetworkMessage.WriteIntToPacket(this.AnimationIndex, CompressionBasic.AnimationIndexCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.AnimationSpeed != 1f);
			if (this.AnimationSpeed != 1f)
			{
				GameNetworkMessage.WriteFloatToPacket(this.AnimationSpeed, CompressionBasic.AnimationSpeedCompressionInfo);
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0000B290 File Offset: 0x00009490
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0000B298 File Offset: 0x00009498
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set animation: ",
				this.AnimationIndex,
				" on channel: ",
				this.ChannelNo,
				" of MissionObject with ID: ",
				this.MissionObject.Id.Id,
				this.MissionObject.Id.CreatedAtRuntime ? " (Created at runtime)" : "",
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
