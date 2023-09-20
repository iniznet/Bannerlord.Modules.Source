﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public class SettlementClaimantPreliminaryDecision : KingdomDecision
	{
		internal static void AutoGeneratedStaticCollectObjectsSettlementClaimantPreliminaryDecision(object o, List<object> collectedObjects)
		{
			((SettlementClaimantPreliminaryDecision)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Settlement);
			collectedObjects.Add(this._ownerClan);
		}

		internal static object AutoGeneratedGetMemberValueSettlement(object o)
		{
			return ((SettlementClaimantPreliminaryDecision)o).Settlement;
		}

		internal static object AutoGeneratedGetMemberValue_ownerClan(object o)
		{
			return ((SettlementClaimantPreliminaryDecision)o)._ownerClan;
		}

		public SettlementClaimantPreliminaryDecision(Clan proposerClan, Settlement settlement)
			: base(proposerClan)
		{
			this.Settlement = settlement;
			this._ownerClan = settlement.OwnerClan;
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsAnnexationDecisionAllowed(this.Settlement);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAnnexation(base.ProposerClan);
		}

		public override TextObject GetGeneralTitle()
		{
			TextObject textObject = new TextObject("{=XFr4IfXf} Change Owner of {SETTLEMENT}", null);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject = new TextObject("{=a48xlNUb}Should the owner of {SETTLEMENT} change?", null);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject = new TextObject("{=a48xlNUb}Should the owner of {SETTLEMENT} change?", null);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject = new TextObject("{=hBJbnoDn}{FACTION_LEADER} will decide if the owner of {SETTLEMENT} will change.", null);
			textObject.SetTextVariable("FACTION_LEADER", this.DetermineChooser().Leader.Name);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject = new TextObject("{=JHR4ySCf}As {?IS_FEMALE}queen{?}king{\\?} you must decide if the owner of {SETTLEMENT} should change.", null);
			textObject.SetTextVariable("IS_FEMALE", this.DetermineChooser().Leader.IsFemale ? 1 : 0);
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			return textObject;
		}

		public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
		{
			float num = 0f;
			float num2 = 50f;
			List<Clan> list = new List<Clan>();
			if (this._ownerClan.Kingdom != null)
			{
				list.AddRange(this._ownerClan.Kingdom.Clans.ToList<Clan>());
			}
			else
			{
				list.Add(this._ownerClan);
			}
			foreach (Clan clan in list)
			{
				int relation = clan.Leader.GetRelation(this._ownerClan.Leader);
				bool shouldSettlementOwnerChange = ((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)candidateOutcome).ShouldSettlementOwnerChange;
				if ((!shouldSettlementOwnerChange && relation > 0) || (shouldSettlementOwnerChange && relation <= 0))
				{
					num += num2 * (float)MathF.Abs(relation);
				}
			}
			return num;
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			yield return new SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome(true);
			yield return new SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome(false);
			yield break;
		}

		public override Clan DetermineChooser()
		{
			return ((Kingdom)this.Settlement.MapFaction).RulingClan;
		}

		protected override bool ShouldBeCancelledInternal()
		{
			return this.Settlement.MapFaction != base.Kingdom;
		}

		public float CalculateSupport(Clan clan)
		{
			return this.DetermineSupport(clan, new SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome(true));
		}

		public override float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome)
		{
			bool shouldSettlementOwnerChange = ((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)possibleOutcome).ShouldSettlementOwnerChange;
			int num = clan.GetRelationWithClan(this._ownerClan);
			if (clan == this._ownerClan)
			{
				num = 300;
			}
			float num2 = 20f;
			if (this.Settlement.OwnerClan == clan)
			{
				num2 *= 20f;
			}
			else
			{
				num2 += (float)num * 0.7f;
				float totalStrength = this.Settlement.OwnerClan.TotalStrength;
				num2 += totalStrength * 0.01f;
			}
			float num3 = 0f;
			if (shouldSettlementOwnerChange)
			{
				num3 = -num2;
			}
			else if (!shouldSettlementOwnerChange)
			{
				num3 = num2;
			}
			return num3;
		}

		public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				if (((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)decisionOutcome).ShouldSettlementOwnerChange)
				{
					decisionOutcome.SetSponsor(base.ProposerClan);
				}
				else
				{
					base.AssignDefaultSponsor(decisionOutcome);
				}
			}
		}

		protected override int GetInfluenceCostOfSupportInternal(Supporter.SupportWeights supportWeight)
		{
			if (this.Settlement.IsTown)
			{
				if (supportWeight == Supporter.SupportWeights.SlightlyFavor)
				{
					return 20;
				}
				if (supportWeight == Supporter.SupportWeights.StronglyFavor)
				{
					return 60;
				}
				if (supportWeight != Supporter.SupportWeights.FullyPush)
				{
					return 0;
				}
				return 200;
			}
			else
			{
				if (supportWeight == Supporter.SupportWeights.SlightlyFavor)
				{
					return 15;
				}
				if (supportWeight == Supporter.SupportWeights.StronglyFavor)
				{
					return 50;
				}
				if (supportWeight != Supporter.SupportWeights.FullyPush)
				{
					return 0;
				}
				return 150;
			}
		}

		public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
		{
			if (((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)chosenOutcome).ShouldSettlementOwnerChange)
			{
				SettlementClaimantDecision settlementClaimantDecision = new SettlementClaimantDecision(base.ProposerClan, this.Settlement, null, this._ownerClan);
				settlementClaimantDecision.IsEnforced = true;
				base.ProposerClan.Kingdom.AddDecision(settlementClaimantDecision, true);
			}
		}

		public override TextObject GetSecondaryEffects()
		{
			return new TextObject("{=CsZfuPOe}Voting against or in favor of the current owner of the settlement will affect your relation with that clan accordingly.", null);
		}

		public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				bool shouldSettlementOwnerChange = ((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)decisionOutcome).ShouldSettlementOwnerChange;
				foreach (Supporter supporter in decisionOutcome.SupporterList)
				{
					if (supporter.Clan.Leader != this._ownerClan.Leader)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(supporter.Clan.Leader, this._ownerClan.Leader, Campaign.Current.Models.DiplomacyModel.GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(supporter.Clan.Leader, shouldSettlementOwnerChange), true);
					}
				}
			}
		}

		public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false)
		{
			TextObject textObject = TextObject.Empty;
			if (((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)chosenOutcome).ShouldSettlementOwnerChange)
			{
				if (supportStatus == KingdomDecision.SupportStatus.Majority)
				{
					textObject = new TextObject("{=Zo65bOpH}{RULER.NAME} has decided to give {SETTLEMENT} to a new clan with the support of with {?RULER.GENDER}her{?}his{\\?} council.", null);
				}
				else if (supportStatus == KingdomDecision.SupportStatus.Minority)
				{
					textObject = new TextObject("{=w3sfcpoa}{RULER.NAME} has decided to give {SETTLEMENT} to a new clan despite the opposition of {?RULER.GENDER}her{?}his{\\?} council", null);
				}
				else
				{
					textObject = new TextObject("{=JzALLCEX}{RULER.NAME} has decided to give {SETTLEMENT} to a new clan, with {?RULER.GENDER}her{?}his{\\?} council evenly split on the matter.", null);
				}
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Majority)
			{
				textObject = new TextObject("{=vkeHpUEB}{RULER.NAME} has decided against giving {SETTLEMENT} to a new clan with the support of with {?RULER.GENDER}her{?}his{\\?} council.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Minority)
			{
				textObject = new TextObject("{=9Cbeagow}{RULER.NAME} has decided against giving {SETTLEMENT} to a new clan over the objections of {?RULER.GENDER}her{?}his{\\?} council.", null);
			}
			else
			{
				textObject = new TextObject("{=fP8NHthR}{RULER.NAME} has decided against giving {SETTLEMENT} to a new clan, with {?RULER.GENDER}her{?}his{\\?} council evenly split on the matter.", null);
			}
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			textObject.SetTextVariable("KINGDOM", this.Settlement.MapFaction.InformalName);
			StringHelpers.SetCharacterProperties("RULER", this.Settlement.MapFaction.Leader.CharacterObject, textObject, false);
			return textObject;
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.FirstOrDefault((DecisionOutcome t) => ((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)t).ShouldSettlementOwnerChange);
		}

		public override KingdomDecision GetFollowUpDecision()
		{
			return base.Kingdom.UnresolvedDecisions.FirstOrDefault(delegate(KingdomDecision x)
			{
				SettlementClaimantDecision settlementClaimantDecision;
				return (settlementClaimantDecision = x as SettlementClaimantDecision) != null && settlementClaimantDecision.Settlement == this.Settlement;
			});
		}

		[SaveableField(400)]
		public readonly Settlement Settlement;

		[SaveableField(401)]
		private Clan _ownerClan;

		public class SettlementClaimantPreliminaryOutcome : DecisionOutcome
		{
			internal static void AutoGeneratedStaticCollectObjectsSettlementClaimantPreliminaryOutcome(object o, List<object> collectedObjects)
			{
				((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValueShouldSettlementOwnerChange(object o)
			{
				return ((SettlementClaimantPreliminaryDecision.SettlementClaimantPreliminaryOutcome)o).ShouldSettlementOwnerChange;
			}

			public override TextObject GetDecisionTitle()
			{
				TextObject textObject = new TextObject("{=kakxnaN5}{?SUPPORT}Yes{?}No{\\?}", null);
				textObject.SetTextVariable("SUPPORT", this.ShouldSettlementOwnerChange ? 1 : 0);
				return textObject;
			}

			public override TextObject GetDecisionDescription()
			{
				if (this.ShouldSettlementOwnerChange)
				{
					return new TextObject("{=1bbsq6uw}We should award this fief to a new clan", null);
				}
				return new TextObject("{=uBcEUdxu}Ownership of the fief should remain as it is", null);
			}

			public override string GetDecisionLink()
			{
				return null;
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return null;
			}

			public SettlementClaimantPreliminaryOutcome(bool shouldSettlementOwnerChange)
			{
				this.ShouldSettlementOwnerChange = shouldSettlementOwnerChange;
			}

			[SaveableField(400)]
			public bool ShouldSettlementOwnerChange;
		}
	}
}
