using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class DefaultIssueEffects
	{
		private static DefaultIssueEffects Instance
		{
			get
			{
				return Campaign.Current.DefaultIssueEffects;
			}
		}

		public static IssueEffect SettlementLoyalty
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementLoyalty;
			}
		}

		public static IssueEffect SettlementSecurity
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementSecurity;
			}
		}

		public static IssueEffect SettlementMilitia
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementMilitia;
			}
		}

		public static IssueEffect SettlementProsperity
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementProsperity;
			}
		}

		public static IssueEffect VillageHearth
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectVillageHearth;
			}
		}

		public static IssueEffect SettlementFood
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementFood;
			}
		}

		public static IssueEffect SettlementTax
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementTax;
			}
		}

		public static IssueEffect SettlementGarrison
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectSettlementGarrison;
			}
		}

		public static IssueEffect HalfVillageProduction
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectHalfVillageProduction;
			}
		}

		public static IssueEffect IssueOwnerPower
		{
			get
			{
				return DefaultIssueEffects.Instance._issueEffectIssueOwnerPower;
			}
		}

		public static IssueEffect ClanInfluence
		{
			get
			{
				return DefaultIssueEffects.Instance._clanInfluence;
			}
		}

		public DefaultIssueEffects()
		{
			this.RegisterAll();
		}

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

		private IssueEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<IssueEffect>(new IssueEffect(stringId));
		}

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

		private IssueEffect _issueEffectSettlementGarrison;

		private IssueEffect _issueEffectSettlementLoyalty;

		private IssueEffect _issueEffectSettlementSecurity;

		private IssueEffect _issueEffectSettlementMilitia;

		private IssueEffect _issueEffectSettlementProsperity;

		private IssueEffect _issueEffectVillageHearth;

		private IssueEffect _issueEffectSettlementFood;

		private IssueEffect _issueEffectSettlementTax;

		private IssueEffect _issueEffectHalfVillageProduction;

		private IssueEffect _issueEffectIssueOwnerPower;

		private IssueEffect _clanInfluence;
	}
}
