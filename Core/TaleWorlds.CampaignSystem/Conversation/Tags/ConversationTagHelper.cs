using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000251 RID: 593
	public static class ConversationTagHelper
	{
		// Token: 0x06001EF9 RID: 7929 RVA: 0x00087F01 File Offset: 0x00086101
		public static bool UsesHighRegister(CharacterObject character)
		{
			return ConversationTagHelper.EducatedClass(character) && !ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x00087F16 File Offset: 0x00086116
		public static bool UsesLowRegister(CharacterObject character)
		{
			return !ConversationTagHelper.EducatedClass(character) && !ConversationTagHelper.TribalVoiceGroup(character);
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x00087F2C File Offset: 0x0008612C
		public static bool TribalVoiceGroup(CharacterObject character)
		{
			return character.Culture.StringId == "sturgia" || character.Culture.StringId == "aserai" || character.Culture.StringId == "khuzait" || character.Culture.StringId == "battania" || character.Culture.StringId == "vlandia" || character.Culture.StringId == "nord" || character.Culture.StringId == "vakken";
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x00087FE0 File Offset: 0x000861E0
		public static bool EducatedClass(CharacterObject character)
		{
			bool flag = false;
			if (character.HeroObject != null)
			{
				Clan clan = character.HeroObject.Clan;
				if (clan != null && clan.IsNoble)
				{
					flag = true;
				}
				if (character.HeroObject.IsMerchant)
				{
					flag = true;
				}
				if (character.HeroObject.GetTraitLevel(DefaultTraits.Siegecraft) >= 5 || character.HeroObject.GetTraitLevel(DefaultTraits.Surgery) >= 5)
				{
					flag = true;
				}
				if (character.HeroObject.IsGangLeader)
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x0008805C File Offset: 0x0008625C
		public static int TraitCompatibility(Hero hero1, Hero hero2, TraitObject trait)
		{
			int traitLevel = hero1.GetTraitLevel(trait);
			int traitLevel2 = hero2.GetTraitLevel(trait);
			if (traitLevel > 0 && traitLevel2 > 0)
			{
				return 1;
			}
			if (traitLevel < 0 || traitLevel2 < 0)
			{
				return MathF.Abs(traitLevel - traitLevel2) * -1;
			}
			return 0;
		}
	}
}
