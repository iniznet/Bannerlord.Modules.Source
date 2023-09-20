﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeClanLeaderAction
	{
		private static void ApplyInternal(Clan clan, Hero newLeader = null)
		{
			Hero leader = clan.Leader;
			if (newLeader == null)
			{
				Dictionary<Hero, int> heirApparents = clan.GetHeirApparents();
				if (heirApparents.Count == 0)
				{
					return;
				}
				int highestPoint = heirApparents.OrderByDescending((KeyValuePair<Hero, int> h) => h.Value).FirstOrDefault<KeyValuePair<Hero, int>>().Value;
				newLeader = heirApparents.Where((KeyValuePair<Hero, int> h) => h.Value.Equals(highestPoint)).GetRandomElementInefficiently<KeyValuePair<Hero, int>>().Key;
			}
			GiveGoldAction.ApplyBetweenCharacters(leader, newLeader, leader.Gold, true);
			if (newLeader.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(newLeader);
			}
			if (!newLeader.IsPrisoner && !newLeader.IsFugitive && !newLeader.IsReleased && !newLeader.IsTraveling)
			{
				MobileParty mobileParty = newLeader.PartyBelongedTo;
				if (mobileParty == null)
				{
					mobileParty = clan.CreateNewMobileParty(newLeader);
				}
				if (mobileParty.LeaderHero != newLeader)
				{
					mobileParty.ChangePartyLeader(newLeader);
				}
			}
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (hero != newLeader)
				{
					int relationChangeAfterClanLeaderIsDead = Campaign.Current.Models.DiplomacyModel.GetRelationChangeAfterClanLeaderIsDead(leader, hero);
					int heroRelation = CharacterRelationManager.GetHeroRelation(newLeader, hero);
					newLeader.SetPersonalRelation(hero, heroRelation + relationChangeAfterClanLeaderIsDead);
				}
			}
			clan.SetLeader(newLeader);
			CampaignEventDispatcher.Instance.OnClanLeaderChanged(leader, newLeader);
		}

		public static void ApplyWithSelectedNewLeader(Clan clan, Hero newLeader)
		{
			ChangeClanLeaderAction.ApplyInternal(clan, newLeader);
		}

		public static void ApplyWithoutSelectedNewLeader(Clan clan)
		{
			ChangeClanLeaderAction.ApplyInternal(clan, null);
		}
	}
}
