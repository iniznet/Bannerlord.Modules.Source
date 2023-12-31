﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	public abstract class KingdomDecision
	{
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._kingdom);
			collectedObjects.Add(this.ProposerClan);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.TriggerTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueProposerClan(object o)
		{
			return ((KingdomDecision)o).ProposerClan;
		}

		internal static object AutoGeneratedGetMemberValueTriggerTime(object o)
		{
			return ((KingdomDecision)o).TriggerTime;
		}

		internal static object AutoGeneratedGetMemberValue_isEnforced(object o)
		{
			return ((KingdomDecision)o)._isEnforced;
		}

		internal static object AutoGeneratedGetMemberValue_playerExamined(object o)
		{
			return ((KingdomDecision)o)._playerExamined;
		}

		internal static object AutoGeneratedGetMemberValue_kingdom(object o)
		{
			return ((KingdomDecision)o)._kingdom;
		}

		public Kingdom Kingdom
		{
			get
			{
				return this._kingdom ?? this.ProposerClan.Kingdom;
			}
		}

		[SaveableProperty(4)]
		public Clan ProposerClan { get; private set; }

		public bool IsEnforced
		{
			get
			{
				return this._isEnforced;
			}
			set
			{
				this._isEnforced = value;
			}
		}

		public bool PlayerExamined
		{
			get
			{
				return this._playerExamined;
			}
			set
			{
				this._playerExamined = value;
			}
		}

		public bool NotifyPlayer
		{
			get
			{
				return this._notifyPlayer || this.IsEnforced;
			}
			set
			{
				this._notifyPlayer = value;
			}
		}

		public bool IsPlayerParticipant
		{
			get
			{
				return this.Kingdom == Clan.PlayerClan.Kingdom && !Clan.PlayerClan.IsUnderMercenaryService;
			}
		}

		[SaveableProperty(3)]
		public CampaignTime TriggerTime { get; protected set; }

		protected KingdomDecision(Clan proposerClan)
		{
			this.ProposerClan = proposerClan;
			this._kingdom = proposerClan.Kingdom;
			this.TriggerTime = CampaignTime.HoursFromNow((float)this.HoursToWait);
		}

		public virtual bool IsKingsVoteAllowed
		{
			get
			{
				return true;
			}
		}

		public abstract bool IsAllowed();

		public int GetInfluenceCost(Clan sponsorClan)
		{
			int proposalInfluenceCost = this.GetProposalInfluenceCost();
			if (sponsorClan != Clan.PlayerClan)
			{
				return proposalInfluenceCost;
			}
			return proposalInfluenceCost;
		}

		public abstract int GetProposalInfluenceCost();

		public abstract TextObject GetGeneralTitle();

		public abstract TextObject GetSupportTitle();

		public abstract TextObject GetChooseTitle();

		public abstract TextObject GetSupportDescription();

		public abstract TextObject GetChooseDescription();

		public virtual float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
		{
			return 1f;
		}

		public abstract IEnumerable<DecisionOutcome> DetermineInitialCandidates();

		public MBList<DecisionOutcome> NarrowDownCandidates(MBList<DecisionOutcome> initialCandidates, int maxCandidateCount)
		{
			foreach (DecisionOutcome decisionOutcome in initialCandidates)
			{
				decisionOutcome.InitialMerit = this.CalculateMeritOfOutcome(decisionOutcome);
			}
			return this.SortDecisionOutcomes(initialCandidates).Take(maxCandidateCount).ToMBList<DecisionOutcome>();
		}

		public abstract Clan DetermineChooser();

		public IEnumerable<Supporter> DetermineSupporters()
		{
			foreach (Clan clan in this.Kingdom.Clans)
			{
				if (!clan.IsUnderMercenaryService)
				{
					yield return new Supporter(clan);
				}
			}
			List<Clan>.Enumerator enumerator = default(List<Clan>.Enumerator);
			yield break;
			yield break;
		}

		protected virtual bool ShouldBeCancelledInternal()
		{
			return false;
		}

		protected virtual bool CanProposerClanChangeOpinion()
		{
			return false;
		}

		public bool ShouldBeCancelled()
		{
			if (this.ProposerClan.Kingdom != this.Kingdom)
			{
				return true;
			}
			if (!this.IsAllowed())
			{
				return true;
			}
			if (this.ShouldBeCancelledInternal())
			{
				return true;
			}
			if (this.ProposerClan == Clan.PlayerClan)
			{
				return false;
			}
			MBList<DecisionOutcome> mblist = this.NarrowDownCandidates(this.DetermineInitialCandidates().ToMBList<DecisionOutcome>(), 3);
			DecisionOutcome queriedDecisionOutcome = this.GetQueriedDecisionOutcome(mblist);
			this.DetermineSponsors(mblist);
			Supporter.SupportWeights supportWeights;
			DecisionOutcome decisionOutcome = this.DetermineSupportOption(new Supporter(this.ProposerClan), mblist, out supportWeights, true);
			bool flag = this.ProposerClan.Influence < (float)this.GetInfluenceCostOfSupport(this.ProposerClan, Supporter.SupportWeights.SlightlyFavor) * 1.5f;
			bool flag2 = mblist.Any((DecisionOutcome t) => t.SponsorClan != null && t.SponsorClan.IsEliminated);
			bool flag3 = supportWeights == Supporter.SupportWeights.StayNeutral || decisionOutcome == null;
			bool flag4 = decisionOutcome != queriedDecisionOutcome || (decisionOutcome == queriedDecisionOutcome && flag3);
			return flag2 || (mblist.Any((DecisionOutcome t) => t.SponsorClan == this.ProposerClan) && !flag && ((!this.CanProposerClanChangeOpinion() && flag4) || (this.CanProposerClanChangeOpinion() && flag3)));
		}

		protected virtual int HoursToWait
		{
			get
			{
				return 48;
			}
		}

		public bool NeedsPlayerResolution
		{
			get
			{
				return this.Kingdom == Clan.PlayerClan.Kingdom && (this.IsEnforced || (this.TriggerTime.IsPast && this.Kingdom.RulingClan == Clan.PlayerClan));
			}
		}

		public DecisionOutcome DetermineSupportOption(Supporter supporter, MBReadOnlyList<DecisionOutcome> possibleOutcomes, out Supporter.SupportWeights supportWeightOfSelectedOutcome, bool calculateRelationshipEffect)
		{
			Supporter.SupportWeights supportWeights = Supporter.SupportWeights.Choose;
			DecisionOutcome decisionOutcome = null;
			DecisionOutcome decisionOutcome2 = null;
			float num = float.MinValue;
			float num2 = 0f;
			int num3 = 0;
			Clan clan = supporter.Clan;
			foreach (DecisionOutcome decisionOutcome3 in possibleOutcomes)
			{
				float num4 = this.DetermineSupport(supporter.Clan, decisionOutcome3);
				if (num4 > num)
				{
					decisionOutcome = decisionOutcome3;
					num = num4;
				}
				if (num4 < num2)
				{
					decisionOutcome2 = decisionOutcome3;
					num2 = num4;
				}
				num3++;
			}
			if (decisionOutcome != null)
			{
				float num5 = num;
				if (decisionOutcome2 != null)
				{
					num5 -= 0.5f * num2;
				}
				float num6 = num5;
				if (clan.Influence < num6 * 2f)
				{
					num6 *= 0.5f;
					if (num6 > clan.Influence * 0.7f)
					{
						num6 = clan.Influence * 0.7f;
					}
				}
				else if (clan.Influence > num6 * 10f)
				{
					num6 *= 1.5f;
				}
				if (decisionOutcome.Likelihood > 0.65f)
				{
					num6 *= 1.6f * (1.2f - decisionOutcome.Likelihood);
				}
				if (calculateRelationshipEffect && decisionOutcome.SponsorClan != null)
				{
					int num7 = (int)(100f - MathF.Clamp((float)clan.Leader.GetRelation(decisionOutcome.SponsorClan.Leader), -100f, 100f));
					float num8 = MathF.Lerp(0.2f, 1.8f, 1f - (float)num7 / 200f, 1E-05f);
					num6 *= num8;
				}
				if (num6 > (float)this.GetInfluenceCostOfSupport(supporter.Clan, Supporter.SupportWeights.FullyPush))
				{
					supportWeights = Supporter.SupportWeights.FullyPush;
				}
				else if (num6 > (float)this.GetInfluenceCostOfSupport(supporter.Clan, Supporter.SupportWeights.StronglyFavor))
				{
					supportWeights = Supporter.SupportWeights.StronglyFavor;
				}
				else if (num6 > (float)this.GetInfluenceCostOfSupport(supporter.Clan, Supporter.SupportWeights.SlightlyFavor))
				{
					supportWeights = Supporter.SupportWeights.SlightlyFavor;
				}
			}
			while (supportWeights >= Supporter.SupportWeights.SlightlyFavor && supporter.Clan != null && supporter.Clan.Influence < (float)this.GetInfluenceCostOfSupport(supporter.Clan, supportWeights))
			{
				supportWeights--;
			}
			supportWeightOfSelectedOutcome = supportWeights;
			if (supportWeights == Supporter.SupportWeights.StayNeutral || supportWeights == Supporter.SupportWeights.Choose)
			{
				return null;
			}
			return decisionOutcome;
		}

		public abstract float DetermineSupport(Clan clan, DecisionOutcome possibleOutcome);

		public abstract void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes);

		protected void AssignDefaultSponsor(DecisionOutcome outcome)
		{
			if (outcome.SupporterList.Count > 0)
			{
				Supporter.SupportWeights maxWeight = outcome.SupporterList.Max((Supporter t) => t.SupportWeight);
				Supporter supporter = outcome.SupporterList.First((Supporter t) => t.SupportWeight == maxWeight);
				outcome.SetSponsor(supporter.Clan);
			}
		}

		public abstract void ApplyChosenOutcome(DecisionOutcome chosenOutcome);

		public int GetInfluenceCost(DecisionOutcome decisionOutcome, Clan clan, Supporter.SupportWeights supportWeight)
		{
			int num = 0;
			switch (supportWeight)
			{
			case Supporter.SupportWeights.Choose:
				num = 0;
				break;
			case Supporter.SupportWeights.StayNeutral:
				num = 0;
				break;
			case Supporter.SupportWeights.SlightlyFavor:
				num = this.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.SlightlyFavor);
				break;
			case Supporter.SupportWeights.StronglyFavor:
				num = this.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.StronglyFavor);
				break;
			case Supporter.SupportWeights.FullyPush:
				num = this.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.FullyPush);
				break;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Election\\KingdomDecision.cs", "GetInfluenceCost", 334);
				break;
			}
			return num;
		}

		public abstract TextObject GetSecondaryEffects();

		public abstract void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome);

		public abstract TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus, bool isShortVersion = false);

		public MBList<DecisionOutcome> SortDecisionOutcomes(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.OrderByDescending((DecisionOutcome k) => k.InitialMerit).ToMBList<DecisionOutcome>();
		}

		public abstract DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes);

		public bool IsSingleClanDecision()
		{
			return this.Kingdom.Clans.Count == 1;
		}

		public virtual float CalculateRelationshipEffectWithSponsor(Clan clan)
		{
			float num = 0.8f;
			return (float)clan.Leader.GetRelation(this.ProposerClan.Leader) * num;
		}

		public int GetInfluenceCostOfSupport(Clan clan, Supporter.SupportWeights supportWeight)
		{
			float influenceCostOfSupportInternal = (float)this.GetInfluenceCostOfSupportInternal(supportWeight);
			float num = 1f;
			if (clan.Leader.GetPerkValue(DefaultPerks.Charm.FlexibleEthics))
			{
				num += DefaultPerks.Charm.FlexibleEthics.PrimaryBonus;
			}
			return (int)(influenceCostOfSupportInternal * num);
		}

		protected virtual int GetInfluenceCostOfSupportInternal(Supporter.SupportWeights supportWeight)
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
				return 150;
			default:
				throw new ArgumentOutOfRangeException("supportWeight", supportWeight, null);
			}
		}

		public virtual bool OnShowDecision()
		{
			return true;
		}

		public virtual KingdomDecision GetFollowUpDecision()
		{
			return null;
		}

		[SaveableField(0)]
		private static bool _notificationsEnabled = true;

		[SaveableField(1)]
		private bool _isEnforced;

		[SaveableField(2)]
		private bool _playerExamined;

		private bool _notifyPlayer = KingdomDecision._notificationsEnabled;

		[SaveableField(10)]
		private Kingdom _kingdom;

		public KingdomDecision.SupportStatus SupportStatusOfFinalDecision;

		public enum SupportStatus
		{
			Equal,
			Majority,
			Minority
		}
	}
}
