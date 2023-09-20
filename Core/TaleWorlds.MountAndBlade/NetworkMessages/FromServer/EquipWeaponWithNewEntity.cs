using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000079 RID: 121
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class EquipWeaponWithNewEntity : GameNetworkMessage
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0000945A File Offset: 0x0000765A
		// (set) Token: 0x060004CD RID: 1229 RVA: 0x00009462 File Offset: 0x00007662
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x0000946B File Offset: 0x0000766B
		// (set) Token: 0x060004CF RID: 1231 RVA: 0x00009473 File Offset: 0x00007673
		public EquipmentIndex SlotIndex { get; private set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0000947C File Offset: 0x0000767C
		// (set) Token: 0x060004D1 RID: 1233 RVA: 0x00009484 File Offset: 0x00007684
		public Agent Agent { get; private set; }

		// Token: 0x060004D2 RID: 1234 RVA: 0x0000948D File Offset: 0x0000768D
		public EquipWeaponWithNewEntity(Agent agent, EquipmentIndex slot, MissionWeapon weapon)
		{
			this.Agent = agent;
			this.SlotIndex = slot;
			this.Weapon = weapon;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x000094AA File Offset: 0x000076AA
		public EquipWeaponWithNewEntity()
		{
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x000094B2 File Offset: 0x000076B2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x000094DC File Offset: 0x000076DC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0000951E File Offset: 0x0000771E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00009528 File Offset: 0x00007728
		protected override string OnGetLogFormat()
		{
			if (this.Agent == null)
			{
				return "Not equipping weapon because there is no agent to equip it to,";
			}
			return string.Concat(new object[]
			{
				"Equip weapon with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" from SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
