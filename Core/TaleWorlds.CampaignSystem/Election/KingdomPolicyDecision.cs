﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public class KingdomPolicyDecision : KingdomDecision
	{
		internal static void AutoGeneratedStaticCollectObjectsKingdomPolicyDecision(object o, List<object> collectedObjects)
		{
			((KingdomPolicyDecision)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Policy);
			collectedObjects.Add(this._kingdomPolicies);
		}

		internal static object AutoGeneratedGetMemberValuePolicy(object o)
		{
			return ((KingdomPolicyDecision)o).Policy;
		}

		internal static object AutoGeneratedGetMemberValue_isInvertedDecision(object o)
		{
			return ((KingdomPolicyDecision)o)._isInvertedDecision;
		}

		internal static object AutoGeneratedGetMemberValue_kingdomPolicies(object o)
		{
			return ((KingdomPolicyDecision)o)._kingdomPolicies;
		}

		public KingdomPolicyDecision(Clan proposerClan, PolicyObject policy, bool isInvertedDecision = false)
			: base(proposerClan)
		{
			this.Policy = policy;
			this._isInvertedDecision = isInvertedDecision;
			this._kingdomPolicies = new List<PolicyObject>(base.Kingdom.ActivePolicies);
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsPolicyDecisionAllowed(this.Policy);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfPolicyProposalAndDisavowal();
		}

		public override TextObject GetGeneralTitle()
		{
			return this.Policy.Name;
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject;
			if (this._isInvertedDecision)
			{
				textObject = new TextObject("{=XGcST2dB}Vote to disavow {POLICY_NAME}", null);
			}
			else
			{
				textObject = new TextObject("{=iiH5gKzE}Vote for {POLICY_NAME}", null);
			}
			textObject.SetTextVariable("POLICY_NAME", this.Policy.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject;
			if (this._isInvertedDecision)
			{
				textObject = new TextObject("{=9DaDtQbo}Disavow {POLICY_NAME}", null);
			}
			else
			{
				textObject = new TextObject("{=!}{POLICY_NAME}", null);
			}
			textObject.SetTextVariable("POLICY_NAME", this.Policy.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject;
			if (this._isInvertedDecision)
			{
				textObject = new TextObject("{=ZPv3uCOb}{FACTION_LEADER} proposes disavowing the policy of {POLICY_NAME}. You can pick your stance regarding this decision.", null);
			}
			else
			{
				textObject = new TextObject("{=jFOva44m}{FACTION_LEADER} proposes the policy of {POLICY_NAME}. You can pick your stance regarding this decision.", null);
			}
			textObject.SetTextVariable("FACTION_LEADER", this.DetermineChooser().Leader.Name);
			textObject.SetTextVariable("POLICY_NAME", this.Policy.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject;
			if (this._isInvertedDecision)
			{
				textObject = new TextObject("{=1AU2jnNV}As {?IS_FEMALE}queen{?}king{\\?} you must decide whether to disavow the policy of {POLICY_NAME}.", null);
			}
			else
			{
				textObject = new TextObject("{=0EqPRs21}As {?IS_FEMALE}queen{?}king{\\?} you must decide whether to enforce the policy of {POLICY_NAME}.", null);
			}
			textObject.SetTextVariable("IS_FEMALE", this.DetermineChooser().Leader.IsFemale ? 1 : 0);
			textObject.SetTextVariable("POLICY_NAME", this.Policy.Name);
			return textObject;
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			yield return new KingdomPolicyDecision.PolicyDecisionOutcome(true);
			yield return new KingdomPolicyDecision.PolicyDecisionOutcome(false);
			yield break;
		}

		public override Clan DetermineChooser()
		{
			return base.Kingdom.RulingClan;
		}

		public float CalculateSupport(Clan clan)
		{
			return this.DetermineSupport(clan, new KingdomPolicyDecision.PolicyDecisionOutcome(true));
		}

		protected override bool ShouldBeCancelledInternal()
		{
			if (!this._isInvertedDecision)
			{
				return base.Kingdom.ActivePolicies.Contains(this.Policy);
			}
			return !base.Kingdom.ActivePolicies.Contains(this.Policy);
		}

		public override float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome)
		{
			KingdomPolicyDecision.PolicyDecisionOutcome policyDecisionOutcome = possibleOutcome as KingdomPolicyDecision.PolicyDecisionOutcome;
			float num = 0.1f;
			float num2 = 0.1f;
			float num3 = 0.1f;
			if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
			{
				num2 += 1f;
				num3 -= 1.5f;
				num -= 0.4f;
			}
			else if (clan.IsMinorFaction)
			{
				num += 1f;
				num3 -= 1.5f;
				num2 -= 0.3f;
			}
			else if (clan.Tier >= 3)
			{
				num -= 1.3f;
				num3 += (float)clan.Tier * 0.2f;
				num2 -= 1.3f;
			}
			else if (clan.Tier == 2)
			{
				num2 -= 0.1f;
				num3 += 0.4f;
				num -= 0.5f;
			}
			CultureObject culture = clan.Culture;
			num += 0.6f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Egalitarian) - 0.9f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic);
			num3 += 0.6f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic) - 0.9f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Egalitarian) - 0.5f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian);
			num2 += 0.8f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian) - 1.3f * (float)clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic);
			float num4 = this.Policy.EgalitarianWeight * num + this.Policy.OligarchicWeight * num3 + this.Policy.AuthoritarianWeight * num2;
			if (this._isInvertedDecision)
			{
				num4 = -num4;
			}
			float num5;
			if (policyDecisionOutcome.ShouldDecisionBeEnforced)
			{
				num5 = 60f;
			}
			else
			{
				num5 = -100f;
			}
			return num4 * num5;
		}

		public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				if (((KingdomPolicyDecision.PolicyDecisionOutcome)decisionOutcome).ShouldDecisionBeEnforced)
				{
					decisionOutcome.SetSponsor(base.ProposerClan);
				}
				else
				{
					base.AssignDefaultSponsor(decisionOutcome);
				}
			}
		}

		public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
		{
			bool shouldDecisionBeEnforced = ((KingdomPolicyDecision.PolicyDecisionOutcome)chosenOutcome).ShouldDecisionBeEnforced;
			if (shouldDecisionBeEnforced && !this._isInvertedDecision)
			{
				base.Kingdom.AddPolicy(this.Policy);
				return;
			}
			if (shouldDecisionBeEnforced && this._isInvertedDecision)
			{
				base.Kingdom.RemovePolicy(this.Policy);
			}
		}

		public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false)
		{
			TextObject textObject;
			if ((((KingdomPolicyDecision.PolicyDecisionOutcome)chosenOutcome).ShouldDecisionBeEnforced && !this._isInvertedDecision) || (!((KingdomPolicyDecision.PolicyDecisionOutcome)chosenOutcome).ShouldDecisionBeEnforced && this._isInvertedDecision))
			{
				if (!this._kingdomPolicies.Contains(this.Policy))
				{
					textObject = new TextObject("{=Lbs2bNlg}The {KINGDOM} will start {POLICY_DESCRIPTION} ({POLICY}). {POLICY_SUPPORT}", null);
				}
				else
				{
					textObject = new TextObject("{=jnYwiCAz}The {KINGDOM} will continue {POLICY_DESCRIPTION} ({POLICY}). {POLICY_SUPPORT}", null);
				}
			}
			else if (this._kingdomPolicies.Contains(this.Policy))
			{
				textObject = new TextObject("{=2BVDp7Tg}The {KINGDOM} will stop {POLICY_DESCRIPTION} ({POLICY}). {POLICY_SUPPORT}", null);
			}
			else
			{
				textObject = new TextObject("{=faUooB7V}The {KINGDOM} will not be {POLICY_DESCRIPTION} ({POLICY}). {POLICY_SUPPORT}", null);
			}
			textObject.SetTextVariable("KINGDOM", base.Kingdom.InformalName);
			textObject.SetTextVariable("POLICY", this.Policy.Name);
			textObject.SetTextVariable("POLICY_DESCRIPTION", this.Policy.LogEntryDescription);
			if (isShortVersion || base.IsSingleClanDecision())
			{
				textObject.SetTextVariable("POLICY_SUPPORT", TextObject.Empty);
			}
			else
			{
				textObject.SetTextVariable("POLICY_SUPPORT", "{=bqEO389P}This decision caused a split in the council.");
				if (supportStatus == KingdomDecision.SupportStatus.Majority)
				{
					textObject.SetTextVariable("POLICY_SUPPORT", "{=3W67kdtc}This decision had the support of the council.");
				}
				if (supportStatus == KingdomDecision.SupportStatus.Minority)
				{
					textObject.SetTextVariable("POLICY_SUPPORT", "{=b6MgRYlM}This decision was opposed by most of the council.");
				}
			}
			return textObject;
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.FirstOrDefault((DecisionOutcome t) => ((KingdomPolicyDecision.PolicyDecisionOutcome)t).ShouldDecisionBeEnforced);
		}

		public override TextObject GetSecondaryEffects()
		{
			return this.Policy.SecondaryEffects;
		}

		public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
		}

		[SaveableField(200)]
		public readonly PolicyObject Policy;

		[SaveableField(203)]
		private bool _isInvertedDecision;

		[SaveableField(202)]
		private List<PolicyObject> _kingdomPolicies;

		public class PolicyDecisionOutcome : DecisionOutcome
		{
			internal static void AutoGeneratedStaticCollectObjectsPolicyDecisionOutcome(object o, List<object> collectedObjects)
			{
				((KingdomPolicyDecision.PolicyDecisionOutcome)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValueShouldDecisionBeEnforced(object o)
			{
				return ((KingdomPolicyDecision.PolicyDecisionOutcome)o).ShouldDecisionBeEnforced;
			}

			[SaveableProperty(200)]
			public bool ShouldDecisionBeEnforced { get; private set; }

			public override TextObject GetDecisionTitle()
			{
				TextObject textObject = new TextObject("{=kakxnaN5}{?SUPPORT}Yes{?}No{\\?}", null);
				textObject.SetTextVariable("SUPPORT", this.ShouldDecisionBeEnforced ? 1 : 0);
				return textObject;
			}

			public override TextObject GetDecisionDescription()
			{
				if (this.ShouldDecisionBeEnforced)
				{
					return new TextObject("{=pWyxaauF}We support this proposal", null);
				}
				return new TextObject("{=BktSNgY4}We oppose this proposal", null);
			}

			public override string GetDecisionLink()
			{
				return null;
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return null;
			}

			public PolicyDecisionOutcome(bool shouldBeEnforced)
			{
				this.ShouldDecisionBeEnforced = shouldBeEnforced;
			}
		}
	}
}