using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWieldedItemIndex : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public bool IsLeftHand { get; private set; }

		public bool IsWieldedInstantly { get; private set; }

		public bool IsWieldedOnSpawn { get; private set; }

		public EquipmentIndex WieldedItemIndex { get; private set; }

		public int MainHandCurrentUsageIndex { get; private set; }

		public SetWieldedItemIndex(int agentIndex, bool isLeftHand, bool isWieldedInstantly, bool isWieldedOnSpawn, EquipmentIndex wieldedItemIndex, int mainHandCurUsageIndex)
		{
			this.AgentIndex = agentIndex;
			this.IsLeftHand = isLeftHand;
			this.IsWieldedInstantly = isWieldedInstantly;
			this.IsWieldedOnSpawn = isWieldedOnSpawn;
			this.WieldedItemIndex = wieldedItemIndex;
			this.MainHandCurrentUsageIndex = mainHandCurUsageIndex;
		}

		public SetWieldedItemIndex()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.IsLeftHand = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedInstantly = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedOnSpawn = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.WieldedItemIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			this.MainHandCurrentUsageIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteBoolToPacket(this.IsLeftHand);
			GameNetworkMessage.WriteBoolToPacket(this.IsWieldedInstantly);
			GameNetworkMessage.WriteBoolToPacket(this.IsWieldedOnSpawn);
			GameNetworkMessage.WriteIntToPacket((int)this.WieldedItemIndex, CompressionMission.WieldSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MainHandCurrentUsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set Wielded Item Index to: ", this.WieldedItemIndex, " on Agent with agent-index: ", this.AgentIndex });
		}
	}
}
