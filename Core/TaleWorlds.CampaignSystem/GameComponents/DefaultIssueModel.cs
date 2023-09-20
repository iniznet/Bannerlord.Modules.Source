using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultIssueModel : IssueModel
	{
		public override int IssueOwnerCoolDownInDays
		{
			get
			{
				return 30;
			}
		}

		public override float GetIssueDifficultyMultiplier()
		{
			return MBMath.ClampFloat(Campaign.Current.PlayerProgress, 0.1f, 1f);
		}

		public override void GetIssueEffectsOfSettlement(IssueEffect issueEffect, Settlement settlement, ref ExplainedNumber explainedNumber)
		{
			Hero leader = settlement.OwnerClan.Leader;
			if (leader != null && leader.IsAlive && leader.Issue != null)
			{
				this.GetIssueEffectOfHeroInternal(issueEffect, leader, ref explainedNumber, DefaultIssueModel.SettlementIssuesText);
			}
			foreach (Hero hero in settlement.HeroesWithoutParty)
			{
				if (hero.Issue != null)
				{
					this.GetIssueEffectOfHeroInternal(issueEffect, hero, ref explainedNumber, DefaultIssueModel.SettlementIssuesText);
				}
			}
			if (settlement.IsTown || settlement.IsCastle)
			{
				foreach (Village village in settlement.BoundVillages)
				{
					foreach (Hero hero2 in village.Settlement.Notables)
					{
						if (hero2.Issue != null)
						{
							this.GetIssueEffectOfHeroInternal(issueEffect, hero2, ref explainedNumber, DefaultIssueModel.RelatedSettlementIssuesText);
						}
					}
				}
			}
		}

		public override void GetIssueEffectOfHero(IssueEffect issueEffect, Hero hero, ref ExplainedNumber explainedNumber)
		{
			this.GetIssueEffectOfHeroInternal(issueEffect, hero, ref explainedNumber, DefaultIssueModel.HeroIssueText);
		}

		public override void GetIssueEffectOfClan(IssueEffect issueEffect, Clan clan, ref ExplainedNumber explainedNumber)
		{
			float num = 0f;
			foreach (Hero hero in clan.Lords)
			{
				if (hero.Issue != null)
				{
					IssueBase issue = hero.Issue;
					num += issue.GetActiveIssueEffectAmount(issueEffect);
				}
			}
			explainedNumber.Add(num, DefaultIssueModel.ClanIssuesText, null);
		}

		public override ValueTuple<int, int> GetCausalityForHero(Hero alternativeSolutionHero, IssueBase issue)
		{
			ValueTuple<SkillObject, int> issueAlternativeSolutionSkill = this.GetIssueAlternativeSolutionSkill(alternativeSolutionHero, issue);
			int skillValue = alternativeSolutionHero.GetSkillValue(issueAlternativeSolutionSkill.Item1);
			float num = 0.8f;
			if (skillValue != 0)
			{
				num = (float)(issueAlternativeSolutionSkill.Item2 / skillValue) * 0.1f;
			}
			num = MBMath.ClampFloat(num, 0.2f, 0.8f);
			int num2 = MathF.Ceiling((float)issue.GetTotalAlternativeSolutionNeededMenCount() * num);
			return new ValueTuple<int, int>(MBMath.ClampInt(2 * (num2 / 3), 1, num2), num2);
		}

		public override float GetFailureRiskForHero(Hero alternativeSolutionHero, IssueBase issue)
		{
			ValueTuple<SkillObject, int> issueAlternativeSolutionSkill = this.GetIssueAlternativeSolutionSkill(alternativeSolutionHero, issue);
			return MBMath.ClampFloat((float)(issueAlternativeSolutionSkill.Item2 - alternativeSolutionHero.GetSkillValue(issueAlternativeSolutionSkill.Item1)) * 0.5f / 100f, 0f, 0.9f);
		}

		public override CampaignTime GetDurationOfResolutionForHero(Hero alternativeSolutionHero, IssueBase issue)
		{
			ValueTuple<SkillObject, int> issueAlternativeSolutionSkill = this.GetIssueAlternativeSolutionSkill(alternativeSolutionHero, issue);
			int skillValue = alternativeSolutionHero.GetSkillValue(issueAlternativeSolutionSkill.Item1);
			float num = 10f;
			if (skillValue != 0)
			{
				num = MBMath.ClampFloat((float)(issueAlternativeSolutionSkill.Item2 / skillValue), 0f, 10f);
			}
			return CampaignTime.Days((float)issue.GetBaseAlternativeSolutionDurationInDays() + 2f * num);
		}

		public override int GetTroopsRequiredForHero(Hero alternativeSolutionHero, IssueBase issue)
		{
			ValueTuple<SkillObject, int> issueAlternativeSolutionSkill = this.GetIssueAlternativeSolutionSkill(alternativeSolutionHero, issue);
			int skillValue = alternativeSolutionHero.GetSkillValue(issueAlternativeSolutionSkill.Item1);
			float num = 1.2f;
			if (skillValue != 0)
			{
				num = (float)issueAlternativeSolutionSkill.Item2 / (float)skillValue;
			}
			num = MBMath.ClampFloat(num, 0.2f, 1.2f);
			return (int)((float)issue.AlternativeSolutionBaseNeededMenCount * num);
		}

		public override ValueTuple<SkillObject, int> GetIssueAlternativeSolutionSkill(Hero hero, IssueBase issue)
		{
			return issue.GetAlternativeSolutionSkill(hero);
		}

		private void GetIssueEffectOfHeroInternal(IssueEffect issueEffect, Hero hero, ref ExplainedNumber explainedNumber, TextObject customText)
		{
			float activeIssueEffectAmount = hero.Issue.GetActiveIssueEffectAmount(issueEffect);
			if (activeIssueEffectAmount != 0f)
			{
				explainedNumber.Add(activeIssueEffectAmount, customText, null);
			}
		}

		private static readonly TextObject SettlementIssuesText = new TextObject("{=EQLgVYk0}Settlement Issues", null);

		private static readonly TextObject HeroIssueText = GameTexts.FindText("str_issues", null);

		private static readonly TextObject RelatedSettlementIssuesText = new TextObject("{=umNyHc3A}Bound Village Issues", null);

		private static readonly TextObject ClanIssuesText = new TextObject("{=jdl8G8JS}Clan Issues", null);
	}
}
