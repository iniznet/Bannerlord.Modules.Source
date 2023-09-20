using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveEquippedWeapon : GameNetworkMessage
	{
		public EquipmentIndex SlotIndex { get; private set; }

		public Agent Agent { get; private set; }

		public RemoveEquippedWeapon(Agent a, EquipmentIndex slot)
		{
			this.Agent = a;
			this.SlotIndex = slot;
		}

		public RemoveEquippedWeapon()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Remove equipped weapon from SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
