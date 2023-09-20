using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateAgentVisuals : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int VisualsIndex { get; private set; }

		public BasicCharacterObject Character { get; private set; }

		public Equipment Equipment { get; private set; }

		public int BodyPropertiesSeed { get; private set; }

		public bool IsFemale { get; private set; }

		public int SelectedEquipmentSetIndex { get; private set; }

		public int TroopCountInFormation { get; private set; }

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

		public CreateAgentVisuals()
		{
		}

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
			this.BodyPropertiesSeed = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RandomSeedCompressionInfo, ref flag);
			this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.SelectedEquipmentSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			this.TroopCountInFormation = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			return flag;
		}

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
			GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionBasic.RandomSeedCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
			GameNetworkMessage.WriteIntToPacket(this.SelectedEquipmentSetIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.TroopCountInFormation, CompressionBasic.PlayerCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents;
		}

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
