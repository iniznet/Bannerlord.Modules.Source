using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.BoardGames;
using SandBox.BoardGames.MissionLogics;
using SandBox.Conversation;
using SandBox.Conversation.MissionLogics;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x02000094 RID: 148
	public class BoardGameCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x000326BF File Offset: 0x000308BF
		public IEnumerable<Settlement> WonBoardGamesInOneWeekInSettlement
		{
			get
			{
				foreach (Settlement settlement in this._wonBoardGamesInOneWeekInSettlement.Keys)
				{
					yield return settlement;
				}
				Dictionary<Settlement, CampaignTime>.KeyCollection.Enumerator enumerator = default(Dictionary<Settlement, CampaignTime>.KeyCollection.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x000326D0 File Offset: 0x000308D0
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnPlayerBoardGameOverEvent.AddNonSerializedListener(this, new Action<Hero, BoardGameHelper.BoardGameState>(this.OnPlayerBoardGameOver));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00032767 File Offset: 0x00030967
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Hero, List<CampaignTime>>>("_heroAndBoardGameTimeDictionary", ref this._heroAndBoardGameTimeDictionary);
			dataStore.SyncData<Dictionary<Settlement, CampaignTime>>("_wonBoardGamesInOneWeekInSettlement", ref this._wonBoardGamesInOneWeekInSettlement);
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0003278D File Offset: 0x0003098D
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00032798 File Offset: 0x00030998
		private void WeeklyTick()
		{
			this.DeleteOldBoardGamesOfChampion();
			foreach (Hero hero in this._heroAndBoardGameTimeDictionary.Keys.ToList<Hero>())
			{
				this.DeleteOldBoardGamesOfHero(hero);
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x000327FC File Offset: 0x000309FC
		private void OnPlayerBoardGameOver(Hero opposingHero, BoardGameHelper.BoardGameState state)
		{
			if (opposingHero != null)
			{
				this.GameEndWithHero(opposingHero);
				if (state == 1)
				{
					this._opposingHeroExtraXPGained = this._difficulty != 2 && MBRandom.RandomFloat <= 0.5f;
					SkillLevelingManager.OnBoardGameWonAgainstLord(opposingHero, this._difficulty, this._opposingHeroExtraXPGained);
					float num = 0.1f;
					num += ((opposingHero.IsFemale != Hero.MainHero.IsFemale) ? 0.1f : 0f);
					num += (float)Hero.MainHero.GetSkillValue(DefaultSkills.Charm) / 100f;
					num += ((opposingHero.GetTraitLevel(DefaultTraits.Calculating) == 1) ? 0.2f : 0f);
					bool flag = MBRandom.RandomFloat <= num;
					bool flag2 = opposingHero.MapFaction == Hero.MainHero.MapFaction && this._difficulty == 2 && MBRandom.RandomFloat <= 0.4f;
					bool flag3 = this._difficulty == 2;
					if (flag)
					{
						ChangeRelationAction.ApplyPlayerRelation(opposingHero, 1, true, true);
						this._relationGained = true;
					}
					else if (flag2)
					{
						GainKingdomInfluenceAction.ApplyForBoardGameWon(opposingHero, 1f);
						this._influenceGained = true;
					}
					else if (flag3)
					{
						GainRenownAction.Apply(Hero.MainHero, 1f, false);
						this._renownGained = true;
					}
					else
					{
						this._gainedNothing = true;
					}
				}
			}
			else if (state == 1)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._betAmount, false);
				if (this._betAmount > 0)
				{
					this.PlayerWonAgainstTavernChampion();
				}
			}
			else if (state == 2)
			{
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, this._betAmount, false);
			}
			this.SetBetAmount(0);
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00032984 File Offset: 0x00030B84
		public void InitializeConversationVars()
		{
			ICampaignMission campaignMission = CampaignMission.Current;
			string text;
			if (campaignMission == null)
			{
				text = null;
			}
			else
			{
				Location location = campaignMission.Location;
				text = ((location != null) ? location.StringId : null);
			}
			if (!(text == "lordshall"))
			{
				ICampaignMission campaignMission2 = CampaignMission.Current;
				string text2;
				if (campaignMission2 == null)
				{
					text2 = null;
				}
				else
				{
					Location location2 = campaignMission2.Location;
					text2 = ((location2 != null) ? location2.StringId : null);
				}
				if (!(text2 == "tavern"))
				{
					return;
				}
			}
			CultureObject boardGameCulture = this.GetBoardGameCulture();
			CultureObject.BoardGameType boardGame = boardGameCulture.BoardGame;
			if (boardGame == -1)
			{
				MBDebug.ShowWarning("Boardgame not yet implemented, or not found.");
			}
			if (boardGame != -1)
			{
				MBTextManager.SetTextVariable("GAME_NAME", GameTexts.FindText("str_boardgame_name", boardGame.ToString()), false);
				MBTextManager.SetTextVariable("CULTURE_NAME", boardGameCulture.Name, false);
				MBTextManager.SetTextVariable("DIFFICULTY", GameTexts.FindText("str_boardgame_difficulty", this._difficulty.ToString()), false);
				MBTextManager.SetTextVariable("BET_AMOUNT", this._betAmount.ToString(), false);
				MBTextManager.SetTextVariable("IS_BETTING", (this._betAmount > 0) ? 1 : 0);
				Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetBoardGame(boardGame);
			}
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x00032AA0 File Offset: 0x00030CA0
		public void OnMissionStarted(IMission mission)
		{
			Mission mission2 = (Mission)mission;
			if (Mission.Current.Scene != null)
			{
				Mission.Current.Scene.FindEntityWithTag("boardgame") != null;
			}
			if (Mission.Current.Scene != null && Mission.Current.Scene.FindEntityWithTag("boardgame_holder") != null && CampaignMission.Current.Location != null && (CampaignMission.Current.Location.StringId == "lordshall" || CampaignMission.Current.Location.StringId == "tavern"))
			{
				mission2.AddMissionBehavior(new MissionBoardGameLogic());
				this.InitializeBoardGame();
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00032B65 File Offset: 0x00030D65
		private CultureObject GetBoardGameCulture()
		{
			if (CampaignMission.Current.Location.StringId == "lordshall")
			{
				return Settlement.CurrentSettlement.OwnerClan.Culture;
			}
			return Settlement.CurrentSettlement.Culture;
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00032B9C File Offset: 0x00030D9C
		private void InitializeBoardGame()
		{
			if (Campaign.Current.GameMode != 1)
			{
				return;
			}
			CultureObject.BoardGameType boardGame = this.GetBoardGameCulture().BoardGame;
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("boardgame_holder");
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			Mission.Current.Scene.RemoveEntity(gameEntity, 92);
			GameEntity gameEntity2 = GameEntity.Instantiate(Mission.Current.Scene, "BoardGame" + boardGame.ToString() + "_FullSetup", true);
			GameEntity gameEntity3 = gameEntity2;
			MatrixFrame matrixFrame = globalFrame.TransformToParent(gameEntity2.GetFrame());
			gameEntity3.SetGlobalFrame(ref matrixFrame);
			GameEntity firstChildEntityWithTag = MBExtensions.GetFirstChildEntityWithTag(gameEntity2, "dice_board");
			if (firstChildEntityWithTag != null && firstChildEntityWithTag.HasScriptOfType<VertexAnimator>())
			{
				firstChildEntityWithTag.GetFirstScriptOfType<VertexAnimator>().StopAndGoToEnd();
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00032C61 File Offset: 0x00030E61
		public void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (this._heroAndBoardGameTimeDictionary.ContainsKey(victim))
			{
				this._heroAndBoardGameTimeDictionary.Remove(victim);
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00032C80 File Offset: 0x00030E80
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (settlement.IsTown && CampaignMission.Current != null)
			{
				Location location = CampaignMission.Current.Location;
				int num;
				if (location != null && location.StringId == "tavern" && unusedUsablePointCount.TryGetValue("spawnpoint_tavernkeeper", out num) && num > 0)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(BoardGameCampaignBehavior.CreateGameHost), settlement.Culture, 0, 1);
				}
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00032CF4 File Offset: 0x00030EF4
		private static LocationCharacter CreateGameHost(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject tavernGamehost = culture.TavernGamehost;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(tavernGamehost.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(tavernGamehost, ref num, ref num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(tavernGamehost, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), "gambler_npc", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_villager"), true, false, null, false, false, true);
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x00032DA4 File Offset: 0x00030FA4
		protected void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("talk_common_to_taverngamehost", "start", "close_window", "{GAME_MASTER_INTRO}", () => this.conversation_talk_common_to_taverngamehost_on_condition() && !BoardGameCampaignBehavior.taverngamehost_player_sitting_now_on_condition(), null, 100, null);
			campaignGameStarter.AddDialogLine("talk_common_to_taverngamehost", "start", "taverngamehost_talk", "{=LGrzKlET}Let me know how much of a challenge you can stand and we'll get started. I'm ready to offer you a {DIFFICULTY} challenge and {?IS_BETTING}a bet of {BET_AMOUNT}{GOLD_ICON}.{?}friendly game.{\\?}", () => this.conversation_talk_common_to_taverngamehost_on_condition() && BoardGameCampaignBehavior.taverngamehost_player_sitting_now_on_condition(), null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_game", "taverngamehost_talk", "taverngamehost_think_play", "{=BdpW8gUM}That looks good, let's play!", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_change_difficulty", "taverngamehost_talk", "taverngamehost_change_difficulty", "{=MbwG7Gy8}Can I change the difficulty?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_change_bet", "taverngamehost_talk", "taverngamehost_change_bet", "{=PbDK3PIi}Can I change the amount we're betting?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_game_history", "taverngamehost_talk", "taverngamehost_learn_history", "{=YM7etEzu}What exactly is {GAME_NAME}?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_reject", "taverngamehost_talk", "close_window", "{=N7BFbQmT}I'm not interested.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("taverngamehost_start_playing_ask_accept", "taverngamehost_think_play", "taverngamehost_start_play", "{=GrHJYz7O}Very well. Now, what side do you want?", new ConversationSentence.OnConditionDelegate(this.taverngame_host_play_game_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_start_playing_ask_decline", "taverngamehost_think_play", "taverngamehost_talk", "{=bTnmpqU4}I'm afraid I don't have time for another game.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=7tuyySmq}I'll start.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_seega_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=J9fJlz2Y}You can start.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_seega_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=HdT5YyAb}I'll be white.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_puluc_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=i8HysulS}I'll be black.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_puluc_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=HdT5YyAb}I'll be white.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_konane_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=i8HysulS}I'll be black.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_konane_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=HdT5YyAb}I'll be white.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_mutorere_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=i8HysulS}I'll be black.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_mutorere_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=EnOOqaqf}I'll be sheep.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_baghchal_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=QjtOAyKE}I'll be wolves.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_baghchal_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_first", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=qsavxffL}I'll be attackers.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_tablut_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_one_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_last", "taverngamehost_start_play", "taverngamehost_confirm_play", "{=WD7vOalb}I'll be defenders.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_tablut_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_set_player_two_starts_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_back", "taverngamehost_start_play", "start", "{=dUSfRYYH}Just a minute..", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_start_playing_now", "taverngamehost_confirm_play", "close_window", "{=aB1EZssb}Great, let's begin!", null, new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_play_game_on_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("taverngamehost_ask_difficulty", "taverngamehost_change_difficulty", "taverngamehost_changing_difficulty", "{=9VR0VeNT}Yes, how easy should I make things for you?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_change_difficulty_easy", "taverngamehost_changing_difficulty", "start", "{=j9Weia10}Easy", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_difficulty_easy_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_change_difficulty_normal", "taverngamehost_changing_difficulty", "start", "{=8UBfIenN}Normal", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_difficulty_normal_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_change_difficulty_hard", "taverngamehost_changing_difficulty", "start", "{=OnaJowBF}Hard. Don't hold back or you'll regret it.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_difficulty_hard_on_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("taverngamehost_ask_betting", "taverngamehost_change_bet", "taverngamehost_changing_bet", "{=T5jd4m69}That will only make this more fun. How much were you thinking?", new ConversationSentence.OnConditionDelegate(this.conversation_taverngamehost_talk_place_bet_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_100_denars", "taverngamehost_changing_bet", "start", "{=T29epQk3}100{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_can_bet_100_denars_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_100_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_200_denars", "taverngamehost_changing_bet", "start", "{=mHm5SLhb}200{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_can_bet_200_denars_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_200_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_300_denars", "taverngamehost_changing_bet", "start", "{=LnbzQIz6}300{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_can_bet_300_denars_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_300_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_400_denars", "taverngamehost_changing_bet", "start", "{=ck36TZFP}400{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_can_bet_400_denars_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_400_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_500_denars", "taverngamehost_changing_bet", "start", "{=YHTTPKMb}500{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_can_bet_500_denars_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_500_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_0_denars", "taverngamehost_changing_bet", "start", "{=lVx35dWp}On second thought, let's keep this match friendly.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_bet_0_denars_on_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("taverngamehost_deny_betting", "taverngamehost_change_bet", "taverngamehost_changing_difficulty_for_bet", "{=4xtBNkjN}Unfortunately, I only allow betting when I'm playing at my best. You'll have to up the difficulty.", new ConversationSentence.OnConditionDelegate(this.conversation_taverngamehost_talk_not_place_bet_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_changing_difficulty_for_bet_yes", "taverngamehost_changing_difficulty_for_bet", "taverngamehost_change_bet_2", "{=i4xzuOJE}Sure, I'll play at the hardest level.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_taverngamehost_difficulty_hard_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_changing_difficulty_for_bet_no", "taverngamehost_changing_difficulty_for_bet", "start", "{=2ynnnR4c}I'd prefer to keep the difficulty where it's at.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("taverngamehost_ask_betting_2", "taverngamehost_change_bet_2", "taverngamehost_changing_bet", "{=GfHssUYV}Now, feel free to place a bet.", new ConversationSentence.OnConditionDelegate(this.conversation_taverngamehost_talk_place_bet_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_seega", "taverngamehost_learn_history", "taverngamehost_after_history", "{=9PUvbZzD}{GAME_NAME} is a traditional game within the {CULTURE_NAME}. It is a game of calm strategy. You start by placing your pieces on the board, crafting a trap for your enemy to fall into. Then you battle across the board, capturing and eliminating your oponent.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_seega_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_puluc", "taverngamehost_learn_history", "taverngamehost_after_history", "{=sVcJTu7K}{GAME_NAME} is fast and harsh, as warfare should be. Capture as much as possible to keep your opponent weakened and demoralized. But behind this endless offense, there should always be a strong defense to punish any attempt from your opponent to regain control.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_puluc_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_mutorere", "taverngamehost_learn_history", "taverngamehost_after_history", "{=SV0IEWD2}{GAME_NAME} is a game of anticipation. With no possibility of capturing, all your effort should be on reading your opponent and planning further ahead than him.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_mutorere_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_konane", "taverngamehost_learn_history", "taverngamehost_after_history", "{=tVb0nWxm}War is all about sacrifice. In {GAME_NAME} you must make sure that your opponent sacrifices more than you do. Every move can expose you or your opponent and must be carefully considered.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_konane_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_baghchal", "taverngamehost_learn_history", "taverngamehost_after_history", "{=mo4rbYvm}A couple of powerful wolves against a flock of helpless sheep. {GAME_NAME} is a game of uneven odds and seemingly all-powerful adversaries. But through strategy and sacrifice, even the sheep can dominate the wolves.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_baghchal_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("taverngamehost_tell_history_tablut", "taverngamehost_learn_history", "taverngamehost_after_history", "{=nMzfnOFG}{GAME_NAME} is a game of incredibly uneven odds. A weakened and trapped king must try to escape from a horde of attackers who assault from every direction. Ironic how we, the once all-powerful {CULTURE_NAME}, have now fallen in the same position.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_taverngamehost_talk_is_tablut_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_history_back", "taverngamehost_after_history", "start", "{=QP7L2YLG}Sounds fun.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("taverngamehost_player_history_leave", "taverngamehost_after_history", "close_window", "{=Ng6Rrlr6}I'd rather do something else", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("lord_player_play_game", "hero_main_options", "lord_answer_to_play_boardgame", "{=3hv4P5OO}Would you care to pass the time with a game of {GAME_NAME}?", new ConversationSentence.OnConditionDelegate(this.conversation_lord_talk_game_on_condition), null, 2, null, null);
			campaignGameStarter.AddPlayerLine("lord_player_cancel_boardgame", "hero_main_options", "lord_answer_to_cancel_play_boardgame", "{=ySk7bD8P}Actually, I have other things to do. Maybe later.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_lord_talk_cancel_game_on_condition), null, 2, null, null);
			campaignGameStarter.AddDialogLine("lord_agrees_cancel_play", "lord_answer_to_cancel_play_boardgame", "close_window", "{=dzXaXKaC}Very well.", null, new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_lord_talk_cancel_game_on_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("lord_player_ask_to_play_boardgame_again", "hero_main_options", "lord_answer_to_play_again_boardgame", "{=U342eACh}Would you like to play another round of {GAME_NAME}?", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.conversation_lord_talk_game_again_on_condition), null, 2, null, null);
			campaignGameStarter.AddDialogLine("lord_answer_to_play_boardgame_again_accept", "lord_answer_to_play_again_boardgame", "close_window", "{=aD1BoB3c}Yes. Let's have another round.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_play_game_on_condition), new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_lord_play_game_again_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("lord_answer_to_play_boardgame_again_decline", "lord_answer_to_play_again_boardgame", "hero_main_options", "{=fqKVojaV}No, not now.", null, new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_lord_dont_play_game_again_on_consequence), 100, null);
			campaignGameStarter.AddDialogLine("lord_after_player_win_boardgame", "start", "close_window", "{=!}{PLAYER_GAME_WON_LORD_STRING}", new ConversationSentence.OnConditionDelegate(this.lord_after_player_win_boardgame_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("lord_after_lord_win_boardgame", "start", "hero_main_options", "{=dC6YhgPP}Ah. A good match, that.", new ConversationSentence.OnConditionDelegate(BoardGameCampaignBehavior.lord_after_lord_win_boardgame_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("lord_agrees_play", "lord_answer_to_play_boardgame", "lord_setup_game", "{=!}{GAME_AGREEMENT_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_play_game_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_detect_difficulty_consequence), 100, null);
			campaignGameStarter.AddPlayerLine("lord_player_start_game", "lord_setup_game", "close_window", "{=bAy9PdrF}Let's begin, then.", null, new ConversationSentence.OnConsequenceDelegate(BoardGameCampaignBehavior.conversation_lord_play_game_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("lord_player_leave", "lord_setup_game", "close_window", "{=OQgBim7l}Actually, I have other things to do.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("lord_refuses_play", "lord_answer_to_play_boardgame", "close_window", "{=!}{LORD_REJECT_GAME_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_reject_game_condition), null, 100, null);
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x000337E0 File Offset: 0x000319E0
		private bool conversation_lord_reject_game_condition()
		{
			TextObject textObject = ((Hero.OneToOneConversationHero.GetRelationWithPlayer() > -20f) ? new TextObject("{=aRDcoLX0}Now is not a good time, {PLAYER.NAME}. ", null) : new TextObject("{=GLRrAj61}I do not wish to play games with the likes of you.", null));
			MBTextManager.SetTextVariable("LORD_REJECT_GAME_STRING", textObject, false);
			return true;
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00033824 File Offset: 0x00031A24
		private bool conversation_talk_common_to_taverngamehost_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.Occupation != 14)
			{
				return false;
			}
			this.InitializeConversationVars();
			MBTextManager.SetTextVariable("GAME_MASTER_INTRO", "{=HDhLMbt7}Greetings, traveler. Do you play {GAME_NAME}? I am reckoned a master of this game, the traditional pastime of the {CULTURE_NAME}. If you are interested in playing, take a seat and we'll start.", false);
			if (Settlement.CurrentSettlement.OwnerClan == Hero.MainHero.Clan || Settlement.CurrentSettlement.MapFaction.Leader == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("GAME_MASTER_INTRO", "{=yN4imaGo}Your {?PLAYER.GENDER}ladyship{?}lordship{\\?}... This is quite the honor. Do you play {GAME_NAME}? It's the traditional pastime of the {CULTURE_NAME}, and I am reckoned a master. If you wish to play a game, please, take a seat and we'll start.", false);
			}
			return true;
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00033894 File Offset: 0x00031A94
		private void conversation_taverngamehost_bet_0_denars_on_consequence()
		{
			this.SetBetAmount(0);
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x000338A0 File Offset: 0x00031AA0
		private static bool conversation_taverngamehost_can_bet_100_denars_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			CharacterObject characterObject = (CharacterObject)Agent.Main.Character;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 100;
			bool flag2 = characterObject.HeroObject.Gold >= 100;
			return flag && flag2;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x000338F5 File Offset: 0x00031AF5
		private void conversation_taverngamehost_bet_100_denars_on_consequence()
		{
			this.SetBetAmount(100);
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00033900 File Offset: 0x00031B00
		private static bool conversation_taverngamehost_can_bet_200_denars_on_condition()
		{
			CharacterObject characterObject = (CharacterObject)ConversationMission.OneToOneConversationAgent.Character;
			CharacterObject characterObject2 = (CharacterObject)Agent.Main.Character;
			bool flag = !characterObject.IsHero || characterObject.HeroObject.Gold >= 200;
			bool flag2 = characterObject2.HeroObject.Gold >= 200;
			return flag && flag2;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x00033965 File Offset: 0x00031B65
		private void conversation_taverngamehost_bet_200_denars_on_consequence()
		{
			this.SetBetAmount(200);
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x00033974 File Offset: 0x00031B74
		private static bool conversation_taverngamehost_can_bet_300_denars_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			CharacterObject characterObject = (CharacterObject)Agent.Main.Character;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 300;
			bool flag2 = characterObject.HeroObject.Gold >= 300;
			return flag && flag2;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x000339CF File Offset: 0x00031BCF
		private void conversation_taverngamehost_bet_300_denars_on_consequence()
		{
			this.SetBetAmount(300);
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x000339DC File Offset: 0x00031BDC
		private static bool conversation_taverngamehost_can_bet_400_denars_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			CharacterObject characterObject = (CharacterObject)Agent.Main.Character;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 400;
			bool flag2 = characterObject.HeroObject.Gold >= 400;
			return flag && flag2;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00033A37 File Offset: 0x00031C37
		private void conversation_taverngamehost_bet_400_denars_on_consequence()
		{
			this.SetBetAmount(400);
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00033A44 File Offset: 0x00031C44
		private static bool conversation_taverngamehost_can_bet_500_denars_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			CharacterObject characterObject = (CharacterObject)Agent.Main.Character;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 500;
			bool flag2 = characterObject.HeroObject.Gold >= 500;
			return flag && flag2;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00033A9F File Offset: 0x00031C9F
		private bool taverngame_host_play_game_on_condition()
		{
			if (this._betAmount == 0)
			{
				return true;
			}
			this.DeleteOldBoardGamesOfChampion();
			return !this._wonBoardGamesInOneWeekInSettlement.ContainsKey(Settlement.CurrentSettlement);
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00033AC4 File Offset: 0x00031CC4
		private void conversation_taverngamehost_bet_500_denars_on_consequence()
		{
			this.SetBetAmount(500);
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00033AD1 File Offset: 0x00031CD1
		private void conversation_taverngamehost_difficulty_easy_on_consequence()
		{
			this.SetDifficulty(0);
			this.SetBetAmount(0);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00033AE1 File Offset: 0x00031CE1
		private void conversation_taverngamehost_difficulty_normal_on_consequence()
		{
			this.SetDifficulty(1);
			this.SetBetAmount(0);
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x00033AF1 File Offset: 0x00031CF1
		private void conversation_taverngamehost_difficulty_hard_on_consequence()
		{
			this.SetDifficulty(2);
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x00033AFA File Offset: 0x00031CFA
		private static void conversation_lord_play_game_again_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().DetectOpposingAgent();
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().StartBoardGame();
			};
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00033B39 File Offset: 0x00031D39
		private static void conversation_lord_dont_play_game_again_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetGameOver(GameOverEnum.PlayerCanceledTheGame);
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x00033B4C File Offset: 0x00031D4C
		private void conversation_lord_detect_difficulty_consequence()
		{
			int skillValue = ConversationMission.OneToOneConversationCharacter.GetSkillValue(DefaultSkills.Steward);
			if (skillValue >= 0 && skillValue < 50)
			{
				this.SetDifficulty(0);
				return;
			}
			if (skillValue >= 50 && skillValue < 100)
			{
				this.SetDifficulty(1);
				return;
			}
			if (skillValue >= 100)
			{
				this.SetDifficulty(2);
			}
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00033B98 File Offset: 0x00031D98
		private static void conversation_taverngamehost_set_player_one_starts_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetStartingPlayer(true);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00033BAA File Offset: 0x00031DAA
		private static void conversation_taverngamehost_set_player_two_starts_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetStartingPlayer(false);
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00033BBC File Offset: 0x00031DBC
		private static void conversation_taverngamehost_play_game_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().DetectOpposingAgent();
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().StartBoardGame();
			};
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00033BFC File Offset: 0x00031DFC
		private bool conversation_taverngamehost_talk_place_bet_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 100;
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return flag && this._difficulty == 2;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00033C44 File Offset: 0x00031E44
		private bool conversation_taverngamehost_talk_not_place_bet_on_condition()
		{
			CharacterObject oneToOneConversationCharacter = ConversationMission.OneToOneConversationCharacter;
			bool flag = !oneToOneConversationCharacter.IsHero || oneToOneConversationCharacter.HeroObject.Gold >= 100;
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return flag && this._difficulty != 2;
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00033C90 File Offset: 0x00031E90
		private static bool conversation_taverngamehost_talk_is_seega_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 0;
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00033CB8 File Offset: 0x00031EB8
		private static bool conversation_taverngamehost_talk_is_puluc_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 1;
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00033CE0 File Offset: 0x00031EE0
		private static bool conversation_taverngamehost_talk_is_mutorere_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 3;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00033D08 File Offset: 0x00031F08
		private static bool conversation_taverngamehost_talk_is_konane_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 2;
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00033D30 File Offset: 0x00031F30
		private static bool conversation_taverngamehost_talk_is_baghchal_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 5;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00033D58 File Offset: 0x00031F58
		private static bool conversation_taverngamehost_talk_is_tablut_on_condition()
		{
			MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
			return missionBehavior != null && missionBehavior.CurrentBoardGame == 4;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x00033D80 File Offset: 0x00031F80
		public static bool taverngamehost_player_sitting_now_on_condition()
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("gambler_player");
			if (gameEntity != null)
			{
				Chair chair = MBExtensions.CollectObjects<Chair>(gameEntity).FirstOrDefault<Chair>();
				return chair != null && Agent.Main != null && chair.IsAgentFullySitting(Agent.Main);
			}
			return false;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00033DD0 File Offset: 0x00031FD0
		private bool conversation_lord_talk_game_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.Occupation == 3)
			{
				ICampaignMission campaignMission = CampaignMission.Current;
				string text;
				if (campaignMission == null)
				{
					text = null;
				}
				else
				{
					Location location = campaignMission.Location;
					text = ((location != null) ? location.StringId : null);
				}
				if (text == "lordshall" && MissionBoardGameLogic.IsBoardGameAvailable())
				{
					this.InitializeConversationVars();
					return true;
				}
			}
			return false;
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00033E23 File Offset: 0x00032023
		private static bool conversation_lord_talk_game_again_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.Occupation == 3 && MissionBoardGameLogic.IsThereActiveBoardGameWithHero(Hero.OneToOneConversationHero) && Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().IsGameInProgress;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00033E50 File Offset: 0x00032050
		private static bool conversation_lord_talk_cancel_game_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.Occupation == 3 && MissionBoardGameLogic.IsThereActiveBoardGameWithHero(Hero.OneToOneConversationHero) && (Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().IsOpposingAgentMovingToPlayingChair || !Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().IsGameInProgress);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00033E9D File Offset: 0x0003209D
		private static void conversation_lord_talk_cancel_game_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetGameOver(GameOverEnum.PlayerCanceledTheGame);
			};
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x00033ED0 File Offset: 0x000320D0
		private static bool lord_after_lord_win_boardgame_condition()
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			return missionBoardGameLogic != null && missionBoardGameLogic.BoardGameFinalState != null && missionBoardGameLogic.BoardGameFinalState != 1;
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x00033F08 File Offset: 0x00032108
		private bool lord_after_player_win_boardgame_condition()
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			if (missionBoardGameLogic != null && missionBoardGameLogic.BoardGameFinalState == 1)
			{
				if (this._relationGained)
				{
					MBTextManager.SetTextVariable("PLAYER_GAME_WON_LORD_STRING", "{=QTfliM5b}I enjoyed our game. Let?s play again later.", false);
				}
				else if (this._influenceGained)
				{
					MBTextManager.SetTextVariable("PLAYER_GAME_WON_LORD_STRING", "{=31oG5njl}You are a sharp thinker. Our kingdom would do well to hear your thoughts on matters of importance.", false);
				}
				else if (this._opposingHeroExtraXPGained)
				{
					MBTextManager.SetTextVariable("PLAYER_GAME_WON_LORD_STRING", "{=nxpyHb77}Well, I am still a novice in this game, but I learned a lot from playing with you.", false);
				}
				else if (this._renownGained)
				{
					MBTextManager.SetTextVariable("PLAYER_GAME_WON_LORD_STRING", "{=k1b5crrx}You are an accomplished player. I will take note of that.", false);
				}
				else if (this._gainedNothing)
				{
					MBTextManager.SetTextVariable("PLAYER_GAME_WON_LORD_STRING", "{=HzabMi4t}That was a fun game. Thank you.", false);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x00033FBC File Offset: 0x000321BC
		private bool conversation_lord_play_game_on_condition()
		{
			if (this.CanPlayerPlayBoardGameAgainstHero(Hero.OneToOneConversationHero))
			{
				string text = "DrinkingInTavernTag";
				if (MissionConversationLogic.Current.ConversationManager.IsTagApplicable(text, Hero.OneToOneConversationHero.CharacterObject))
				{
					MBTextManager.SetTextVariable("GAME_AGREEMENT_STRING", "{=LztDzy8W}Why not? I'm not going anywhere right now, and I could use another drink.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt)
				{
					MBTextManager.SetTextVariable("GAME_AGREEMENT_STRING", "{=2luygc8o}Mm. I suppose. Takes my mind off all these problems I have to deal with.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaEarnest)
				{
					MBTextManager.SetTextVariable("GAME_AGREEMENT_STRING", "{=349mwgWC}Certainly. A good game always keeps the mind active and fresh.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaIronic)
				{
					MBTextManager.SetTextVariable("GAME_AGREEMENT_STRING", "{=rGaaVBBT}Ah. Very well. I don't mind testing your mettle.", false);
				}
				else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaSoftspoken)
				{
					MBTextManager.SetTextVariable("GAME_AGREEMENT_STRING", "{=idPV1Csj}Yes... Why not? I have nothing too urgent right now.", false);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x000340B1 File Offset: 0x000322B1
		private static void conversation_lord_play_game_on_consequence()
		{
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().DetectOpposingAgent();
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x000340C2 File Offset: 0x000322C2
		public void PlayerWonAgainstTavernChampion()
		{
			if (!this._wonBoardGamesInOneWeekInSettlement.ContainsKey(Settlement.CurrentSettlement))
			{
				this._wonBoardGamesInOneWeekInSettlement.Add(Settlement.CurrentSettlement, CampaignTime.Now);
			}
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x000340EC File Offset: 0x000322EC
		private void GameEndWithHero(Hero hero)
		{
			if (this._heroAndBoardGameTimeDictionary.ContainsKey(hero))
			{
				this._heroAndBoardGameTimeDictionary[hero].Add(CampaignTime.Now);
				return;
			}
			this._heroAndBoardGameTimeDictionary.Add(hero, new List<CampaignTime>());
			this._heroAndBoardGameTimeDictionary[hero].Add(CampaignTime.Now);
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00034148 File Offset: 0x00032348
		private bool CanPlayerPlayBoardGameAgainstHero(Hero hero)
		{
			if (hero.GetRelationWithPlayer() < 0f)
			{
				return false;
			}
			this.DeleteOldBoardGamesOfHero(hero);
			if (this._heroAndBoardGameTimeDictionary.ContainsKey(hero))
			{
				List<CampaignTime> list = this._heroAndBoardGameTimeDictionary[hero];
				return 3 > list.Count;
			}
			return true;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00034194 File Offset: 0x00032394
		private void DeleteOldBoardGamesOfChampion()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if (this._wonBoardGamesInOneWeekInSettlement.ContainsKey(settlement) && this._wonBoardGamesInOneWeekInSettlement[settlement].ElapsedWeeksUntilNow >= 1f)
				{
					this._wonBoardGamesInOneWeekInSettlement.Remove(settlement);
				}
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x00034218 File Offset: 0x00032418
		private void DeleteOldBoardGamesOfHero(Hero hero)
		{
			if (this._heroAndBoardGameTimeDictionary.ContainsKey(hero))
			{
				List<CampaignTime> list = this._heroAndBoardGameTimeDictionary[hero];
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (list[i].ElapsedDaysUntilNow > 1f)
					{
						list.RemoveAt(i);
					}
				}
				if (Extensions.IsEmpty<CampaignTime>(list))
				{
					this._heroAndBoardGameTimeDictionary.Remove(hero);
				}
			}
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00034285 File Offset: 0x00032485
		public void SetBetAmount(int bet)
		{
			this._betAmount = bet;
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetBetAmount(bet);
			MBTextManager.SetTextVariable("BET_AMOUNT", bet.ToString(), false);
			MBTextManager.SetTextVariable("IS_BETTING", (bet > 0) ? 1 : 0);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x000342C2 File Offset: 0x000324C2
		private void SetDifficulty(BoardGameHelper.AIDifficulty difficulty)
		{
			this._difficulty = difficulty;
			Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().SetCurrentDifficulty(difficulty);
			MBTextManager.SetTextVariable("DIFFICULTY", GameTexts.FindText("str_boardgame_difficulty", difficulty.ToString()), false);
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x00034300 File Offset: 0x00032500
		[CommandLineFunctionality.CommandLineArgumentFunction("win_board_game", "campaign")]
		public static string WinCurrentGame(List<string> strings)
		{
			Mission mission = Mission.Current;
			MissionBoardGameLogic missionBoardGameLogic = ((mission != null) ? mission.GetMissionBehavior<MissionBoardGameLogic>() : null);
			if (missionBoardGameLogic == null)
			{
				return "There is no board game.";
			}
			missionBoardGameLogic.PlayerOneWon("str_boardgame_victory_message");
			return "Ok";
		}

		// Token: 0x040002EB RID: 747
		private const int NumberOfBoardGamesCanPlayerPlayAgainstHeroPerDay = 3;

		// Token: 0x040002EC RID: 748
		private Dictionary<Hero, List<CampaignTime>> _heroAndBoardGameTimeDictionary = new Dictionary<Hero, List<CampaignTime>>();

		// Token: 0x040002ED RID: 749
		private Dictionary<Settlement, CampaignTime> _wonBoardGamesInOneWeekInSettlement = new Dictionary<Settlement, CampaignTime>();

		// Token: 0x040002EE RID: 750
		private BoardGameHelper.AIDifficulty _difficulty;

		// Token: 0x040002EF RID: 751
		private int _betAmount;

		// Token: 0x040002F0 RID: 752
		private bool _influenceGained;

		// Token: 0x040002F1 RID: 753
		private bool _renownGained;

		// Token: 0x040002F2 RID: 754
		private bool _opposingHeroExtraXPGained;

		// Token: 0x040002F3 RID: 755
		private bool _relationGained;

		// Token: 0x040002F4 RID: 756
		private bool _gainedNothing;
	}
}
