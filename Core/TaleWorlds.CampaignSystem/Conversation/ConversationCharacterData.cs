using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001EE RID: 494
	public struct ConversationCharacterData : ISerializableObject
	{
		// Token: 0x06001D42 RID: 7490 RVA: 0x00084740 File Offset: 0x00082940
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

		// Token: 0x06001D43 RID: 7491 RVA: 0x00084780 File Offset: 0x00082980
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

		// Token: 0x06001D44 RID: 7492 RVA: 0x000847E8 File Offset: 0x000829E8
		void ISerializableObject.SerializeTo(IWriter writer)
		{
			writer.WriteUInt(this.Character.Id.InternalValue);
			writer.WriteInt((this.Party == null) ? (-1) : this.Party.Index);
			writer.WriteBool(this.NoHorse);
			writer.WriteBool(this.NoWeapon);
			writer.WriteBool(this.SpawnedAfterFight);
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x00084850 File Offset: 0x00082A50
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

		// Token: 0x0400091F RID: 2335
		public CharacterObject Character;

		// Token: 0x04000920 RID: 2336
		public PartyBase Party;

		// Token: 0x04000921 RID: 2337
		public bool NoHorse;

		// Token: 0x04000922 RID: 2338
		public bool NoWeapon;

		// Token: 0x04000923 RID: 2339
		public bool NoBodyguards;

		// Token: 0x04000924 RID: 2340
		public bool SpawnedAfterFight;

		// Token: 0x04000925 RID: 2341
		public bool IsCivilianEquipmentRequiredForLeader;

		// Token: 0x04000926 RID: 2342
		public bool IsCivilianEquipmentRequiredForBodyGuardCharacters;
	}
}
