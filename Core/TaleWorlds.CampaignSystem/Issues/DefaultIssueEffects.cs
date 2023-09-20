using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x02000322 RID: 802
	public class DefaultIssueEffects
	{
		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x06002D9A RID: 11674 RVA: 0x000BEDC4 File Offset: 0x000BCFC4
		private static DefaultIssueEffects Instance
		{
			get
			{
				return Campaign.Current.DefaultIssueEffects;
			}
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x06002D9B RID: 11675 RVA: 0x000BEDD0 File Offset: 0x000BCFD0
		public static IssueEffect SettlementLoyalty
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementLoyalty;
			}
		}

		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x06002D9C RID: 11676 RVA: 0x000BEDDC File Offset: 0x000BCFDC
		public static IssueEffect SettlementSecurity
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementSecurity;
			}
		}

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x06002D9D RID: 11677 RVA: 0x000BEDE8 File Offset: 0x000BCFE8
		public static IssueEffect SettlementMilitia
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementMilitia;
			}
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06002D9E RID: 11678 RVA: 0x000BEDF4 File Offset: 0x000BCFF4
		public static IssueEffect SettlementProsperity
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementProsperity;
			}
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x06002D9F RID: 11679 RVA: 0x000BEE00 File Offset: 0x000BD000
		public static IssueEffect VillageHearth
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectVillageHearth;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06002DA0 RID: 11680 RVA: 0x000BEE0C File Offset: 0x000BD00C
		public static IssueEffect SettlementFood
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementFood;
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x06002DA1 RID: 11681 RVA: 0x000BEE18 File Offset: 0x000BD018
		public static IssueEffect SettlementTax
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementTax;
			}
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06002DA2 RID: 11682 RVA: 0x000BEE24 File Offset: 0x000BD024
		public static IssueEffect SettlementGarrison
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementGarrison;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06002DA3 RID: 11683 RVA: 0x000BEE30 File Offset: 0x000BD030
		public static IssueEffect HalfVillageProduction
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectHalfVillageProduction;
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06002DA4 RID: 11684 RVA: 0x000BEE3C File Offset: 0x000BD03C
		public static IssueEffect IssueOwnerPower
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectIssueOwnerPower;
			}
		}

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06002DA5 RID: 11685 RVA: 0x000BEE48 File Offset: 0x000BD048
		public static IssueEffect ClanInfluence
		{
			get
			{
				return DefaultIssueEffects.Instance._clanInfluence;
			}
		}

		// Token: 0x06002DA6 RID: 11686 RVA: 0x000BEE54 File Offset: 0x000BD054
		public DefaultIssueEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x06002DA7 RID: 11687 RVA: 0x000BEE64 File Offset: 0x000BD064
		private void RegisterAll()
		{
			this._issueEffectSettlementLoyalty = this.Create("issue_effect_settlement_loyalty");
			this._issueEffectSettlementSecurity = this.Create("issue_effect_settlement_security");
			this._issueEffectSettlementMilitia = this.Create("issue_effect_settlement_militia");
			this._issueEffectSettlementProsperity = this.Create("issue_effect_settlement_prosperity");
			this._issueEffectVillageHearth = this.Create("issue_effect_village_hearth");
			this._issueEffectSettlementFood = this.Create("issue_effect_settlement_food");
			this._issueEffectSettlementTax = this.Create("issue_effect_settlement_tax");
			this._issueEffectSettlementGarrison = this.Create("issue_effect_settlement_garrison");
			this._issueEffectHalfVillageProduction = this.Create("issue_effect_half_village_production");
			this._issueEffectIssueOwnerPower = this.Create("issue_effect_issue_owner_power");
			this._clanInfluence = this.Create("issue_effect_clan_influence");
			this.InitializeAll();
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x000BEF32 File Offset: 0x000BD132
		private IssueEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<IssueEffect>(new IssueEffect(stringId));
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x000BEF4C File Offset: 0x000BD14C
		private void InitializeAll()
		{
			this._issueEffectSettlementLoyalty.Initialize(new TextObject("{=YO0x7ZAo}Loyalty", null), new TextObject("{=xAWvm25T}Effects settlement's loyalty.", null));
			this._issueEffectSettlementSecurity.Initialize(new TextObject("{=MqCH7R4A}Security", null), new TextObject("{=h117Qj3E}Effects settlement's security.", null));
			this._issueEffectSettlementMilitia.Initialize(new TextObject("{=gsVtO9A7}Militia", null), new TextObject("{=dTmPV82D}Effects settlement's militia.", null));
			this._issueEffectSettlementProsperity.Initialize(new TextObject("{=IagYTD5O}Prosperity", null), new TextObject("{=ETye0JMY}Effects settlement's prosperity.", null));
			this._issueEffectVillageHearth.Initialize(new TextObject("{=f5X5uU0m}Village Hearth", null), new TextObject("{=7TbVhbT9}Effects village's hearth.", null));
			this._issueEffectSettlementFood.Initialize(new TextObject("{=qSi4DlT4}Food", null), new TextObject("{=onDsUkUl}Effects settlement's food.", null));
			this._issueEffectSettlementTax.Initialize(new TextObject("{=2awf1tei}Tax", null), new TextObject("{=q2Ovtr1s}Effects settlement's tax.", null));
			this._issueEffectSettlementGarrison.Initialize(new TextObject("{=jlgjLDo7}Garrison", null), new TextObject("{=WJ7SnBgN}Effects settlement's garrison.", null));
			this._issueEffectHalfVillageProduction.Initialize(new TextObject("{=bGyrPe8c}Production", null), new TextObject("{=arbaXvQf}Effects village's production.", null));
			this._issueEffectIssueOwnerPower.Initialize(new TextObject("{=gGXelWQX}Issue owner power", null), new TextObject("{=tjudHtDB}Effects the power of issue owner in the settlement.", null));
			this._clanInfluence.Initialize(new TextObject("{=KN6khbSl}Clan Influence", null), new TextObject("{=y2aLOwOs}Effects the influence of clan.", null));
		}

		// Token: 0x04000DB2 RID: 3506
		private IssueEffect _issueEffectSettlementGarrison;

		// Token: 0x04000DB3 RID: 3507
		private IssueEffect _issueEffectSettlementLoyalty;

		// Token: 0x04000DB4 RID: 3508
		private IssueEffect _issueEffectSettlementSecurity;

		// Token: 0x04000DB5 RID: 3509
		private IssueEffect _issueEffectSettlementMilitia;

		// Token: 0x04000DB6 RID: 3510
		private IssueEffect _issueEffectSettlementProsperity;

		// Token: 0x04000DB7 RID: 3511
		private IssueEffect _issueEffectVillageHearth;

		// Token: 0x04000DB8 RID: 3512
		private IssueEffect _issueEffectSettlementFood;

		// Token: 0x04000DB9 RID: 3513
		private IssueEffect _issueEffectSettlementTax;

		// Token: 0x04000DBA RID: 3514
		private IssueEffect _issueEffectHalfVillageProduction;

		// Token: 0x04000DBB RID: 3515
		private IssueEffect _issueEffectIssueOwnerPower;

		// Token: 0x04000DBC RID: 3516
		private IssueEffect _clanInfluence;
	}
}
