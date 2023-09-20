using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CrimeCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterGameCreated));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterGameCreated));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroDeath));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnDailyTick()
		{
			foreach (Clan clan in Clan.NonBanditFactions)
			{
				float dailyCrimeRatingChange = clan.DailyCrimeRatingChange;
				if (!dailyCrimeRatingChange.ApproximatelyEqualsTo(0f, 1E-05f))
				{
					ChangeCrimeRatingAction.Apply(clan, dailyCrimeRatingChange, false);
				}
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				float dailyCrimeRatingChange2 = kingdom.DailyCrimeRatingChange;
				if (!dailyCrimeRatingChange2.ApproximatelyEqualsTo(0f, 1E-05f))
				{
					ChangeCrimeRatingAction.Apply(kingdom, dailyCrimeRatingChange2, false);
				}
			}
		}

		private void OnAfterGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenu("town_inside_criminal", "{=XgA2JgVR}You are brought to the town square to face judgment.", new OnInitDelegate(CrimeCampaignBehavior.town_inside_criminal_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_pay_by_punishment", "{=8iDpmu0L}Accept corporal punishment", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_pay_by_punishment_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_pay_by_punishment_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_give_punishment_and_money", "{=Xi1wpR2L}Accept corporal punishment and pay {FINE}{GOLD_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_punishment_and_money_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_punishment_and_money_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_give_your_life", "{=bVi0JKSx}You will be executed. You must face it as bravely as you can", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_your_life_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_your_life_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_pay_by_influence", "{=1cMS6415}Pay {FINE}{INFLUENCE_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_influence_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_influence_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_pay_by_money", "{=870ZCp1J}Pay {FINE}{GOLD_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_money_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_money_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_inside_criminal", "criminal_inside_menu_ignore_charges", "{=UQhRKJb9}Ignore the charges", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_ignore_charges_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_ignore_charges_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_discuss_criminal_surrender", "{=lwVwe4qU}You are discussing the terms of your surrender.", new OnInitDelegate(CrimeCampaignBehavior.town_discuss_criminal_surrender_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_pay_by_punishment", "{=8iDpmu0L}Accept corporal punishment", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_pay_by_punishment_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_pay_by_punishment_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_give_punishment_and_money", "{=Xi1wpR2L}Accept corporal punishment and pay {FINE}{GOLD_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_punishment_and_money_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_punishment_and_money_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_give_your_life", "{=VSzwMDJ2}You will be put to death", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_your_life_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_your_life_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_pay_by_influence", "{=1cMS6415}Pay {FINE}{INFLUENCE_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_influence_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_influence_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_pay_by_money", "{=870ZCp1J}Pay {FINE}{GOLD_ICON}", new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_money_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.criminal_inside_menu_give_money_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_discuss_criminal_surrender", "town_discuss_criminal_surrender_back", GameTexts.FindText("str_back", null).ToString(), new GameMenuOption.OnConditionDelegate(CrimeCampaignBehavior.town_discuss_criminal_surrender_on_condition), new GameMenuOption.OnConsequenceDelegate(CrimeCampaignBehavior.town_discuss_criminal_surrender_back_on_consequence), true, -1, false, null);
		}

		private void OnHeroDeath(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim == Hero.MainHero)
			{
				foreach (Clan clan in Clan.NonBanditFactions)
				{
					ChangeCrimeRatingAction.Apply(clan, -clan.MainHeroCrimeRating, true);
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					ChangeCrimeRatingAction.Apply(kingdom, -kingdom.MainHeroCrimeRating, true);
				}
			}
		}

		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			if (side1Faction == Hero.MainHero.MapFaction || side2Faction == Hero.MainHero.MapFaction)
			{
				IFaction faction = ((side1Faction == Hero.MainHero.MapFaction) ? side2Faction : side1Faction);
				float num = (float)Campaign.Current.Models.CrimeModel.DeclareWarCrimeRatingThreshold * 0.5f;
				if (faction.MainHeroCrimeRating > num)
				{
					ChangeCrimeRatingAction.Apply(faction, num - faction.MainHeroCrimeRating, true);
				}
			}
		}

		private static bool CanPayCriminalRatingValueWith(IFaction faction, CrimeModel.PaymentMethod paymentMethod)
		{
			if (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingModerate(Settlement.CurrentSettlement.MapFaction))
			{
				if (paymentMethod == CrimeModel.PaymentMethod.Gold)
				{
					return true;
				}
				if (CrimeCampaignBehavior.IsCriminalPlayerInSameKingdomOf(faction))
				{
					if (paymentMethod == CrimeModel.PaymentMethod.Influence)
					{
						return true;
					}
				}
				else if (paymentMethod == CrimeModel.PaymentMethod.Punishment)
				{
					return true;
				}
			}
			else if (Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction))
			{
				if (CrimeCampaignBehavior.IsCriminalPlayerInSameKingdomOf(faction))
				{
					if (paymentMethod == CrimeModel.PaymentMethod.Gold)
					{
						return true;
					}
					if (paymentMethod == CrimeModel.PaymentMethod.Influence)
					{
						return true;
					}
				}
				else
				{
					if (paymentMethod.HasAnyFlag(CrimeModel.PaymentMethod.Execution))
					{
						return Hero.MainHero.Gold < (int)PayForCrimeAction.GetClearCrimeCost(faction, CrimeModel.PaymentMethod.Gold);
					}
					if (paymentMethod.HasAllFlags(CrimeModel.PaymentMethod.Gold | CrimeModel.PaymentMethod.Punishment))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsCriminalPlayerInSameKingdomOf(IFaction faction)
		{
			Clan clan = faction as Clan;
			return Hero.MainHero.Clan == faction || Hero.MainHero.Clan.Kingdom == faction || (clan != null && Hero.MainHero.Clan.Kingdom == clan.Kingdom);
		}

		[GameMenuInitializationHandler("town_discuss_criminal_surrender")]
		[GameMenuInitializationHandler("town_inside_criminal")]
		public static void game_menu_town_criminal_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.Town.WaitMeshName);
		}

		public static void town_inside_criminal_on_init(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.CurrentSettlement == null)
			{
				PlayerEncounter.EnterSettlement();
			}
		}

		public static void town_discuss_criminal_surrender_on_init(MenuCallbackArgs args)
		{
		}

		public static bool criminal_inside_menu_pay_by_punishment_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			return CrimeCampaignBehavior.CanPayCriminalRatingValueWith(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Punishment);
		}

		public static void criminal_inside_menu_pay_by_punishment_on_consequence(MenuCallbackArgs args)
		{
			PayForCrimeAction.Apply(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Punishment);
			if (Hero.MainHero.DeathMark != KillCharacterAction.KillCharacterActionDetail.Murdered)
			{
				if (Campaign.Current.CurrentMenuContext != null)
				{
					if (Settlement.CurrentSettlement.IsCastle)
					{
						GameMenu.SwitchToMenu("castle_outside");
						return;
					}
					GameMenu.SwitchToMenu("town_outside");
					return;
				}
				else
				{
					PlayerEncounter.Finish(true);
				}
			}
		}

		public static bool criminal_inside_menu_give_money_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Trade;
			int num = (int)PayForCrimeAction.GetClearCrimeCost(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold);
			args.Text.SetTextVariable("FINE", num);
			args.Text.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			if (Hero.MainHero.Gold < num)
			{
				args.Tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", null);
				args.IsEnabled = false;
			}
			return CrimeCampaignBehavior.CanPayCriminalRatingValueWith(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold);
		}

		public static void criminal_inside_menu_give_money_on_consequence(MenuCallbackArgs args)
		{
			PayForCrimeAction.Apply(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold);
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle_outside");
				return;
			}
			GameMenu.SwitchToMenu("town_outside");
		}

		public static bool criminal_inside_menu_give_influence_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Bribe;
			float clearCrimeCost = PayForCrimeAction.GetClearCrimeCost(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Influence);
			args.Text.SetTextVariable("FINE", clearCrimeCost.ToString("F1"));
			args.Text.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
			if (Clan.PlayerClan.Influence < clearCrimeCost)
			{
				args.Tooltip = new TextObject("{=rMagXCrI}You don't have enough influence to get the charges dropped.", null);
				args.IsEnabled = false;
			}
			return CrimeCampaignBehavior.CanPayCriminalRatingValueWith(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Influence);
		}

		public static void criminal_inside_menu_give_influence_on_consequence(MenuCallbackArgs args)
		{
			PayForCrimeAction.Apply(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Influence);
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle_outside");
				return;
			}
			GameMenu.SwitchToMenu("town_outside");
		}

		public static bool criminal_inside_menu_give_punishment_and_money_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			int num = (int)PayForCrimeAction.GetClearCrimeCost(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold);
			args.Text.SetTextVariable("FINE", num);
			args.Text.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			if (Hero.MainHero.Gold < num)
			{
				args.Tooltip = new TextObject("{=ETKyjOkJ}You don't have enough denars to pay the fine.", null);
				args.IsEnabled = false;
			}
			return CrimeCampaignBehavior.CanPayCriminalRatingValueWith(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold | CrimeModel.PaymentMethod.Punishment);
		}

		public static void criminal_inside_menu_give_punishment_and_money_on_consequence(MenuCallbackArgs args)
		{
			PayForCrimeAction.Apply(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Gold | CrimeModel.PaymentMethod.Punishment);
			if (Hero.MainHero.DeathMark != KillCharacterAction.KillCharacterActionDetail.Murdered)
			{
				if (Campaign.Current.CurrentMenuContext != null)
				{
					if (Settlement.CurrentSettlement.IsCastle)
					{
						GameMenu.SwitchToMenu("castle_outside");
						return;
					}
					GameMenu.SwitchToMenu("town_outside");
					return;
				}
				else
				{
					PlayerEncounter.Finish(true);
				}
			}
		}

		public static bool criminal_inside_menu_give_your_life_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			return CrimeCampaignBehavior.CanPayCriminalRatingValueWith(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Execution);
		}

		public static void criminal_inside_menu_give_your_life_on_consequence(MenuCallbackArgs args)
		{
			PayForCrimeAction.Apply(Settlement.CurrentSettlement.MapFaction, CrimeModel.PaymentMethod.Execution);
		}

		public static bool criminal_inside_menu_ignore_charges_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return CrimeCampaignBehavior.IsCriminalPlayerInSameKingdomOf(Settlement.CurrentSettlement.MapFaction);
		}

		public static void criminal_inside_menu_ignore_charges_on_consequence(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle");
				return;
			}
			GameMenu.SwitchToMenu("town");
		}

		public static void town_discuss_criminal_surrender_back_on_consequence(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle_guard");
				return;
			}
			GameMenu.SwitchToMenu("town_guard");
		}

		public static bool town_discuss_criminal_surrender_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}
	}
}
