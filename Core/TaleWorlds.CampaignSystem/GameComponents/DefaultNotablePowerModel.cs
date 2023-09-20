using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultNotablePowerModel : NotablePowerModel
	{
		public override int NotableDisappearPowerLimit
		{
			get
			{
				return 100;
			}
		}

		public override ExplainedNumber CalculateDailyPowerChangeForHero(Hero hero, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			if (!hero.IsActive)
			{
				return explainedNumber;
			}
			if (hero.Power > (float)this.RegularNotableMaxPowerLevel)
			{
				this.CalculateDailyPowerChangeForInfluentialNotables(hero, ref explainedNumber);
			}
			this.CalculateDailyPowerChangePerPropertyOwned(hero, ref explainedNumber);
			if (hero.Issue != null)
			{
				this.CalculatePowerChangeFromIssues(hero, ref explainedNumber);
			}
			if (hero.IsArtisan)
			{
				explainedNumber.Add(-0.1f, this._propertyEffect, null);
			}
			if (hero.IsGangLeader)
			{
				explainedNumber.Add(-0.4f, this._propertyEffect, null);
			}
			if (hero.IsRuralNotable)
			{
				explainedNumber.Add(0.1f, this._propertyEffect, null);
			}
			if (hero.IsHeadman)
			{
				explainedNumber.Add(0.1f, this._propertyEffect, null);
			}
			if (hero.IsMerchant)
			{
				explainedNumber.Add(0.2f, this._propertyEffect, null);
			}
			if (hero.CurrentSettlement != null)
			{
				if (hero.CurrentSettlement.IsVillage && hero.CurrentSettlement.Village.Bound.IsCastle)
				{
					explainedNumber.Add(0.1f, this._propertyEffect, null);
				}
				if (hero.SupporterOf == hero.CurrentSettlement.OwnerClan)
				{
					this.CalculateDailyPowerChangeForAffiliationWithRulerClan(ref explainedNumber);
				}
			}
			return explainedNumber;
		}

		public override int RegularNotableMaxPowerLevel
		{
			get
			{
				return this.NotablePowerRanks[1].MinPowerValue;
			}
		}

		private void CalculateDailyPowerChangePerPropertyOwned(Hero hero, ref ExplainedNumber explainedNumber)
		{
			float num = 0.1f;
			int count = hero.OwnedAlleys.Count;
			explainedNumber.Add(num * (float)count, this._propertyEffect, null);
		}

		private void CalculateDailyPowerChangeForAffiliationWithRulerClan(ref ExplainedNumber explainedNumber)
		{
			float num = 0.2f;
			explainedNumber.Add(num, this._rulerClanEffect, null);
		}

		private void CalculateDailyPowerChangeForInfluentialNotables(Hero hero, ref ExplainedNumber explainedNumber)
		{
			float num = -1f * ((hero.Power - (float)this.RegularNotableMaxPowerLevel) / 500f);
			explainedNumber.Add(num, this._currentRankEffect, null);
		}

		private void CalculatePowerChangeFromIssues(Hero hero, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectOfHero(DefaultIssueEffects.IssueOwnerPower, hero, ref explainedNumber);
		}

		public override TextObject GetPowerRankName(Hero hero)
		{
			return this.GetPowerRank(hero).Name;
		}

		public override float GetInfluenceBonusToClan(Hero hero)
		{
			return this.GetPowerRank(hero).InfluenceBonus;
		}

		private DefaultNotablePowerModel.NotablePowerRank GetPowerRank(Hero hero)
		{
			int num = 0;
			for (int i = 0; i < this.NotablePowerRanks.Length; i++)
			{
				if (hero.Power > (float)this.NotablePowerRanks[i].MinPowerValue)
				{
					num = i;
				}
			}
			return this.NotablePowerRanks[num];
		}

		public override int GetInitialPower()
		{
			float randomFloat = MBRandom.RandomFloat;
			if (randomFloat < 0.2f)
			{
				return MBRandom.RandomInt((int)((float)(this.NotablePowerRanks[0].MinPowerValue + this.NotablePowerRanks[1].MinPowerValue) * 0.5f), this.NotablePowerRanks[1].MinPowerValue);
			}
			if (randomFloat >= 0.8f)
			{
				return MBRandom.RandomInt(this.NotablePowerRanks[2].MinPowerValue, (int)((float)this.NotablePowerRanks[2].MinPowerValue * 2f));
			}
			return MBRandom.RandomInt(this.NotablePowerRanks[1].MinPowerValue, this.NotablePowerRanks[2].MinPowerValue);
		}

		private DefaultNotablePowerModel.NotablePowerRank[] NotablePowerRanks = new DefaultNotablePowerModel.NotablePowerRank[]
		{
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=aTeuX4L0}Regular", null), 0, 0.05f),
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=nTETQEmy}Influential", null), 100, 0.1f),
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=UCpyo9hw}Powerful", null), 200, 0.15f)
		};

		private TextObject _currentRankEffect = new TextObject("{=7j9uHxLM}Current Rank Effect", null);

		private TextObject _militiaEffect = new TextObject("{=R1MaIgOb}Militia Effect", null);

		private TextObject _rulerClanEffect = new TextObject("{=JE3RTqx5}Ruler Clan Effect", null);

		private TextObject _propertyEffect = new TextObject("{=yDomN9L2}Property Effect", null);

		private struct NotablePowerRank
		{
			public NotablePowerRank(TextObject name, int minPowerValue, float influenceBonus)
			{
				this.Name = name;
				this.MinPowerValue = minPowerValue;
				this.InfluenceBonus = influenceBonus;
			}

			public readonly TextObject Name;

			public readonly int MinPowerValue;

			public readonly float InfluenceBonus;
		}
	}
}
