﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public class ExpelClanFromKingdomDecision : KingdomDecision
	{
		internal static void AutoGeneratedStaticCollectObjectsExpelClanFromKingdomDecision(object o, List<object> collectedObjects)
		{
			((ExpelClanFromKingdomDecision)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.ClanToExpel);
			collectedObjects.Add(this.OldKingdom);
		}

		internal static object AutoGeneratedGetMemberValueClanToExpel(object o)
		{
			return ((ExpelClanFromKingdomDecision)o).ClanToExpel;
		}

		internal static object AutoGeneratedGetMemberValueOldKingdom(object o)
		{
			return ((ExpelClanFromKingdomDecision)o).OldKingdom;
		}

		public ExpelClanFromKingdomDecision(Clan proposerClan, Clan clan)
			: base(proposerClan)
		{
			this.ClanToExpel = clan;
			this.OldKingdom = clan.Kingdom;
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsExpulsionDecisionAllowed(this.ClanToExpel);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfExpellingClan();
		}

		public override TextObject GetGeneralTitle()
		{
			TextObject textObject = new TextObject("{=pF92DagG}Expel {CLAN_NAME} from {KINGDOM_NAME}", null);
			textObject.SetTextVariable("CLAN_NAME", this.ClanToExpel.Name);
			textObject.SetTextVariable("KINGDOM_NAME", this.OldKingdom.Name);
			return textObject;
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject = new TextObject("{=ZwpWX8Zx}Vote for expelling {CLAN_NAME} from the kingdom", null);
			textObject.SetTextVariable("CLAN_NAME", this.ClanToExpel.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject = new TextObject("{=pF92DagG}Expel {CLAN_NAME} from {KINGDOM_NAME}", null);
			textObject.SetTextVariable("CLAN_NAME", this.ClanToExpel.Name);
			textObject.SetTextVariable("KINGDOM_NAME", this.OldKingdom.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject = new TextObject("{=eTr0XHas}{FACTION_LEADER} will decide if {CLAN_NAME} will be expelled from {KINGDOM_NAME}. You can pick your stance regarding this decision.", null);
			textObject.SetTextVariable("FACTION_LEADER", this.DetermineChooser().Leader.Name);
			textObject.SetTextVariable("CLAN_NAME", this.ClanToExpel.Name);
			textObject.SetTextVariable("KINGDOM_NAME", this.OldKingdom.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject = new TextObject("{=J8brFxIW}As {?IS_FEMALE}queen{?}king{\\?} you must decide if {CLAN_NAME} will be expelled from kingdom.", null);
			textObject.SetTextVariable("IS_FEMALE", this.DetermineChooser().Leader.IsFemale ? 1 : 0);
			textObject.SetTextVariable("CLAN_NAME", this.ClanToExpel.Name);
			return textObject;
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			yield return new ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome(true);
			yield return new ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome(false);
			yield break;
		}

		public override Clan DetermineChooser()
		{
			return this.OldKingdom.RulingClan;
		}

		protected override bool ShouldBeCancelledInternal()
		{
			return !base.Kingdom.Clans.Contains(this.ClanToExpel);
		}

		public override float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome)
		{
			bool shouldBeExpelled = ((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)possibleOutcome).ShouldBeExpelled;
			float num = 3.5f;
			float num2 = (float)FactionManager.GetRelationBetweenClans(this.ClanToExpel, clan) * num;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 10000f;
			foreach (Settlement settlement in this.ClanToExpel.Settlements)
			{
				num3 += settlement.GetSettlementValueForFaction(this.OldKingdom) * 0.005f;
			}
			if (clan.Leader.GetTraitLevel(DefaultTraits.Calculating) > 0)
			{
				num5 = this.ClanToExpel.Influence * 0.05f + this.ClanToExpel.Renown * 0.02f;
			}
			if (clan.Leader.GetTraitLevel(DefaultTraits.Commander) > 0)
			{
				foreach (WarPartyComponent warPartyComponent in this.ClanToExpel.WarPartyComponents)
				{
					num4 += (float)warPartyComponent.MobileParty.MemberRoster.TotalManCount * 0.01f;
				}
			}
			float num7 = num6 + num2 + num3 + num4 + num5;
			float num8;
			if (shouldBeExpelled)
			{
				num8 = -num7;
			}
			else
			{
				num8 = num7;
			}
			return num8;
		}

		public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				if (((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)decisionOutcome).ShouldBeExpelled)
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
			if (((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)chosenOutcome).ShouldBeExpelled)
			{
				int relationCostOfExpellingClanFromKingdom = Campaign.Current.Models.DiplomacyModel.GetRelationCostOfExpellingClanFromKingdom();
				foreach (Supporter supporter in chosenOutcome.SupporterList)
				{
					if (((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)chosenOutcome).ShouldBeExpelled && this.ClanToExpel.Leader != supporter.Clan.Leader)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(this.ClanToExpel.Leader, supporter.Clan.Leader, relationCostOfExpellingClanFromKingdom, true);
					}
				}
				ChangeKingdomAction.ApplyByLeaveKingdom(this.ClanToExpel, true);
			}
		}

		public override TextObject GetSecondaryEffects()
		{
			return new TextObject("{=fJY9uosa}All supporters gain some relations with each other and lose a large amount of relations with the expelled clan.", null);
		}

		public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
		}

		public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false)
		{
			TextObject textObject;
			if (((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)chosenOutcome).ShouldBeExpelled)
			{
				if (base.IsSingleClanDecision())
				{
					textObject = new TextObject("{=h5eTEYON}{RULER.NAME} has expelled the {CLAN} clan from the {KINGDOM}.", null);
				}
				else if (supportStatus == KingdomDecision.SupportStatus.Majority)
				{
					textObject = new TextObject("{=rd229FYG}{RULER.NAME} has expelled the {CLAN} clan from the {KINGDOM} with the support of {?RULER.GENDER}her{?}his{\\?} council.", null);
				}
				else if (supportStatus == KingdomDecision.SupportStatus.Minority)
				{
					textObject = new TextObject("{=G3qGLAeQ}{RULER.NAME} has expelled the {CLAN} clan from the {KINGDOM} against the wishes of {?RULER.GENDER}her{?}his{\\?} council.", null);
				}
				else
				{
					textObject = new TextObject("{=m6OVl6Dg}{RULER.NAME} has expelled the {CLAN} clan from the {KINGDOM}, with {?RULER.GENDER}her{?}his{\\?} council evenly split on the matter.", null);
				}
			}
			else if (base.IsSingleClanDecision())
			{
				textObject = new TextObject("{=mvkKP6OE}{RULER.NAME} chose not to expel the {CLAN} clan from the {KINGDOM}.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Majority)
			{
				textObject = new TextObject("{=yBL3TzXw}{RULER.NAME} chose not to expel the {CLAN} clan from the {KINGDOM} with the support of {?RULER.GENDER}her{?}his{\\?} council.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Minority)
			{
				textObject = new TextObject("{=940TwBPs}{RULER.NAME} chose not to expel the {CLAN} clan from the {KINGDOM} over the objections of {?RULER.GENDER}her{?}his{\\?} council.", null);
			}
			else
			{
				textObject = new TextObject("{=Oe1NdVLe}{RULER.NAME} chose not to expel the {CLAN} clan from the {KINGDOM} with {?RULER.GENDER}her{?}his{\\?} council evenly split on the matter.", null);
			}
			textObject.SetTextVariable("CLAN", this.ClanToExpel.Name);
			textObject.SetTextVariable("KINGDOM", this.OldKingdom.Name);
			StringHelpers.SetCharacterProperties("RULER", this.OldKingdom.Leader.CharacterObject, textObject, false);
			return textObject;
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.FirstOrDefault((DecisionOutcome t) => ((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)t).ShouldBeExpelled);
		}

		private const float ClanFiefModifier = 0.005f;

		[SaveableField(100)]
		public readonly Clan ClanToExpel;

		[SaveableField(102)]
		public readonly Kingdom OldKingdom;

		public class ExpelClanDecisionOutcome : DecisionOutcome
		{
			internal static void AutoGeneratedStaticCollectObjectsExpelClanDecisionOutcome(object o, List<object> collectedObjects)
			{
				((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValueShouldBeExpelled(object o)
			{
				return ((ExpelClanFromKingdomDecision.ExpelClanDecisionOutcome)o).ShouldBeExpelled;
			}

			public ExpelClanDecisionOutcome(bool shouldBeExpelled)
			{
				this.ShouldBeExpelled = shouldBeExpelled;
			}

			public override TextObject GetDecisionTitle()
			{
				TextObject textObject = new TextObject("{=kakxnaN5}{?SUPPORT}Yes{?}No{\\?}", null);
				textObject.SetTextVariable("SUPPORT", this.ShouldBeExpelled ? 1 : 0);
				return textObject;
			}

			public override TextObject GetDecisionDescription()
			{
				if (this.ShouldBeExpelled)
				{
					return new TextObject("{=s8z5Ugvm}The clan should be expelled", null);
				}
				return new TextObject("{=b2InhEeP}We oppose expelling the clan", null);
			}

			public override string GetDecisionLink()
			{
				return null;
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return null;
			}

			[SaveableField(100)]
			public readonly bool ShouldBeExpelled;
		}
	}
}
