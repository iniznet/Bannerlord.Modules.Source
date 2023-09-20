using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeMachineMovementDistance : GameNetworkMessage
	{
		public MissionObjectId UsableMachineId { get; private set; }

		public float Distance { get; private set; }

		public SetSiegeMachineMovementDistance(MissionObjectId usableMachineId, float distance)
		{
			this.UsableMachineId = usableMachineId;
			this.Distance = distance;
		}

		public SetSiegeMachineMovementDistance()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMachineId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.UsableMachineId);
			GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.PositionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set Movement Distance: ", this.Distance, " of SiegeMachine with ID: ", this.UsableMachineId });
		}
	}
}
