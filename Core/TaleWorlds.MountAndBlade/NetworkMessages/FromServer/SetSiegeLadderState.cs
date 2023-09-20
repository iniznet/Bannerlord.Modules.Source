using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeLadderState : GameNetworkMessage
	{
		public MissionObjectId SiegeLadderId { get; private set; }

		public SiegeLadder.LadderState State { get; private set; }

		public SetSiegeLadderState(MissionObjectId siegeLadderId, SiegeLadder.LadderState state)
		{
			this.SiegeLadderId = siegeLadderId;
			this.State = state;
		}

		public SetSiegeLadderState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeLadderId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.State = (SiegeLadder.LadderState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SiegeLadderId);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeLadderStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set SiegeLadder State to: ", this.State, " on SiegeLadderState with ID: ", this.SiegeLadderId });
		}
	}
}
