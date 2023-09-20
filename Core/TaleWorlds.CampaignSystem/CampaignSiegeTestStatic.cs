using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200006E RID: 110
	public static class CampaignSiegeTestStatic
	{
		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x00042F34 File Offset: 0x00041134
		public static MobileParty AttackerParty
		{
			get
			{
				return CampaignSiegeTestStatic._testParties[CampaignSiegeTestStatic._attackerPartyIndex];
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000E93 RID: 3731 RVA: 0x00042F41 File Offset: 0x00041141
		public static MobileParty DefenderParty
		{
			get
			{
				return CampaignSiegeTestStatic._testParties[CampaignSiegeTestStatic._defenderPartyIndex];
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000E94 RID: 3732 RVA: 0x00042F4E File Offset: 0x0004114E
		public static bool IsSiegeTestBuild
		{
			get
			{
				if (!CampaignSiegeTestStatic._siegeTest)
				{
					return false;
				}
				if (Campaign.Current == null || Campaign.Current.GameManager == null)
				{
					throw new Exception("Campaign or GameManager has not been initialized.");
				}
				return CampaignOptions.IsSiegeTestBuild;
			}
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x00042F7C File Offset: 0x0004117C
		public static void AddGameMenu(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenu("siege_test_menu", "{=!}Siege Test", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_leave", "{=!}Normal campaign.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.DisableSiegeTest();
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_siege_attacker", "{=!}Siege Attacker test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationAttackerConsequence(false);
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_siege_defender", "{=!}Siege Defender test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationDefenderConsequence(false);
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_siege_lh_attacker", "{=!}Siege LH Attacker test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationAttackerConsequence(true);
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_siege_lh_defender", "{=!}Siege LH Defender test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationDefenderConsequence(true);
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_sally_out", "{=!}SallyOut test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationSallyOutConsequence();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_dialog_testing", "{=!}Dialogs test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationDialogsConsequence();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("siege_test_menu", "siege_test_menu_hideout", "{=!}Hideout test.", null, delegate(MenuCallbackArgs args)
			{
				CampaignSiegeTestStatic.PreparationHideoutConsequence();
			}, false, -1, false, null);
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00043168 File Offset: 0x00041368
		private static void PreparationAttackerConsequence(bool forceLordsHall = false)
		{
			CampaignCheats.SetMainPartyAttackable(new List<string> { "0" });
			if (forceLordsHall)
			{
				CampaignSiegeTestStatic.PreparePartiesForLH();
			}
			else
			{
				CampaignSiegeTestStatic.PrepareParties();
			}
			CampaignSiegeTestStatic._attackerPartyIndex = 0;
			CampaignSiegeTestStatic._defenderPartyIndex = 1;
			CampaignSiegeTestStatic.SetPartiesForTest();
			MobileParty.MainParty.CurrentSettlement = CampaignSiegeTestStatic._settlement;
			Campaign.Current.SiegeEventManager.StartSiegeEvent(CampaignSiegeTestStatic._settlement, CampaignSiegeTestStatic.AttackerParty);
			CampaignSiegeTestStatic._settlement.Party.SiegeEvent.SiegeTestPreparation();
			PlayerEncounter.RestartPlayerEncounter(CampaignSiegeTestStatic._settlement.Party, CampaignSiegeTestStatic.AttackerParty.Party, true);
			PlayerEncounter.StartBattle();
			CampaignSiegeTestStatic.DefenderParty.Party.MapEventSide = MapEvent.PlayerMapEvent.DefenderSide;
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Attacker, false, null);
			GameMenu.ExitToLast();
			if (forceLordsHall)
			{
				CampaignSiegeTestStatic._settlement.ResetSiegeState();
				while (CampaignSiegeTestStatic._settlement.CurrentSiegeState != Settlement.SiegeState.InTheLordsHall)
				{
					CampaignSiegeTestStatic._settlement.SetNextSiegeState();
				}
			}
			PlayerSiege.StartSiegeMission(false, null);
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x0004325C File Offset: 0x0004145C
		private static void PreparationDefenderConsequence(bool forceLordsHall = false)
		{
			CampaignCheats.SetMainPartyAttackable(new List<string> { "0" });
			if (forceLordsHall)
			{
				CampaignSiegeTestStatic.PreparePartiesForLH();
			}
			else
			{
				CampaignSiegeTestStatic.PrepareParties();
			}
			CampaignSiegeTestStatic._attackerPartyIndex = 1;
			CampaignSiegeTestStatic._defenderPartyIndex = 0;
			CampaignSiegeTestStatic.SetPartiesForTest();
			Campaign.Current.SiegeEventManager.StartSiegeEvent(CampaignSiegeTestStatic._settlement, CampaignSiegeTestStatic.AttackerParty);
			CampaignSiegeTestStatic._settlement.Party.SiegeEvent.SiegeTestPreparation();
			MobileParty.MainParty.CurrentSettlement = CampaignSiegeTestStatic._settlement;
			StartBattleAction.ApplyStartAssaultAgainstWalls(CampaignSiegeTestStatic.AttackerParty, CampaignSiegeTestStatic._settlement);
			PlayerEncounter.RestartPlayerEncounter(CampaignSiegeTestStatic._settlement.Party, PartyBase.MainParty, true);
			PlayerEncounter.JoinBattle(BattleSideEnum.Defender);
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Defender, false, CampaignSiegeTestStatic._settlement);
			GameMenu.ExitToLast();
			if (forceLordsHall)
			{
				CampaignSiegeTestStatic._settlement.ResetSiegeState();
				while (CampaignSiegeTestStatic._settlement.CurrentSiegeState != Settlement.SiegeState.InTheLordsHall)
				{
					CampaignSiegeTestStatic._settlement.SetNextSiegeState();
				}
			}
			PlayerSiege.StartSiegeMission(false, null);
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x00043344 File Offset: 0x00041544
		private static void PreparationDialogsConsequence()
		{
			CampaignSiegeTestStatic.DisableSiegeTest();
			GameMenu.ExitToLast();
			Kingdom southernEmpire = Campaign.Current.CampaignObjectManager.Find<Kingdom>("empire_s");
			Kingdom westernEmpire = Campaign.Current.CampaignObjectManager.Find<Kingdom>("empire_w");
			ChangeKingdomAction.ApplyByJoinToKingdom(Hero.MainHero.Clan, westernEmpire, true);
			ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(Clan.PlayerClan, true);
			Hero heroObject = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_1_15").HeroObject;
			heroObject.SetHasMet();
			MarriageAction.Apply(heroObject, Hero.MainHero, true);
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero.IsLord && hero.MapFaction == heroObject.MapFaction)
				{
					if (hero.Clan == heroObject.Clan)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, Hero.MainHero, 40, false);
					}
					else if (hero.GetTraitLevel(DefaultTraits.Mercy) < 0)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, Hero.MainHero, -30, false);
					}
					hero.SetHasMet();
				}
			}
			IEnumerable<MobileParty> all = MobileParty.All;
			Func<MobileParty, bool> <>9__0;
			Func<MobileParty, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (MobileParty x) => x.MapFaction == westernEmpire);
			}
			Func<Settlement, bool> <>9__1;
			foreach (MobileParty mobileParty in all.Where(func))
			{
				IEnumerable<Settlement> all2 = Settlement.All;
				Func<Settlement, bool> func2;
				if ((func2 = <>9__1) == null)
				{
					func2 = (<>9__1 = (Settlement x) => x.Party.MapFaction == southernEmpire && x.IsVillage);
				}
				using (IEnumerator<Settlement> enumerator3 = all2.Where(func2).GetEnumerator())
				{
					if (!enumerator3.MoveNext())
					{
						break;
					}
					ChangeVillageStateAction.ApplyBySettingToLooted(enumerator3.Current, mobileParty);
					break;
				}
			}
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00043540 File Offset: 0x00041740
		private static void PreparationSallyOutConsequence()
		{
			CampaignCheats.SetMainPartyAttackable(new List<string> { "0" });
			CampaignSiegeTestStatic.PrepareParties();
			CampaignSiegeTestStatic._attackerPartyIndex = 1;
			CampaignSiegeTestStatic._defenderPartyIndex = 0;
			CampaignSiegeTestStatic.SetPartiesForTest();
			MobileParty.MainParty.CurrentSettlement = CampaignSiegeTestStatic._settlement;
			Campaign.Current.SiegeEventManager.StartSiegeEvent(CampaignSiegeTestStatic._settlement, CampaignSiegeTestStatic.AttackerParty);
			CampaignSiegeTestStatic._settlement.Party.SiegeEvent.SiegeTestPreparation();
			PlayerEncounter.RestartPlayerEncounter(PartyBase.MainParty, CampaignSiegeTestStatic.AttackerParty.Party, true);
			PlayerEncounter.Current.ForceSallyOut = true;
			PlayerEncounter.StartBattle();
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Defender, false, CampaignSiegeTestStatic._settlement);
			GameMenu.ExitToLast();
			PlayerSiege.StartSiegeMission(true, null);
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x000435F4 File Offset: 0x000417F4
		private static void PrepareParties()
		{
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>("town_B3");
			Settlement object2 = MBObjectManager.Instance.GetObject<Settlement>("town_B2");
			object2.Party.MemberRoster.Clear();
			object2.Town.OwnerClan = Hero.MainHero.Clan;
			bool flag = false;
			foreach (MobileParty mobileParty in object2.Parties.ToList<MobileParty>())
			{
				if (!flag && mobileParty.StringId.Contains("garrison"))
				{
					mobileParty.MemberRoster.Clear();
					mobileParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_volunteer"), 25, false, 0, 0, true, -1);
					mobileParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_hero"), 30, false, 0, 0, true, -1);
					flag = true;
				}
				else
				{
					LeaveSettlementAction.ApplyForParty(mobileParty);
					mobileParty.Position2D = (@object.IsVillage ? @object.Position2D : @object.GatePosition);
				}
			}
			CampaignSiegeTestStatic._settlement = object2;
			CampaignSiegeTestStatic._testParties = new MobileParty[2];
			MobileParty mainParty = MobileParty.MainParty;
			mainParty.MemberRoster.RemoveIf((TroopRosterElement rosterElement) => rosterElement.Character != null && rosterElement.Character != CharacterObject.PlayerCharacter);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_volunteer"), 25, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_trained_warrior"), 20, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_falxman"), 10, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_oathsworn"), 10, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_highborn_warrior"), 30, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_hero"), 30, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_fian"), 30, false, 0, 0, true, -1);
			CampaignSiegeTestStatic._testParties[0] = mainParty;
			MobileParty party1 = MobileParty.All.FirstOrDefault((MobileParty mParty) => mParty.IsLordParty && mParty.LeaderHero != null && mParty != MobileParty.MainParty);
			party1.MemberRoster.RemoveIf((TroopRosterElement item) => item.Character != party1.LeaderHero.CharacterObject);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit"), 0, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_infantryman"), 50, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_trained_infantryman"), 20, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_menavliaton"), 60, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_sergeant_crossbowman"), 30, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_equite"), 20, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_archer"), 25, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_trained_archer"), 18, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_elite_menavliaton"), 28, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_legionary"), 35, false, 0, 0, true, -1);
			party1.Ai.DisableAi();
			CampaignSiegeTestStatic._testParties[1] = party1;
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x00043A5C File Offset: 0x00041C5C
		private static void PreparePartiesForLH()
		{
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>("town_B3");
			Settlement object2 = MBObjectManager.Instance.GetObject<Settlement>("town_B2");
			object2.Party.MemberRoster.Clear();
			object2.Town.OwnerClan = Hero.MainHero.Clan;
			bool flag = false;
			foreach (MobileParty mobileParty in object2.Parties.ToList<MobileParty>())
			{
				if (!flag && mobileParty.StringId.Contains("garrison"))
				{
					mobileParty.MemberRoster.Clear();
					mobileParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_volunteer"), 2, false, 0, 0, true, -1);
					mobileParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_hero"), 3, false, 0, 0, true, -1);
					flag = true;
				}
				else
				{
					LeaveSettlementAction.ApplyForParty(mobileParty);
					mobileParty.Position2D = (@object.IsVillage ? @object.Position2D : @object.GatePosition);
				}
			}
			CampaignSiegeTestStatic._settlement = object2;
			CampaignSiegeTestStatic._testParties = new MobileParty[2];
			MobileParty mainParty = MobileParty.MainParty;
			mainParty.MemberRoster.RemoveIf((TroopRosterElement rosterElement) => rosterElement.Character != null && rosterElement.Character != CharacterObject.PlayerCharacter);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_volunteer"), 2, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_trained_warrior"), 2, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_falxman"), 1, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_oathsworn"), 1, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_highborn_warrior"), 3, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_hero"), 3, false, 0, 0, true, -1);
			mainParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("battanian_fian"), 3, false, 0, 0, true, -1);
			CampaignSiegeTestStatic._testParties[0] = mainParty;
			MobileParty party1 = MobileParty.All.FirstOrDefault((MobileParty mParty) => mParty.IsLordParty && mParty.LeaderHero != null && mParty != MobileParty.MainParty);
			party1.MemberRoster.RemoveIf((TroopRosterElement item) => item.Character != party1.LeaderHero.CharacterObject);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit"), 0, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_infantryman"), 5, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_trained_infantryman"), 2, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_infantryman"), 6, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_sergeant_crossbowman"), 3, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_equite"), 2, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_archer"), 2, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_archer"), 1, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_legionary"), 2, false, 0, 0, true, -1);
			party1.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_menavliaton"), 3, false, 0, 0, true, -1);
			party1.Ai.DisableAi();
			CampaignSiegeTestStatic._testParties[1] = party1;
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x00043EB0 File Offset: 0x000420B0
		private static void PreparationHideoutConsequence()
		{
			CampaignCheats.SetMainPartyAttackable(new List<string> { "0" });
			Settlement settlement = Settlement.All.FirstOrDefault((Settlement s) => s.IsHideout && s.Parties.Count > 0);
			if (settlement == null)
			{
				return;
			}
			MobileParty.MainParty.Position2D = settlement.Position2D;
			PlayerEncounter.RestartPlayerEncounter(settlement.Party, PartyBase.MainParty, true);
			if (Settlement.CurrentSettlement.OwnerClan != null)
			{
				ChangeRelationAction.ApplyPlayerRelation(Settlement.CurrentSettlement.OwnerClan.Leader, -10, true, true);
			}
			PlayerEncounter.StartBattle();
			foreach (MobileParty mobileParty in settlement.Parties)
			{
				mobileParty.Party.MapEventSide = PlayerEncounter.Battle.DefenderSide;
			}
			PlayerEncounter.Update();
			CampaignMission.OpenHideoutBattleMission(Settlement.CurrentSettlement.Hideout.SceneName, null);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00043FBC File Offset: 0x000421BC
		private static void SetPartiesForTest()
		{
			if (CampaignSiegeTestStatic.AttackerParty.MapFaction == CampaignSiegeTestStatic._settlement.MapFaction || !FactionManager.IsAtWarAgainstFaction(CampaignSiegeTestStatic.AttackerParty.MapFaction, CampaignSiegeTestStatic._settlement.MapFaction))
			{
				Kingdom kingdom = Kingdom.All.FirstOrDefault((Kingdom f) => FactionManager.IsAtWarAgainstFaction(f, CampaignSiegeTestStatic._settlement.MapFaction));
				if (kingdom == null)
				{
					kingdom = Kingdom.All.FirstOrDefault((Kingdom f) => f != CampaignSiegeTestStatic._settlement.MapFaction);
					DeclareWarAction.ApplyByDefault(kingdom, CampaignSiegeTestStatic._settlement.MapFaction);
				}
				ChangeKingdomAction.ApplyByJoinToKingdom(CampaignSiegeTestStatic.AttackerParty.LeaderHero.Clan, kingdom, true);
			}
			if (CampaignSiegeTestStatic.DefenderParty.MapFaction != CampaignSiegeTestStatic._settlement.MapFaction)
			{
				ChangeKingdomAction.ApplyByJoinToKingdom(CampaignSiegeTestStatic.DefenderParty.LeaderHero.Clan, (Kingdom)CampaignSiegeTestStatic._settlement.MapFaction, true);
			}
			CampaignSiegeTestStatic.AttackerParty.Position2D = (CampaignSiegeTestStatic._settlement.IsTown ? CampaignSiegeTestStatic._settlement.GatePosition : CampaignSiegeTestStatic._settlement.Position2D);
			CampaignSiegeTestStatic.DefenderParty.Position2D = CampaignSiegeTestStatic._settlement.Position2D;
			if (!CampaignSiegeTestStatic.DefenderParty.IsMainParty)
			{
				EnterSettlementAction.ApplyForParty(CampaignSiegeTestStatic.DefenderParty, CampaignSiegeTestStatic._settlement);
			}
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0004410C File Offset: 0x0004230C
		public static void DisableSiegeTest()
		{
			CampaignSiegeTestStatic._siegeTest = false;
			CampaignSiegeTestStatic.Destruct();
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00044119 File Offset: 0x00042319
		public static void Destruct()
		{
			CampaignSiegeTestStatic._settlement = null;
			CampaignSiegeTestStatic._testParties = null;
			CampaignSiegeTestStatic._attackerPartyIndex = -1;
			CampaignSiegeTestStatic._defenderPartyIndex = -1;
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00044133 File Offset: 0x00042333
		[GameMenuInitializationHandler("siege_test_menu")]
		private static void game_menu_siege_test_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_besieging");
		}

		// Token: 0x0400043D RID: 1085
		public static int SiegeLevel = 1;

		// Token: 0x0400043E RID: 1086
		public static string SiegeTestSceneName = "vlandia_castle_005a";

		// Token: 0x0400043F RID: 1087
		public const string SettlementId = "town_B2";

		// Token: 0x04000440 RID: 1088
		public const string SettlementToMoveId = "town_B3";

		// Token: 0x04000441 RID: 1089
		private static MobileParty[] _testParties;

		// Token: 0x04000442 RID: 1090
		private static Settlement _settlement;

		// Token: 0x04000443 RID: 1091
		private static int _attackerPartyIndex = -1;

		// Token: 0x04000444 RID: 1092
		private static int _defenderPartyIndex = -1;

		// Token: 0x04000445 RID: 1093
		private static bool _siegeTest = true;
	}
}
