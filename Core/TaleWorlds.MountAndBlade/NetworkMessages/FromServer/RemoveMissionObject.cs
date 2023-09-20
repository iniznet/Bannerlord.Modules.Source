using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveMissionObject : GameNetworkMessage
	{
		public MissionObjectId ObjectId { get; private set; }

		public RemoveMissionObject(MissionObjectId objectId)
		{
			this.ObjectId = objectId;
		}

		public RemoveMissionObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return "Remove MissionObject with ID: " + this.ObjectId;
		}
	}
}
