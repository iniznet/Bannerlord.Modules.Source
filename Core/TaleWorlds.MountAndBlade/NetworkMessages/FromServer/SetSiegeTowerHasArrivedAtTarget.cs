using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerHasArrivedAtTarget : GameNetworkMessage
	{
		public MissionObjectId SiegeTowerId { get; private set; }

		public SetSiegeTowerHasArrivedAtTarget(MissionObjectId siegeTowerId)
		{
			this.SiegeTowerId = siegeTowerId;
		}

		public SetSiegeTowerHasArrivedAtTarget()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeTowerId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SiegeTowerId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeapons;
		}

		protected override string OnGetLogFormat()
		{
			return "SiegeTower with ID: " + this.SiegeTowerId + " has arrived at its target.";
		}
	}
}
