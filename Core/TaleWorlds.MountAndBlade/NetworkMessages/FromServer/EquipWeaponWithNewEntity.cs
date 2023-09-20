using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipWeaponWithNewEntity : GameNetworkMessage
	{
		public MissionWeapon Weapon { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public int AgentIndex { get; private set; }

		public EquipWeaponWithNewEntity(int agentIndex, EquipmentIndex slot, MissionWeapon weapon)
		{
			this.AgentIndex = agentIndex;
			this.SlotIndex = slot;
			this.Weapon = weapon;
		}

		public EquipWeaponWithNewEntity()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			if (this.AgentIndex < 0)
			{
				return "Not equipping weapon because there is no agent to equip it to,";
			}
			return string.Concat(new object[]
			{
				"Equip weapon with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" from SlotIndex: ",
				this.SlotIndex,
				" on agent with agent-index: ",
				this.AgentIndex
			});
		}
	}
}
