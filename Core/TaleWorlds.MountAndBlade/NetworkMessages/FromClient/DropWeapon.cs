using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DropWeapon : GameNetworkMessage
	{
		public bool IsDefendPressed { get; private set; }

		public EquipmentIndex ForcedSlotIndexToDropWeaponFrom { get; private set; }

		public DropWeapon(bool isDefendPressed, EquipmentIndex forcedSlotIndexToDropWeaponFrom)
		{
			this.IsDefendPressed = isDefendPressed;
			this.ForcedSlotIndexToDropWeaponFrom = forcedSlotIndexToDropWeaponFrom;
		}

		public DropWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.IsDefendPressed = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.ForcedSlotIndexToDropWeaponFrom = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.IsDefendPressed);
			GameNetworkMessage.WriteIntToPacket((int)this.ForcedSlotIndexToDropWeaponFrom, CompressionMission.WieldSlotCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			bool flag = this.ForcedSlotIndexToDropWeaponFrom != EquipmentIndex.None;
			return "Dropping " + ((!flag) ? "equipped" : "") + " weapon" + (flag ? (" " + (int)this.ForcedSlotIndexToDropWeaponFrom) : "");
		}
	}
}
