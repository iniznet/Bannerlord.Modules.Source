﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public class SettlementClaimantDecision : KingdomDecision
	{
		internal static void AutoGeneratedStaticCollectObjectsSettlementClaimantDecision(object o, List<object> collectedObjects)
		{
			((SettlementClaimantDecision)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Settlement);
			collectedObjects.Add(this.ClanToExclude);
			collectedObjects.Add(this._capturerHero);
		}

		internal static object AutoGeneratedGetMemberValueSettlement(object o)
		{
			return ((SettlementClaimantDecision)o).Settlement;
		}

		internal static object AutoGeneratedGetMemberValueClanToExclude(object o)
		{
			return ((SettlementClaimantDecision)o).ClanToExclude;
		}

		internal static object AutoGeneratedGetMemberValue_capturerHero(object o)
		{
			return ((SettlementClaimantDecision)o)._capturerHero;
		}

		public SettlementClaimantDecision(Clan proposerClan, Settlement settlement, Hero capturerHero, Clan clanToExclude)
			: base(proposerClan)
		{
			this.Settlement = settlement;
			this._capturerHero = capturerHero;
			this.ClanToExclude = clanToExclude;
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsAnnexationDecisionAllowed(this.Settlement);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAnnexation(base.ProposerClan);
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject = new TextObject("{=Of7XnP5c}Vote for the new owner of {SETTLEMENT_NAME}", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetGeneralTitle()
		{
			TextObject textObject = new TextObject("{=2qZ81jPG}Owner of {SETTLEMENT_NAME}", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject = new TextObject("{=J4UMplzb}{FACTION_LEADER} will decide who will own {SETTLEMENT_NAME}. You can give your support to one of the candidates.", null);
			textObject.SetTextVariable("FACTION_LEADER", this.DetermineChooser().Leader.Name);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject = new TextObject("{=2qZ81jPG}Owner of {SETTLEMENT_NAME}", null);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject = new TextObject("{=xzq78nVm}As {?IS_FEMALE}queen{?}king{\\?} you must decide who will own {SETTLEMENT_NAME}.", null);
			textObject.SetTextVariable("IS_FEMALE", this.DetermineChooser().Leader.IsFemale ? 1 : 0);
			textObject.SetTextVariable("SETTLEMENT_NAME", this.Settlement.Name);
			return textObject;
		}

		protected override bool ShouldBeCancelledInternal()
		{
			return this.Settlement.MapFaction != base.Kingdom;
		}

		protected override bool CanProposerClanChangeOpinion()
		{
			return true;
		}

		public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
		{
			SettlementClaimantDecision.ClanAsDecisionOutcome clanAsDecisionOutcome = (SettlementClaimantDecision.ClanAsDecisionOutcome)candidateOutcome;
			Clan clan = clanAsDecisionOutcome.Clan;
			float num = 0f;
			int num2 = 0;
			float num3 = Campaign.MapDiagonal + 1f;
			float num4 = Campaign.MapDiagonal + 1f;
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.OwnerClan == clanAsDecisionOutcome.Clan && settlement.IsFortification && this.Settlement != settlement)
				{
					num += settlement.GetSettlementValueForFaction(clanAsDecisionOutcome.Clan.Kingdom);
					float num5;
					if (Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, this.Settlement, num4, out num5))
					{
						if (num5 < num3)
						{
							num4 = num3;
							num3 = num5;
						}
						else if (num5 < num4)
						{
							num4 = num5;
						}
					}
					num2++;
				}
			}
			float num6 = Campaign.AverageDistanceBetweenTwoFortifications * 1.5f;
			float num7 = num6 * 0.25f;
			float num8 = num6;
			if (num4 < Campaign.MapDiagonal)
			{
				num8 = (num4 + num3) / 2f;
			}
			else if (num3 < Campaign.MapDiagonal)
			{
				num8 = num3;
			}
			float num9 = MathF.Pow(num6 / MathF.Max(num7, MathF.Min(400f, num8)), 0.5f);
			float num10 = clan.TotalStrength;
			if (this.Settlement.OwnerClan == clan && this.Settlement.Town != null && this.Settlement.Town.GarrisonParty != null)
			{
				num10 -= this.Settlement.Town.GarrisonParty.Party.TotalStrength;
				if (num10 < 0f)
				{
					num10 = 0f;
				}
			}
			float settlementValueForFaction = this.Settlement.GetSettlementValueForFaction(clanAsDecisionOutcome.Clan.Kingdom);
			bool flag = clanAsDecisionOutcome.Clan.Leader == clanAsDecisionOutcome.Clan.Kingdom.Leader;
			float num11 = ((num2 == 0) ? 30f : 0f);
			float num12 = (flag ? 60f : 0f);
			float num13 = ((this.Settlement.Town != null && this.Settlement.Town.LastCapturedBy == clanAsDecisionOutcome.Clan) ? 30f : 0f);
			float num14 = ((clanAsDecisionOutcome.Clan.Leader == Hero.MainHero) ? 30f : 0f);
			float num15 = ((clanAsDecisionOutcome.Clan.Leader.Gold < 30000) ? MathF.Min(30f, 30f - (float)clanAsDecisionOutcome.Clan.Leader.Gold / 1000f) : 0f);
			return ((float)clan.Tier * 30f + num10 / 10f + num11 + num13 + num12 + num15 + num14) / (num + settlementValueForFaction) * num9 * 200000f;
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			Kingdom kingdom = (Kingdom)this.Settlement.MapFaction;
			List<SettlementClaimantDecision.ClanAsDecisionOutcome> list = new List<SettlementClaimantDecision.ClanAsDecisionOutcome>();
			foreach (Clan clan in kingdom.Clans)
			{
				if (clan != this.ClanToExclude && !clan.IsUnderMercenaryService && !clan.IsEliminated && !clan.Leader.IsDead)
				{
					list.Add(new SettlementClaimantDecision.ClanAsDecisionOutcome(clan));
				}
			}
			return list;
		}

		public override Clan DetermineChooser()
		{
			return ((Kingdom)this.Settlement.MapFaction).RulingClan;
		}

		public override float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome)
		{
			SettlementClaimantDecision.ClanAsDecisionOutcome clanAsDecisionOutcome = (SettlementClaimantDecision.ClanAsDecisionOutcome)possibleOutcome;
			float num = clanAsDecisionOutcome.InitialMerit;
			int traitLevel = clan.Leader.GetTraitLevel(DefaultTraits.Honor);
			num *= MathF.Clamp(1f + (float)traitLevel, 0f, 2f);
			if (clanAsDecisionOutcome.Clan == clan)
			{
				float settlementValueForFaction = this.Settlement.GetSettlementValueForFaction(clan);
				num += 0.2f * settlementValueForFaction * Campaign.Current.Models.DiplomacyModel.DenarsToInfluence();
			}
			else
			{
				float num2 = ((clanAsDecisionOutcome.Clan != clan) ? ((float)FactionManager.GetRelationBetweenClans(clanAsDecisionOutcome.Clan, clan)) : 100f);
				int traitLevel2 = clan.Leader.GetTraitLevel(DefaultTraits.Calculating);
				num *= MathF.Clamp(1f + (float)traitLevel2, 0f, 2f);
				float num3 = num2 * 0.2f * (float)traitLevel2;
				num += num3;
			}
			int traitLevel3 = clan.Leader.GetTraitLevel(DefaultTraits.Calculating);
			float num4 = ((traitLevel3 > 0) ? (0.4f - (float)MathF.Min(2, traitLevel3) * 0.1f) : (0.4f + (float)MathF.Min(2, MathF.Abs(traitLevel3)) * 0.1f));
			float num5 = 1f - num4 * 1.5f;
			num *= num5;
			float num6 = ((clan == clanAsDecisionOutcome.Clan) ? 2f : 1f);
			return num * num6;
		}

		public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
			{
				decisionOutcome.SetSponsor(((SettlementClaimantDecision.ClanAsDecisionOutcome)decisionOutcome).Clan);
			}
		}

		public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
		{
			ChangeOwnerOfSettlementAction.ApplyByKingDecision(((SettlementClaimantDecision.ClanAsDecisionOutcome)chosenOutcome).Clan.Leader, this.Settlement);
		}

		protected override int GetInfluenceCostOfSupportInternal(Supporter.SupportWeights supportWeight)
		{
			switch (supportWeight)
			{
			case Supporter.SupportWeights.Choose:
			case Supporter.SupportWeights.StayNeutral:
				return 0;
			case Supporter.SupportWeights.SlightlyFavor:
				return 20;
			case Supporter.SupportWeights.StronglyFavor:
				return 60;
			case Supporter.SupportWeights.FullyPush:
				return 100;
			default:
				throw new ArgumentOutOfRangeException("supportWeight", supportWeight, null);
			}
		}

		public override TextObject GetSecondaryEffects()
		{
			return new TextObject("{=bHNU9uz2}All supporters gains some relation with the supported candidate clan and might lose with the others.", null);
		}

		public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
		}

		public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false)
		{
			TextObject textObject = TextObject.Empty;
			bool flag = ((SettlementClaimantDecision.ClanAsDecisionOutcome)chosenOutcome).Clan.Leader == this.Settlement.MapFaction.Leader;
			if (supportStatus == KingdomDecision.SupportStatus.Majority && flag)
			{
				textObject = new TextObject("{=Zckbdm4Z}{RULER.NAME} of the {KINGDOM} takes {SETTLEMENT} as {?RULER.GENDER}her{?}his{\\?} fief with {?RULER.GENDER}her{?}his{\\?} council's support.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Minority && flag)
			{
				textObject = new TextObject("{=qa4FlTWS}{RULER.NAME} of the {KINGDOM} takes {SETTLEMENT} as {?RULER.GENDER}her{?}his{\\?} fief despite {?RULER.GENDER}her{?}his{\\?} council's opposition.", null);
			}
			else if (flag)
			{
				textObject = new TextObject("{=5bBAOHmC}{RULER.NAME} of the {KINGDOM} takes {SETTLEMENT} as {?RULER.GENDER}her{?}his{\\?} fief, with {?RULER.GENDER}her{?}his{\\?} council evenly split.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Majority)
			{
				textObject = new TextObject("{=0nhqJewP}{RULER.NAME} of the {KINGDOM} grants {SETTLEMENT} to {LEADER.NAME} with {?RULER.GENDER}her{?}his{\\?} council's support.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Minority)
			{
				textObject = new TextObject("{=Ktpia7Pa}{RULER.NAME} of the {KINGDOM} grants {SETTLEMENT} to {LEADER.NAME} despite {?RULER.GENDER}her{?}his{\\?} council's opposition.", null);
			}
			else
			{
				textObject = new TextObject("{=l5H9x7Lo}{RULER.NAME} of the {KINGDOM} grants {SETTLEMENT} to {LEADER.NAME}, with {?RULER.GENDER}her{?}his{\\?} council evenly split.", null);
			}
			textObject.SetTextVariable("SETTLEMENT", this.Settlement.Name);
			StringHelpers.SetCharacterProperties("LEADER", ((SettlementClaimantDecision.ClanAsDecisionOutcome)chosenOutcome).Clan.Leader.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("RULER", this.Settlement.MapFaction.Leader.CharacterObject, textObject, false);
			textObject.SetTextVariable("KINGDOM", this.Settlement.MapFaction.InformalName);
			textObject.SetTextVariable("CLAN", ((SettlementClaimantDecision.ClanAsDecisionOutcome)chosenOutcome).Clan.Name);
			return textObject;
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.OrderByDescending((DecisionOutcome t) => t.Merit).FirstOrDefault<DecisionOutcome>();
		}

		[SaveableField(300)]
		public readonly Settlement Settlement;

		[SaveableField(301)]
		public readonly Clan ClanToExclude;

		[SaveableField(302)]
		private readonly Hero _capturerHero;

		public class ClanAsDecisionOutcome : DecisionOutcome
		{
			internal static void AutoGeneratedStaticCollectObjectsClanAsDecisionOutcome(object o, List<object> collectedObjects)
			{
				((SettlementClaimantDecision.ClanAsDecisionOutcome)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this.Clan);
			}

			internal static object AutoGeneratedGetMemberValueClan(object o)
			{
				return ((SettlementClaimantDecision.ClanAsDecisionOutcome)o).Clan;
			}

			public override TextObject GetDecisionTitle()
			{
				return this.Clan.Leader.Name;
			}

			public override TextObject GetDecisionDescription()
			{
				TextObject textObject = new TextObject("{=QKIxepj5}The lordship of this fief should go to the {RECIPIENT.CLAN}", null);
				StringHelpers.SetCharacterProperties("RECIPIENT", this.Clan.Leader.CharacterObject, textObject, true);
				return textObject;
			}

			public override string GetDecisionLink()
			{
				return this.Clan.Leader.EncyclopediaLink.ToString();
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return new ImageIdentifier(CharacterCode.CreateFrom(this.Clan.Leader.CharacterObject));
			}

			public ClanAsDecisionOutcome(Clan clan)
			{
				this.Clan = clan;
			}

			[SaveableField(300)]
			public readonly Clan Clan;
		}
	}
}
