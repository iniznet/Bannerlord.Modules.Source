using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SetMachineRotation : GameNetworkMessage
	{
		public MissionObjectId UsableMachineId { get; private set; }

		public float HorizontalRotation { get; private set; }

		public float VerticalRotation { get; private set; }

		public SetMachineRotation(MissionObjectId missionObjectId, float horizontalRotation, float verticalRotation)
		{
			this.UsableMachineId = missionObjectId;
			this.HorizontalRotation = horizontalRotation;
			this.VerticalRotation = verticalRotation;
		}

		public SetMachineRotation()
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
			return "Set rotation of UsableMachine with ID: " + this.UsableMachineId;
		}
	}
}
