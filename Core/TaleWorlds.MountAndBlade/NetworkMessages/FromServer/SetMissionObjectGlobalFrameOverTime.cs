using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectGlobalFrameOverTime : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public float Duration { get; private set; }

		public SetMissionObjectGlobalFrameOverTime(MissionObject missionObject, ref MatrixFrame frame, float duration)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
			this.Duration = duration;
		}

		public SetMissionObjectGlobalFrameOverTime()
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
			this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
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
			GameNetworkMessage.WriteFloatToPacket(this.Duration, CompressionMission.FlagCapturePointDurationCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

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
