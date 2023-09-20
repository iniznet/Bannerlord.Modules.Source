using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200011E RID: 286
	public class DefaultNotablePowerModel : NotablePowerModel
	{
		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001641 RID: 5697 RVA: 0x0006A934 File Offset: 0x00068B34
		public override int NotableDisappearPowerLimit
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0006A938 File Offset: 0x00068B38
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

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001643 RID: 5699 RVA: 0x0006AA71 File Offset: 0x00068C71
		public override int RegularNotableMaxPowerLevel
		{
			get
			{
				return this.NotablePowerRanks[1].MinPowerValue;
			}
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0006AA84 File Offset: 0x00068C84
		private void CalculateDailyPowerChangePerPropertyOwned(Hero hero, ref ExplainedNumber explainedNumber)
		{
			float num = 0.1f;
			int count = hero.OwnedAlleys.Count;
			explainedNumber.Add(num * (float)count, this._propertyEffect, null);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0006AAB4 File Offset: 0x00068CB4
		private void CalculateDailyPowerChangeForAffiliationWithRulerClan(ref ExplainedNumber explainedNumber)
		{
			float num = 0.2f;
			explainedNumber.Add(num, this._rulerClanEffect, null);
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0006AAD8 File Offset: 0x00068CD8
		private void CalculateDailyPowerChangeForInfluentialNotables(Hero hero, ref ExplainedNumber explainedNumber)
		{
			float num = -1f * ((hero.Power - (float)this.RegularNotableMaxPowerLevel) / 500f);
			explainedNumber.Add(num, this._currentRankEffect, null);
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0006AB0E File Offset: 0x00068D0E
		private void CalculatePowerChangeFromIssues(Hero hero, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectOfHero(DefaultIssueEffects.IssueOwnerPower, hero, ref explainedNumber);
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0006AB2B File Offset: 0x00068D2B
		public override TextObject GetPowerRankName(Hero hero)
		{
			return this.GetPowerRank(hero).Name;
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0006AB39 File Offset: 0x00068D39
		public override float GetInfluenceBonusToClan(Hero hero)
		{
			return this.GetPowerRank(hero).InfluenceBonus;
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0006AB48 File Offset: 0x00068D48
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

		// Token: 0x0600164B RID: 5707 RVA: 0x0006AB94 File Offset: 0x00068D94
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

		// Token: 0x040007C1 RID: 1985
		private DefaultNotablePowerModel.NotablePowerRank[] NotablePowerRanks = new DefaultNotablePowerModel.NotablePowerRank[]
		{
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=aTeuX4L0}Regular", null), 0, 0.05f),
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=nTETQEmy}Influential", null), 100, 0.1f),
			new DefaultNotablePowerModel.NotablePowerRank(new TextObject("{=UCpyo9hw}Powerful", null), 200, 0.15f)
		};

		// Token: 0x040007C2 RID: 1986
		private TextObject _currentRankEffect = new TextObject("{=7j9uHxLM}Current Rank Effect", null);

		// Token: 0x040007C3 RID: 1987
		private TextObject _militiaEffect = new TextObject("{=R1MaIgOb}Militia Effect", null);

		// Token: 0x040007C4 RID: 1988
		private TextObject _rulerClanEffect = new TextObject("{=JE3RTqx5}Ruler Clan Effect", null);

		// Token: 0x040007C5 RID: 1989
		private TextObject _propertyEffect = new TextObject("{=yDomN9L2}Property Effect", null);

		// Token: 0x0200050D RID: 1293
		private struct NotablePowerRank
		{
			// Token: 0x0600423A RID: 16954 RVA: 0x00134DC8 File Offset: 0x00132FC8
			public NotablePowerRank(TextObject name, int minPowerValue, float influenceBonus)
			{
				this.Name = name;
				this.MinPowerValue = minPowerValue;
				this.InfluenceBonus = influenceBonus;
			}

			// Token: 0x040015AF RID: 5551
			public readonly TextObject Name;

			// Token: 0x040015B0 RID: 5552
			public readonly int MinPowerValue;

			// Token: 0x040015B1 RID: 5553
			public readonly float InfluenceBonus;
		}
	}
}
