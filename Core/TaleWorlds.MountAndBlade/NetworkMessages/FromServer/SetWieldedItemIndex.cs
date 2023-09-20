using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWieldedItemIndex : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public bool IsLeftHand { get; private set; }

		public bool IsWieldedInstantly { get; private set; }

		public bool IsWieldedOnSpawn { get; private set; }

		public EquipmentIndex WieldedItemIndex { get; private set; }

		public int MainHandCurrentUsageIndex { get; private set; }

		public SetWieldedItemIndex(Agent agent, bool isLeftHand, bool isWieldedInstantly, bool isWieldedOnSpawn, EquipmentIndex wieldedItemIndex, int mainHandCurUsageIndex)
		{
			this.Agent = agent;
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
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.IsLeftHand = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedInstantly = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsWieldedOnSpawn = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.WieldedItemIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			this.MainHandCurrentUsageIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
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
			return string.Concat(new object[]
			{
				"Set Wielded Item Index to: ",
				this.WieldedItemIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
