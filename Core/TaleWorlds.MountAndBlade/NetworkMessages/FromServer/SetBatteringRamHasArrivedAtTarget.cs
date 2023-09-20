using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetBatteringRamHasArrivedAtTarget : GameNetworkMessage
	{
		public MissionObjectId BatteringRamId { get; private set; }

		public SetBatteringRamHasArrivedAtTarget(MissionObjectId batteringRamId)
		{
			this.BatteringRamId = batteringRamId;
		}

		public SetBatteringRamHasArrivedAtTarget()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.BatteringRamId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.BatteringRamId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Battering Ram with ID: " + this.BatteringRamId + " has arrived at its target.";
		}
	}
}
