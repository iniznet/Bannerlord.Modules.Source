using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateAgent : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public int MountAgentIndex { get; private set; }

		public NetworkCommunicator Peer { get; private set; }

		public BasicCharacterObject Character { get; private set; }

		public Monster Monster { get; private set; }

		public MissionEquipment MissionEquipment { get; private set; }

		public Equipment SpawnEquipment { get; private set; }

		public BodyProperties BodyPropertiesValue { get; private set; }

		public int BodyPropertiesSeed { get; private set; }

		public bool IsFemale { get; private set; }

		public int TeamIndex { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec2 Direction { get; private set; }

		public int FormationIndex { get; private set; }

		public bool IsPlayerAgent { get; private set; }

		public uint ClothingColor1 { get; private set; }

		public uint ClothingColor2 { get; private set; }

		public CreateAgent(int agentIndex, BasicCharacterObject character, Monster monster, Equipment spawnEquipment, MissionEquipment missionEquipment, BodyProperties bodyPropertiesValue, int bodyPropertiesSeed, bool isFemale, int agentTeamIndex, int agentFormationIndex, uint clothingColor1, uint clothingColor2, int mountAgentIndex, Equipment mountAgentSpawnEquipment, bool isPlayerAgent, Vec3 position, Vec2 direction, NetworkCommunicator peer)
		{
			this.AgentIndex = agentIndex;
			this.MountAgentIndex = mountAgentIndex;
			this.Peer = peer;
			this.Character = character;
			this.Monster = monster;
			this.SpawnEquipment = new Equipment();
			this.MissionEquipment = new MissionEquipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this.MissionEquipment[equipmentIndex] = missionEquipment[equipmentIndex];
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.ArmorItemEndSlot; equipmentIndex2++)
			{
				this.SpawnEquipment[equipmentIndex2] = spawnEquipment.GetEquipmentFromSlot(equipmentIndex2);
			}
			if (this.MountAgentIndex >= 0)
			{
				this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = mountAgentSpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
				this.SpawnEquipment[EquipmentIndex.HorseHarness] = mountAgentSpawnEquipment[EquipmentIndex.HorseHarness];
			}
			else
			{
				this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = default(EquipmentElement);
				this.SpawnEquipment[EquipmentIndex.HorseHarness] = default(EquipmentElement);
			}
			this.BodyPropertiesValue = bodyPropertiesValue;
			this.BodyPropertiesSeed = bodyPropertiesSeed;
			this.IsFemale = isFemale;
			this.TeamIndex = agentTeamIndex;
			this.Position = position;
			this.Direction = direction;
			this.FormationIndex = agentFormationIndex;
			this.ClothingColor1 = clothingColor1;
			this.ClothingColor2 = clothingColor2;
			this.IsPlayerAgent = isPlayerAgent;
		}

		public CreateAgent()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Character = (BasicCharacterObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			this.Monster = (Monster)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.MountAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.SpawnEquipment = new Equipment();
			this.MissionEquipment = new MissionEquipment();
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				this.MissionEquipment[equipmentIndex] = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				this.SpawnEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex2, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref flag));
			}
			this.IsPlayerAgent = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.BodyPropertiesSeed = ((!this.IsPlayerAgent) ? GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RandomSeedCompressionInfo, ref flag) : 0);
			this.BodyPropertiesValue = GameNetworkMessage.ReadBodyPropertiesFromPacket(ref flag);
			this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.TeamIndex = GameNetworkMessage.ReadTeamIndexFromPacket(ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref flag).Normalized();
			this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FormationClassCompressionInfo, ref flag);
			this.ClothingColor1 = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref flag);
			this.ClothingColor2 = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Character, CompressionBasic.GUIDCompressionInfo);
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Monster, CompressionBasic.GUIDCompressionInfo);
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteAgentIndexToPacket(this.MountAgentIndex);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				ModuleNetworkData.WriteWeaponReferenceToPacket(this.MissionEquipment[equipmentIndex]);
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.NumAllWeaponSlots; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				ModuleNetworkData.WriteItemReferenceToPacket(this.SpawnEquipment.GetEquipmentFromSlot(equipmentIndex2));
			}
			GameNetworkMessage.WriteBoolToPacket(this.IsPlayerAgent);
			if (!this.IsPlayerAgent)
			{
				GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionBasic.RandomSeedCompressionInfo);
			}
			GameNetworkMessage.WriteBodyPropertiesToPacket(this.BodyPropertiesValue);
			GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
			GameNetworkMessage.WriteTeamIndexToPacket(this.TeamIndex);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionMission.FormationClassCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.ClothingColor1, CompressionBasic.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.ClothingColor2, CompressionBasic.ColorCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

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
