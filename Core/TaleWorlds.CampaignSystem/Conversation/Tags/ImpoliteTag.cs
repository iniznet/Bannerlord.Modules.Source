using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200021B RID: 539
	public class ImpoliteTag : ConversationTag
	{
		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06001E57 RID: 7767 RVA: 0x000874E2 File Offset: 0x000856E2
		public override string StringId
		{
			get
			{
				return "ImpoliteTag";
			}
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x000874EC File Offset: 0x000856EC
		public override bool IsApplicableTo(CharacterObject character)
		{
			if (!character.IsHero)
			{
				return false;
			}
			int heroRelation = CharacterRelationManager.GetHeroRelation(character.HeroObject, Hero.MainHero);
			return (character.HeroObject.IsLord || character.HeroObject.IsMerchant || character.HeroObject.IsGangLeader) && Clan.PlayerClan.Renown < 100f && heroRelation < 1 && character.GetTraitLevel(DefaultTraits.Mercy) + character.GetTraitLevel(DefaultTraits.Generosity) < 0;
		}

		// Token: 0x0400099D RID: 2461
		public const string Id = "ImpoliteTag";
	}
}
