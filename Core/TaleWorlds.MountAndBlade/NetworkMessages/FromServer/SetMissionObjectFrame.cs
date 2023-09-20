using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrame : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public SetMissionObjectFrame(MissionObjectId missionObjectId, ref MatrixFrame frame)
		{
			this.MissionObjectId = missionObjectId;
			this.Frame = frame;
		}

		public SetMissionObjectFrame()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Set Frame on MissionObject with ID: " + this.MissionObjectId;
		}
	}
}
