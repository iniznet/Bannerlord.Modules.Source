using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerHasArrivedAtTarget : GameNetworkMessage
	{
		public SiegeTower SiegeTower { get; private set; }

		public SetSiegeTowerHasArrivedAtTarget(SiegeTower siegeTower)
		{
			this.SiegeTower = siegeTower;
		}

		public SetSiegeTowerHasArrivedAtTarget()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeTower = missionObject as SiegeTower;
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeTower);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeapons;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"SiegeTower with ID: ",
				this.SiegeTower.Id,
				" and name: ",
				this.SiegeTower.GameEntity.Name,
				" has arrived at its target."
			});
		}
	}
}
