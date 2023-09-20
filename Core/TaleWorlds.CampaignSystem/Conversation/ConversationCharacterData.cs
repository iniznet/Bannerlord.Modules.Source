using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Conversation
{
	public struct ConversationCharacterData : ISerializableObject
	{
		public ConversationCharacterData(CharacterObject character, PartyBase party = null, bool noHorse = false, bool noWeapon = false, bool spawnAfterFight = false, bool isCivilianEquipmentRequiredForLeader = false, bool isCivilianEquipmentRequiredForBodyGuardCharacters = false, bool noBodyguards = false)
		{
			this.Character = character;
			this.Party = party;
			this.NoHorse = noHorse;
			this.NoWeapon = noWeapon;
			this.NoBodyguards = noBodyguards;
			this.SpawnedAfterFight = spawnAfterFight;
			this.IsCivilianEquipmentRequiredForLeader = isCivilianEquipmentRequiredForLeader;
			this.IsCivilianEquipmentRequiredForBodyGuardCharacters = isCivilianEquipmentRequiredForBodyGuardCharacters;
		}

		void ISerializableObject.DeserializeFrom(IReader reader)
		{
			MBGUID mbguid = new MBGUID(reader.ReadUInt());
			this.Character = (CharacterObject)MBObjectManager.Instance.GetObject(mbguid);
			int num = reader.ReadInt();
			this.Party = ConversationCharacterData.FindParty(num);
			this.NoHorse = reader.ReadBool();
			this.NoWeapon = reader.ReadBool();
			this.SpawnedAfterFight = reader.ReadBool();
		}

		void ISerializableObject.SerializeTo(IWriter writer)
		{
			writer.WriteUInt(this.Character.Id.InternalValue);
			writer.WriteInt((this.Party == null) ? (-1) : this.Party.Index);
			writer.WriteBool(this.NoHorse);
			writer.WriteBool(this.NoWeapon);
			writer.WriteBool(this.SpawnedAfterFight);
		}

		private static PartyBase FindParty(int index)
		{
			MobileParty mobileParty = Campaign.Current.CampaignObjectManager.Find<MobileParty>((MobileParty x) => x.Party.Index == index);
			if (mobileParty != null)
			{
				return mobileParty.Party;
			}
			Settlement settlement = Settlement.All.FirstOrDefaultQ((Settlement x) => x.Party.Index == index);
			if (settlement != null)
			{
				return settlement.Party;
			}
			return null;
		}

		public CharacterObject Character;

		public PartyBase Party;

		public bool NoHorse;

		public bool NoWeapon;

		public bool NoBodyguards;

		public bool SpawnedAfterFight;

		public bool IsCivilianEquipmentRequiredForLeader;

		public bool IsCivilianEquipmentRequiredForBodyGuardCharacters;
	}
}
