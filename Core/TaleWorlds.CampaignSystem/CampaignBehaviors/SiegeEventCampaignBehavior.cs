using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D5 RID: 981
	public class SiegeEventCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CEB RID: 3307
		// (get) Token: 0x06003B20 RID: 15136 RVA: 0x00116044 File Offset: 0x00114244
		private TextObject _currentSiegeDescription
		{
			get
			{
				if (PlayerSiege.PlayerSiegeEvent == null)
				{
					return TextObject.Empty;
				}
				TextObject textObject = ((PlayerSiege.PlayerSide == BattleSideEnum.Attacker) ? this._attackerSummaryText : this._defenderSummaryText);
				Settlement settlement = PlayerEncounter.EncounterSettlement ?? PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
				textObject.SetTextVariable("SETTLEMENT", settlement.Name);
				Hero leaderOfSiegeEvent = Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(PlayerSiege.PlayerSiegeEvent, PlayerSiege.PlayerSide);
				if (leaderOfSiegeEvent == Hero.MainHero)
				{
					TextObject textObject2 = ((PlayerSiege.PlayerSide == BattleSideEnum.Attacker) ? new TextObject("{=0DpoSNky}You are commanding the besiegers.", null) : new TextObject("{=W0FR7yy0}You are commanding the defenders.", null));
					textObject.SetTextVariable("FURTHER_EXPLANATION", textObject2);
				}
				else if (leaderOfSiegeEvent != null)
				{
					TextObject textObject3 = ((PlayerSiege.PlayerSide == BattleSideEnum.Attacker) ? new TextObject("{=d2spYiHG}{LEADER.NAME} is commanding the besiegers.", null) : new TextObject("{=Ja8dMYHi}{LEADER.NAME} is commanding the defenders.", null));
					StringHelpers.SetCharacterProperties("LEADER", leaderOfSiegeEvent.CharacterObject, textObject3, false);
					textObject.SetTextVariable("FURTHER_EXPLANATION", textObject3);
				}
				else
				{
					textObject.SetTextVariable("FURTHER_EXPLANATION", TextObject.Empty);
				}
				return textObject;
			}
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x00116150 File Offset: 0x00114350
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.AISiegeEventStarted));
			CampaignEvents.SiegeEngineBuiltEvent.AddNonSerializedListener(this, new Action<SiegeEvent, BattleSideEnum, SiegeEngineType>(this.OnSiegeEngineBuilt));
			CampaignEvents.OnSiegeEngineDestroyedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, BattleSideEnum, SiegeEngineType>(this.OnSiegeEngineDestroyed));
			CampaignEvents.OnSiegeBombardmentHitEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, SiegeBombardTargets>(this.OnSiegeEngineHit));
			CampaignEvents.OnSiegeBombardmentWallHitEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, bool>(this.OnSiegeBombardmentWallHit));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnPeaceDeclared));
		}

		// Token: 0x06003B22 RID: 15138 RVA: 0x001161FE File Offset: 0x001143FE
		private void OnPeaceDeclared(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			if (Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu() == "menu_siege_strategies")
			{
				Campaign.Current.CurrentMenuContext.Refresh();
			}
		}

		// Token: 0x06003B23 RID: 15139 RVA: 0x0011622F File Offset: 0x0011442F
		private void OnSiegeBombardmentWallHit(MobileParty party, Settlement settlement, BattleSideEnum battleSide, SiegeEngineType siegeEngine, bool isWallCracked)
		{
			if (isWallCracked && party != null)
			{
				SkillLevelingManager.OnWallBreached(party);
			}
		}

		// Token: 0x06003B24 RID: 15140 RVA: 0x0011623E File Offset: 0x0011443E
		private void OnSiegeEngineHit(MobileParty party, Settlement settlement, BattleSideEnum side, SiegeEngineType engine, SiegeBombardTargets target)
		{
			if (target == SiegeBombardTargets.RangedEngines)
			{
				this.BombardHitEngineCasualties(settlement.SiegeEvent.GetSiegeEventSide(side), engine);
			}
		}

		// Token: 0x06003B25 RID: 15141 RVA: 0x0011625C File Offset: 0x0011445C
		private void OnSiegeEngineDestroyed(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum lostSide, SiegeEngineType siegeEngine)
		{
			SiegeEventModel siegeEventModel = Campaign.Current.Models.SiegeEventModel;
			SiegeEvent siegeEvent = besiegedSettlement.SiegeEvent;
			MobileParty effectiveSiegePartyForSide = siegeEventModel.GetEffectiveSiegePartyForSide(siegeEvent, lostSide);
			MobileParty effectiveSiegePartyForSide2 = siegeEventModel.GetEffectiveSiegePartyForSide(siegeEvent, lostSide.GetOppositeSide());
			if (effectiveSiegePartyForSide2 != null)
			{
				SkillLevelingManager.OnSiegeEngineDestroyed(effectiveSiegePartyForSide2, siegeEngine);
			}
			float casualtyChance = Campaign.Current.Models.SiegeEventModel.GetCasualtyChance(effectiveSiegePartyForSide, siegeEvent, lostSide);
			if (MBRandom.RandomFloat <= casualtyChance)
			{
				ISiegeEventSide siegeEventSide = siegeEvent.GetSiegeEventSide(lostSide);
				int num = siegeEventModel.GetSiegeEngineDestructionCasualties(siegeEvent, siegeEventSide.BattleSide, siegeEngine);
				BattleSideEnum oppositeSide = siegeEventSide.BattleSide.GetOppositeSide();
				if (effectiveSiegePartyForSide2 != null && oppositeSide == BattleSideEnum.Attacker && effectiveSiegePartyForSide2.HasPerk(DefaultPerks.Tactics.PickThemOfTheWalls, false) && MBRandom.RandomFloat < DefaultPerks.Tactics.PickThemOfTheWalls.PrimaryBonus)
				{
					num *= 2;
				}
				if (oppositeSide == BattleSideEnum.Defender)
				{
					Hero governor = siegeEvent.BesiegedSettlement.Town.Governor;
					if (governor != null && governor.GetPerkValue(DefaultPerks.Tactics.PickThemOfTheWalls) && MBRandom.RandomFloat < DefaultPerks.Tactics.PickThemOfTheWalls.SecondaryBonus)
					{
						num *= 2;
					}
				}
				this.KillRandomTroopsOfEnemy(siegeEventSide, num);
			}
		}

		// Token: 0x06003B26 RID: 15142 RVA: 0x00116368 File Offset: 0x00114568
		private void OnSiegeEngineBuilt(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngineType)
		{
			MobileParty effectiveSiegePartyForSide = Campaign.Current.Models.SiegeEventModel.GetEffectiveSiegePartyForSide(siegeEvent, side);
			if (effectiveSiegePartyForSide != null)
			{
				SkillLevelingManager.OnSiegeEngineBuilt(effectiveSiegePartyForSide, siegeEngineType);
				if (effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Apprenticeship, false))
				{
					for (int i = 0; i < effectiveSiegePartyForSide.MemberRoster.Count; i++)
					{
						CharacterObject characterAtIndex = effectiveSiegePartyForSide.MemberRoster.GetCharacterAtIndex(i);
						if (!characterAtIndex.IsHero)
						{
							int elementNumber = effectiveSiegePartyForSide.MemberRoster.GetElementNumber(i);
							effectiveSiegePartyForSide.MemberRoster.AddXpToTroop(elementNumber * (int)DefaultPerks.Engineering.Apprenticeship.PrimaryBonus, characterAtIndex);
						}
					}
				}
			}
		}

		// Token: 0x06003B27 RID: 15143 RVA: 0x001163F8 File Offset: 0x001145F8
		private int KillRandomTroopsOfEnemy(ISiegeEventSide siegeEventSide, int count)
		{
			SiegeEvent siegeEvent = siegeEventSide.SiegeEvent;
			int num = siegeEventSide.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Sum((PartyBase p) => p.NumberOfRegularMembers);
			if (num == 0 || count == 0)
			{
				return 0;
			}
			int num2 = 0;
			foreach (PartyBase partyBase in siegeEventSide.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
			{
				float siegeBombardmentHitSurgeryChance = Campaign.Current.Models.PartyHealingModel.GetSiegeBombardmentHitSurgeryChance(partyBase);
				float num3 = (float)partyBase.NumberOfRegularMembers / (float)num;
				float randomFloat = MBRandom.RandomFloat;
				int num4 = MathF.Min(MBRandom.RoundRandomized((float)(count - num2) * (num3 + randomFloat)), count);
				if (num4 > 0)
				{
					int num5 = MathF.Round((float)num4 * siegeBombardmentHitSurgeryChance);
					num2 += num4;
					num4 -= num5;
					siegeEventSide.OnTroopsKilledOnSide(num4);
					partyBase.MemberRoster.KillNumberOfMenRandomly(num4, false);
					Debug.Print(string.Concat(new object[]
					{
						siegeEvent.BesiegedSettlement.Name,
						" - ",
						siegeEventSide.BattleSide.ToString(),
						" ",
						partyBase.Name,
						" => ",
						num4,
						" casualties"
					}), 0, Debug.DebugColor.Purple, 137438953472UL);
					if (num5 > 0)
					{
						partyBase.MemberRoster.WoundNumberOfTroopsRandomly(num5);
						Debug.Print(string.Concat(new object[]
						{
							siegeEvent.BesiegedSettlement.Name,
							" - ",
							siegeEventSide.BattleSide.ToString(),
							" ",
							partyBase.Name,
							" => ",
							num5,
							" wounded casualties"
						}), 0, Debug.DebugColor.Purple, 137438953472UL);
					}
				}
				if (num2 >= count)
				{
					break;
				}
			}
			return num2;
		}

		// Token: 0x06003B28 RID: 15144 RVA: 0x00116614 File Offset: 0x00114814
		private void BombardHitEngineCasualties(ISiegeEventSide siegeEventSide, SiegeEngineType attackerEngineType)
		{
			SiegeEvent siegeEvent = siegeEventSide.SiegeEvent;
			Settlement besiegedSettlement = siegeEvent.BesiegedSettlement;
			BesiegerCamp besiegerCamp = siegeEvent.BesiegerCamp;
			MobileParty effectiveSiegePartyForSide = Campaign.Current.Models.SiegeEventModel.GetEffectiveSiegePartyForSide(siegeEvent, siegeEventSide.BattleSide);
			ISiegeEventSide siegeEventSide2 = siegeEvent.GetSiegeEventSide(siegeEventSide.BattleSide.GetOppositeSide());
			siegeEvent.GetSiegeEventSide(siegeEventSide.BattleSide.GetOppositeSide());
			float siegeEngineHitChance = Campaign.Current.Models.SiegeEventModel.GetSiegeEngineHitChance(attackerEngineType, siegeEventSide.BattleSide, SiegeBombardTargets.People, besiegedSettlement.Town);
			if (MBRandom.RandomFloat < siegeEngineHitChance)
			{
				int colleteralDamageCasualties = Campaign.Current.Models.SiegeEventModel.GetColleteralDamageCasualties(attackerEngineType, effectiveSiegePartyForSide);
				int num = this.KillRandomTroopsOfEnemy(siegeEventSide2, colleteralDamageCasualties);
				if (num > 0)
				{
					CampaignEventDispatcher.Instance.OnSiegeBombardmentHit(besiegerCamp.BesiegerParty, besiegedSettlement, siegeEventSide.BattleSide, attackerEngineType, SiegeBombardTargets.People);
					Debug.Print(string.Concat(new object[]
					{
						besiegedSettlement.Name,
						" - ",
						siegeEventSide.BattleSide.ToString(),
						" ",
						attackerEngineType.Name,
						" hit the enemy engine. A total of ",
						num,
						" casualties"
					}), 0, Debug.DebugColor.Purple, 137438953472UL);
				}
			}
		}

		// Token: 0x06003B29 RID: 15145 RVA: 0x0011675C File Offset: 0x0011495C
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003B2A RID: 15146 RVA: 0x0011675E File Offset: 0x0011495E
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x06003B2B RID: 15147 RVA: 0x00116767 File Offset: 0x00114967
		private void AISiegeEventStarted(SiegeEvent siegeEvent)
		{
			this.SetDefaultTactics(siegeEvent, BattleSideEnum.Attacker);
			this.SetDefaultTactics(siegeEvent, BattleSideEnum.Defender);
		}

		// Token: 0x06003B2C RID: 15148 RVA: 0x0011677C File Offset: 0x0011497C
		protected void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddWaitGameMenu("menu_siege_strategies", "{=!}{CURRENT_STRATEGY}", new OnInitDelegate(this.game_menu_siege_strategies_on_init), null, null, new OnTickDelegate(this.game_menu_siege_strategies_on_tick), GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.Encounter, 0f, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_lead_assault", "{=mjOcwUSA}Lead an assault", new GameMenuOption.OnConditionDelegate(SiegeEventCampaignBehavior.game_menu_siege_strategies_lead_assault_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_siege_strategies_lead_assault_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_order_troops", "{=TtGJqRI5}Send troops", new GameMenuOption.OnConditionDelegate(SiegeEventCampaignBehavior.game_menu_siege_strategies_order_assault_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_order_an_assault_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_request_parley", "{=2xVbLS5r}Request a parley", new GameMenuOption.OnConditionDelegate(SiegeEventCampaignBehavior.menu_defender_side_request_audience_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_defender_side_request_audience_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_break_out", "{=dFcgXnQq}Break out", new GameMenuOption.OnConditionDelegate(SiegeEventCampaignBehavior.menu_defender_siege_break_out_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_defender_siege_break_out_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_sally_out", "{=!}{SALLY_OUT_BUTTON_TEXT}", new GameMenuOption.OnConditionDelegate(SiegeEventCampaignBehavior.menu_sally_out_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_sally_out_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.menu_siege_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_siege_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_leave_army", "{=hSdJ0UUv}Leave Army", new GameMenuOption.OnConditionDelegate(this.menu_siege_strategies_passive_wait_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.menu_siege_strategies_passive_wait_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("menu_siege_strategies_break_siege", "{=!}{SIEGE_LEAVE_TEXT}", new OnInitDelegate(this.menu_break_siege_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies_break_siege", "menu_siege_strategies_break_siege_return", "{=25ifdWOy}Return to siege", new GameMenuOption.OnConditionDelegate(this.return_siege_on_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("menu_siege_strategies");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies_break_siege", "menu_siege_strategies_break_siege_go_on", "{=TGYJUUn0}Go on.", new GameMenuOption.OnConditionDelegate(this.leave_siege_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_end_besieging_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("menu_siege_safe_passage_accepted", "Besiegers have agreed to allow safe passage for your party.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_safe_passage_accepted", "menu_siege_safe_passage_accepted_leave", "Leave", new GameMenuOption.OnConditionDelegate(this.leave_siege_on_condition), new GameMenuOption.OnConsequenceDelegate(SiegeEventCampaignBehavior.menu_siege_leave_on_consequence), true, -1, false, null);
		}

		// Token: 0x06003B2D RID: 15149 RVA: 0x001169EC File Offset: 0x00114BEC
		private void game_menu_siege_strategies_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (!(genericStateMenu != "menu_siege_strategies"))
			{
				args.MenuContext.GameMenu.GetText().SetTextVariable("CURRENT_STRATEGY", this._currentSiegeDescription);
				Campaign.Current.GameMenuManager.RefreshMenuOptions(args.MenuContext);
				return;
			}
			if (!string.IsNullOrEmpty(genericStateMenu))
			{
				GameMenu.SwitchToMenu(genericStateMenu);
				return;
			}
			GameMenu.ExitToLast();
		}

		// Token: 0x06003B2E RID: 15150 RVA: 0x00116A66 File Offset: 0x00114C66
		private void game_menu_siege_strategies_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("CURRENT_STRATEGY", this._currentSiegeDescription, false);
		}

		// Token: 0x06003B2F RID: 15151 RVA: 0x00116A79 File Offset: 0x00114C79
		private static void menu_siege_strategies_lead_assault_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.IsActive)
			{
				PlayerEncounter.LeaveEncounter = false;
			}
			else
			{
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, PlayerSiege.PlayerSiegeEvent.BesiegedSettlement);
			}
			GameMenu.SwitchToMenu("assault_town");
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x00116AA8 File Offset: 0x00114CA8
		private static void menu_order_an_assault_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.IsActive)
			{
				PlayerEncounter.LeaveEncounter = false;
			}
			else
			{
				PlayerEncounter.Start();
				PlayerEncounter.Current.SetupFields(PartyBase.MainParty, PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.Party);
			}
			GameMenu.SwitchToMenu("assault_town_order_attack");
		}

		// Token: 0x06003B31 RID: 15153 RVA: 0x00116AE6 File Offset: 0x00114CE6
		private bool menu_siege_strategies_order_troops_on_condition(MenuCallbackArgs args)
		{
			args.IsEnabled = MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x00116B1C File Offset: 0x00114D1C
		private bool menu_siege_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty) && ((PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Defender && !MobileParty.MainParty.MapFaction.IsAtWarWith(PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapFaction)) || (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Attacker));
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x00116B9C File Offset: 0x00114D9C
		private bool menu_siege_strategies_passive_wait_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Attacker && (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty) && MobileParty.MainParty.BesiegedSettlement.SiegeEvent.BesiegerCamp.BesiegerParty != MobileParty.MainParty;
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x00116C0C File Offset: 0x00114E0C
		private void menu_break_siege_on_init(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.SiegeEvent.BesiegerCamp.BesiegerParty == MobileParty.MainParty)
			{
				MBTextManager.SetTextVariable("SIEGE_LEAVE_TEXT", this._removeSiegeCompletelyText, false);
			}
			else
			{
				MBTextManager.SetTextVariable("SIEGE_LEAVE_TEXT", this._leaveSiegeText, false);
			}
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
		}

		// Token: 0x06003B35 RID: 15157 RVA: 0x00116C63 File Offset: 0x00114E63
		private bool return_siege_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003B36 RID: 15158 RVA: 0x00116C6E File Offset: 0x00114E6E
		private bool leave_siege_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x06003B37 RID: 15159 RVA: 0x00116C79 File Offset: 0x00114E79
		private void menu_siege_strategies_passive_wait_leave_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ExitToLast();
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				PlayerSiege.ClosePlayerSiege();
			}
			MobileParty.MainParty.BesiegerCamp = null;
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003B38 RID: 15160 RVA: 0x00116CA4 File Offset: 0x00114EA4
		private static bool game_menu_siege_strategies_order_assault_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.OrderTroopsToAttack;
			if (MobileParty.MainParty.BesiegedSettlement == null)
			{
				return false;
			}
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(MobileParty.MainParty.BesiegedSettlement.SiegeEvent, PlayerSiege.PlayerSide) == Hero.MainHero)
			{
				Settlement settlement = ((PlayerEncounter.EncounteredParty != null) ? PlayerEncounter.EncounteredParty.Settlement : PlayerSiege.PlayerSiegeEvent.BesiegedSettlement);
				if (PlayerSiege.PlayerSide == BattleSideEnum.Attacker && !settlement.SiegeEvent.BesiegerCamp.IsPreparationComplete)
				{
					args.IsEnabled = false;
					args.Tooltip = SiegeEventCampaignBehavior._waitSiegeEquipmentsText;
				}
				else
				{
					bool flag = MobileParty.MainParty.MemberRoster.GetTroopRoster().Any(delegate(TroopRosterElement x)
					{
						if (x.Character.IsHero)
						{
							return x.Character != CharacterObject.PlayerCharacter && !x.Character.HeroObject.IsWounded;
						}
						return x.Number > x.WoundedNumber;
					});
					if (!flag && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
					{
						foreach (MobileParty mobileParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
						{
							flag = mobileParty.MemberRoster.GetTroopRoster().Any(delegate(TroopRosterElement x)
							{
								if (x.Character.IsHero)
								{
									return x.Character != CharacterObject.PlayerCharacter && !x.Character.HeroObject.IsWounded;
								}
								return x.Number > x.WoundedNumber;
							});
							if (flag)
							{
								break;
							}
						}
					}
					if (!flag)
					{
						args.IsEnabled = false;
						args.Tooltip = new TextObject("{=ao9bhAhf}You are not leading any troops", null);
					}
				}
			}
			else
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=1Hd19nq5}You are not in command of this siege.", null);
			}
			return true;
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x00116E60 File Offset: 0x00115060
		private static bool game_menu_siege_strategies_lead_assault_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.LeadAssault;
			if (MobileParty.MainParty.BesiegedSettlement == null)
			{
				return false;
			}
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(MobileParty.MainParty.BesiegedSettlement.SiegeEvent, PlayerSiege.PlayerSide) == Hero.MainHero)
			{
				Settlement settlement = ((PlayerEncounter.EncounteredParty != null) ? PlayerEncounter.EncounteredParty.Settlement : PlayerSiege.PlayerSiegeEvent.BesiegedSettlement);
				if (PlayerSiege.PlayerSide == BattleSideEnum.Attacker && !settlement.SiegeEvent.BesiegerCamp.IsPreparationComplete)
				{
					args.IsEnabled = false;
					args.Tooltip = SiegeEventCampaignBehavior._waitSiegeEquipmentsText;
				}
				else if (Hero.MainHero.IsWounded)
				{
					args.IsEnabled = false;
					args.Tooltip = SiegeEventCampaignBehavior._woundedAssaultText;
				}
			}
			else
			{
				args.IsEnabled = false;
				args.Tooltip = SiegeEventCampaignBehavior._noCommandText;
			}
			return true;
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x00116F34 File Offset: 0x00115134
		private static void LeaveSiege()
		{
			MobileParty.MainParty.BesiegerCamp = null;
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				MobileParty.MainParty.Army.AIBehavior = Army.AIBehaviorFlags.Unassigned;
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			GameMenu.ExitToLast();
		}

		// Token: 0x06003B3B RID: 15163 RVA: 0x00116F94 File Offset: 0x00115194
		private static void menu_siege_leave_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.BesiegerCamp == null)
			{
				if (PlayerEncounter.Current != null && MobileParty.MainParty.CurrentSettlement != null)
				{
					if (MobileParty.MainParty.Army != null)
					{
						MobileParty.MainParty.Army = null;
					}
					PlayerSiege.ClosePlayerSiege();
					PlayerEncounter.LeaveSettlement();
					PlayerEncounter.Finish(true);
					return;
				}
				GameMenu.ExitToLast();
				return;
			}
			else
			{
				if (MobileParty.MainParty.BesiegerCamp.BesiegerParty == MobileParty.MainParty)
				{
					GameMenu.SwitchToMenu("menu_siege_strategies_break_siege");
					return;
				}
				SiegeEventCampaignBehavior.LeaveSiege();
				return;
			}
		}

		// Token: 0x06003B3C RID: 15164 RVA: 0x00117014 File Offset: 0x00115214
		private static void menu_end_besieging_on_consequence(MenuCallbackArgs args)
		{
			SiegeEventCampaignBehavior.LeaveSiege();
		}

		// Token: 0x06003B3D RID: 15165 RVA: 0x0011701C File Offset: 0x0011521C
		private static bool menu_defender_side_request_audience_on_condition(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSide != BattleSideEnum.Defender)
			{
				return false;
			}
			if (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Defender && !MobileParty.MainParty.MapFaction.IsAtWarWith(PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapFaction))
			{
				return false;
			}
			Settlement settlement = Settlement.CurrentSettlement ?? PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
			if (settlement.SiegeEvent != null)
			{
				if (!settlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.LeaderHero != null && party.MobileParty.IsLordParty))
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=rO704KOG}There is no one with the authority to talk to you.", null);
				}
			}
			if (PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent != null)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=1UO0yMBr}You can not parley during an ongoing battle.", null);
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			return true;
		}

		// Token: 0x06003B3E RID: 15166 RVA: 0x00117110 File Offset: 0x00115310
		private static void menu_defender_side_request_audience_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("request_meeting_with_besiegers");
		}

		// Token: 0x06003B3F RID: 15167 RVA: 0x0011711C File Offset: 0x0011531C
		private static bool menu_sally_out_on_condition(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSide != BattleSideEnum.Defender)
			{
				return false;
			}
			if (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Defender && !MobileParty.MainParty.MapFaction.IsAtWarWith(PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapFaction))
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=UqaNs3ck}You are not at war with the besiegers.", null);
			}
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(PlayerSiege.PlayerSiegeEvent, PlayerSiege.PlayerSide) != Hero.MainHero && (PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent == null || !PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent.IsSallyOut))
			{
				args.IsEnabled = false;
				TextObject textObject = new TextObject("{=OmGHXuZB}You are not in command of the defenders.", null);
				args.Tooltip = textObject;
			}
			if (PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent != null && PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent.IsSallyOut)
			{
				args.Text.SetTextVariable("SALLY_OUT_BUTTON_TEXT", new TextObject("{=fyNNCOFK}Join the sally out", null));
			}
			else
			{
				args.Text.SetTextVariable("SALLY_OUT_BUTTON_TEXT", new TextObject("{=KKB2vNFr}Sally out", null));
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return true;
		}

		// Token: 0x06003B40 RID: 15168 RVA: 0x00117268 File Offset: 0x00115468
		private static void menu_sally_out_on_consequence(MenuCallbackArgs args)
		{
			MobileParty besiegerParty = Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty;
			if (besiegerParty.Party.MapEvent != null)
			{
				besiegerParty.Party.MapEvent.FinalizeEvent();
			}
			EncounterManager.StartPartyEncounter(MobileParty.MainParty.Party, besiegerParty.Party);
		}

		// Token: 0x06003B41 RID: 15169 RVA: 0x001172BC File Offset: 0x001154BC
		private static bool menu_defender_siege_break_out_on_condition(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSide != BattleSideEnum.Defender)
			{
				return false;
			}
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(PlayerSiege.PlayerSiegeEvent, PlayerSiege.PlayerSide) != Hero.MainHero)
			{
				args.Tooltip = new TextObject("{=OmGHXuZB}You are not in command of the defenders.", null);
				args.IsEnabled = false;
			}
			if (PlayerSiege.PlayerSiegeEvent != null && PlayerSiege.PlayerSide == BattleSideEnum.Defender && !MobileParty.MainParty.MapFaction.IsAtWarWith(PlayerSiege.PlayerSiegeEvent.BesiegerCamp.BesiegerParty.MapFaction))
			{
				return false;
			}
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingOutOfBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement(mainParty, siegeEvent);
			Army army = mainParty.Army;
			int num = ((army != null) ? army.TotalRegularCount : mainParty.MemberRoster.TotalRegulars);
			if (lostTroopCountForBreakingOutOfBesiegedSettlement > num)
			{
				args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
				args.IsEnabled = false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
			return Hero.MainHero.MapFaction != siegeEvent.BesiegerCamp.BesiegerParty.MapFaction;
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x001173D1 File Offset: 0x001155D1
		private static void menu_defender_siege_break_out_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("break_out_menu");
		}

		// Token: 0x06003B43 RID: 15171 RVA: 0x001173DD File Offset: 0x001155DD
		private void menu_siege_select_strategy_leave_on_consequence(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			GameMenu.SwitchToMenu("menu_siege_strategies");
		}

		// Token: 0x06003B44 RID: 15172 RVA: 0x001173F1 File Offset: 0x001155F1
		private void SetTactic(SiegeEvent siegeEvent, BattleSideEnum side, SiegeStrategy strategy)
		{
			siegeEvent.GetSiegeEventSide(side).SetSiegeStrategy(strategy);
		}

		// Token: 0x06003B45 RID: 15173 RVA: 0x00117400 File Offset: 0x00115600
		private void SetDefaultTactics(SiegeEvent siegeEvent, BattleSideEnum side)
		{
			IEnumerable<SiegeStrategy> enumerable = ((side == BattleSideEnum.Attacker) ? DefaultSiegeStrategies.AllAttackerStrategies : DefaultSiegeStrategies.AllDefenderStrategies);
			SiegeStrategy siegeStrategy = null;
			float num = float.MinValue;
			foreach (SiegeStrategy siegeStrategy2 in enumerable)
			{
				float num2 = Campaign.Current.Models.SiegeEventModel.GetSiegeStrategyScore(siegeEvent, side, siegeStrategy2) * (0.5f + 0.5f * MBRandom.RandomFloat);
				if (num2 > num)
				{
					num = num2;
					siegeStrategy = siegeStrategy2;
				}
			}
			this.SetTactic(siegeEvent, side, siegeStrategy);
		}

		// Token: 0x06003B46 RID: 15174 RVA: 0x00117498 File Offset: 0x00115698
		[GameMenuInitializationHandler("menu_siege_strategies")]
		private static void game_menu_siege_strategies_background_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_besieging");
		}

		// Token: 0x04001219 RID: 4633
		private readonly TextObject _attackerSummaryText = new TextObject("{=sbmWGPYG}You are besieging {SETTLEMENT}. {FURTHER_EXPLANATION}", null);

		// Token: 0x0400121A RID: 4634
		private readonly TextObject _defenderSummaryText = new TextObject("{=l5YipTe3}{SETTLEMENT} is under siege. {FURTHER_EXPLANATION}", null);

		// Token: 0x0400121B RID: 4635
		private readonly TextObject _removeSiegeCompletelyText = new TextObject("{=5ZDCnrDQ}This will end the siege. You cannot take your siege engines with you, and they will be destroyed.", null);

		// Token: 0x0400121C RID: 4636
		private readonly TextObject _leaveSiegeText = new TextObject("{=176K8dcb}You will end the siege if you leave. Are you sure?", null);

		// Token: 0x0400121D RID: 4637
		private static readonly TextObject _waitSiegeEquipmentsText = new TextObject("{=bCuxzp1N}You need to wait for the siege equipment to be prepared.", null);

		// Token: 0x0400121E RID: 4638
		private static readonly TextObject _woundedAssaultText = new TextObject("{=gzYuWR28}You are wounded, and in no condition to lead an assault.", null);

		// Token: 0x0400121F RID: 4639
		private static readonly TextObject _noCommandText = new TextObject("{=1Hd19nq5}You are not in command of this siege.", null);
	}
}
