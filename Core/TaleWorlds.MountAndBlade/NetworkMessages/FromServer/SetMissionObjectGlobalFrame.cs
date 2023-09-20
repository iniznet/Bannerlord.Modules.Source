using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectGlobalFrame : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public SetMissionObjectGlobalFrame(MissionObject missionObject, ref MatrixFrame frame)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
		}

		public SetMissionObjectGlobalFrame()
		{
		}

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

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

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
