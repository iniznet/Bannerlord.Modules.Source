using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x02000280 RID: 640
	public class TournamentCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600218E RID: 8590 RVA: 0x0008E918 File Offset: 0x0008CB18
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.TownRebelliosStateChanged.AddNonSerializedListener(this, new Action<Town, bool>(this.OnTownRebelliousStateChanged));
			CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventStarted));
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x0008E9F4 File Offset: 0x0008CBF4
		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			Campaign.Current.TournamentManager.InitializeLeaderboardEntry(Hero.MainHero, 0);
			this.InitializeTournamentLeaderboard();
			for (int i = 0; i < 3; i++)
			{
				foreach (Town town in Town.AllTowns)
				{
					if (town.IsTown)
					{
						this.ConsiderStartOrEndTournament(town);
					}
				}
			}
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x0008EA78 File Offset: 0x0008CC78
		private void OnDailyTick()
		{
			Hero leaderBoardLeader = Campaign.Current.TournamentManager.GetLeaderBoardLeader();
			if (leaderBoardLeader != null && leaderBoardLeader.IsAlive && leaderBoardLeader.Clan != null)
			{
				leaderBoardLeader.Clan.AddRenown(1f, true);
			}
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x0008EABC File Offset: 0x0008CCBC
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			foreach (Town town in Town.AllTowns)
			{
				TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(town);
				if (tournamentGame != null && tournamentGame.Prize != null && (tournamentGame.Prize == DefaultItems.Trash || !tournamentGame.Prize.IsReady))
				{
					tournamentGame.UpdateTournamentPrize(false, true);
				}
			}
			foreach (KeyValuePair<Town, CampaignTime> keyValuePair in this._lastCreatedTournamentDatesInTowns.ToList<KeyValuePair<Town, CampaignTime>>())
			{
				if (keyValuePair.Value.ElapsedDaysUntilNow >= 15f)
				{
					this._lastCreatedTournamentDatesInTowns.Remove(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x0008EBB0 File Offset: 0x0008CDB0
		private void OnTownRebelliousStateChanged(Town town, bool rebelliousState)
		{
			if (town.InRebelliousState)
			{
				TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(town);
				if (tournamentGame != null)
				{
					Campaign.Current.TournamentManager.ResolveTournament(tournamentGame, town);
				}
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x0008EBEC File Offset: 0x0008CDEC
		private void OnSiegeEventStarted(SiegeEvent siegeEvent)
		{
			Town town = siegeEvent.BesiegedSettlement.Town;
			if (town != null)
			{
				TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(town);
				if (tournamentGame != null)
				{
					Campaign.Current.TournamentManager.ResolveTournament(tournamentGame, town);
				}
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x0008EC2D File Offset: 0x0008CE2D
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Town, CampaignTime>>("_lastCreatedTournamentTimesInTowns", ref this._lastCreatedTournamentDatesInTowns);
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x0008EC41 File Offset: 0x0008CE41
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			Campaign.Current.TournamentManager.DeleteLeaderboardEntry(victim);
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x0008EC53 File Offset: 0x0008CE53
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x0008EC63 File Offset: 0x0008CE63
		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsTown)
			{
				this.ConsiderStartOrEndTournament(settlement.Town);
			}
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x0008EC7C File Offset: 0x0008CE7C
		private void ConsiderStartOrEndTournament(Town town)
		{
			CampaignTime campaignTime;
			if (!this._lastCreatedTournamentDatesInTowns.TryGetValue(town, out campaignTime) || campaignTime.ElapsedDaysUntilNow >= 15f)
			{
				ITournamentManager tournamentManager = Campaign.Current.TournamentManager;
				TournamentGame tournamentGame = tournamentManager.GetTournamentGame(town);
				if (tournamentGame != null && tournamentGame.CreationTime.ElapsedDaysUntilNow >= (float)tournamentGame.RemoveTournamentAfterDays)
				{
					tournamentManager.ResolveTournament(tournamentGame, town);
				}
				if (tournamentGame == null)
				{
					if (MBRandom.RandomFloat < Campaign.Current.Models.TournamentModel.GetTournamentStartChance(town))
					{
						tournamentManager.AddTournament(Campaign.Current.Models.TournamentModel.CreateTournament(town));
						if (!this._lastCreatedTournamentDatesInTowns.ContainsKey(town))
						{
							this._lastCreatedTournamentDatesInTowns.Add(town, CampaignTime.Now);
							return;
						}
						this._lastCreatedTournamentDatesInTowns[town] = CampaignTime.Now;
						return;
					}
				}
				else if (tournamentGame.CreationTime.ElapsedDaysUntilNow < (float)tournamentGame.RemoveTournamentAfterDays && MBRandom.RandomFloat < Campaign.Current.Models.TournamentModel.GetTournamentEndChance(tournamentGame))
				{
					tournamentManager.ResolveTournament(tournamentGame, town);
				}
			}
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x0008ED8C File Offset: 0x0008CF8C
		private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			if (winner.IsHero && winner.HeroObject.Clan != null)
			{
				winner.HeroObject.Clan.AddRenown((float)Campaign.Current.Models.TournamentModel.GetRenownReward(winner.HeroObject, town), true);
				GainKingdomInfluenceAction.ApplyForDefault(winner.HeroObject, (float)Campaign.Current.Models.TournamentModel.GetInfluenceReward(winner.HeroObject, town));
			}
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x0008EE02 File Offset: 0x0008D002
		private float GetTournamentSimulationScore(Hero hero)
		{
			return Campaign.Current.Models.TournamentModel.GetTournamentSimulationScore(hero.CharacterObject);
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x0008EE20 File Offset: 0x0008D020
		private void InitializeTournamentLeaderboard()
		{
			Hero[] array = Hero.AllAliveHeroes.Where((Hero x) => x.IsLord && this.GetTournamentSimulationScore(x) > 1.5f).ToArray<Hero>();
			int numLeaderboardVictoriesAtGameStart = Campaign.Current.Models.TournamentModel.GetNumLeaderboardVictoriesAtGameStart();
			if (array.Length < 3)
			{
				return;
			}
			List<Hero> list = new List<Hero>();
			for (int i = 0; i < numLeaderboardVictoriesAtGameStart; i++)
			{
				list.Clear();
				for (int j = 0; j < 16; j++)
				{
					Hero hero = array[MBRandom.RandomInt(array.Length)];
					list.Add(hero);
				}
				Hero hero2 = null;
				float num = 0f;
				foreach (Hero hero3 in list)
				{
					float num2 = this.GetTournamentSimulationScore(hero3) * (0.8f + 0.2f * MBRandom.RandomFloat);
					if (num2 > num)
					{
						num = num2;
						hero2 = hero3;
					}
				}
				Campaign.Current.TournamentManager.AddLeaderboardEntry(hero2);
				hero2.Clan.AddRenown((float)Campaign.Current.Models.TournamentModel.GetRenownReward(hero2, null), true);
			}
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x0008EF50 File Offset: 0x0008D150
		protected void AddDialogs(CampaignGameStarter campaignGameSystemStarter)
		{
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x0008EF54 File Offset: 0x0008D154
		protected void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("town_arena", "join_tournament", "{=LN09ZLXZ}Join the tournament", new GameMenuOption.OnConditionDelegate(this.game_menu_join_tournament_on_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("menu_town_tournament_join");
			}, false, 1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_arena", "mno_tournament_event_watch", "{=6bQIRaIl}Watch the tournament", new GameMenuOption.OnConditionDelegate(this.game_menu_tournament_watch_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_tournament_watch_current_game_on_consequence), false, 2, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_arena", "mno_see_tournament_leaderboard", "{=vGF5S2hE}Leaderboard", new GameMenuOption.OnConditionDelegate(TournamentCampaignBehavior.game_menu_town_arena_see_leaderboard_on_condition), null, false, 3, false, null);
			campaignGameSystemStarter.AddGameMenu("menu_town_tournament_join", "{=5Adr6toM}{MENU_TEXT}", new OnInitDelegate(this.game_menu_tournament_join_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_town_tournament_join", "mno_tournament_event_1", "{=es0Y3Bxc}Join", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Mission;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(this.game_menu_tournament_join_current_game_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_town_tournament_join", "mno_tournament_leave", "{=3sRdGQou}Leave", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("town_arena");
			}, true, -1, false, null);
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x0008F0B6 File Offset: 0x0008D2B6
		[GameMenuEventHandler("town_arena", "mno_see_tournament_leaderboard", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_ui_town_arena_see_leaderboard_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTournamentLeaderboards();
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x0008F0C4 File Offset: 0x0008D2C4
		private bool game_menu_join_tournament_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x0008F101 File Offset: 0x0008D301
		private static bool game_menu_town_arena_see_leaderboard_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leaderboard;
			return Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown;
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x0008F120 File Offset: 0x0008D320
		[GameMenuInitializationHandler("menu_town_tournament_join")]
		private static void game_menu_ui_town_ui_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Town.WaitMeshName);
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x0008F14C File Offset: 0x0008D34C
		private void game_menu_tournament_join_on_init(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			tournamentGame.UpdateTournamentPrize(true, false);
			GameTexts.SetVariable("MENU_TEXT", tournamentGame.GetMenuText());
		}

		// Token: 0x060021A3 RID: 8611 RVA: 0x0008F18C File Offset: 0x0008D38C
		private void game_menu_tournament_join_current_game_on_consequence(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			GameMenu.SwitchToMenu("town");
			tournamentGame.PrepareForTournamentGame(true);
			Campaign.Current.TournamentManager.OnPlayerJoinTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x0008F1E0 File Offset: 0x0008D3E0
		private bool game_menu_tournament_watch_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.WatchTournament, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x0008F21C File Offset: 0x0008D41C
		private void game_menu_tournament_watch_current_game_on_consequence(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			GameMenu.SwitchToMenu("town");
			tournamentGame.PrepareForTournamentGame(false);
			Campaign.Current.TournamentManager.OnPlayerWatchTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
		}

		// Token: 0x04000A73 RID: 2675
		private const int TournamentCooldownDurationAsDays = 15;

		// Token: 0x04000A74 RID: 2676
		private Dictionary<Town, CampaignTime> _lastCreatedTournamentDatesInTowns = new Dictionary<Town, CampaignTime>();
	}
}
