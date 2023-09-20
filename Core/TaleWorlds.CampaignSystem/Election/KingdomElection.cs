using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
	// Token: 0x0200027B RID: 635
	public class KingdomElection
	{
		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x06002118 RID: 8472 RVA: 0x0008C7BA File Offset: 0x0008A9BA
		public MBReadOnlyList<DecisionOutcome> PossibleOutcomes
		{
			get
			{
				return this._possibleOutcomes;
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x06002119 RID: 8473 RVA: 0x0008C7C2 File Offset: 0x0008A9C2
		// (set) Token: 0x0600211A RID: 8474 RVA: 0x0008C7CA File Offset: 0x0008A9CA
		[SaveableProperty(7)]
		public bool IsCancelled { get; private set; }

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x0600211B RID: 8475 RVA: 0x0008C7D3 File Offset: 0x0008A9D3
		public bool IsPlayerSupporter
		{
			get
			{
				return this.PlayerAsSupporter != null;
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x0600211C RID: 8476 RVA: 0x0008C7DE File Offset: 0x0008A9DE
		private Supporter PlayerAsSupporter
		{
			get
			{
				return this._supporters.FirstOrDefault((Supporter x) => x.IsPlayer);
			}
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x0600211D RID: 8477 RVA: 0x0008C80A File Offset: 0x0008AA0A
		public bool IsPlayerChooser
		{
			get
			{
				return this._chooser.Leader.IsHumanPlayerCharacter;
			}
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x0008C81C File Offset: 0x0008AA1C
		public KingdomElection(KingdomDecision decision)
		{
			this._decision = decision;
			this.Setup();
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x0008C834 File Offset: 0x0008AA34
		private void Setup()
		{
			MBList<DecisionOutcome> mblist = this._decision.DetermineInitialCandidates().ToMBList<DecisionOutcome>();
			this._possibleOutcomes = this._decision.NarrowDownCandidates(mblist, 3);
			this._supporters = this._decision.DetermineSupporters().ToList<Supporter>();
			this._chooser = this._decision.DetermineChooser();
			this._decision.DetermineSponsors(this._possibleOutcomes);
			this._hasPlayerVoted = false;
			this.IsCancelled = false;
			foreach (DecisionOutcome decisionOutcome in this._possibleOutcomes)
			{
				decisionOutcome.InitialSupport = this.DetermineInitialSupport(decisionOutcome);
			}
			float num = this._possibleOutcomes.Sum((DecisionOutcome x) => x.InitialSupport);
			foreach (DecisionOutcome decisionOutcome2 in this._possibleOutcomes)
			{
				decisionOutcome2.Likelihood = ((num == 0f) ? 0f : (decisionOutcome2.InitialSupport / num));
			}
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x0008C980 File Offset: 0x0008AB80
		public void StartElection()
		{
			this.Setup();
			this.DetermineSupport(this._possibleOutcomes, false);
			this._decision.DetermineSponsors(this._possibleOutcomes);
			this.UpdateSupport(this._possibleOutcomes);
			if (this._decision.ShouldBeCancelled())
			{
				Debug.Print("SELIM_DEBUG - " + this._decision.GetSupportTitle() + " has been cancelled", 0, Debug.DebugColor.White, 17592186044416UL);
				this.IsCancelled = true;
				return;
			}
			if (!this.IsPlayerSupporter || this._ignorePlayerSupport)
			{
				this.ReadyToAiChoose();
				return;
			}
			if (this._decision.IsSingleClanDecision())
			{
				this._chosenOutcome = this._possibleOutcomes.FirstOrDefault((DecisionOutcome t) => t.SponsorClan != null && t.SponsorClan == Clan.PlayerClan);
				Supporter supporter = new Supporter(Clan.PlayerClan);
				supporter.SupportWeight = Supporter.SupportWeights.FullyPush;
				this._chosenOutcome.AddSupport(supporter);
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x0008CA70 File Offset: 0x0008AC70
		private float DetermineInitialSupport(DecisionOutcome possibleOutcome)
		{
			float num = 0f;
			foreach (Supporter supporter in this._supporters)
			{
				if (!supporter.IsPlayer)
				{
					num += MathF.Clamp(this._decision.DetermineSupport(supporter.Clan, possibleOutcome), 0f, 100f);
				}
			}
			return num;
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x0008CAF0 File Offset: 0x0008ACF0
		public void StartElectionWithoutPlayer()
		{
			this._ignorePlayerSupport = true;
			this.StartElection();
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x0008CAFF File Offset: 0x0008ACFF
		public float GetLikelihoodForOutcome(int outcomeNo)
		{
			if (outcomeNo >= 0 && outcomeNo < this._possibleOutcomes.Count)
			{
				return this._possibleOutcomes[outcomeNo].Likelihood;
			}
			return 0f;
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x0008CB2C File Offset: 0x0008AD2C
		public float GetLikelihoodForSponsor(Clan sponsor)
		{
			foreach (DecisionOutcome decisionOutcome in this._possibleOutcomes)
			{
				if (decisionOutcome.SponsorClan == sponsor)
				{
					return decisionOutcome.Likelihood;
				}
			}
			Debug.FailedAssert("This clan is not a sponsor of any of the outcomes.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Election\\KingdomDecisionMaker.cs", "GetLikelihoodForSponsor", 151);
			return -1f;
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x0008CBAC File Offset: 0x0008ADAC
		private void DetermineSupport(MBReadOnlyList<DecisionOutcome> possibleOutcomes, bool calculateRelationshipEffect)
		{
			foreach (Supporter supporter in this._supporters)
			{
				if (!supporter.IsPlayer)
				{
					Supporter.SupportWeights supportWeights = Supporter.SupportWeights.StayNeutral;
					DecisionOutcome decisionOutcome = this._decision.DetermineSupportOption(supporter, possibleOutcomes, out supportWeights, calculateRelationshipEffect);
					if (decisionOutcome != null)
					{
						supporter.SupportWeight = supportWeights;
						decisionOutcome.AddSupport(supporter);
					}
				}
			}
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x0008CC24 File Offset: 0x0008AE24
		private void UpdateSupport(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			foreach (DecisionOutcome decisionOutcome in this._possibleOutcomes)
			{
				foreach (Supporter supporter in new List<Supporter>(decisionOutcome.SupporterList))
				{
					decisionOutcome.ResetSupport(supporter);
				}
			}
			this.DetermineSupport(possibleOutcomes, true);
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x0008CCC0 File Offset: 0x0008AEC0
		private void ReadyToAiChoose()
		{
			this._chosenOutcome = this.GetAiChoice(this._possibleOutcomes);
			if (this._decision.OnShowDecision())
			{
				this.ApplyChosenOutcome();
			}
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x0008CCE8 File Offset: 0x0008AEE8
		private void ApplyChosenOutcome()
		{
			this._decision.ApplyChosenOutcome(this._chosenOutcome);
			this._decision.SupportStatusOfFinalDecision = this.GetSupportStatusOfDecisionOutcome(this._chosenOutcome);
			this.HandleInfluenceCosts();
			this.ApplySecondaryEffects(this._possibleOutcomes, this._chosenOutcome);
			for (int i = 0; i < this._possibleOutcomes.Count; i++)
			{
				if (this._possibleOutcomes[i].SponsorClan != null)
				{
					foreach (Supporter supporter in this._possibleOutcomes[i].SupporterList)
					{
						if (supporter.Clan.Leader != this._possibleOutcomes[i].SponsorClan.Leader && supporter.Clan == Clan.PlayerClan)
						{
							int num = this.GetRelationChangeWithSponsor(supporter.Clan.Leader, supporter.SupportWeight, false);
							if (num != 0)
							{
								num *= ((this._possibleOutcomes.Count > 2) ? 2 : 1);
								ChangeRelationAction.ApplyRelationChangeBetweenHeroes(supporter.Clan.Leader, this._possibleOutcomes[i].SponsorClan.Leader, num, true);
							}
						}
					}
					for (int j = 0; j < this._possibleOutcomes.Count; j++)
					{
						if (i != j)
						{
							foreach (Supporter supporter2 in this._possibleOutcomes[j].SupporterList)
							{
								if (supporter2.Clan.Leader != this._possibleOutcomes[i].SponsorClan.Leader && supporter2.Clan == Clan.PlayerClan)
								{
									int relationChangeWithSponsor = this.GetRelationChangeWithSponsor(supporter2.Clan.Leader, supporter2.SupportWeight, true);
									if (relationChangeWithSponsor != 0)
									{
										ChangeRelationAction.ApplyRelationChangeBetweenHeroes(supporter2.Clan.Leader, this._possibleOutcomes[i].SponsorClan.Leader, relationChangeWithSponsor, true);
									}
								}
							}
						}
					}
				}
			}
			this._decision.Kingdom.RemoveDecision(this._decision);
			this._decision.Kingdom.OnKingdomDecisionConcluded();
			CampaignEventDispatcher.Instance.OnKingdomDecisionConcluded(this._decision, this._chosenOutcome, this.IsPlayerChooser || this._hasPlayerVoted);
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x0008CF80 File Offset: 0x0008B180
		public int GetRelationChangeWithSponsor(Hero opposerOrSupporter, Supporter.SupportWeights supportWeight, bool isOpposingSides)
		{
			int num = 0;
			Clan clan = opposerOrSupporter.Clan;
			if (supportWeight == Supporter.SupportWeights.FullyPush)
			{
				num = (int)((float)this._decision.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.FullyPush) / 20f);
			}
			else if (supportWeight == Supporter.SupportWeights.StronglyFavor)
			{
				num = (int)((float)this._decision.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.StronglyFavor) / 20f);
			}
			else if (supportWeight == Supporter.SupportWeights.SlightlyFavor)
			{
				num = (int)((float)this._decision.GetInfluenceCostOfSupport(clan, Supporter.SupportWeights.SlightlyFavor) / 20f);
			}
			int num2 = (isOpposingSides ? (num * -1) : (num * 2));
			if (isOpposingSides && opposerOrSupporter.Culture.HasFeat(DefaultCulturalFeats.SturgianDecisionPenaltyFeat))
			{
				num2 += (int)((float)num2 * DefaultCulturalFeats.SturgianDecisionPenaltyFeat.EffectBonus);
			}
			return num2;
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x0008D01C File Offset: 0x0008B21C
		private void HandleInfluenceCosts()
		{
			DecisionOutcome decisionOutcome = this._possibleOutcomes[0];
			foreach (DecisionOutcome decisionOutcome2 in this._possibleOutcomes)
			{
				if (decisionOutcome2.TotalSupportPoints > decisionOutcome.TotalSupportPoints)
				{
					decisionOutcome = decisionOutcome2;
				}
				for (int i = 0; i < decisionOutcome2.SupporterList.Count; i++)
				{
					Clan clan = decisionOutcome2.SupporterList[i].Clan;
					int num = this._decision.GetInfluenceCost(decisionOutcome2, clan, decisionOutcome2.SupporterList[i].SupportWeight);
					if (this._supporters.Count == 1)
					{
						num = 0;
					}
					if (this._chosenOutcome != decisionOutcome2)
					{
						num /= 2;
					}
					if (decisionOutcome2 == this._chosenOutcome || !clan.Leader.GetPerkValue(DefaultPerks.Charm.GoodNatured))
					{
						ChangeClanInfluenceAction.Apply(clan, (float)(-(float)num));
					}
				}
			}
			if (this._chosenOutcome != decisionOutcome)
			{
				int influenceRequiredToOverrideKingdomDecision = Campaign.Current.Models.ClanPoliticsModel.GetInfluenceRequiredToOverrideKingdomDecision(decisionOutcome, this._chosenOutcome, this._decision);
				ChangeClanInfluenceAction.Apply(this._chooser, (float)(-(float)influenceRequiredToOverrideKingdomDecision));
			}
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x0008D160 File Offset: 0x0008B360
		private void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
		{
			this._decision.ApplySecondaryEffects(possibleOutcomes, chosenOutcome);
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x0008D16F File Offset: 0x0008B36F
		private int GetInfluenceRequiredToOverrideDecision(DecisionOutcome popularOutcome, DecisionOutcome overridingOutcome)
		{
			return Campaign.Current.Models.ClanPoliticsModel.GetInfluenceRequiredToOverrideKingdomDecision(popularOutcome, overridingOutcome, this._decision);
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x0008D190 File Offset: 0x0008B390
		private DecisionOutcome GetAiChoice(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
		{
			this.DetermineOfficialSupport();
			DecisionOutcome decisionOutcome = possibleOutcomes.MaxBy((DecisionOutcome t) => t.TotalSupportPoints);
			DecisionOutcome decisionOutcome2 = decisionOutcome;
			if (this._decision.IsKingsVoteAllowed)
			{
				DecisionOutcome decisionOutcome3 = possibleOutcomes.MaxBy((DecisionOutcome t) => this._decision.DetermineSupport(this._chooser, t));
				float num = this._decision.DetermineSupport(this._chooser, decisionOutcome3);
				float num2 = this._decision.DetermineSupport(this._chooser, decisionOutcome);
				float num3 = num - num2;
				num3 = MathF.Min(num3, this._chooser.Influence);
				if (num3 > 10f)
				{
					float num4 = 300f + (float)this.GetInfluenceRequiredToOverrideDecision(decisionOutcome, decisionOutcome3);
					if (num3 > num4)
					{
						float num5 = num4 / num3;
						if (MBRandom.RandomFloat > num5)
						{
							decisionOutcome2 = decisionOutcome3;
						}
					}
				}
			}
			return decisionOutcome2;
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x0008D260 File Offset: 0x0008B460
		public TextObject GetChosenOutcomeText()
		{
			return this._decision.GetChosenOutcomeText(this._chosenOutcome, this._decision.SupportStatusOfFinalDecision, false);
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x0008D280 File Offset: 0x0008B480
		private KingdomDecision.SupportStatus GetSupportStatusOfDecisionOutcome(DecisionOutcome chosenOutcome)
		{
			KingdomDecision.SupportStatus supportStatus = KingdomDecision.SupportStatus.Equal;
			float num = chosenOutcome.WinChance * 100f;
			int num2 = 50;
			if (num > (float)(num2 + 5))
			{
				supportStatus = KingdomDecision.SupportStatus.Majority;
			}
			else if (num < (float)(num2 - 5))
			{
				supportStatus = KingdomDecision.SupportStatus.Minority;
			}
			return supportStatus;
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x0008D2B4 File Offset: 0x0008B4B4
		public void DetermineOfficialSupport()
		{
			new List<Tuple<DecisionOutcome, float>>();
			float num = 0.001f;
			foreach (DecisionOutcome decisionOutcome in this._possibleOutcomes)
			{
				float num2 = 0f;
				foreach (Supporter supporter in decisionOutcome.SupporterList)
				{
					num2 += (float)MathF.Max(0, supporter.SupportWeight - Supporter.SupportWeights.StayNeutral);
				}
				decisionOutcome.TotalSupportPoints = num2;
				num += decisionOutcome.TotalSupportPoints;
			}
			foreach (DecisionOutcome decisionOutcome2 in this._possibleOutcomes)
			{
				decisionOutcome2.TotalSupportPoints /= num;
			}
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x0008D3BC File Offset: 0x0008B5BC
		public int GetInfluenceCostOfOutcome(DecisionOutcome outcome, Clan supporter, Supporter.SupportWeights weight)
		{
			return this._decision.GetInfluenceCostOfSupport(supporter, weight);
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x0008D3CB File Offset: 0x0008B5CB
		public TextObject GetSecondaryEffects()
		{
			return this._decision.GetSecondaryEffects();
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x0008D3D8 File Offset: 0x0008B5D8
		public void OnPlayerSupport(DecisionOutcome decisionOutcome, Supporter.SupportWeights supportWeight)
		{
			if (!this.IsPlayerChooser)
			{
				foreach (DecisionOutcome decisionOutcome2 in this._possibleOutcomes)
				{
					decisionOutcome2.ResetSupport(this.PlayerAsSupporter);
				}
				this._hasPlayerVoted = true;
				if (decisionOutcome != null)
				{
					this.PlayerAsSupporter.SupportWeight = supportWeight;
					decisionOutcome.AddSupport(this.PlayerAsSupporter);
					return;
				}
			}
			else
			{
				this._chosenOutcome = decisionOutcome;
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x0008D460 File Offset: 0x0008B660
		public void ApplySelection()
		{
			if (!this.IsCancelled)
			{
				if (this._chooser != Clan.PlayerClan)
				{
					this.ReadyToAiChoose();
					return;
				}
				this.ApplyChosenOutcome();
			}
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x0008D484 File Offset: 0x0008B684
		public MBList<DecisionOutcome> GetSortedDecisionOutcomes()
		{
			return this._decision.SortDecisionOutcomes(this._possibleOutcomes);
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x0008D497 File Offset: 0x0008B697
		public TextObject GetGeneralTitle()
		{
			return this._decision.GetGeneralTitle();
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x0008D4A4 File Offset: 0x0008B6A4
		public TextObject GetTitle()
		{
			if (this.IsPlayerChooser)
			{
				return this._decision.GetChooseTitle();
			}
			return this._decision.GetSupportTitle();
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x0008D4C5 File Offset: 0x0008B6C5
		public TextObject GetDescription()
		{
			if (this.IsPlayerChooser)
			{
				return this._decision.GetChooseDescription();
			}
			return this._decision.GetSupportDescription();
		}

		// Token: 0x04000A59 RID: 2649
		[SaveableField(0)]
		private readonly KingdomDecision _decision;

		// Token: 0x04000A5A RID: 2650
		private MBList<DecisionOutcome> _possibleOutcomes;

		// Token: 0x04000A5B RID: 2651
		[SaveableField(2)]
		private List<Supporter> _supporters;

		// Token: 0x04000A5C RID: 2652
		[SaveableField(3)]
		private Clan _chooser;

		// Token: 0x04000A5D RID: 2653
		[SaveableField(4)]
		private DecisionOutcome _chosenOutcome;

		// Token: 0x04000A5E RID: 2654
		[SaveableField(5)]
		private bool _ignorePlayerSupport;

		// Token: 0x04000A5F RID: 2655
		[SaveableField(6)]
		private bool _hasPlayerVoted;
	}
}
