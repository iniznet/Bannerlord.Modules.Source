﻿using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	public class ChangePlayerCharacterAction
	{
		public static void Apply(Hero hero)
		{
			Hero mainHero = Hero.MainHero;
			MobileParty mainParty = MobileParty.MainParty;
			Game.Current.PlayerTroop = hero.CharacterObject;
			CampaignEventDispatcher.Instance.OnBeforePlayerCharacterChanged(mainHero, hero);
			bool flag;
			Campaign.Current.OnPlayerCharacterChanged(out flag);
			if (mainParty != MobileParty.MainParty && mainParty.IsActive)
			{
				if (mainParty.MemberRoster.TotalManCount == 0)
				{
					DestroyPartyAction.Apply(null, mainParty);
				}
				else
				{
					mainParty.LordPartyComponent.ChangePartyOwner(Hero.MainHero);
				}
			}
			if (hero.IsPrisoner)
			{
				PlayerCaptivity.OnPlayerCharacterChanged();
			}
			CampaignEventDispatcher.Instance.OnPlayerCharacterChanged(mainHero, hero, MobileParty.MainParty, flag);
			PartyBase.MainParty.Visuals.SetMapIconAsDirty();
			Campaign.Current.MainHeroIllDays = -1;
		}
	}
}
