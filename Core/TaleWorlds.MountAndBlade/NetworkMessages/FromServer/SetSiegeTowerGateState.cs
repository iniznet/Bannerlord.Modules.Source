using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeTowerGateState : GameNetworkMessage
	{
		public MissionObjectId SiegeTowerId { get; private set; }

		public SiegeTower.GateState State { get; private set; }

		public SetSiegeTowerGateState(MissionObjectId siegeTowerId, SiegeTower.GateState state)
		{
			this.SiegeTowerId = siegeTowerId;
			this.State = state;
		}

		public SetSiegeTowerGateState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeTowerId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.State = (SiegeTower.GateState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SiegeTowerId);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set SiegeTower State to: ", this.State, " on SiegeTower with ID: ", this.SiegeTowerId });
		}
	}
}
