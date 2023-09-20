using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000026 RID: 38
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class DropWeapon : GameNetworkMessage
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600012D RID: 301 RVA: 0x000036F1 File Offset: 0x000018F1
		// (set) Token: 0x0600012E RID: 302 RVA: 0x000036F9 File Offset: 0x000018F9
		public bool IsDefendPressed { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00003702 File Offset: 0x00001902
		// (set) Token: 0x06000130 RID: 304 RVA: 0x0000370A File Offset: 0x0000190A
		public EquipmentIndex ForcedSlotIndexToDropWeaponFrom { get; private set; }

		// Token: 0x06000131 RID: 305 RVA: 0x00003713 File Offset: 0x00001913
		public DropWeapon(bool isDefendPressed, EquipmentIndex forcedSlotIndexToDropWeaponFrom)
		{
			this.IsDefendPressed = isDefendPressed;
			this.ForcedSlotIndexToDropWeaponFrom = forcedSlotIndexToDropWeaponFrom;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00003729 File Offset: 0x00001929
		public DropWeapon()
		{
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00003734 File Offset: 0x00001934
		protected override bool OnRead()
		{
			bool flag = true;
			this.IsDefendPressed = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.ForcedSlotIndexToDropWeaponFrom = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00003763 File Offset: 0x00001963
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.IsDefendPressed);
			GameNetworkMessage.WriteIntToPacket((int)this.ForcedSlotIndexToDropWeaponFrom, CompressionMission.WieldSlotCompressionInfo);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00003780 File Offset: 0x00001980
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00003784 File Offset: 0x00001984
		protected override string OnGetLogFormat()
		{
			bool flag = this.ForcedSlotIndexToDropWeaponFrom != EquipmentIndex.None;
			return "Dropping " + ((!flag) ? "equipped" : "") + " weapon" + (flag ? (" " + (int)this.ForcedSlotIndexToDropWeaponFrom) : "");
		}
	}
}
