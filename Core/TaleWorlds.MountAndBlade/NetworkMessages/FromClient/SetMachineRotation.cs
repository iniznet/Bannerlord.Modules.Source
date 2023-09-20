using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SetMachineRotation : GameNetworkMessage
	{
		public UsableMachine UsableMachine { get; private set; }

		public float HorizontalRotation { get; private set; }

		public float VerticalRotation { get; private set; }

		public SetMachineRotation(UsableMachine missionObject, float horizontalRotation, float verticalRotation)
		{
			this.UsableMachine = missionObject;
			this.HorizontalRotation = horizontalRotation;
			this.VerticalRotation = verticalRotation;
		}

		public SetMachineRotation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMachine = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMachine;
			this.HorizontalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			this.VerticalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableMachine);
			GameNetworkMessage.WriteFloatToPacket(this.HorizontalRotation, CompressionBasic.HighResRadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.VerticalRotation, CompressionBasic.HighResRadianCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set rotation of UsableMachine with ID: ",
				this.UsableMachine.Id,
				" and with name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
