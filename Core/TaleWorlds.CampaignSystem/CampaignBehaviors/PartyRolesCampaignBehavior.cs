using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003B9 RID: 953
	public class PartyRolesCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060038B2 RID: 14514 RVA: 0x001029F0 File Offset: 0x00100BF0
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

		// Token: 0x060038B3 RID: 14515 RVA: 0x00102A9E File Offset: 0x00100C9E
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060038B4 RID: 14516 RVA: 0x00102AA0 File Offset: 0x00100CA0
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (victim.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(victim);
			}
		}

		// Token: 0x060038B5 RID: 14517 RVA: 0x00102AB6 File Offset: 0x00100CB6
		private void OnHeroPrisonerTaken(PartyBase party, Hero prisoner)
		{
			if (prisoner.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(prisoner);
			}
		}

		// Token: 0x060038B6 RID: 14518 RVA: 0x00102ACC File Offset: 0x00100CCC
		private void OnGovernorChanged(Town fortification, Hero oldGovernor, Hero newGovernor)
		{
			if (((newGovernor != null) ? newGovernor.Clan : null) == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(newGovernor);
			}
		}

		// Token: 0x060038B7 RID: 14519 RVA: 0x00102AE8 File Offset: 0x00100CE8
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

		// Token: 0x060038B8 RID: 14520 RVA: 0x00102B6C File Offset: 0x00100D6C
		private void OnCompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			this.RemovePartyRoleIfExist(companion);
		}

		// Token: 0x060038B9 RID: 14521 RVA: 0x00102B75 File Offset: 0x00100D75
		private void OnHeroGetsBusy(Hero hero, HeroGetsBusyReasons heroGetsBusyReason)
		{
			if (hero.Clan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(hero);
			}
		}

		// Token: 0x060038BA RID: 14522 RVA: 0x00102B8B File Offset: 0x00100D8B
		private void OnHeroChangedClan(Hero hero, Clan oldClan)
		{
			if (oldClan == Clan.PlayerClan)
			{
				this.RemovePartyRoleIfExist(hero);
			}
		}

		// Token: 0x060038BB RID: 14523 RVA: 0x00102B9C File Offset: 0x00100D9C
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
