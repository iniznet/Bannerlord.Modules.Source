using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009C RID: 156
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectGlobalFrameOverTime : GameNetworkMessage
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x0000BB62 File Offset: 0x00009D62
		// (set) Token: 0x06000655 RID: 1621 RVA: 0x0000BB6A File Offset: 0x00009D6A
		public MissionObject MissionObject { get; private set; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x0000BB73 File Offset: 0x00009D73
		// (set) Token: 0x06000657 RID: 1623 RVA: 0x0000BB7B File Offset: 0x00009D7B
		public MatrixFrame Frame { get; private set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x0000BB84 File Offset: 0x00009D84
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x0000BB8C File Offset: 0x00009D8C
		public float Duration { get; private set; }

		// Token: 0x0600065A RID: 1626 RVA: 0x0000BB95 File Offset: 0x00009D95
		public SetMissionObjectGlobalFrameOverTime(MissionObject missionObject, ref MatrixFrame frame, float duration)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
			this.Duration = duration;
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0000BBB7 File Offset: 0x00009DB7
		public SetMissionObjectGlobalFrameOverTime()
		{
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0000BBC0 File Offset: 0x00009DC0
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			Vec3 vec2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			Vec3 vec3 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag);
			Vec3 vec4 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref flag);
			Vec3 vec5 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			if (flag)
			{
				this.Frame = new MatrixFrame(new Mat3(vec, vec2, vec3), vec5);
				this.Frame.Scale(vec4);
			}
			this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0000BC5C File Offset: 0x00009E5C
		protected override void OnWrite()
		{
			Vec3 scaleVector = this.Frame.rotation.GetScaleVector();
			MatrixFrame frame = this.Frame;
			frame.Scale(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z, -1f));
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteVec3ToPacket(frame.rotation.f, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(frame.rotation.s, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(frame.rotation.u, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(scaleVector, CompressionBasic.ScaleCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(frame.origin, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Duration, CompressionMission.FlagCapturePointDurationCompressionInfo);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0000BD2E File Offset: 0x00009F2E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0000BD38 File Offset: 0x00009F38
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Move-to-global-frame on MissionObject with ID: ",
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
