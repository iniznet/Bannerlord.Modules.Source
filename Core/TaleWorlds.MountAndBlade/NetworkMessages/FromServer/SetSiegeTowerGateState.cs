using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerGateState : GameNetworkMessage
	{
		public SiegeTower SiegeTower { get; private set; }

		public SiegeTower.GateState State { get; private set; }

		public SetSiegeTowerGateState(SiegeTower siegeTower, SiegeTower.GateState state)
		{
			this.SiegeTower = siegeTower;
			this.State = state;
		}

		public SetSiegeTowerGateState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeTower = missionObject as SiegeTower;
			this.State = (SiegeTower.GateState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeTower);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set SiegeTower State to: ",
				this.State,
				" on SiegeTower with ID: ",
				this.SiegeTower.Id,
				" and name: ",
				this.SiegeTower.GameEntity.Name
			});
		}
	}
}
