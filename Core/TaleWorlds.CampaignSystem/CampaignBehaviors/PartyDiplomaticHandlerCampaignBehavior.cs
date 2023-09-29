using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PartyDiplomaticHandlerCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnMapEventContinuityNeedsUpdateEvent.AddNonSerializedListener(this, new Action<IFaction>(this.OnMapEventContinuityNeedsUpdate));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnSessionLaunched(CampaignGameStarter gameSystemInitializer)
		{
			gameSystemInitializer.AddGameMenu("hostile_action_end_by_peace", "{=1rbg3Hz2}The {FACTION_1} and {FACTION_2} are no longer enemies.", new OnInitDelegate(this.game_menu_hostile_action_end_by_peace_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("hostile_action_end_by_peace", "hostile_action_en_by_peace_end", "{=WVkc4UgX}Continue.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.Finish(true);
					return;
				}
				GameMenu.ExitToLast();
			}, true, -1, false, null);
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool arg5)
		{
			if (newKingdom != null)
			{
				CampaignEventDispatcher.Instance.OnMapEventContinuityNeedsUpdate(newKingdom);
				return;
			}
			CampaignEventDispatcher.Instance.OnMapEventContinuityNeedsUpdate(clan);
		}

		private void OnMapEventContinuityNeedsUpdate(IFaction faction)
		{
			this.CheckMapEvents(faction);
			this.CheckSiegeEvents(faction);
			this.CheckFactionPartiesAndSettlements(faction);
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero hero1, Hero hero2, Hero hero3, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (detail != ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege && settlement.SiegeEvent != null)
			{
				this.CheckSiegeEventContinuity(settlement.SiegeEvent);
			}
			this.CheckSettlementSuitabilityForParties(settlement.Parties);
		}

		private void CheckFactionPartiesAndSettlements(IFaction faction)
		{
			this.CheckSettlementSuitabilityForParties(faction.WarPartyComponents.Select((WarPartyComponent x) => x.MobileParty));
			foreach (Settlement settlement in faction.Settlements)
			{
				this.CheckSettlementSuitabilityForParties(settlement.Parties);
			}
		}

		private void CheckMapEvents(IFaction faction)
		{
			MapEventManager mapEventManager = Campaign.Current.MapEventManager;
			List<MapEvent> list = ((mapEventManager != null) ? mapEventManager.MapEvents : null);
			List<MapEvent> list2 = new List<MapEvent>();
			Func<PartyBase, bool> <>9__0;
			foreach (MapEvent mapEvent in list)
			{
				IEnumerable<PartyBase> involvedParties = mapEvent.InvolvedParties;
				Func<PartyBase, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (PartyBase x) => x.MapFaction == faction);
				}
				if (involvedParties.Any(func))
				{
					list2.Add(mapEvent);
				}
			}
			foreach (MapEvent mapEvent2 in list2)
			{
				foreach (MapEventParty mapEventParty in mapEvent2.AttackerSide.Parties.ToList<MapEventParty>())
				{
					if (!mapEvent2.CanPartyJoinBattle(mapEventParty.Party, BattleSideEnum.Attacker))
					{
						if (mapEvent2.IsPlayerMapEvent)
						{
							IFaction faction2 = mapEventParty.Party.MapFaction;
							if (mapEvent2.PlayerSide == BattleSideEnum.Attacker)
							{
								faction2 = mapEvent2.DefenderSide.Parties.First((MapEventParty x) => !x.Party.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction)).Party.MapFaction;
							}
							this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent = faction2;
							GameMenu.ActivateGameMenu("hostile_action_end_by_peace");
						}
						mapEventParty.Party.MapEventSide = null;
					}
				}
			}
		}

		private void CheckSiegeEvents(IFaction faction)
		{
			SiegeEventManager siegeEventManager = Campaign.Current.SiegeEventManager;
			List<SiegeEvent> list = ((siegeEventManager != null) ? siegeEventManager.SiegeEvents : null);
			List<SiegeEvent> list2 = new List<SiegeEvent>();
			Func<PartyBase, bool> <>9__0;
			foreach (SiegeEvent siegeEvent in list)
			{
				if (!siegeEvent.ReadyToBeRemoved)
				{
					IEnumerable<PartyBase> involvedPartiesForEventType = siegeEvent.GetInvolvedPartiesForEventType(siegeEvent.GetCurrentBattleType());
					Func<PartyBase, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (PartyBase x) => x.MapFaction == faction);
					}
					if (involvedPartiesForEventType.Any(func))
					{
						list2.Add(siegeEvent);
					}
				}
			}
			foreach (SiegeEvent siegeEvent2 in list2)
			{
				this.CheckSiegeEventContinuity(siegeEvent2);
			}
		}

		private void CheckSiegeEventContinuity(SiegeEvent siegeEvent)
		{
			foreach (PartyBase partyBase in siegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(siegeEvent.GetCurrentBattleType()).ToList<PartyBase>())
			{
				if (!siegeEvent.CanPartyJoinSide(partyBase, BattleSideEnum.Attacker))
				{
					if (PlayerSiege.PlayerSiegeEvent == siegeEvent && PlayerSiege.PlayerSide == BattleSideEnum.Attacker)
					{
						IFaction mapFaction = siegeEvent.BesiegedSettlement.MapFaction;
						if (mapFaction != Clan.PlayerClan.MapFaction)
						{
							this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent = mapFaction;
							GameMenu.ActivateGameMenu("hostile_action_end_by_peace");
						}
					}
					partyBase.MobileParty.BesiegerCamp = null;
				}
			}
		}

		private void CheckSettlementSuitabilityForParties(IEnumerable<MobileParty> parties)
		{
			foreach (MobileParty mobileParty in parties.ToList<MobileParty>())
			{
				if (mobileParty.CurrentSettlement != null && mobileParty.MapFaction.IsAtWarWith(mobileParty.CurrentSettlement.MapFaction))
				{
					if (mobileParty != MobileParty.MainParty)
					{
						if (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty)
						{
							if (mobileParty.Army != null && mobileParty.Army.Parties.Contains(MobileParty.MainParty))
							{
								GameMenu.SwitchToMenu("army_left_settlement_due_to_war_declaration");
							}
							else
							{
								Settlement currentSettlement = mobileParty.CurrentSettlement;
								LeaveSettlementAction.ApplyForParty(mobileParty);
								SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, currentSettlement);
							}
						}
					}
					else
					{
						GameMenu.SwitchToMenu("fortification_crime_rating");
					}
				}
			}
		}

		[GameMenuInitializationHandler("hostile_action_end_by_peace")]
		public static void hostile_action_end_by_peace_on_init(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.BesiegedSettlement != null)
			{
				args.MenuContext.SetBackgroundMeshName(MobileParty.MainParty.BesiegedSettlement.SettlementComponent.WaitMeshName);
				return;
			}
			if (MobileParty.MainParty.LastVisitedSettlement != null)
			{
				args.MenuContext.SetBackgroundMeshName(MobileParty.MainParty.LastVisitedSettlement.SettlementComponent.WaitMeshName);
				return;
			}
			if (PlayerEncounter.EncounterSettlement != null)
			{
				args.MenuContext.SetBackgroundMeshName(PlayerEncounter.EncounterSettlement.SettlementComponent.WaitMeshName);
				return;
			}
			Debug.FailedAssert("no menu background to initialize!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PartyDiplomaticHandlerCampaignBehavior.cs", "hostile_action_end_by_peace_on_init", 224);
		}

		private void game_menu_hostile_action_end_by_peace_on_init(MenuCallbackArgs args)
		{
			if (this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent == null)
			{
				StanceLink stanceLink = (from x in MobileParty.MainParty.MapFaction.Stances
					where !x.IsAtWar
					orderby x.PeaceDeclarationDate.ElapsedHoursUntilNow
					select x).FirstOrDefault<StanceLink>();
				this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent = ((stanceLink.Faction1 != Clan.PlayerClan.MapFaction) ? stanceLink.Faction1 : stanceLink.Faction2);
			}
			GameTexts.SetVariable("FACTION_1", Clan.PlayerClan.MapFaction.EncyclopediaLinkWithName);
			GameTexts.SetVariable("FACTION_2", this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent.EncyclopediaLinkWithName);
			if (PlayerEncounter.Battle != null)
			{
				PlayerEncounter.Battle.DiplomaticallyFinished = true;
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<IFaction>("_lastFactionMadePeaceWithCausedPlayerToLeaveEvent", ref this._lastFactionMadePeaceWithCausedPlayerToLeaveEvent);
		}

		private IFaction _lastFactionMadePeaceWithCausedPlayerToLeaveEvent;
	}
}
