using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000072 RID: 114
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateAgent : GameNetworkMessage
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x000081F1 File Offset: 0x000063F1
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x000081F9 File Offset: 0x000063F9
		public int AgentIndex { get; private set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00008202 File Offset: 0x00006402
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x0000820A File Offset: 0x0000640A
		public int MountAgentIndex { get; private set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00008213 File Offset: 0x00006413
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0000821B File Offset: 0x0000641B
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00008224 File Offset: 0x00006424
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0000822C File Offset: 0x0000642C
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00008235 File Offset: 0x00006435
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x0000823D File Offset: 0x0000643D
		public Monster Monster { get; private set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00008246 File Offset: 0x00006446
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0000824E File Offset: 0x0000644E
		public MissionEquipment SpawnMissionEquipment { get; private set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00008257 File Offset: 0x00006457
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x0000825F File Offset: 0x0000645F
		public Equipment SpawnEquipment { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00008268 File Offset: 0x00006468
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x00008270 File Offset: 0x00006470
		public BodyProperties BodyPropertiesValue { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x00008279 File Offset: 0x00006479
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x00008281 File Offset: 0x00006481
		public int BodyPropertiesSeed { get; private set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0000828A File Offset: 0x0000648A
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x00008292 File Offset: 0x00006492
		public bool IsFemale { get; private set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x0000829B File Offset: 0x0000649B
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x000082A3 File Offset: 0x000064A3
		public Team Team { get; private set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x000082AC File Offset: 0x000064AC
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x000082B4 File Offset: 0x000064B4
		public Vec3 Position { get; private set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x000082BD File Offset: 0x000064BD
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x000082C5 File Offset: 0x000064C5
		public Vec2 Direction { get; private set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x000082CE File Offset: 0x000064CE
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x000082D6 File Offset: 0x000064D6
		public int FormationIndex { get; private set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x000082DF File Offset: 0x000064DF
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x000082E7 File Offset: 0x000064E7
		public bool IsPlayerAgent { get; private set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x000082F0 File Offset: 0x000064F0
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x000082F8 File Offset: 0x000064F8
		public uint ClothingColor1 { get; private set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x00008301 File Offset: 0x00006501
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x00008309 File Offset: 0x00006509
		public uint ClothingColor2 { get; private set; }

		// Token: 0x0600045E RID: 1118 RVA: 0x00008314 File Offset: 0x00006514
		public CreateAgent(Agent agent, bool isPlayerAgent, Vec3 position, Vec2 direction, NetworkCommunicator peer)
		{
			this.AgentIndex = agent.Index;
			bool flag = agent.MountAgent != null && agent.MountAgent.RiderAgent == agent;
			this.MountAgentIndex = (flag ? agent.MountAgent.Index : (-1));
			this.Peer = peer;
			this.Character = agent.Character;
			this.Monster = agent.Monster;
			this.SpawnEquipment = new Equipment();
			this.SpawnMissionEquipment = new MissionEquipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this.SpawnMissionEquipment[equipmentIndex] = agent.Equipment[equipmentIndex];
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.ArmorItemEndSlot; equipmentIndex2++)
			{
				this.SpawnEquipment[equipmentIndex2] = agent.SpawnEquipment.GetEquipmentFromSlot(equipmentIndex2);
			}
			if (flag)
			{
				this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = agent.MountAgent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
				this.SpawnEquipment[EquipmentIndex.HorseHarness] = agent.MountAgent.SpawnEquipment[EquipmentIndex.HorseHarness];
			}
			else
			{
				this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = default(EquipmentElement);
				this.SpawnEquipment[EquipmentIndex.HorseHarness] = default(EquipmentElement);
			}
			this.BodyPropertiesValue = agent.BodyPropertiesValue;
			this.BodyPropertiesSeed = agent.BodyPropertiesSeed;
			this.IsFemale = agent.IsFemale;
			this.Team = agent.Team;
			this.Position = position;
			this.Direction = direction;
			Formation formation = agent.Formation;
			this.FormationIndex = ((formation != null) ? formation.Index : (-1));
			this.ClothingColor1 = agent.ClothingColor1;
			this.ClothingColor2 = agent.ClothingColor2;
			this.IsPlayerAgent = isPlayerAgent;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x000084C8 File Offset: 0x000066C8
		public CreateAgent()
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x000084D0 File Offset: 0x000066D0
		protected override bool OnRead()
		{
			bool flag = true;
			this.Character = (BasicCharacterObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			this.Monster = (Monster)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref flag);
			this.MountAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref flag);
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.SpawnEquipment = new Equipment();
			this.SpawnMissionEquipment = new MissionEquipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this.SpawnMissionEquipment[equipmentIndex] = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				this.SpawnEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex2, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref flag));
			}
			this.IsPlayerAgent = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.BodyPropertiesSeed = ((!this.IsPlayerAgent) ? GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.RandomSeedCompressionInfo, ref flag) : 0);
			this.BodyPropertiesValue = GameNetworkMessage.ReadBodyPropertiesFromPacket(ref flag);
			this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag).Normalized();
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref flag);
			this.ClothingColor1 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.ClothingColor2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0000865C File Offset: 0x0000685C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Character, CompressionBasic.GUIDCompressionInfo);
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Monster, CompressionBasic.GUIDCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AgentIndex, CompressionMission.AgentCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MountAgentIndex, CompressionMission.AgentCompressionInfo);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				ModuleNetworkData.WriteWeaponReferenceToPacket(this.SpawnMissionEquipment[equipmentIndex]);
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				ModuleNetworkData.WriteItemReferenceToPacket(this.SpawnEquipment.GetEquipmentFromSlot(equipmentIndex2));
			}
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerAgent);
			if (!this.IsPlayerAgent)
			{
				GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionGeneric.RandomSeedCompressionInfo);
			}
			GameNetworkMessage.WriteBodyPropertiesToPacket(this.BodyPropertiesValue);
			GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
			GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.ClothingColor1, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.ClothingColor2, CompressionGeneric.ColorCompressionInfo);
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00008783 File Offset: 0x00006983
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0000878C File Offset: 0x0000698C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create an agent with index: ",
				this.AgentIndex,
				(this.Peer != null) ? string.Concat(new object[]
				{
					", belonging to peer with Name: ",
					this.Peer.UserName,
					", and peer-index: ",
					this.Peer.Index
				}) : "",
				(this.MountAgentIndex == -1) ? "" : (", owning a mount with index: " + this.MountAgentIndex)
			});
		}
	}
}
