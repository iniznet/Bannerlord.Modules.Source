using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x02000099 RID: 153
	public class ControllerEffectsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000779 RID: 1913 RVA: 0x0003B3A8 File Offset: 0x000395A8
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0003B3AC File Offset: 0x000395AC
		public override void RegisterEvents()
		{
			CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.OnVillageRaid));
			CampaignEvents.OnSiegeBombardmentWallHitEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, bool>(this.OnSiegeBombardmentWallHit));
			CampaignEvents.OnSiegeEngineDestroyedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, BattleSideEnum, SiegeEngineType>(this.OnSiegeEngineDestroyed));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
			CampaignEvents.OnPeaceOfferedToPlayerEvent.AddNonSerializedListener(this, new Action<IFaction, int>(this.OnPeaceOfferedToPlayer));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroLevelUp));
			CampaignEvents.KingdomDecisionAdded.AddNonSerializedListener(this, new Action<KingdomDecision, bool>(this.OnKingdomDecisionAdded));
			CampaignEvents.OnMainPartyStarvingEvent.AddNonSerializedListener(this, new Action(this.OnMainPartyStarving));
			CampaignEvents.RebellionFinished.AddNonSerializedListener(this, new Action<Settlement, Clan>(this.OnRebellionFinished));
			CampaignEvents.OnHideoutSpottedEvent.AddNonSerializedListener(this, new Action<PartyBase, PartyBase>(this.OnHideoutSpotted));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
			CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.OnHeroOrPartyTradedGold));
			CampaignEvents.PartyAttachedAnotherParty.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyAttachedAnotherParty));
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0003B512 File Offset: 0x00039712
		private void OnHeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			if (giver.Item1 == Hero.MainHero && Hero.MainHero.Gold == 0)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0003B53E File Offset: 0x0003973E
		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			if (side1Faction == Clan.PlayerClan.MapFaction || side2Faction == Clan.PlayerClan.MapFaction)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0003B56B File Offset: 0x0003976B
		private void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
			if (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0003B598 File Offset: 0x00039798
		private void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
		{
			this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0003B5AB File Offset: 0x000397AB
		private void OnPartyAttachedAnotherParty(MobileParty party)
		{
			if (party.Army != null && party.Army.LeaderParty == MobileParty.MainParty)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0003B5D8 File Offset: 0x000397D8
		private void OnRebellionFinished(Settlement settlement, Clan oldOwnerClan)
		{
			if (oldOwnerClan == Clan.PlayerClan)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
			}
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0003B5F3 File Offset: 0x000397F3
		private void OnMainPartyStarving()
		{
			this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x0003B606 File Offset: 0x00039806
		private void OnPeaceOfferedToPlayer(IFaction opponentFaction, int tributeAmount)
		{
			this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0003B619 File Offset: 0x00039819
		private void OnKingdomDecisionAdded(KingdomDecision decision, bool isPlayerInvolved)
		{
			if (isPlayerInvolved)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 2);
			}
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0003B62F File Offset: 0x0003982F
		private void OnHeroLevelUp(Hero hero, bool shouldNotify)
		{
			if (hero == Hero.MainHero && !(GameStateManager.Current.ActiveState is GameLoadingState))
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0003B65B File Offset: 0x0003985B
		private void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
			if (isPlayersArmy || army.Parties.Contains(MobileParty.MainParty))
			{
				this.SetRumbleWithRandomValues((float)army.TotalManCount / 2000f, (float)army.TotalManCount / 1000f, 5);
			}
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0003B693 File Offset: 0x00039893
		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			if (faction1 == Clan.PlayerClan.MapFaction || faction2 == Clan.PlayerClan.MapFaction)
			{
				this.SetRumbleWithRandomValues(0.3f, 0.5f, 3);
			}
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0003B6C0 File Offset: 0x000398C0
		private void OnSiegeEngineDestroyed(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType destroyedEngine)
		{
			if (besiegerParty == MobileParty.MainParty || besiegedSettlement == MobileParty.MainParty.CurrentSettlement)
			{
				this.SetRumbleWithRandomValues(0.05f, 0.3f, 4);
			}
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0003B6E8 File Offset: 0x000398E8
		private void OnSiegeBombardmentWallHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, bool isWallCracked)
		{
			if (isWallCracked && (besiegerParty == MobileParty.MainParty || besiegedSettlement == MobileParty.MainParty.CurrentSettlement))
			{
				this.SetRumbleWithRandomValues(0.3f, 0.8f, 5);
			}
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0003B714 File Offset: 0x00039914
		private void OnVillageRaid(Village village)
		{
			if (MobileParty.MainParty.CurrentSettlement == village.Settlement)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
			}
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0003B739 File Offset: 0x00039939
		private void SetRumbleWithRandomValues(float baseValue = 0f, float ceilingValue = 1f, int frequencyCount = 5)
		{
			this.SetRandomRumbleValues(baseValue, ceilingValue, frequencyCount);
			Input.SetRumbleEffect(this._lowFrequencyLevels, this._lowFrequencyDurations, frequencyCount, this._highFrequencyLevels, this._highFrequencyDurations, frequencyCount);
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0003B764 File Offset: 0x00039964
		private void SetRandomRumbleValues(float baseValue, float ceilingValue, int frequencyCount)
		{
			baseValue = MBMath.ClampFloat(baseValue, 0f, 1f);
			ceilingValue = MBMath.ClampFloat(ceilingValue, 0f, 1f - baseValue);
			frequencyCount = MBMath.ClampInt(frequencyCount, 2, frequencyCount);
			for (int i = 0; i < frequencyCount; i++)
			{
				this._lowFrequencyLevels[i] = baseValue + MBRandom.RandomFloatRanged(ceilingValue);
				this._highFrequencyLevels[i] = baseValue + MBRandom.RandomFloatRanged(ceilingValue);
				this._lowFrequencyDurations[i] = baseValue + MBRandom.RandomFloatRanged(ceilingValue);
				this._highFrequencyDurations[i] = baseValue + MBRandom.RandomFloatRanged(ceilingValue);
			}
		}

		// Token: 0x04000311 RID: 785
		private readonly float[] _lowFrequencyLevels = new float[5];

		// Token: 0x04000312 RID: 786
		private readonly float[] _highFrequencyLevels = new float[5];

		// Token: 0x04000313 RID: 787
		private readonly float[] _lowFrequencyDurations = new float[5];

		// Token: 0x04000314 RID: 788
		private readonly float[] _highFrequencyDurations = new float[5];
	}
}
