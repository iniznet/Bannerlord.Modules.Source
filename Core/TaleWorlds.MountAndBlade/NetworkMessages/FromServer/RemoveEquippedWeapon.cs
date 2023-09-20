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

		public int AgentIndex { get; private set; }

		public RemoveEquippedWeapon(int agentIndex, EquipmentIndex slot)
		{
			this.AgentIndex = agentIndex;
			this.SlotIndex = slot;
		}

		public RemoveEquippedWeapon()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Remove equipped weapon from SlotIndex: ", this.SlotIndex, " on agent with agent-index: ", this.AgentIndex });
		}
	}
}
