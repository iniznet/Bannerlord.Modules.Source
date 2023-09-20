using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PartyRolesCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnGovernorChangedEvent.AddNonSerializedListener(this, new Action<Town, Hero, Hero>(this.OnGovernorChanged));
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartySpawned));
			CampaignEvents.CompanionRemoved.AddNonSerializedListener(this, new Action<Hero, RemoveCompanionAction.RemoveCompanionDetail>(this.OnCompanionRemoved));
			CampaignEvents.OnHeroGetsBusyEvent.AddNonSerializedListener(this, new Action<Hero, HeroGetsBusyReasons>(this.OnHeroGetsBusy));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
			CampaignEvents.OnHeroChangedClanEvent.AddNonSerializedListener(this, new Action<Hero, Clan>(this.OnHeroChangedClan));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (victim.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(victim);
			}
		}

		private void OnHeroPrisonerTaken(PartyBase party, Hero prisoner)
		{
			if (prisoner.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(prisoner);
			}
		}

		private void OnGovernorChanged(Town fortification, Hero oldGovernor, Hero newGovernor)
		{
			if (((newGovernor != null) ? newGovernor.Clan : null) == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(newGovernor);
			}
		}

		private void OnPartySpawned(MobileParty spawnedParty)
		{
			if (spawnedParty.IsLordParty && spawnedParty.ActualClan == Clan.PlayerClan)
			{
				foreach (TroopRosterElement troopRosterElement in spawnedParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character.IsHero)
					{
						this.RemovePartyRoleIfExist(troopRosterElement.Character.HeroObject);
					}
				}
			}
		}

		private void OnCompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			this.RemovePartyRoleIfExist(companion);
		}

		private void OnHeroGetsBusy(Hero hero, HeroGetsBusyReasons heroGetsBusyReason)
		{
			if (hero.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(hero);
			}
		}

		private void OnHeroChangedClan(Hero hero, Clan oldClan)
		{
			if (oldClan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(hero);
			}
		}

		private void RemovePartyRoleIfExist(Hero hero)
		{
			foreach (WarPartyComponent warPartyComponent in Clan.PlayerClan.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty.GetHeroPerkRole(hero) != SkillEffect.PerkRole.None)
				{
					warPartyComponent.MobileParty.RemoveHeroPerkRole(hero);
				}
			}
		}
	}
}
