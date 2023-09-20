using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultMarriageModel : MarriageModel
	{
		public override int MinimumMarriageAgeMale
		{
			get
			{
				return 18;
			}
		}

		public override int MinimumMarriageAgeFemale
		{
			get
			{
				return 18;
			}
		}

		public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
		{
			if (this.IsClanSuitableForMarriage(firstHero.Clan) && this.IsClanSuitableForMarriage(secondHero.Clan))
			{
				Clan clan = firstHero.Clan;
				if (((clan != null) ? clan.Leader : null) == firstHero)
				{
					Clan clan2 = secondHero.Clan;
					if (((clan2 != null) ? clan2.Leader : null) == secondHero)
					{
						return false;
					}
				}
				if (firstHero.IsFemale != secondHero.IsFemale && !this.AreHeroesRelated(firstHero, secondHero, 3))
				{
					Hero courtedHeroInOtherClan = Romance.GetCourtedHeroInOtherClan(firstHero, secondHero);
					if (courtedHeroInOtherClan != null && courtedHeroInOtherClan != secondHero)
					{
						return false;
					}
					Hero courtedHeroInOtherClan2 = Romance.GetCourtedHeroInOtherClan(secondHero, firstHero);
					return (courtedHeroInOtherClan2 == null || courtedHeroInOtherClan2 == firstHero) && firstHero.CanMarry() && secondHero.CanMarry();
				}
			}
			return false;
		}

		public override bool IsClanSuitableForMarriage(Clan clan)
		{
			return clan != null && !clan.IsBanditFaction && clan != CampaignData.NeutralFaction && !clan.IsRebelClan;
		}

		public override float NpcCoupleMarriageChance(Hero firstHero, Hero secondHero)
		{
			if (this.IsCoupleSuitableForMarriage(firstHero, secondHero))
			{
				float num = 0.002f;
				num *= 1f + (firstHero.Age - 18f) / 50f;
				num *= 1f + (secondHero.Age - 18f) / 50f;
				num *= 1f - MathF.Abs(secondHero.Age - firstHero.Age) / 50f;
				if (firstHero.Clan.Kingdom != secondHero.Clan.Kingdom)
				{
					num *= 0.5f;
				}
				float num2 = 0.5f + (float)firstHero.Clan.GetRelationWithClan(secondHero.Clan) / 200f;
				return num * num2;
			}
			return 0f;
		}

		public override bool ShouldNpcMarriageBetweenClansBeAllowed(Clan consideringClan, Clan targetClan)
		{
			return targetClan != consideringClan && !consideringClan.IsAtWarWith(targetClan) && consideringClan.GetRelationWithClan(targetClan) >= -50;
		}

		public override List<Hero> GetAdultChildrenSuitableForMarriage(Hero hero)
		{
			List<Hero> list = new List<Hero>();
			foreach (Hero hero2 in hero.Children)
			{
				if (hero2.CanMarry())
				{
					list.Add(hero2);
				}
			}
			return list;
		}

		private bool AreHeroesRelatedAux1(Hero firstHero, Hero secondHero, int ancestorDepth)
		{
			return firstHero == secondHero || (ancestorDepth > 0 && ((secondHero.Mother != null && this.AreHeroesRelatedAux1(firstHero, secondHero.Mother, ancestorDepth - 1)) || (secondHero.Father != null && this.AreHeroesRelatedAux1(firstHero, secondHero.Father, ancestorDepth - 1))));
		}

		private bool AreHeroesRelatedAux2(Hero firstHero, Hero secondHero, int ancestorDepth, int secondAncestorDepth)
		{
			return this.AreHeroesRelatedAux1(firstHero, secondHero, secondAncestorDepth) || (ancestorDepth > 0 && ((firstHero.Mother != null && this.AreHeroesRelatedAux2(firstHero.Mother, secondHero, ancestorDepth - 1, secondAncestorDepth)) || (firstHero.Father != null && this.AreHeroesRelatedAux2(firstHero.Father, secondHero, ancestorDepth - 1, secondAncestorDepth))));
		}

		private bool AreHeroesRelated(Hero firstHero, Hero secondHero, int ancestorDepth)
		{
			return this.AreHeroesRelatedAux2(firstHero, secondHero, ancestorDepth, ancestorDepth);
		}

		public override int GetEffectiveRelationIncrease(Hero firstHero, Hero secondHero)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(20f, false, null);
			SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Charm, DefaultSkillEffects.CharmRelationBonus, firstHero.IsFemale ? secondHero.CharacterObject : firstHero.CharacterObject, ref explainedNumber, -1, true, 0);
			return MathF.Round(explainedNumber.ResultNumber);
		}

		public override bool IsSuitableForMarriage(Hero maidenOrSuitor)
		{
			if (maidenOrSuitor.IsActive && maidenOrSuitor.Spouse == null && maidenOrSuitor.IsLord && !maidenOrSuitor.IsMinorFactionHero && !maidenOrSuitor.IsNotable && !maidenOrSuitor.IsTemplate)
			{
				MobileParty partyBelongedTo = maidenOrSuitor.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.MapEvent : null) == null)
				{
					MobileParty partyBelongedTo2 = maidenOrSuitor.PartyBelongedTo;
					if (((partyBelongedTo2 != null) ? partyBelongedTo2.Army : null) == null)
					{
						if (maidenOrSuitor.IsFemale)
						{
							return maidenOrSuitor.CharacterObject.Age >= (float)this.MinimumMarriageAgeFemale;
						}
						return maidenOrSuitor.CharacterObject.Age >= (float)this.MinimumMarriageAgeMale;
					}
				}
			}
			return false;
		}

		public override Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero)
		{
			if (firstHero.IsHumanPlayerCharacter)
			{
				return firstHero.Clan;
			}
			if (secondHero.IsHumanPlayerCharacter)
			{
				return secondHero.Clan;
			}
			if (firstHero.Clan.Leader == firstHero)
			{
				return firstHero.Clan;
			}
			if (secondHero.Clan.Leader == secondHero)
			{
				return secondHero.Clan;
			}
			if (!firstHero.IsFemale)
			{
				return firstHero.Clan;
			}
			return secondHero.Clan;
		}

		private const float BaseMarriageChanceForNpcs = 0.002f;
	}
}
