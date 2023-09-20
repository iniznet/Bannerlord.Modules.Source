using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetBatteringRamHasArrivedAtTarget : GameNetworkMessage
	{
		public BatteringRam BatteringRam { get; private set; }

		public SetBatteringRamHasArrivedAtTarget(BatteringRam batteringRam)
		{
			this.BatteringRam = batteringRam;
		}

		public SetBatteringRamHasArrivedAtTarget()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BatteringRam = missionObject as BatteringRam;
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.BatteringRam);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Battering Ram with ID: ",
				this.BatteringRam.Id,
				" and name: ",
				this.BatteringRam.GameEntity.Name,
				" has arrived at its target."
			});
		}
	}
}
