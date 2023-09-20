using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetSiegeMachineMovementDistance : GameNetworkMessage
	{
		public UsableMachine UsableMachine { get; private set; }

		public float Distance { get; private set; }

		public SetSiegeMachineMovementDistance(UsableMachine usableMachine, float distance)
		{
			this.UsableMachine = usableMachine;
			this.Distance = distance;
		}

		public SetSiegeMachineMovementDistance()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.UsableMachine = missionObject as UsableMachine;
			this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableMachine);
			GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.PositionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Movement Distance: ",
				this.Distance,
				" of SiegeMachine with ID: ",
				this.UsableMachine.Id,
				" and name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
