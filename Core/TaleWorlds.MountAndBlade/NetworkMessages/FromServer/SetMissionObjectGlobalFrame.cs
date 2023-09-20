using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200009B RID: 155
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectGlobalFrame : GameNetworkMessage
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x0000B974 File Offset: 0x00009B74
		// (set) Token: 0x0600064B RID: 1611 RVA: 0x0000B97C File Offset: 0x00009B7C
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0000B985 File Offset: 0x00009B85
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x0000B98D File Offset: 0x00009B8D
		public MatrixFrame Frame { get; private set; }

		// Token: 0x0600064E RID: 1614 RVA: 0x0000B996 File Offset: 0x00009B96
		public SetMissionObjectGlobalFrame(MissionObject missionObject, ref MatrixFrame frame)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0000B9B1 File Offset: 0x00009BB1
		public SetMissionObjectGlobalFrame()
		{
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0000B9BC File Offset: 0x00009BBC
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
			return flag;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0000BA48 File Offset: 0x00009C48
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
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0000BB0A File Offset: 0x00009D0A
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0000BB14 File Offset: 0x00009D14
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Global Frame on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
