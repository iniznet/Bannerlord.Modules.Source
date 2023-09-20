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
	public class TournamentCampaignBehavior : CampaignBehaviorBase
	{
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

		private void OnDailyTick()
		{
			Hero leaderBoardLeader = Campaign.Current.TournamentManager.GetLeaderBoardLeader();
			if (leaderBoardLeader != null && leaderBoardLeader.IsAlive && leaderBoardLeader.Clan != null)
			{
				leaderBoardLeader.Clan.AddRenown(1f, true);
			}
		}

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

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Town, CampaignTime>>("_lastCreatedTournamentTimesInTowns", ref this._lastCreatedTournamentDatesInTowns);
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			Campaign.Current.TournamentManager.DeleteLeaderboardEntry(victim);
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
			this.AddGameMenus(campaignGameStarter);
		}

		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsTown)
			{
				this.ConsiderStartOrEndTournament(settlement.Town);
			}
		}

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

		private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			if (winner.IsHero && winner.HeroObject.Clan != null)
			{
				winner.HeroObject.Clan.AddRenown((float)Campaign.Current.Models.TournamentModel.GetRenownReward(winner.HeroObject, town), true);
				GainKingdomInfluenceAction.ApplyForDefault(winner.HeroObject, (float)Campaign.Current.Models.TournamentModel.GetInfluenceReward(winner.HeroObject, town));
			}
		}

		private float GetTournamentSimulationScore(Hero hero)
		{
			return Campaign.Current.Models.TournamentModel.GetTournamentSimulationScore(hero.CharacterObject);
		}

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

		protected void AddDialogs(CampaignGameStarter campaignGameSystemStarter)
		{
		}

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

		[GameMenuEventHandler("town_arena", "mno_see_tournament_leaderboard", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_ui_town_arena_see_leaderboard_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTournamentLeaderboards();
		}

		private bool game_menu_join_tournament_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.JoinTournament, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		private static bool game_menu_town_arena_see_leaderboard_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leaderboard;
			return Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown;
		}

		[GameMenuInitializationHandler("menu_town_tournament_join")]
		private static void game_menu_ui_town_ui_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Town.WaitMeshName);
		}

		private void game_menu_tournament_join_on_init(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			tournamentGame.UpdateTournamentPrize(true, false);
			GameTexts.SetVariable("MENU_TEXT", tournamentGame.GetMenuText());
		}

		private void game_menu_tournament_join_current_game_on_consequence(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			GameMenu.SwitchToMenu("town");
			tournamentGame.PrepareForTournamentGame(true);
			Campaign.Current.TournamentManager.OnPlayerJoinTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
		}

		private bool game_menu_tournament_watch_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.WatchTournament, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		private void game_menu_tournament_watch_current_game_on_consequence(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			GameMenu.SwitchToMenu("town");
			tournamentGame.PrepareForTournamentGame(false);
			Campaign.Current.TournamentManager.OnPlayerWatchTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
		}

		private const int TournamentCooldownDurationAsDays = 15;

		private Dictionary<Town, CampaignTime> _lastCreatedTournamentDatesInTowns = new Dictionary<Town, CampaignTime>();
	}
}
