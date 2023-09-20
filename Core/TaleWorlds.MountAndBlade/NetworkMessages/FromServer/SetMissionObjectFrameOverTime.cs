using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrameOverTime : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public float Duration { get; private set; }

		public SetMissionObjectFrameOverTime(MissionObjectId missionObjectId, ref MatrixFrame frame, float duration)
		{
			this.MissionObjectId = missionObjectId;
			this.Frame = frame;
			this.Duration = duration;
		}

		public SetMissionObjectFrameOverTime()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
			GameNetworkMessage.WriteFloatToPacket(this.Duration, CompressionMission.FlagCapturePointDurationCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set Move-to-frame on MissionObject with ID: ", this.MissionObjectId, " over a period of ", this.Duration, " seconds." });
		}
	}
}
