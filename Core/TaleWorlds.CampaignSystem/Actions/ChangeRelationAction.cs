using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeRelationAction
	{
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

		public static void ApplyPlayerRelation(Hero gainedRelationWith, int relation, bool affectRelatives = true, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(Hero.MainHero, gainedRelationWith, relation, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Default);
		}

		public static void ApplyRelationChangeBetweenHeroes(Hero hero, Hero gainedRelationWith, int relationChange, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(hero, gainedRelationWith, relationChange, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Default);
		}

		public static void ApplyEmissaryRelation(Hero emissary, Hero gainedRelationWith, int relationChange, bool showQuickNotification = true)
		{
			ChangeRelationAction.ApplyInternal(emissary, gainedRelationWith, relationChange, showQuickNotification, ChangeRelationAction.ChangeRelationDetail.Emissary);
		}

		public enum ChangeRelationDetail
		{
			Default,
			Emissary
		}
	}
}
