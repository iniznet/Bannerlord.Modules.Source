using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMachineTargetRotation : GameNetworkMessage
	{
		public MissionObjectId UsableMachineId { get; private set; }

		public float HorizontalRotation { get; private set; }

		public float VerticalRotation { get; private set; }

		public SetMachineTargetRotation(MissionObjectId usableMachineId, float horizontalRotaiton, float verticalRotation)
		{
			this.UsableMachineId = usableMachineId;
			this.HorizontalRotation = horizontalRotaiton;
			this.VerticalRotation = verticalRotation;
		}

		public SetMachineTargetRotation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMachineId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.HorizontalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			this.VerticalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.UsableMachineId);
			GameNetworkMessage.WriteFloatToPacket(this.HorizontalRotation, CompressionBasic.HighResRadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.VerticalRotation, CompressionBasic.HighResRadianCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Set target rotation of UsableMachine with ID: " + this.UsableMachineId;
		}
	}
}
