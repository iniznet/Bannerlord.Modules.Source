using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003BE RID: 958
	public class PlayerArmyWaitBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003901 RID: 14593 RVA: 0x00104329 File Offset: 0x00102529
		public PlayerArmyWaitBehavior()
		{
			this._leadingArmyDescriptionText = GameTexts.FindText("str_you_are_leading_army", null);
			this._armyDescriptionText = GameTexts.FindText("str_army_of_HERO", null);
			this._disbandingArmyDescriptionText = new TextObject("{=Yan3ZG1w}Disbanding Army!", null);
		}

		// Token: 0x06003902 RID: 14594 RVA: 0x00104364 File Offset: 0x00102564
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.OnTick));
		}

		// Token: 0x06003903 RID: 14595 RVA: 0x001043B6 File Offset: 0x001025B6
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003904 RID: 14596 RVA: 0x001043B8 File Offset: 0x001025B8
		private void AddMenus(CampaignGameStarter starter)
		{
			starter.AddWaitGameMenu("army_wait", "{=0gwQGnm4}{ARMY_OWNER_TEXT} {ARMY_BEHAVIOR}", new OnInitDelegate(this.wait_menu_army_wait_on_init), new OnConditionDelegate(this.wait_menu_army_wait_on_condition), null, new OnTickDelegate(this.ArmyWaitMenuTick), GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
			starter.AddGameMenuOption("army_wait", "leave_army", "{=hSdJ0UUv}Leave Army", new GameMenuOption.OnConditionDelegate(this.wait_menu_army_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.wait_menu_army_leave_on_consequence), true, -1, false, null);
			starter.AddGameMenuOption("army_wait", "abandon_army", "{=0vnegjxf}Abandon Army", new GameMenuOption.OnConditionDelegate(this.wait_menu_army_abandon_on_condition), new GameMenuOption.OnConsequenceDelegate(this.wait_menu_army_abandon_on_consequence), true, -1, false, null);
			starter.AddWaitGameMenu("army_wait_at_settlement", "{=0gwQGnm4}{ARMY_OWNER_TEXT} {ARMY_BEHAVIOR}", new OnInitDelegate(this.wait_menu_army_wait_at_settlement_on_init), new OnConditionDelegate(this.wait_menu_army_wait_on_condition), null, new OnTickDelegate(this.wait_menu_army_wait_at_settlement_on_tick), GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
			starter.AddGameMenuOption("army_wait_at_settlement", "enter_settlement", "{=eabR87ne}Enter Settlement", new GameMenuOption.OnConditionDelegate(this.wait_menu_army_enter_settlement_on_condition), new GameMenuOption.OnConsequenceDelegate(this.wait_menu_army_enter_settlement_on_consequence), false, -1, false, null);
			starter.AddGameMenuOption("army_wait_at_settlement", "leave_army", "{=hSdJ0UUv}Leave Army", new GameMenuOption.OnConditionDelegate(this.wait_menu_army_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.wait_menu_army_leave_on_consequence), true, -1, false, null);
			starter.AddGameMenu("army_dispersed", "{=!}{ARMY_DISPERSE_REASON}", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			starter.AddGameMenuOption("army_dispersed", "army_dispersed_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.army_dispersed_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.army_dispersed_continue_on_consequence), false, -1, false, null);
		}

		// Token: 0x06003905 RID: 14597 RVA: 0x0010454C File Offset: 0x0010274C
		private void ArmyWaitMenuTick(MenuCallbackArgs args, CampaignTime dt)
		{
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (!(genericStateMenu != "army_wait"))
			{
				this.RefreshArmyTexts(args);
				return;
			}
			args.MenuContext.GameMenu.EndWait();
			if (!string.IsNullOrEmpty(genericStateMenu))
			{
				GameMenu.SwitchToMenu(genericStateMenu);
				return;
			}
			GameMenu.ExitToLast();
		}

		// Token: 0x06003906 RID: 14598 RVA: 0x001045A7 File Offset: 0x001027A7
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddMenus(starter);
		}

		// Token: 0x06003907 RID: 14599 RVA: 0x001045B0 File Offset: 0x001027B0
		private void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
			if (isPlayersArmy && army.LeaderParty != MobileParty.MainParty && Campaign.Current.CurrentMenuContext != null)
			{
				Campaign.Current.CurrentMenuContext.GameMenu.EndWait();
				GameMenu.SwitchToMenu("army_dispersed");
				TextObject textObject = TextObject.Empty;
				if (reason == Army.ArmyDispersionReason.NoActiveWar)
				{
					textObject = new TextObject("{=tvAdOGzc}{ARMY_LEADER}'s army has disbanded. The kingdom is now at peace.", null);
				}
				else if (reason == Army.ArmyDispersionReason.CohesionDepleted)
				{
					textObject = new TextObject("{=5wwO7ozf}{ARMY_LEADER}'s army has disbanded due to a lack of cohesion.", null);
				}
				else if (reason == Army.ArmyDispersionReason.FoodProblem)
				{
					textObject = new TextObject("{=eVdUaG3x}{ARMY_LEADER}'s army has disbanded due to a lack of food.", null);
				}
				else
				{
					textObject = new TextObject("{=FXPvGTEa}Army you are in is dispersed.", null);
				}
				textObject.SetTextVariable("ARMY_LEADER", army.LeaderParty.LeaderHero.Name);
				MBTextManager.SetTextVariable("ARMY_DISPERSE_REASON", textObject, false);
			}
		}

		// Token: 0x06003908 RID: 14600 RVA: 0x00104674 File Offset: 0x00102874
		private void wait_menu_army_wait_on_init(MenuCallbackArgs args)
		{
			Army army = MobileParty.MainParty.Army;
			bool flag;
			if (army == null)
			{
				flag = null != null;
			}
			else
			{
				MobileParty leaderParty = army.LeaderParty;
				flag = ((leaderParty != null) ? leaderParty.LeaderHero : null) != null;
			}
			if (flag)
			{
				this._armyDescriptionText.SetTextVariable("HERO", army.LeaderParty.LeaderHero.Name);
				args.MenuTitle = this._armyDescriptionText;
			}
			else
			{
				args.MenuTitle = this._disbandingArmyDescriptionText;
			}
			this.RefreshArmyTexts(args);
		}

		// Token: 0x06003909 RID: 14601 RVA: 0x001046E8 File Offset: 0x001028E8
		private void wait_menu_army_wait_at_settlement_on_init(MenuCallbackArgs args)
		{
			if (!PlayerEncounter.InsideSettlement && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				PlayerEncounter.EnterSettlement();
			}
			this._armyDescriptionText.SetTextVariable("HERO", MobileParty.MainParty.Army.LeaderParty.LeaderHero.Name);
			args.MenuTitle = this._armyDescriptionText;
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = true;
			}
			this.RefreshArmyTexts(args);
		}

		// Token: 0x0600390A RID: 14602 RVA: 0x00104768 File Offset: 0x00102968
		private void wait_menu_army_wait_at_settlement_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (genericStateMenu != "army_wait_at_settlement")
			{
				args.MenuContext.GameMenu.EndWait();
				if (!string.IsNullOrEmpty(genericStateMenu))
				{
					GameMenu.SwitchToMenu(genericStateMenu);
					return;
				}
				GameMenu.ExitToLast();
			}
		}

		// Token: 0x0600390B RID: 14603 RVA: 0x001047BB File Offset: 0x001029BB
		[GameMenuInitializationHandler("army_wait")]
		private static void wait_menu_ui_army_wait_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Hero.MainHero.MapFaction.Culture.EncounterBackgroundMesh);
		}

		// Token: 0x0600390C RID: 14604 RVA: 0x001047DC File Offset: 0x001029DC
		private void RefreshArmyTexts(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null)
			{
				TextObject text = args.MenuContext.GameMenu.GetText();
				if (MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
				{
					TextObject textObject = GameTexts.FindText("str_you_are_following_army", null);
					textObject.SetTextVariable("ARMY_LEADER", MobileParty.MainParty.Army.LeaderParty.LeaderHero.Name);
					text.SetTextVariable("ARMY_OWNER_TEXT", textObject);
					text.SetTextVariable("ARMY_BEHAVIOR", MobileParty.MainParty.Army.GetBehaviorText(false));
					return;
				}
				text.SetTextVariable("ARMY_OWNER_TEXT", this._leadingArmyDescriptionText);
				text.SetTextVariable("ARMY_BEHAVIOR", "");
			}
		}

		// Token: 0x0600390D RID: 14605 RVA: 0x0010489D File Offset: 0x00102A9D
		private bool wait_menu_army_wait_on_condition(MenuCallbackArgs args)
		{
			return true;
		}

		// Token: 0x0600390E RID: 14606 RVA: 0x001048A0 File Offset: 0x00102AA0
		private bool wait_menu_army_abandon_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			if (MobileParty.MainParty.Army == null || (MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.BesiegedSettlement == null))
			{
				return false;
			}
			args.Tooltip = GameTexts.FindText("str_abandon_army", null);
			args.Tooltip.SetTextVariable("INFLUENCE_COST", Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAbandoningArmy());
			return true;
		}

		// Token: 0x0600390F RID: 14607 RVA: 0x0010491C File Offset: 0x00102B1C
		private bool wait_menu_army_enter_settlement_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return (MobileParty.MainParty.Army != null && MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.BesiegedSettlement == null) || (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty && MobileParty.MainParty.Army.LeaderParty.LastVisitedSettlement != null && MobileParty.MainParty.Army.LeaderParty.LastVisitedSettlement.Position2D.Distance(MobileParty.MainParty.Army.LeaderParty.Position2D) < 1f);
		}

		// Token: 0x06003910 RID: 14608 RVA: 0x001049D8 File Offset: 0x00102BD8
		private void wait_menu_army_enter_settlement_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty && MobileParty.MainParty.CurrentSettlement == null)
			{
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, MobileParty.MainParty.Army.LeaderParty.LastVisitedSettlement);
				return;
			}
			Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
			if (currentSettlement.IsTown)
			{
				GameMenu.ActivateGameMenu("town");
				return;
			}
			if (currentSettlement.IsCastle)
			{
				GameMenu.ActivateGameMenu("castle");
				return;
			}
			GameMenu.ActivateGameMenu("village");
		}

		// Token: 0x06003911 RID: 14609 RVA: 0x00104A6D File Offset: 0x00102C6D
		private bool wait_menu_army_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.BesiegedSettlement == null;
		}

		// Token: 0x06003912 RID: 14610 RVA: 0x00104AA0 File Offset: 0x00102CA0
		private void wait_menu_army_leave_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
			}
			else
			{
				GameMenu.ExitToLast();
			}
			if (Settlement.CurrentSettlement != null)
			{
				LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
				PartyBase.MainParty.Visuals.SetMapIconAsDirty();
			}
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003913 RID: 14611 RVA: 0x00104AEC File Offset: 0x00102CEC
		private void wait_menu_army_abandon_on_consequence(MenuCallbackArgs args)
		{
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfAbandoningArmy()));
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
			}
			else
			{
				GameMenu.ExitToLast();
			}
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003914 RID: 14612 RVA: 0x00104B38 File Offset: 0x00102D38
		private void OnTick(float dt)
		{
			if (MobileParty.MainParty.AttachedTo != null)
			{
				MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
				string text;
				if (currentMenuContext == null)
				{
					text = null;
				}
				else
				{
					GameMenu gameMenu = currentMenuContext.GameMenu;
					text = ((gameMenu != null) ? gameMenu.StringId : null);
				}
				Settlement settlement;
				if (text == "army_wait" && (settlement = MobileParty.MainParty.AttachedTo.Army.AiBehaviorObject as Settlement) != null && settlement.SiegeEvent != null && Hero.MainHero.PartyBelongedTo.Army.LeaderParty.BesiegedSettlement == settlement)
				{
					PlayerSiege.StartPlayerSiege(BattleSideEnum.Attacker, false, settlement);
					PlayerSiege.StartSiegePreparation();
				}
			}
		}

		// Token: 0x06003915 RID: 14613 RVA: 0x00104BD0 File Offset: 0x00102DD0
		private void army_dispersed_continue_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.CurrentSettlement == null)
			{
				GameMenu.ExitToLast();
				return;
			}
			if (MobileParty.MainParty.CurrentSettlement.IsVillage)
			{
				GameMenu.SwitchToMenu("village");
				return;
			}
			if (MobileParty.MainParty.CurrentSettlement.IsTown)
			{
				GameMenu.SwitchToMenu((MobileParty.MainParty.CurrentSettlement.SiegeEvent != null) ? "menu_siege_strategies" : "town");
				return;
			}
			if (MobileParty.MainParty.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu((MobileParty.MainParty.CurrentSettlement.SiegeEvent != null) ? "menu_siege_strategies" : "castle");
				return;
			}
			LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
		}

		// Token: 0x06003916 RID: 14614 RVA: 0x00104C80 File Offset: 0x00102E80
		private bool army_dispersed_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003917 RID: 14615 RVA: 0x00104C8C File Offset: 0x00102E8C
		[GameMenuInitializationHandler("army_wait_at_settlement")]
		private static void game_menu_army_wait_at_settlement_on_init(MenuCallbackArgs args)
		{
			Settlement settlement = ((Settlement.CurrentSettlement != null) ? Settlement.CurrentSettlement : ((MobileParty.MainParty.LastVisitedSettlement != null) ? MobileParty.MainParty.LastVisitedSettlement : MobileParty.MainParty.AttachedTo.LastVisitedSettlement));
			args.MenuContext.SetBackgroundMeshName(settlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06003918 RID: 14616 RVA: 0x00104CE5 File Offset: 0x00102EE5
		[GameMenuInitializationHandler("army_dispersed")]
		private static void game_menu_army_dispersed_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_fallback");
		}

		// Token: 0x040011C1 RID: 4545
		private readonly TextObject _leadingArmyDescriptionText;

		// Token: 0x040011C2 RID: 4546
		private readonly TextObject _armyDescriptionText;

		// Token: 0x040011C3 RID: 4547
		private readonly TextObject _disbandingArmyDescriptionText;
	}
}
