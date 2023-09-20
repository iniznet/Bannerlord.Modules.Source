using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectDisabled : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public SetMissionObjectDisabled(MissionObjectId missionObjectId)
		{
			this.MissionObjectId = missionObjectId;
		}

		public SetMissionObjectDisabled()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return "Mission Object with ID: " + this.MissionObjectId + " has been disabled.";
		}
	}
}
