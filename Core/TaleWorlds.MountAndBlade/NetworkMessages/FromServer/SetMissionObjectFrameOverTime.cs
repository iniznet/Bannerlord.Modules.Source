using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrameOverTime : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public float Duration { get; private set; }

		public SetMissionObjectFrameOverTime(MissionObject missionObject, ref MatrixFrame frame, float duration)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
			this.Duration = duration;
		}

		public SetMissionObjectFrameOverTime()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
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
