using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000432 RID: 1074
	public static class ChangeRelationAction
	{
		// Token: 0x06003EBD RID: 16061 RVA: 0x0012C038 File Offset: 0x0012A238
		private static void ApplyInternal(Hero originalHero, Hero originalGainedRelationWith, int relationChange, bool showQuickNotification, ChangeRelationAction.ChangeRelationDetail detail)
		{
			if (relationChange > 0)
			{
				relationChange = MBRandom.RoundRandomized(Campaign.Current.Models.DiplomacyModel.GetRelationIncreaseFactor(originalHero, originalGainedRelationWith, (float)relationChange));
			}
			if (relationChange != 0)
			{
				Hero hero;
				Hero hero2;
				Campaign.Current.Models.DiplomacyModel.GetHeroesForEffectiveRelation(originalHero, originalGainedRelationWith, out hero, out hero2);
				int num = CharacterRelationManager.GetHeroRelation(hero, hero2) + relationChange;
				num = MBMath.ClampInt(num, -100, 100);
				hero.SetPersonalRelation(hero2, num);
				CampaignEventDispatcher.Instance.OnHeroRelationChanged(hero, hero2, relationChange, showQuickNotification, detail, originalHero, originalGainedRelationWith);
			}
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x0012C0B4 File Offset: 0x0012A2B4
		public static void ApplyPlayerRelation(Hero gainedRelationWith, int relation, bool affectRelatives = true, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(Hero.MainHero, gainedRelationWith, relation, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Default);
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x0012C0C4 File Offset: 0x0012A2C4
		public static void ApplyRelationChangeBetweenHeroes(Hero hero, Hero gainedRelationWith, int relationChange, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(hero, gainedRelationWith, relationChange, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Default);
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x0012C0D0 File Offset: 0x0012A2D0
		public static void ApplyEmissaryRelation(Hero emissary, Hero gainedRelationWith, int relationChange, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(emissary, gainedRelationWith, relationChange, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Emissary);
		}

		// Token: 0x02000759 RID: 1881
		public enum ChangeRelationDetail
		{
			// Token: 0x04001E2F RID: 7727
			Default,
			// Token: 0x04001E30 RID: 7728
			Emissary
		}
	}
}
