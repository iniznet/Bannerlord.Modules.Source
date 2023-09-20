using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMachineTargetRotation : GameNetworkMessage
	{
		public UsableMachine UsableMachine { get; private set; }

		public float HorizontalRotation { get; private set; }

		public float VerticalRotation { get; private set; }

		public SetMachineTargetRotation(UsableMachine usableMachine, float horizontalRotaiton, float verticalRotation)
		{
			this.UsableMachine = usableMachine;
			this.HorizontalRotation = horizontalRotaiton;
			this.VerticalRotation = verticalRotation;
		}

		public SetMachineTargetRotation()
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
				"Set target rotation of UsableMachine with ID: ",
				this.UsableMachine.Id,
				" and with name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
