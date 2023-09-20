using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithMissionObject : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public ApplyOrderWithMissionObject(MissionObjectId missionObjectId)
		{
			this.MissionObjectId = missionObjectId;
		}

		public ApplyOrderWithMissionObject()
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
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return "Apply order to MissionObject with ID: " + this.MissionObjectId + " and with name ";
		}
	}
}
