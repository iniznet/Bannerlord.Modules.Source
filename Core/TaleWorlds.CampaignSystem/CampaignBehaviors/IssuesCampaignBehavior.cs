using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A3 RID: 931
	public class IssuesCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003743 RID: 14147 RVA: 0x000F8A64 File Offset: 0x000F6C64
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x17000CD0 RID: 3280
		// (get) Token: 0x06003744 RID: 14148 RVA: 0x000F8AFC File Offset: 0x000F6CFC
		private Settlement CurrentTickSettlement
		{
			get
			{
				CampaignTime campaignTime = new CampaignTime(CampaignTime.Days(1f).NumTicks / (long)this._settlements.Length);
				int num = (int)(CampaignTime.Now.NumTicks / campaignTime.NumTicks) % this._settlements.Length;
				return this._settlements[num];
			}
		}

		// Token: 0x06003745 RID: 14149 RVA: 0x000F8B54 File Offset: 0x000F6D54
		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			Settlement[] array = Village.All.Select((Village x) => x.Settlement).ToArray<Settlement>();
			int num = MathF.Ceiling(0.7f * (float)array.Length);
			Settlement[] array2 = Town.AllTowns.Select((Town x) => x.Settlement).ToArray<Settlement>();
			int num2 = MathF.Ceiling(0.8f * (float)array2.Length);
			int num3 = Hero.AllAliveHeroes.Count((Hero x) => x.IsLord && x.Clan != null && !x.Clan.IsBanditFaction && !x.IsChild);
			int num4 = MathF.Ceiling(0.120000005f * (float)num3);
			int num5 = num + num2 + num4;
			Campaign.Current.ConversationManager.DisableSentenceSort();
			this._additionalFrequencyScore = -0.4f;
			array.Shuffle<Settlement>();
			this.CreateRandomSettlementIssues(array, 2, num, num5);
			array2.Shuffle<Settlement>();
			this.CreateRandomSettlementIssues(array2, 3, num2, num5);
			Clan[] array3 = Clan.NonBanditFactions.Where((Clan x) => x.Heroes.Count != 0).ToArray<Clan>();
			array3.Shuffle<Clan>();
			this.CreateRandomClanIssues(array3, num4, num5);
			this._additionalFrequencyScore = 0.2f;
			Campaign.Current.ConversationManager.EnableSentenceSort();
		}

		// Token: 0x06003746 RID: 14150 RVA: 0x000F8CBC File Offset: 0x000F6EBC
		private void OnSettlementTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			Settlement currentTickSettlement = this.CurrentTickSettlement;
			int num = Campaign.Current.IssueManager.Issues.Count((KeyValuePair<Hero, IssueBase> x) => !x.Value.IsTriedToSolveBefore);
			int num2 = currentTickSettlement.HeroesWithoutParty.Count((Hero x) => x.Issue != null);
			int num3 = (currentTickSettlement.IsTown ? 1 : 1);
			int num4 = (currentTickSettlement.IsTown ? 3 : 2);
			if (num4 > 0 && num2 < num4 && (num2 < num3 || MBRandom.RandomFloat < this.GetIssueGenerationChance(num2, num4)))
			{
				this.CreateAnIssueForSettlementNotables(currentTickSettlement, num + 1);
			}
		}

		// Token: 0x06003747 RID: 14151 RVA: 0x000F8D74 File Offset: 0x000F6F74
		private void DailyTickClan(Clan clan)
		{
			if (this.IsClanSuitableForIssueCreation(clan))
			{
				int num = Campaign.Current.IssueManager.Issues.Count((KeyValuePair<Hero, IssueBase> x) => !x.Value.IsTriedToSolveBefore);
				int num2 = clan.Heroes.Count((Hero x) => x.Issue != null);
				int num3 = clan.Heroes.Count((Hero x) => x.IsAlive && !x.IsChild && x.IsLord);
				int num4 = MathF.Ceiling((float)num3 * 0.1f);
				int num5 = MathF.Floor((float)num3 * 0.2f);
				if (num5 > 0 && num2 < num5 && (num2 < num4 || MBRandom.RandomFloat < this.GetIssueGenerationChance(num2, num5)))
				{
					this.CreateAnIssueForClanNobles(clan, num + 1);
				}
			}
		}

		// Token: 0x06003748 RID: 14152 RVA: 0x000F8E57 File Offset: 0x000F7057
		private bool IsClanSuitableForIssueCreation(Clan clan)
		{
			return clan.Heroes.Count > 0 && !clan.IsBanditFaction;
		}

		// Token: 0x06003749 RID: 14153 RVA: 0x000F8E74 File Offset: 0x000F7074
		private void OnGameLoaded(CampaignGameStarter obj)
		{
			this._additionalFrequencyScore = 0.2f;
			List<IssueBase> list = new List<IssueBase>();
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
			{
				if (keyValuePair.Key.IsNotable && keyValuePair.Key.CurrentSettlement == null)
				{
					list.Add(keyValuePair.Value);
				}
			}
			foreach (IssueBase issueBase in list)
			{
				issueBase.CompleteIssueWithCancel(null);
			}
		}

		// Token: 0x0600374A RID: 14154 RVA: 0x000F8F40 File Offset: 0x000F7140
		private float GetIssueGenerationChance(int currentIssueCount, int maxIssueCount)
		{
			float num = 1f - (float)currentIssueCount / (float)maxIssueCount;
			return 0.3f * num * num;
		}

		// Token: 0x0600374B RID: 14155 RVA: 0x000F8F64 File Offset: 0x000F7164
		private void CreateRandomSettlementIssues(Settlement[] shuffledSettlementArray, int maxIssueCountPerSettlement, int desiredIssueCount, int totalDesiredIssueCount)
		{
			int num = shuffledSettlementArray.Length;
			int[] array = new int[num];
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while (num2 < num && num4 < desiredIssueCount)
			{
				int num6 = (num4 + num2 + num3) % num;
				if (array[num6] < num5)
				{
					num3++;
				}
				else if (array[num6] < maxIssueCountPerSettlement && this.CreateAnIssueForSettlementNotables(shuffledSettlementArray[num6], totalDesiredIssueCount))
				{
					num4++;
					array[num6]++;
				}
				else
				{
					num2++;
				}
			}
		}

		// Token: 0x0600374C RID: 14156 RVA: 0x000F8FD8 File Offset: 0x000F71D8
		private void CreateRandomClanIssues(Clan[] shuffledClanArray, int desiredIssueCount, int totalDesiredIssueCount)
		{
			int num = shuffledClanArray.Length;
			int num2 = 0;
			int num3 = 0;
			while (num2 < num && num3 < desiredIssueCount)
			{
				if (this.CreateAnIssueForClanNobles(shuffledClanArray[(num3 + num2) % num], totalDesiredIssueCount))
				{
					num3++;
				}
				else
				{
					num2++;
				}
			}
		}

		// Token: 0x0600374D RID: 14157 RVA: 0x000F9014 File Offset: 0x000F7214
		private bool CreateAnIssueForSettlementNotables(Settlement settlement, int totalDesiredIssueCount)
		{
			List<IssuesCampaignBehavior.IssueData> list = new List<IssuesCampaignBehavior.IssueData>();
			IssueManager issueManager = Campaign.Current.IssueManager;
			foreach (Hero hero in settlement.Notables)
			{
				if (hero.Issue == null && hero.CanHaveQuestsOrIssues())
				{
					List<PotentialIssueData> list2 = Campaign.Current.IssueManager.CheckForIssues(hero);
					int num = list2.SumQ((PotentialIssueData x) => this.GetFrequencyScore(x.Frequency));
					foreach (PotentialIssueData potentialIssueData in list2)
					{
						if (potentialIssueData.IsValid)
						{
							float num2 = this.CalculateIssueScoreForNotable(potentialIssueData, settlement, totalDesiredIssueCount, num);
							if (num2 > 0f && !issueManager.HasIssueCoolDown(potentialIssueData.IssueType, hero))
							{
								list.Add(new IssuesCampaignBehavior.IssueData(potentialIssueData, hero, num2));
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				List<ValueTuple<IssuesCampaignBehavior.IssueData, float>> list3 = new List<ValueTuple<IssuesCampaignBehavior.IssueData, float>>();
				foreach (IssuesCampaignBehavior.IssueData issueData in list)
				{
					list3.Add(new ValueTuple<IssuesCampaignBehavior.IssueData, float>(issueData, issueData.Score));
				}
				IssuesCampaignBehavior.IssueData issueData2 = MBRandom.ChooseWeighted<IssuesCampaignBehavior.IssueData>(list3);
				Campaign.Current.IssueManager.CreateNewIssue(issueData2.PotentialIssueData, issueData2.Hero);
				return true;
			}
			return false;
		}

		// Token: 0x0600374E RID: 14158 RVA: 0x000F91B0 File Offset: 0x000F73B0
		private bool CreateAnIssueForClanNobles(Clan clan, int totalDesiredIssueCount)
		{
			List<IssuesCampaignBehavior.IssueData> list = new List<IssuesCampaignBehavior.IssueData>();
			IssueManager issueManager = Campaign.Current.IssueManager;
			foreach (Hero hero in clan.Lords)
			{
				if (hero.Clan != Clan.PlayerClan && hero.CanHaveQuestsOrIssues() && hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && (hero.IsActive || hero.IsPrisoner) && hero.Issue == null)
				{
					List<PotentialIssueData> list2 = Campaign.Current.IssueManager.CheckForIssues(hero);
					int num = list2.SumQ((PotentialIssueData x) => this.GetFrequencyScore(x.Frequency));
					foreach (PotentialIssueData potentialIssueData in list2)
					{
						if (potentialIssueData.IsValid)
						{
							float num2 = this.CalculateIssueScoreForClan(potentialIssueData, clan, totalDesiredIssueCount, num);
							if (num2 > 0f && !issueManager.HasIssueCoolDown(potentialIssueData.IssueType, hero))
							{
								list.Add(new IssuesCampaignBehavior.IssueData(potentialIssueData, hero, num2));
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				IssuesCampaignBehavior.IssueData issueData = list.OrderByDescending((IssuesCampaignBehavior.IssueData x) => x.Score).First<IssuesCampaignBehavior.IssueData>();
				Campaign.Current.IssueManager.CreateNewIssue(issueData.PotentialIssueData, issueData.Hero);
				return true;
			}
			return false;
		}

		// Token: 0x0600374F RID: 14159 RVA: 0x000F9378 File Offset: 0x000F7578
		private float CalculateIssueScoreForClan(in PotentialIssueData pid, Clan clan, int totalDesiredIssueCount, int totalFrequencyScore)
		{
			foreach (Hero hero in clan.Heroes)
			{
				if (hero.Issue != null)
				{
					Type type = hero.Issue.GetType();
					PotentialIssueData potentialIssueData = pid;
					if (type == potentialIssueData.IssueType)
					{
						return 0f;
					}
				}
			}
			return this.CalculateIssueScoreInternal(pid, totalDesiredIssueCount, totalFrequencyScore);
		}

		// Token: 0x06003750 RID: 14160 RVA: 0x000F9400 File Offset: 0x000F7600
		private float CalculateIssueScoreForNotable(in PotentialIssueData pid, Settlement settlement, int totalDesiredIssueCount, int totalFrequencyScore)
		{
			foreach (Hero hero in settlement.Notables)
			{
				if (hero.Issue != null)
				{
					Type type = hero.Issue.GetType();
					PotentialIssueData potentialIssueData = pid;
					if (type == potentialIssueData.IssueType)
					{
						return 0f;
					}
				}
			}
			return this.CalculateIssueScoreInternal(pid, totalDesiredIssueCount, totalFrequencyScore);
		}

		// Token: 0x06003751 RID: 14161 RVA: 0x000F9488 File Offset: 0x000F7688
		private float CalculateIssueScoreInternal(in PotentialIssueData pid, int totalDesiredIssueCount, int totalFrequencyScore)
		{
			PotentialIssueData potentialIssueData = pid;
			float num = (float)this.GetFrequencyScore(potentialIssueData.Frequency) / (float)totalFrequencyScore;
			float num2;
			if (totalDesiredIssueCount == 0)
			{
				num2 = 1f;
			}
			else
			{
				int num3 = 0;
				foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues)
				{
					Type type = keyValuePair.Value.GetType();
					potentialIssueData = pid;
					if (type == potentialIssueData.IssueType)
					{
						num3++;
					}
				}
				num2 = (float)num3 / (float)totalDesiredIssueCount;
			}
			float num4 = 1f + this._additionalFrequencyScore - num2 / num;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			else if (num4 < this._additionalFrequencyScore)
			{
				num4 *= 0.01f;
			}
			else if (num4 < this._additionalFrequencyScore + 0.4f)
			{
				num4 *= 0.1f;
			}
			return num * num4;
		}

		// Token: 0x06003752 RID: 14162 RVA: 0x000F9584 File Offset: 0x000F7784
		private int GetFrequencyScore(IssueBase.IssueFrequency frequency)
		{
			int num = 0;
			switch (frequency)
			{
			case IssueBase.IssueFrequency.VeryCommon:
				num = 6;
				break;
			case IssueBase.IssueFrequency.Common:
				num = 3;
				break;
			case IssueBase.IssueFrequency.Rare:
				num = 1;
				break;
			}
			return num;
		}

		// Token: 0x06003753 RID: 14163 RVA: 0x000F95B4 File Offset: 0x000F77B4
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			CharacterObject characterObject;
			if (party == null)
			{
				characterObject = hero.CharacterObject;
			}
			else
			{
				Hero leaderHero = party.LeaderHero;
				characterObject = ((leaderHero != null) ? leaderHero.CharacterObject : null);
			}
			CharacterObject characterObject2 = characterObject;
			if (characterObject2 != null && !characterObject2.IsPlayerCharacter && ((party != null) ? party.Army : null) == null && Campaign.Current.GameStarted)
			{
				MBList<IssueBase> mblist = IssueManager.GetIssuesInSettlement(settlement, true).ToMBList<IssueBase>();
				float num = ((settlement.OwnerClan == characterObject2.HeroObject.Clan) ? 0.05f : 0.01f);
				if (mblist.Count > 0 && MBRandom.RandomFloat < num)
				{
					IssueBase randomElement = mblist.GetRandomElement<IssueBase>();
					if (randomElement.CanBeCompletedByAI() && randomElement.IsOngoingWithoutQuest)
					{
						randomElement.CompleteIssueWithAiLord(characterObject2.HeroObject);
					}
				}
			}
		}

		// Token: 0x06003754 RID: 14164 RVA: 0x000F9668 File Offset: 0x000F7868
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver = null)
		{
			if (details == IssueBase.IssueUpdateDetails.IssueFinishedWithSuccess && issueSolver != null && issueSolver.GetPerkValue(DefaultPerks.Charm.Oratory))
			{
				GainRenownAction.Apply(issueSolver, (float)MathF.Round(DefaultPerks.Charm.Oratory.PrimaryBonus), false);
				GainKingdomInfluenceAction.ApplyForDefault(issueSolver, (float)MathF.Round(DefaultPerks.Charm.Oratory.PrimaryBonus));
			}
			if ((details == IssueBase.IssueUpdateDetails.IssueFail || details == IssueBase.IssueUpdateDetails.IssueFinishedWithSuccess || details == IssueBase.IssueUpdateDetails.IssueFinishedWithBetrayal || details == IssueBase.IssueUpdateDetails.IssueTimedOut || details == IssueBase.IssueUpdateDetails.SentTroopsFinishedQuest) && issueSolver != null && issue.IssueOwner != null)
			{
				int num = (issue.IsSolvingWithQuest ? issue.IssueQuest.RelationshipChangeWithQuestGiver : issue.RelationshipChangeWithIssueOwner);
				if (num > 0)
				{
					if (issueSolver.GetPerkValue(DefaultPerks.Trade.DistributedGoods) && issue.IssueOwner.IsArtisan)
					{
						num *= (int)DefaultPerks.Trade.DistributedGoods.PrimaryBonus;
					}
					if (issueSolver.GetPerkValue(DefaultPerks.Trade.LocalConnection) && issue.IssueOwner.IsMerchant)
					{
						num *= (int)DefaultPerks.Trade.LocalConnection.PrimaryBonus;
					}
					ChangeRelationAction.ApplyPlayerRelation(issue.IsSolvingWithQuest ? issue.IssueQuest.QuestGiver : issue.IssueOwner, num, true, true);
				}
				else if (num < 0)
				{
					ChangeRelationAction.ApplyPlayerRelation(issue.IsSolvingWithQuest ? issue.IssueQuest.QuestGiver : issue.IssueOwner, num, true, true);
				}
			}
			if (details == IssueBase.IssueUpdateDetails.IssueCancel || details == IssueBase.IssueUpdateDetails.IssueFail || details == IssueBase.IssueUpdateDetails.IssueFinishedWithSuccess || details == IssueBase.IssueUpdateDetails.IssueFinishedWithBetrayal || details == IssueBase.IssueUpdateDetails.IssueTimedOut || details == IssueBase.IssueUpdateDetails.SentTroopsFinishedQuest || details == IssueBase.IssueUpdateDetails.SentTroopsFailedQuest || details == IssueBase.IssueUpdateDetails.IssueFinishedByAILord)
			{
				Campaign.Current.IssueManager.AddIssueCoolDownData(issue.GetType(), new HeroRelatedIssueCoolDownData(issue.IssueOwner, CampaignTime.DaysFromNow((float)Campaign.Current.Models.IssueModel.IssueOwnerCoolDownInDays)));
			}
		}

		// Token: 0x06003755 RID: 14165 RVA: 0x000F97F7 File Offset: 0x000F79F7
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003756 RID: 14166 RVA: 0x000F97FC File Offset: 0x000F79FC
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			List<Settlement> list = Settlement.All.Where((Settlement x) => x.IsTown || x.IsVillage).ToList<Settlement>();
			this.DeterministicShuffle(list);
			this._settlements = list.ToArray();
			CampaignTime campaignTime = new CampaignTime(CampaignTime.Days(1f).NumTicks / (long)this._settlements.Length);
			CampaignTime campaignTime2 = campaignTime - new CampaignTime(CampaignTime.Now.NumTicks % campaignTime.NumTicks);
			this._periodicEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(campaignTime, campaignTime2);
			this._periodicEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.OnSettlementTick));
			this.AddDialogues(starter);
		}

		// Token: 0x06003757 RID: 14167 RVA: 0x000F98BC File Offset: 0x000F7ABC
		private void DeterministicShuffle(List<Settlement> settlements)
		{
			Random random = new Random(53);
			for (int i = 0; i < settlements.Count; i++)
			{
				int num = random.Next() % settlements.Count;
				Settlement settlement = settlements[i];
				settlements[i] = settlements[num];
				settlements[num] = settlement;
			}
		}

		// Token: 0x06003758 RID: 14168 RVA: 0x000F9910 File Offset: 0x000F7B10
		private void AddDialogues(CampaignGameStarter starter)
		{
			starter.AddDialogLine("issue_not_offered", "issue_offer", "hero_main_options", "{=!}{ISSUE_NOT_OFFERED_EXPLANATION}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_not_offered_condition), new ConversationSentence.OnConsequenceDelegate(this.leave_on_conversation_end_consequence), 100, null);
			starter.AddDialogLine("issue_explanation", "issue_offer", "issue_explanation_player_response", "{=!}{IssueBriefByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offered_begin_condition), new ConversationSentence.OnConsequenceDelegate(this.leave_on_conversation_end_consequence), 100, null);
			starter.AddPlayerLine("issue_explanation_player_response_pre_lord_solution", "issue_explanation_player_response", "issue_lord_solution_brief", "{=!}{IssueAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(this.issue_explanation_player_response_pre_lord_solution_condition), null, 100, null, null);
			starter.AddPlayerLine("issue_explanation_player_response_pre_quest_solution", "issue_explanation_player_response", "issue_quest_solution_brief", "{=!}{IssueAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(this.issue_explanation_player_response_pre_quest_solution_condition), null, 100, null, null);
			starter.AddDialogLine("issue_lord_solution_brief", "issue_lord_solution_brief", "issue_lord_solution_player_response", "{=!}{IssueLordSolutionExplanationByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_lord_solution_brief_condition), null, 100, null);
			starter.AddPlayerLine("issue_lord_solution_player_response", "issue_lord_solution_player_response", "issue_quest_solution_brief", "{=!}{IssuePlayerResponseAfterLordExplanationText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_lord_solution_player_response_condition), null, 100, null, null);
			starter.AddDialogLine("issue_quest_solution_brief_pre_alternative_solution", "issue_quest_solution_brief", "issue_alternative_solution_player_response", "{=!}{IssueQuestSolutionExplanationByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_quest_solution_brief_pre_alternative_solution_condition), null, 100, null);
			starter.AddDialogLine("issue_quest_solution_brief_pre_player_response", "issue_quest_solution_brief", "issue_offer_player_response", "{=!}{IssueQuestSolutionExplanationByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_quest_solution_brief_pre_player_response_condition), null, 100, null);
			starter.AddPlayerLine("issue_alternative_solution_player_response", "issue_alternative_solution_player_response", "issue_alternative_solution_brief", "{=!}{IssuePlayerResponseAfterAlternativeExplanationText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_alternative_solution_player_response_condition), null, 100, null, null);
			starter.AddDialogLine("issue_alternative_solution_brief", "issue_alternative_solution_brief", "issue_offer_player_response", "{=!}{IssueAlternativeSolutionExplanationByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_alternative_solution_brief_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_2_consequence), 100, null);
			starter.AddPlayerLine("issue_offer_player_accept_quest", "issue_offer_player_response", "issue_classic_quest_start", "{=!}{IssueQuestSolutionAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_quest_condition), delegate
			{
				Campaign.Current.IssueManager.StartIssueQuest(Hero.OneToOneConversationHero);
			}, 100, null, null);
			starter.AddPlayerLine("issue_offer_player_accept_alternative", "issue_offer_player_response", "issue_offer_player_accept_alternative_2", "{=!}{IssueAlternativeSolutionAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_clickable_condition), null);
			starter.AddPlayerLine("issue_offer_player_accept_lord", "issue_offer_player_response", "issue_offer_player_accept_lord_2", "{=!}{IssueLordSolutionAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(this.issue_offer_player_accept_lord_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_offer_player_accept_lord_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_lord_clickable_condition), null);
			starter.AddPlayerLine("issue_offer_player_response_reject", "issue_offer_player_response", "issue_offer_hero_response_reject", "{=l549ODcw}Sorry. I can't do that right now.", null, null, 100, null, null);
			starter.AddDialogLine("issue_offer_player_accept_alternative_2", "issue_offer_player_accept_alternative_2", "issue_offer_player_accept_alternative_3", "{=X4ITSQOl}Which of your people can help us?", null, null, 100, null);
			starter.AddRepeatablePlayerLine("issue_offer_player_accept_alternative_3", "issue_offer_player_accept_alternative_3", "issue_offer_player_accept_alternative_4", "{=C2ZGNwwh}{COMPANION.NAME} {COMPANION_SCALED_PARAMETERS}", "{=nomZx5Nw}I am thinking of a different companion.", "issue_offer_player_accept_alternative_2", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_3_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_3_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_3_clickable_condition));
			starter.AddPlayerLine("issue_offer_player_accept_go_back", "issue_offer_player_accept_alternative_3", "issue_offer_hero_response_reject", "{=OymJQD7M}Actually, I don't have any available men right now...", null, null, 100, null, null);
			starter.AddDialogLine("issue_offer_player_accept_alternative_4", "issue_offer_player_accept_alternative_4", "issue_offer_player_accept_alternative_5", "{=!}Party screen goes here", null, new ConversationSentence.OnConsequenceDelegate(this.issue_offer_player_accept_alternative_4_consequence), 100, null);
			starter.AddDialogLine("issue_offer_player_accept_alternative_5_a", "issue_offer_player_accept_alternative_5", "close_window", "{=!}{IssueAlternativeSolutionResponseByIssueGiverText}", new ConversationSentence.OnConditionDelegate(this.issue_offer_player_accept_alternative_5_a_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_5_a_consequence), 100, null);
			starter.AddDialogLine("issue_offer_player_accept_alternative_5_b", "issue_offer_player_accept_alternative_5", "issue_offer_player_response", "{=!}{IssueGiverResponseToRejection}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_hero_response_reject_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_offer_player_accept_alternative_5_b_consequence), 100, null);
			starter.AddPlayerLine("issue_offer_player_back", "issue_offer_player_accept_alternative_5", "issue_offer_player_response", GameTexts.FindText("str_back", null).ToString(), null, null, 100, null, null);
			starter.AddDialogLine("issue_offer_player_accept_lord_2", "issue_offer_player_accept_lord_2", "hero_main_options", "{=!}{IssueLordSolutionResponseByIssueGiverText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_player_accept_lord_2_condition), null, 100, null);
			starter.AddDialogLine("issue_offer_hero_response_reject", "issue_offer_hero_response_reject", "hero_main_options", "{=!}{IssueGiverResponseToRejection}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_offer_hero_response_reject_condition), null, 100, null);
			starter.AddDialogLine("issue_counter_offer_1", "start", "issue_counter_offer_2", "{=!}{IssueLordSolutionCounterOfferBriefByOtherNpcText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_start_condition), null, int.MaxValue, null);
			starter.AddDialogLine("issue_counter_offer_2", "issue_counter_offer_2", "issue_counter_offer_player_response", "{=!}{IssueLordSolutionCounterOfferExplanationByOtherNpcText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_2_condition), null, 100, null);
			starter.AddPlayerLine("issue_counter_offer_player_accept", "issue_counter_offer_player_response", "issue_counter_offer_accepted", "{=!}{IssueLordSolutionCounterOfferAcceptByPlayerText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_player_accept_condition), null, 100, null, null);
			starter.AddDialogLine("issue_counter_offer_accepted", "issue_counter_offer_accepted", "close_window", "{=!}{IssueLordSolutionCounterOfferAcceptResponseByOtherNpcText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_accepted_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_counter_offer_accepted_consequence), 100, null);
			starter.AddPlayerLine("issue_counter_offer_player_reject", "issue_counter_offer_player_response", "issue_counter_offer_reject", "{=!}{IssueLordSolutionCounterOfferDeclineByPlayerText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_player_reject_condition), null, 100, null, null);
			starter.AddDialogLine("issue_counter_offer_reject", "issue_counter_offer_reject", "close_window", "{=!}{IssueLordSolutionCounterOfferDeclineResponseByOtherNpcText}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_counter_offer_reject_condition), new ConversationSentence.OnConsequenceDelegate(IssuesCampaignBehavior.issue_counter_offer_reject_consequence), 100, null);
			starter.AddDialogLine("issue_alternative_solution_discuss", "issue_discuss_alternative_solution", "close_window", "{=!}{IssueDiscussAlternativeSolution}", new ConversationSentence.OnConditionDelegate(IssuesCampaignBehavior.issue_alternative_solution_discussion_condition), new ConversationSentence.OnConsequenceDelegate(this.issue_alternative_solution_discussion_consequence), int.MaxValue, null);
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x000F9ED0 File Offset: 0x000F80D0
		private static bool issue_alternative_solution_discussion_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			if (issueOwnersIssue != null && issueOwnersIssue.IsThereAlternativeSolution && issueOwnersIssue.IsSolvingWithAlternative)
			{
				MBTextManager.SetTextVariable("IssueDiscussAlternativeSolution", issueOwnersIssue.IssueDiscussAlternativeSolution, false);
				return true;
			}
			return false;
		}

		// Token: 0x0600375A RID: 14170 RVA: 0x000F9F0A File Offset: 0x000F810A
		private void issue_alternative_solution_discussion_consequence()
		{
			if (PlayerEncounter.Current != null && Campaign.Current.ConversationManager.ConversationParty == PlayerEncounter.EncounteredMobileParty)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		// Token: 0x0600375B RID: 14171 RVA: 0x000F9F30 File Offset: 0x000F8130
		private static void issue_counter_offer_reject_consequence()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			Campaign.Current.ConversationManager.ConversationEndOneShot += counterOfferersIssue.CompleteIssueWithLordSolutionWithRefuseCounterOffer;
		}

		// Token: 0x0600375C RID: 14172 RVA: 0x000F9F60 File Offset: 0x000F8160
		private static bool issue_counter_offer_reject_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferDeclineResponseByOtherNpcText", counterOfferersIssue.IssueLordSolutionCounterOfferDeclineResponseByOtherNpc, false);
			return true;
		}

		// Token: 0x0600375D RID: 14173 RVA: 0x000F9F88 File Offset: 0x000F8188
		private static bool issue_counter_offer_player_reject_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferDeclineByPlayerText", counterOfferersIssue.IssueLordSolutionCounterOfferDeclineByPlayer, false);
			return true;
		}

		// Token: 0x0600375E RID: 14174 RVA: 0x000F9FB0 File Offset: 0x000F81B0
		private static void issue_counter_offer_accepted_consequence()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			Campaign.Current.ConversationManager.ConversationEndOneShot += counterOfferersIssue.CompleteIssueWithLordSolutionWithAcceptCounterOffer;
		}

		// Token: 0x0600375F RID: 14175 RVA: 0x000F9FE0 File Offset: 0x000F81E0
		private static bool issue_counter_offer_accepted_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferAcceptResponseByOtherNpcText", counterOfferersIssue.IssueLordSolutionCounterOfferAcceptResponseByOtherNpc, false);
			return true;
		}

		// Token: 0x06003760 RID: 14176 RVA: 0x000FA008 File Offset: 0x000F8208
		private static bool issue_counter_offer_player_accept_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferAcceptByPlayerText", counterOfferersIssue.IssueLordSolutionCounterOfferAcceptByPlayer, false);
			return true;
		}

		// Token: 0x06003761 RID: 14177 RVA: 0x000FA030 File Offset: 0x000F8230
		private static bool issue_counter_offer_2_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferExplanationByOtherNpcText", counterOfferersIssue.IssueLordSolutionCounterOfferExplanationByOtherNpc, false);
			return true;
		}

		// Token: 0x06003762 RID: 14178 RVA: 0x000FA058 File Offset: 0x000F8258
		private static bool issue_counter_offer_start_condition()
		{
			IssueBase counterOfferersIssue = IssuesCampaignBehavior.GetCounterOfferersIssue();
			if (counterOfferersIssue != null)
			{
				MBTextManager.SetTextVariable("IssueLordSolutionCounterOfferBriefByOtherNpcText", counterOfferersIssue.IssueLordSolutionCounterOfferBriefByOtherNpc, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003763 RID: 14179 RVA: 0x000FA084 File Offset: 0x000F8284
		private static bool issue_offer_player_accept_lord_2_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionResponseByIssueGiverText", issueOwnersIssue.IssueLordSolutionResponseByIssueGiver, false);
			return true;
		}

		// Token: 0x06003764 RID: 14180 RVA: 0x000FA0AC File Offset: 0x000F82AC
		private void issue_offer_player_accept_alternative_4_consequence()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			int totalAlternativeSolutionNeededMenCount = issueOwnersIssue.GetTotalAlternativeSolutionNeededMenCount();
			if (totalAlternativeSolutionNeededMenCount > 1)
			{
				PartyScreenManager.OpenScreenAsQuest(issueOwnersIssue.AlternativeSolutionSentTroops, new TextObject("{=FbLOFO88}Select troops for mission", null), totalAlternativeSolutionNeededMenCount + 1, issueOwnersIssue.GetTotalAlternativeSolutionDurationInDays(), new PartyPresentationDoneButtonConditionDelegate(this.PartyScreenDoneCondition), new PartyScreenClosedDelegate(IssuesCampaignBehavior.PartyScreenDoneClicked), new IsTroopTransferableDelegate(IssuesCampaignBehavior.TroopTransferableDelegate), null);
				return;
			}
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		// Token: 0x06003765 RID: 14181 RVA: 0x000FA120 File Offset: 0x000F8320
		private static void issue_offer_player_accept_alternative_5_b_consequence()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MobileParty.MainParty.MemberRoster.Add(issueOwnersIssue.AlternativeSolutionSentTroops);
			issueOwnersIssue.AlternativeSolutionSentTroops.Clear();
		}

		// Token: 0x06003766 RID: 14182 RVA: 0x000FA153 File Offset: 0x000F8353
		private static void issue_offer_player_accept_alternative_5_a_consequence()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			issueOwnersIssue.AlternativeSolutionStartConsequence();
			issueOwnersIssue.StartIssueWithAlternativeSolution();
		}

		// Token: 0x06003767 RID: 14183 RVA: 0x000FA168 File Offset: 0x000F8368
		private bool issue_offer_player_accept_alternative_5_a_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueAlternativeSolutionResponseByIssueGiverText", issueOwnersIssue.IssueAlternativeSolutionResponseByIssueGiver, false);
			TextObject textObject;
			return IssuesCampaignBehavior.DoTroopsSatisfyAlternativeSolutionInternal(issueOwnersIssue.AlternativeSolutionSentTroops, out textObject);
		}

		// Token: 0x06003768 RID: 14184 RVA: 0x000FA19C File Offset: 0x000F839C
		private static bool issue_offer_player_accept_alternative_3_clickable_condition(out TextObject explanation)
		{
			bool flag = true;
			explanation = TextObject.Empty;
			Hero hero = ConversationSentence.CurrentProcessedRepeatObject as Hero;
			if (hero == null || hero.PartyBelongedTo != MobileParty.MainParty)
			{
				flag = false;
			}
			else if (!hero.CanHaveQuestsOrIssues())
			{
				explanation = new TextObject("{=DBabgrcC}This hero is not available right now.", null);
				flag = false;
			}
			else if (hero.IsWounded)
			{
				explanation = new TextObject("{=CyrOuz4h}This hero is wounded.", null);
				flag = false;
			}
			else if (hero.IsPregnant)
			{
				explanation = new TextObject("{=BaKOWJb6}This hero is pregnant.", null);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06003769 RID: 14185 RVA: 0x000FA21C File Offset: 0x000F841C
		private static void issue_offer_player_accept_alternative_3_consequence()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			Hero hero = ConversationSentence.SelectedRepeatObject as Hero;
			if (hero != null)
			{
				MobileParty.MainParty.MemberRoster.AddToCounts(hero.CharacterObject, -1, false, 0, 0, true, -1);
				issueOwnersIssue.AlternativeSolutionSentTroops.AddToCounts(hero.CharacterObject, 1, false, 0, 0, true, -1);
				CampaignEventDispatcher.Instance.OnHeroGetsBusy(hero, HeroGetsBusyReasons.SolvesIssue);
			}
		}

		// Token: 0x0600376A RID: 14186 RVA: 0x000FA280 File Offset: 0x000F8480
		private static bool TroopTransferableDelegate(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase leftOwnerParty)
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			return !character.IsHero && !character.IsNotTransferableInPartyScreen && type != PartyScreenLogic.TroopType.Prisoner && issueOwnersIssue.IsTroopTypeNeededByAlternativeSolution(character);
		}

		// Token: 0x0600376B RID: 14187 RVA: 0x000FA2B0 File Offset: 0x000F84B0
		private static void PartyScreenDoneClicked(PartyBase leftOwnerParty, TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, PartyBase rightOwnerParty, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, bool fromCancel)
		{
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		// Token: 0x0600376C RID: 14188 RVA: 0x000FA2C4 File Offset: 0x000F84C4
		private Tuple<bool, TextObject> PartyScreenDoneCondition(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, int leftLimitNum, int rightLimitNum)
		{
			TextObject textObject;
			return new Tuple<bool, TextObject>(IssuesCampaignBehavior.DoTroopsSatisfyAlternativeSolutionInternal(leftMemberRoster, out textObject), textObject);
		}

		// Token: 0x0600376D RID: 14189 RVA: 0x000FA2E0 File Offset: 0x000F84E0
		private static bool DoTroopsSatisfyAlternativeSolutionInternal(TroopRoster troopRoster, out TextObject explanation)
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			explanation = TextObject.Empty;
			int totalAlternativeSolutionNeededMenCount = issueOwnersIssue.GetTotalAlternativeSolutionNeededMenCount();
			if (troopRoster.TotalRegulars >= totalAlternativeSolutionNeededMenCount && troopRoster.TotalRegulars - troopRoster.TotalWoundedRegulars < totalAlternativeSolutionNeededMenCount)
			{
				explanation = new TextObject("{=fjmGXcLW}You have to send healthy troops to this quest.", null);
				return false;
			}
			return issueOwnersIssue.DoTroopsSatisfyAlternativeSolution(troopRoster, out explanation);
		}

		// Token: 0x0600376E RID: 14190 RVA: 0x000FA334 File Offset: 0x000F8534
		private static bool issue_offer_player_accept_alternative_3_condition()
		{
			Hero hero = ConversationSentence.CurrentProcessedRepeatObject as Hero;
			if (hero != null)
			{
				StringHelpers.SetRepeatableCharacterProperties("COMPANION", hero.CharacterObject, false);
			}
			List<TextObject> list = new List<TextObject>();
			IssueModel issueModel = Campaign.Current.Models.IssueModel;
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			bool flag = false;
			if (issueOwnersIssue.AlternativeSolutionHasCasualties)
			{
				ValueTuple<int, int> causalityForHero = issueModel.GetCausalityForHero(hero, issueOwnersIssue);
				if (causalityForHero.Item2 > 0)
				{
					TextObject textObject = TextObject.Empty;
					if (causalityForHero.Item1 == causalityForHero.Item2)
					{
						textObject = new TextObject("{=zPlFvCRm}{NUMBER_OF_TROOPS} troop loss", null);
						textObject.SetTextVariable("NUMBER_OF_TROOPS", causalityForHero.Item1);
					}
					else
					{
						textObject = new TextObject("{=bdlomGZ1}{MIN_NUMBER_OF_TROOPS} - {MAX_NUMBER_OF_TROOPS_LOST} troop loss", null);
						textObject.SetTextVariable("MIN_NUMBER_OF_TROOPS", causalityForHero.Item1);
						textObject.SetTextVariable("MAX_NUMBER_OF_TROOPS_LOST", causalityForHero.Item2);
					}
					flag = true;
					list.Add(textObject);
				}
			}
			if (issueOwnersIssue.AlternativeSolutionHasFailureRisk)
			{
				float num = issueModel.GetFailureRiskForHero(hero, issueOwnersIssue);
				if (num > 0f)
				{
					num = (float)((int)(num * 100f));
					TextObject textObject2 = new TextObject("{=9tLYXGGc}{FAILURE_RISK}% risk of failure", null);
					textObject2.SetTextVariable("FAILURE_RISK", num);
					list.Add(textObject2);
					flag = true;
				}
				else
				{
					TextObject textObject3 = new TextObject("{=way8jWK8}no risk of failure", null);
					list.Add(textObject3);
				}
			}
			if (issueOwnersIssue.AlternativeSolutionHasScaledRequiredTroops)
			{
				int troopsRequiredForHero = issueModel.GetTroopsRequiredForHero(hero, issueOwnersIssue);
				if (troopsRequiredForHero > 0)
				{
					TextObject textObject4 = new TextObject("{=b3bJXMt2}{NUMBER_OF_TROOPS} required troops", null);
					textObject4.SetTextVariable("NUMBER_OF_TROOPS", troopsRequiredForHero);
					list.Add(textObject4);
					flag = true;
				}
			}
			if (issueOwnersIssue.AlternativeSolutionHasScaledDuration)
			{
				CampaignTime durationOfResolutionForHero = issueModel.GetDurationOfResolutionForHero(hero, issueOwnersIssue);
				if (durationOfResolutionForHero > CampaignTime.Days(0f))
				{
					TextObject textObject5 = new TextObject("{=ImatoO4Y}{DURATION_IN_DAYS} required days to complete", null);
					textObject5.SetTextVariable("DURATION_IN_DAYS", (float)durationOfResolutionForHero.ToDays);
					list.Add(textObject5);
					flag = true;
				}
			}
			if (flag)
			{
				ValueTuple<SkillObject, int> issueAlternativeSolutionSkill = issueModel.GetIssueAlternativeSolutionSkill(hero, issueOwnersIssue);
				if (issueAlternativeSolutionSkill.Item1 != null)
				{
					TextObject textObject6 = new TextObject("{=!}{SKILL}: {NUMBER}", null);
					textObject6.SetTextVariable("SKILL", issueAlternativeSolutionSkill.Item1.Name);
					textObject6.SetTextVariable("NUMBER", hero.GetSkillValue(issueAlternativeSolutionSkill.Item1));
					list.Add(textObject6);
				}
			}
			if (list.IsEmpty<TextObject>())
			{
				ConversationSentence.SelectedRepeatLine.SetTextVariable("COMPANION_SCALED_PARAMETERS", TextObject.Empty);
			}
			else
			{
				TextObject textObject7 = GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false);
				TextObject textObject8 = GameTexts.FindText("str_STR_in_parentheses", null);
				textObject8.SetTextVariable("STR", textObject7);
				ConversationSentence.SelectedRepeatLine.SetTextVariable("COMPANION_SCALED_PARAMETERS", textObject8);
			}
			return true;
		}

		// Token: 0x0600376F RID: 14191 RVA: 0x000FA5C4 File Offset: 0x000F87C4
		private static void issue_offer_player_accept_alternative_2_consequence()
		{
			List<Hero> list = new List<Hero>();
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero && !troopRosterElement.Character.IsPlayerCharacter && troopRosterElement.Character.HeroObject.CanHaveQuestsOrIssues())
				{
					list.Add(troopRosterElement.Character.HeroObject);
				}
			}
			ConversationSentence.SetObjectsToRepeatOver(list, 5);
		}

		// Token: 0x06003770 RID: 14192 RVA: 0x000FA664 File Offset: 0x000F8864
		private static bool issue_offer_hero_response_reject_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.GetPersona() == DefaultTraits.PersonaCurt)
			{
				MBTextManager.SetTextVariable("IssueGiverResponseToRejection", new TextObject("{=h2Wle7ZI}Well. That's a pity.", null), false);
			}
			else if (CharacterObject.OneToOneConversationCharacter.GetPersona() == DefaultTraits.PersonaIronic)
			{
				MBTextManager.SetTextVariable("IssueGiverResponseToRejection", new TextObject("{=wbLnJrJA}Ah, well. I can look elsewhere for help, I suppose.", null), false);
			}
			else
			{
				MBTextManager.SetTextVariable("IssueGiverResponseToRejection", new TextObject("{=Uoy2tTZJ}Very well. But perhaps you will reconsider later.", null), false);
			}
			return true;
		}

		// Token: 0x06003771 RID: 14193 RVA: 0x000FA6DC File Offset: 0x000F88DC
		private static bool issue_offer_player_accept_lord_clickable_condition(out TextObject explanation)
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			if (!issueOwnersIssue.LordSolutionCondition(out explanation))
			{
				return false;
			}
			if (Clan.PlayerClan.Influence < (float)issueOwnersIssue.NeededInfluenceForLordSolution)
			{
				explanation = new TextObject("{=hRdhfSs0}You don't have enough influence for this solution. ({NEEDED_INFLUENCE}{INFLUENCE_ICON})", null);
				explanation.SetTextVariable("NEEDED_INFLUENCE", issueOwnersIssue.NeededInfluenceForLordSolution);
				explanation.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
				return false;
			}
			explanation = new TextObject("{=xbvgc8Sp}This solution will cost {INFLUENCE} influence.", null);
			explanation.SetTextVariable("INFLUENCE", issueOwnersIssue.NeededInfluenceForLordSolution);
			return true;
		}

		// Token: 0x06003772 RID: 14194 RVA: 0x000FA762 File Offset: 0x000F8962
		private static void issue_offer_player_accept_lord_consequence()
		{
			Hero.OneToOneConversationHero.Issue.StartIssueWithLordSolution();
		}

		// Token: 0x06003773 RID: 14195 RVA: 0x000FA774 File Offset: 0x000F8974
		private bool issue_offer_player_accept_lord_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			if (issueOwnersIssue.IsThereLordSolution)
			{
				MBTextManager.SetTextVariable("IssueLordSolutionAcceptByPlayerText", issueOwnersIssue.IssueLordSolutionAcceptByPlayer, false);
				return IssuesCampaignBehavior.IssueLordSolutionCondition();
			}
			return false;
		}

		// Token: 0x06003774 RID: 14196 RVA: 0x000FA7A8 File Offset: 0x000F89A8
		private static bool issue_offer_player_accept_alternative_clickable_condition(out TextObject explanation)
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			if ((from m in MobileParty.MainParty.MemberRoster.GetTroopRoster()
				where m.Character.IsHero && !m.Character.IsPlayerCharacter && m.Character.HeroObject.CanHaveQuestsOrIssues()
				select m).IsEmpty<TroopRosterElement>())
			{
				explanation = new TextObject("{=qjpNREwg}You don't have any companions or family members.", null);
				return false;
			}
			if (!issueOwnersIssue.AlternativeSolutionCondition(out explanation))
			{
				return false;
			}
			explanation = TextObject.Empty;
			return true;
		}

		// Token: 0x06003775 RID: 14197 RVA: 0x000FA818 File Offset: 0x000F8A18
		private static bool issue_offer_player_accept_alternative_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			if (issueOwnersIssue.IsThereAlternativeSolution)
			{
				MBTextManager.SetTextVariable("IssueAlternativeSolutionAcceptByPlayerText", issueOwnersIssue.IssueAlternativeSolutionAcceptByPlayer, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003776 RID: 14198 RVA: 0x000FA848 File Offset: 0x000F8A48
		private static bool issue_offer_player_accept_quest_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueQuestSolutionAcceptByPlayerText", issueOwnersIssue.IssueQuestSolutionAcceptByPlayer, false);
			return true;
		}

		// Token: 0x06003777 RID: 14199 RVA: 0x000FA870 File Offset: 0x000F8A70
		private static bool issue_alternative_solution_brief_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueAlternativeSolutionExplanationByIssueGiverText", issueOwnersIssue.IssueAlternativeSolutionExplanationByIssueGiver, false);
			return true;
		}

		// Token: 0x06003778 RID: 14200 RVA: 0x000FA898 File Offset: 0x000F8A98
		private static bool issue_alternative_solution_player_response_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssuePlayerResponseAfterAlternativeExplanationText", issueOwnersIssue.IssuePlayerResponseAfterAlternativeExplanation, false);
			return issueOwnersIssue.IsThereAlternativeSolution;
		}

		// Token: 0x06003779 RID: 14201 RVA: 0x000FA8C4 File Offset: 0x000F8AC4
		private static bool issue_quest_solution_brief_pre_player_response_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueQuestSolutionExplanationByIssueGiverText", issueOwnersIssue.IssueQuestSolutionExplanationByIssueGiver, false);
			return !issueOwnersIssue.IsThereAlternativeSolution;
		}

		// Token: 0x0600377A RID: 14202 RVA: 0x000FA8F4 File Offset: 0x000F8AF4
		private static bool issue_quest_solution_brief_pre_alternative_solution_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueQuestSolutionExplanationByIssueGiverText", issueOwnersIssue.IssueQuestSolutionExplanationByIssueGiver, false);
			return issueOwnersIssue.IsThereAlternativeSolution;
		}

		// Token: 0x0600377B RID: 14203 RVA: 0x000FA920 File Offset: 0x000F8B20
		private static bool issue_lord_solution_player_response_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssuePlayerResponseAfterLordExplanationText", issueOwnersIssue.IssuePlayerResponseAfterLordExplanation, false);
			return true;
		}

		// Token: 0x0600377C RID: 14204 RVA: 0x000FA948 File Offset: 0x000F8B48
		private static bool issue_lord_solution_brief_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueLordSolutionExplanationByIssueGiverText", issueOwnersIssue.IssueLordSolutionExplanationByIssueGiver, false);
			return true;
		}

		// Token: 0x0600377D RID: 14205 RVA: 0x000FA970 File Offset: 0x000F8B70
		private bool issue_explanation_player_response_pre_quest_solution_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueAcceptByPlayerText", issueOwnersIssue.IssueAcceptByPlayer, false);
			return !issueOwnersIssue.IsThereLordSolution || !IssuesCampaignBehavior.IssueLordSolutionCondition();
		}

		// Token: 0x0600377E RID: 14206 RVA: 0x000FA9A8 File Offset: 0x000F8BA8
		private bool issue_explanation_player_response_pre_lord_solution_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			MBTextManager.SetTextVariable("IssueAcceptByPlayerText", issueOwnersIssue.IssueAcceptByPlayer, false);
			return issueOwnersIssue.IsThereLordSolution && IssuesCampaignBehavior.IssueLordSolutionCondition();
		}

		// Token: 0x0600377F RID: 14207 RVA: 0x000FA9DC File Offset: 0x000F8BDC
		private static bool IssueLordSolutionCondition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			return issueOwnersIssue.IssueOwner.CurrentSettlement != null && issueOwnersIssue.IssueOwner.CurrentSettlement.OwnerClan == Clan.PlayerClan;
		}

		// Token: 0x06003780 RID: 14208 RVA: 0x000FAA18 File Offset: 0x000F8C18
		private static bool issue_offered_begin_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			TextObject textObject;
			if (issueOwnersIssue != null && issueOwnersIssue.CheckPreconditions(Hero.OneToOneConversationHero, out textObject))
			{
				MBTextManager.SetTextVariable("IssueBriefByIssueGiverText", issueOwnersIssue.IssueBriefByIssueGiver, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003781 RID: 14209 RVA: 0x000FAA54 File Offset: 0x000F8C54
		private static bool issue_not_offered_condition()
		{
			IssueBase issueOwnersIssue = IssuesCampaignBehavior.GetIssueOwnersIssue();
			TextObject textObject;
			if (issueOwnersIssue != null && !issueOwnersIssue.CheckPreconditions(Hero.OneToOneConversationHero, out textObject))
			{
				MBTextManager.SetTextVariable("ISSUE_NOT_OFFERED_EXPLANATION", textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003782 RID: 14210 RVA: 0x000FAA88 File Offset: 0x000F8C88
		private void leave_on_conversation_end_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += MapEventHelper.OnConversationEnd;
		}

		// Token: 0x06003783 RID: 14211 RVA: 0x000FAAA5 File Offset: 0x000F8CA5
		private static IssueBase GetIssueOwnersIssue()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			if (oneToOneConversationHero == null)
			{
				return null;
			}
			return oneToOneConversationHero.Issue;
		}

		// Token: 0x06003784 RID: 14212 RVA: 0x000FAAB8 File Offset: 0x000F8CB8
		private static IssueBase GetCounterOfferersIssue()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				foreach (IssueBase issueBase in Campaign.Current.IssueManager.Issues.Values)
				{
					if (issueBase.CounterOfferHero == Hero.OneToOneConversationHero && issueBase.IsSolvingWithLordSolution)
					{
						return issueBase;
					}
				}
			}
			return null;
		}

		// Token: 0x04001180 RID: 4480
		private const int MinNotableIssueCountForTowns = 1;

		// Token: 0x04001181 RID: 4481
		private const int MaxNotableIssueCountForTowns = 3;

		// Token: 0x04001182 RID: 4482
		private const int MinNotableIssueCountForVillages = 1;

		// Token: 0x04001183 RID: 4483
		private const int MaxNotableIssueCountForVillages = 2;

		// Token: 0x04001184 RID: 4484
		private const float MinIssuePercentageForClanHeroes = 0.1f;

		// Token: 0x04001185 RID: 4485
		private const float MaxIssuePercentageForClanHeroes = 0.2f;

		// Token: 0x04001186 RID: 4486
		private float _additionalFrequencyScore;

		// Token: 0x04001187 RID: 4487
		private Settlement[] _settlements;

		// Token: 0x04001188 RID: 4488
		private MBCampaignEvent _periodicEvent;

		// Token: 0x020006F8 RID: 1784
		private struct IssueData
		{
			// Token: 0x06005532 RID: 21810 RVA: 0x0016CBD9 File Offset: 0x0016ADD9
			public IssueData(PotentialIssueData issueData, Hero hero, float score)
			{
				this.PotentialIssueData = issueData;
				this.Hero = hero;
				this.Score = score;
			}

			// Token: 0x04001CB0 RID: 7344
			public readonly PotentialIssueData PotentialIssueData;

			// Token: 0x04001CB1 RID: 7345
			public readonly Hero Hero;

			// Token: 0x04001CB2 RID: 7346
			public readonly float Score;
		}
	}
}
