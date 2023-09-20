using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000073 RID: 115
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateAgentVisuals : GameNetworkMessage
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00008830 File Offset: 0x00006A30
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x00008838 File Offset: 0x00006A38
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00008841 File Offset: 0x00006A41
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00008849 File Offset: 0x00006A49
		public int VisualsIndex { get; private set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00008852 File Offset: 0x00006A52
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x0000885A File Offset: 0x00006A5A
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00008863 File Offset: 0x00006A63
		// (set) Token: 0x0600046B RID: 1131 RVA: 0x0000886B File Offset: 0x00006A6B
		public Equipment Equipment { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x00008874 File Offset: 0x00006A74
		// (set) Token: 0x0600046D RID: 1133 RVA: 0x0000887C File Offset: 0x00006A7C
		public int BodyPropertiesSeed { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00008885 File Offset: 0x00006A85
		// (set) Token: 0x0600046F RID: 1135 RVA: 0x0000888D File Offset: 0x00006A8D
		public bool IsFemale { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00008896 File Offset: 0x00006A96
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x0000889E File Offset: 0x00006A9E
		public int SelectedEquipmentSetIndex { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x000088A7 File Offset: 0x00006AA7
		// (set) Token: 0x06000473 RID: 1139 RVA: 0x000088AF File Offset: 0x00006AAF
		public int TroopCountInFormation { get; private set; }

		// Token: 0x06000474 RID: 1140 RVA: 0x000088B8 File Offset: 0x00006AB8
		public CreateAgentVisuals(NetworkCommunicator peer, AgentBuildData agentBuildData, int selectedEquipmentSetIndex, int troopCountInFormation = 0)
		{
			this.Peer = peer;
			this.VisualsIndex = agentBuildData.AgentVisualsIndex;
			this.Character = agentBuildData.AgentCharacter;
			this.BodyPropertiesSeed = agentBuildData.AgentEquipmentSeed;
			this.IsFemale = agentBuildData.AgentIsFemale;
			this.Equipment = new Equipment();
			this.Equipment.FillFrom(agentBuildData.AgentOverridenSpawnEquipment, true);
			this.SelectedEquipmentSetIndex = selectedEquipmentSetIndex;
			this.TroopCountInFormation = troopCountInFormation;
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0000892E File Offset: 0x00006B2E
		public CreateAgentVisuals()
		{
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00008938 File Offset: 0x00006B38
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.VisualsIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			this.Character = (BasicCharacterObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			this.Equipment = new Equipment();
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < (flag2 ? EquipmentIndex.NumEquipmentSetSlots : EquipmentIndex.ArmorItemEndSlot); equipmentIndex++)
			{
				EquipmentElement equipmentElement = ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref flag);
				if (!flag)
				{
					break;
				}
				this.Equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, equipmentElement);
			}
			this.BodyPropertiesSeed = GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.RandomSeedCompressionInfo, ref flag);
			this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.SelectedEquipmentSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			this.TroopCountInFormation = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00008A0C File Offset: 0x00006C0C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.VisualsIndex, CompressionMission.AgentOffsetCompressionInfo);
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Character, CompressionBasic.GUIDCompressionInfo);
			bool flag = this.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null;
			GameNetworkMessage.WriteBoolToPacket(flag);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < (flag ? EquipmentIndex.NumEquipmentSetSlots : EquipmentIndex.ArmorItemEndSlot); equipmentIndex++)
			{
				ModuleNetworkData.WriteItemReferenceToPacket(this.Equipment.GetEquipmentFromSlot(equipmentIndex));
			}
			GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionGeneric.RandomSeedCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
			GameNetworkMessage.WriteIntToPacket(this.SelectedEquipmentSetIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.TroopCountInFormation, CompressionBasic.PlayerCompressionInfo);
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00008AC3 File Offset: 0x00006CC3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00008ACB File Offset: 0x00006CCB
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create AgentVisuals for peer: ",
				this.Peer.UserName,
				", and with Index: ",
				this.VisualsIndex
			});
		}
	}
}
