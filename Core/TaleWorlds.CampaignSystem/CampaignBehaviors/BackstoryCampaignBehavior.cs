﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class BackstoryCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			Hero heroObject = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_1_7").HeroObject;
			Hero heroObject2 = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_1_1").HeroObject;
			LogEntry.AddLogEntry(new CharacterInsultedLogEntry(heroObject, heroObject2, null, ActionNotes.ValorStrategyQuarrel), CampaignTime.Years(1075f) + CampaignTime.Weeks(3f) + CampaignTime.Days(2f));
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(heroObject, heroObject2, -50, false);
			Hero heroObject3 = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_4_1").HeroObject;
			Hero heroObject4 = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_4_16").HeroObject;
			LogEntry.AddLogEntry(new OverruleInfluenceLogEntry(heroObject3, heroObject4), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
			Settlement @object = Game.Current.ObjectManager.GetObject<Settlement>("town_V6");
			LogEntry.AddLogEntry(new SettlementClaimedLogEntry(heroObject4, @object), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
			ClaimSettlementAction.Apply(heroObject4, @object);
			Hero heroObject5 = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_2_1").HeroObject;
			Hero heroObject6 = Game.Current.ObjectManager.GetObject<CharacterObject>("dead_lord_2_2").HeroObject;
			LogEntry.AddLogEntry(new CharacterKilledLogEntry(heroObject6, heroObject5, KillCharacterAction.KillCharacterActionDetail.Murdered), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
			if (!heroObject6.Clan.IsMapFaction && heroObject6.Clan.Leader != null)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(heroObject5, heroObject6.Clan.Leader, -75, false);
			}
			Hero heroObject7 = Game.Current.ObjectManager.GetObject<CharacterObject>("lord_3_5").HeroObject;
			Hero nimr = Game.Current.ObjectManager.GetObject<CharacterObject>("dead_lord_3_1").HeroObject;
			LogEntry.AddLogEntry(new CharacterKilledLogEntry(nimr, heroObject7, KillCharacterAction.KillCharacterActionDetail.Murdered), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
			foreach (Hero hero in nimr.Clan.Heroes)
			{
				if (hero.IsLord && hero.Age < 35f && !hero.IsFemale && hero.GetTraitLevel(DefaultTraits.Mercy) < 1)
				{
					LogEntry.AddLogEntry(new CharacterInsultedLogEntry(hero, heroObject7, nimr.CharacterObject, ActionNotes.VengeanceQuarrel), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
				}
			}
			IEnumerable<Hero> deadOrDisabledHeroes = Hero.DeadOrDisabledHeroes;
			Func<Hero, bool> <>9__0;
			Func<Hero, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (Hero x) => x.Clan == nimr.Clan);
			}
			foreach (Hero hero2 in deadOrDisabledHeroes.Where(func))
			{
				if (hero2.IsLord && hero2.Age < 35f && !hero2.IsFemale && hero2.GetTraitLevel(DefaultTraits.Mercy) < 1)
				{
					LogEntry.AddLogEntry(new CharacterInsultedLogEntry(hero2, heroObject7, nimr.CharacterObject, ActionNotes.VengeanceQuarrel), CampaignTime.Years(1080f) + CampaignTime.Weeks(4f) + CampaignTime.Days(2f));
				}
			}
			if (nimr.Clan.Leader != null)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(heroObject7, nimr.Clan.Leader, -75, false);
			}
		}
	}
}
