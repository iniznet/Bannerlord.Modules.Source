using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeLadderState : GameNetworkMessage
	{
		public SiegeLadder SiegeLadder { get; private set; }

		public SiegeLadder.LadderState State { get; private set; }

		public SetSiegeLadderState(SiegeLadder siegeLadder, SiegeLadder.LadderState state)
		{
			this.SiegeLadder = siegeLadder;
			this.State = state;
		}

		public SetSiegeLadderState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.SiegeLadder = missionObject as SiegeLadder;
			this.State = (SiegeLadder.LadderState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeLadder);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeLadderStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set SiegeLadder State to: ",
				this.State,
				" on SiegeLadderState with ID: ",
				this.SiegeLadder.Id,
				" and name: ",
				this.SiegeLadder.GameEntity.Name
			});
		}
	}
}
