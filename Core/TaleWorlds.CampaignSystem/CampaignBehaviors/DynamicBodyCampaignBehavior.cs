using System;
using Helpers;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class DynamicBodyCampaignBehavior : CampaignBehaviorBase
	{
		private CampaignTime LastSettlementVisitTime
		{
			get
			{
				if (Hero.MainHero.CurrentSettlement != null)
				{
					this._lastSettlementVisitTime = CampaignTime.Now;
				}
				return this._lastSettlementVisitTime;
			}
			set
			{
				this._lastSettlementVisitTime = value;
			}
		}

		private float MaxPlayerWeight
		{
			get
			{
				return MathF.Min(1f, this._unmodifiedWeight * 1.3f);
			}
		}

		private float MinPlayerWeight
		{
			get
			{
				return MathF.Max(0f, this._unmodifiedWeight * 0.7f);
			}
		}

		private float MaxPlayerBuild
		{
			get
			{
				return MathF.Min(1f, this._unmodifiedBuild * 1.3f);
			}
		}

		private float MinPlayerBuild
		{
			get
			{
				return MathF.Max(0f, this._unmodifiedBuild * 0.7f);
			}
		}

		private void DailyTick()
		{
			bool flag = this.LastSettlementVisitTime.ElapsedDaysUntilNow < 1f;
			bool flag2 = Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Party.IsStarving;
			float num = ((Hero.MainHero.CurrentSettlement == null && flag2) ? (-0.1f) : (flag ? 0.025f : (-0.025f)));
			Hero.MainHero.Weight = MBMath.ClampFloat(Hero.MainHero.Weight + num, this.MinPlayerWeight, this.MaxPlayerWeight);
			float num2 = ((MapEvent.PlayerMapEvent != null || PlayerSiege.PlayerSiegeEvent != null || this._lastEncounterTime.ElapsedDaysUntilNow < 2f) ? 0.025f : (-0.015f));
			Hero.MainHero.Build = MBMath.ClampFloat(Hero.MainHero.Build + num2, this.MinPlayerBuild, this.MaxPlayerBuild);
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party != null && party.IsMainParty)
			{
				this.LastSettlementVisitTime = CampaignTime.Now;
			}
		}

		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				this._lastEncounterTime = CampaignTime.Now;
			}
		}

		private void OnPlayerBodyPropertiesChanged()
		{
			this._unmodifiedBuild = Hero.MainHero.Build;
			this._unmodifiedWeight = Hero.MainHero.Weight;
		}

		private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			this._unmodifiedBuild = newPlayer.Build;
			this._unmodifiedWeight = newPlayer.Weight;
		}

		private void OnHeroCreated(Hero hero, bool bornNaturally)
		{
			if (!bornNaturally)
			{
				DynamicBodyProperties dynamicBodyPropertiesBetweenMinMaxRange = CharacterHelper.GetDynamicBodyPropertiesBetweenMinMaxRange(hero.CharacterObject);
				hero.Weight = dynamicBodyPropertiesBetweenMinMaxRange.Weight;
				hero.Build = dynamicBodyPropertiesBetweenMinMaxRange.Build;
			}
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			this.OnPlayerBodyPropertiesChanged();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.OnPlayerBodyPropertiesChangedEvent.AddNonSerializedListener(this, new Action(this.OnPlayerBodyPropertiesChanged));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnPlayerBodyPropertiesChanged));
			CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChanged));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastSettlementVisitTime", ref this._lastSettlementVisitTime);
			dataStore.SyncData<CampaignTime>("_lastEncounterTime", ref this._lastEncounterTime);
			dataStore.SyncData<float>("_unmodifiedWeight", ref this._unmodifiedWeight);
			dataStore.SyncData<float>("_unmodifiedBuild", ref this._unmodifiedBuild);
		}

		private const float DailyBuildDecrease = -0.015f;

		private const float DailyBuildIncrease = 0.025f;

		private const float DailyWeightDecreaseWhenStarving = -0.1f;

		private const float DailyWeightDecreaseWhenNotStarving = -0.025f;

		private const float DailyWeightIncrease = 0.025f;

		private CampaignTime _lastSettlementVisitTime = CampaignTime.Now;

		private CampaignTime _lastEncounterTime = CampaignTime.Now;

		private float _unmodifiedWeight = -1f;

		private float _unmodifiedBuild = -1f;
	}
}
