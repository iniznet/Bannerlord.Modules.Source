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

namespace SandBox.View.Map
{
	public class MapGamepadEffectsView : MapView
	{
		protected internal override void CreateLayout()
		{
			base.CreateLayout();
			this.RegisterEvents();
		}

		protected internal override void OnFinalize()
		{
			base.OnFinalize();
			this.UnregisterEvents();
		}

		private void RegisterEvents()
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

		private void UnregisterEvents()
		{
			CampaignEvents.VillageBeingRaided.ClearListeners(this);
			CampaignEvents.OnSiegeBombardmentWallHitEvent.ClearListeners(this);
			CampaignEvents.OnSiegeEngineDestroyedEvent.ClearListeners(this);
			CampaignEvents.WarDeclared.ClearListeners(this);
			CampaignEvents.OnPeaceOfferedToPlayerEvent.ClearListeners(this);
			CampaignEvents.ArmyDispersed.ClearListeners(this);
			CampaignEvents.HeroLevelledUp.ClearListeners(this);
			CampaignEvents.KingdomDecisionAdded.ClearListeners(this);
			CampaignEvents.OnMainPartyStarvingEvent.ClearListeners(this);
			CampaignEvents.RebellionFinished.ClearListeners(this);
			CampaignEvents.OnHideoutSpottedEvent.ClearListeners(this);
			CampaignEvents.HeroCreated.ClearListeners(this);
			CampaignEvents.MakePeace.ClearListeners(this);
			CampaignEvents.HeroOrPartyTradedGold.ClearListeners(this);
			CampaignEvents.PartyAttachedAnotherParty.ClearListeners(this);
		}

		private void OnVillageRaid(Village village)
		{
			if (MobileParty.MainParty.CurrentSettlement == village.Settlement)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
			}
		}

		private void OnSiegeBombardmentWallHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, bool isWallCracked)
		{
			if (isWallCracked && (besiegerParty == MobileParty.MainParty || besiegedSettlement == MobileParty.MainParty.CurrentSettlement))
			{
				this.SetRumbleWithRandomValues(0.3f, 0.8f, 5);
			}
		}

		private void OnSiegeEngineDestroyed(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType destroyedEngine)
		{
			if (besiegerParty == MobileParty.MainParty || besiegedSettlement == MobileParty.MainParty.CurrentSettlement)
			{
				this.SetRumbleWithRandomValues(0.05f, 0.3f, 4);
			}
		}

		private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			if (faction1 == Clan.PlayerClan.MapFaction || faction2 == Clan.PlayerClan.MapFaction)
			{
				this.SetRumbleWithRandomValues(0.3f, 0.5f, 3);
			}
		}

		private void OnPeaceOfferedToPlayer(IFaction opponentFaction, int tributeAmount)
		{
			this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
		}

		private void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
			if (isPlayersArmy || army.Parties.Contains(MobileParty.MainParty))
			{
				this.SetRumbleWithRandomValues((float)army.TotalManCount / 2000f, (float)army.TotalManCount / 1000f, 5);
			}
		}

		private void OnHeroLevelUp(Hero hero, bool shouldNotify)
		{
			if (hero == Hero.MainHero && !(GameStateManager.Current.ActiveState is GameLoadingState))
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		private void OnKingdomDecisionAdded(KingdomDecision decision, bool isPlayerInvolved)
		{
			if (isPlayerInvolved)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 2);
			}
		}

		private void OnMainPartyStarving()
		{
			this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
		}

		private void OnRebellionFinished(Settlement settlement, Clan oldOwnerClan)
		{
			if (oldOwnerClan == Clan.PlayerClan)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 5);
			}
		}

		private void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
		{
			this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
		}

		private void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
			if (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
			}
		}

		private void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			if (side1Faction == Clan.PlayerClan.MapFaction || side2Faction == Clan.PlayerClan.MapFaction)
			{
				this.SetRumbleWithRandomValues(0.2f, 0.4f, 3);
			}
		}

		private void OnHeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			if (giver.Item1 == Hero.MainHero && Hero.MainHero.Gold == 0)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		private void OnPartyAttachedAnotherParty(MobileParty party)
		{
			if (party.Army != null && party.Army.LeaderParty == MobileParty.MainParty)
			{
				this.SetRumbleWithRandomValues(0.1f, 0.3f, 3);
			}
		}

		protected internal override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (Input.IsKeyDown(14))
			{
				this.SetRumbleWithRandomValues(0.5f, 0f, 5);
			}
		}

		private void SetRumbleWithRandomValues(float baseValue = 0f, float offsetRange = 1f, int frequencyCount = 5)
		{
			this.SetRandomRumbleValues(baseValue, offsetRange, frequencyCount);
		}

		private void SetRandomRumbleValues(float baseValue, float offsetRange, int frequencyCount)
		{
			baseValue = MBMath.ClampFloat(baseValue, 0f, 1f);
			offsetRange = MBMath.ClampFloat(offsetRange, 0f, 1f - baseValue);
			frequencyCount = MBMath.ClampInt(frequencyCount, 2, 5);
			for (int i = 0; i < frequencyCount; i++)
			{
				this._lowFrequencyLevels[i] = baseValue + MBRandom.RandomFloatRanged(offsetRange);
				this._lowFrequencyDurations[i] = baseValue + MBRandom.RandomFloatRanged(offsetRange);
				this._highFrequencyLevels[i] = baseValue + MBRandom.RandomFloatRanged(offsetRange);
				this._highFrequencyDurations[i] = baseValue + MBRandom.RandomFloatRanged(offsetRange);
			}
		}

		private readonly float[] _lowFrequencyLevels = new float[5];

		private readonly float[] _lowFrequencyDurations = new float[5];

		private readonly float[] _highFrequencyLevels = new float[5];

		private readonly float[] _highFrequencyDurations = new float[5];
	}
}
