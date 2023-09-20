using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200006A RID: 106
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToWeaponInAgentEquipmentSlot : GameNetworkMessage
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x000075ED File Offset: 0x000057ED
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x000075F5 File Offset: 0x000057F5
		public MissionWeapon Weapon { get; private set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x000075FE File Offset: 0x000057FE
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x00007606 File Offset: 0x00005806
		public EquipmentIndex SlotIndex { get; private set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060003DA RID: 986 RVA: 0x0000760F File Offset: 0x0000580F
		// (set) Token: 0x060003DB RID: 987 RVA: 0x00007617 File Offset: 0x00005817
		public Agent Agent { get; private set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060003DC RID: 988 RVA: 0x00007620 File Offset: 0x00005820
		// (set) Token: 0x060003DD RID: 989 RVA: 0x00007628 File Offset: 0x00005828
		public MatrixFrame AttachLocalFrame { get; private set; }

		// Token: 0x060003DE RID: 990 RVA: 0x00007631 File Offset: 0x00005831
		public AttachWeaponToWeaponInAgentEquipmentSlot(MissionWeapon weapon, Agent agent, EquipmentIndex slot, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.Agent = agent;
			this.SlotIndex = slot;
			this.AttachLocalFrame = attachLocalFrame;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00007656 File Offset: 0x00005856
		public AttachWeaponToWeaponInAgentEquipmentSlot()
		{
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00007660 File Offset: 0x00005860
		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000076B8 File Offset: 0x000058B8
		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref flag);
			if (flag)
			{
				this.AttachLocalFrame = new MatrixFrame(mat, vec);
			}
			return flag;
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0000771F File Offset: 0x0000591F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00007728 File Offset: 0x00005928
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"AttachWeaponToWeaponInAgentEquipmentSlot with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
