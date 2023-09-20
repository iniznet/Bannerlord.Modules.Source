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
	// Token: 0x0200038D RID: 909
	public class DynamicBodyCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CC2 RID: 3266
		// (get) Token: 0x0600357B RID: 13691 RVA: 0x000E77D8 File Offset: 0x000E59D8
		// (set) Token: 0x0600357C RID: 13692 RVA: 0x000E77F7 File Offset: 0x000E59F7
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

		// Token: 0x17000CC3 RID: 3267
		// (get) Token: 0x0600357D RID: 13693 RVA: 0x000E7800 File Offset: 0x000E5A00
		private float MaxPlayerWeight
		{
			get
			{
				return MathF.Min(1f, this._unmodifiedWeight * 1.3f);
			}
		}

		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x0600357E RID: 13694 RVA: 0x000E7818 File Offset: 0x000E5A18
		private float MinPlayerWeight
		{
			get
			{
				return MathF.Max(0f, this._unmodifiedWeight * 0.7f);
			}
		}

		// Token: 0x17000CC5 RID: 3269
		// (get) Token: 0x0600357F RID: 13695 RVA: 0x000E7830 File Offset: 0x000E5A30
		private float MaxPlayerBuild
		{
			get
			{
				return MathF.Min(1f, this._unmodifiedBuild * 1.3f);
			}
		}

		// Token: 0x17000CC6 RID: 3270
		// (get) Token: 0x06003580 RID: 13696 RVA: 0x000E7848 File Offset: 0x000E5A48
		private float MinPlayerBuild
		{
			get
			{
				return MathF.Max(0f, this._unmodifiedBuild * 0.7f);
			}
		}

		// Token: 0x06003581 RID: 13697 RVA: 0x000E7860 File Offset: 0x000E5A60
		private void DailyTick()
		{
			bool flag = this.LastSettlementVisitTime.ElapsedDaysUntilNow < 1f;
			bool flag2 = Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Party.IsStarving;
			float num = ((Hero.MainHero.CurrentSettlement == null && flag2) ? (-0.1f) : (flag ? 0.025f : (-0.025f)));
			Hero.MainHero.Weight = MBMath.ClampFloat(Hero.MainHero.Weight + num, this.MinPlayerWeight, this.MaxPlayerWeight);
			float num2 = ((MapEvent.PlayerMapEvent != null || PlayerSiege.PlayerSiegeEvent != null || this._lastEncounterTime.ElapsedDaysUntilNow < 2f) ? 0.025f : (-0.015f));
			Hero.MainHero.Build = MBMath.ClampFloat(Hero.MainHero.Build + num2, this.MinPlayerBuild, this.MaxPlayerBuild);
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x000E7950 File Offset: 0x000E5B50
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party != null && party.IsMainParty)
			{
				this.LastSettlementVisitTime = CampaignTime.Now;
			}
		}

		// Token: 0x06003583 RID: 13699 RVA: 0x000E7968 File Offset: 0x000E5B68
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				this._lastEncounterTime = CampaignTime.Now;
			}
		}

		// Token: 0x06003584 RID: 13700 RVA: 0x000E797D File Offset: 0x000E5B7D
		private void OnPlayerBodyPropertiesChanged()
		{
			this._unmodifiedBuild = Hero.MainHero.Build;
			this._unmodifiedWeight = Hero.MainHero.Weight;
		}

		// Token: 0x06003585 RID: 13701 RVA: 0x000E799F File Offset: 0x000E5B9F
		private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			this._unmodifiedBuild = newPlayer.Build;
			this._unmodifiedWeight = newPlayer.Weight;
		}

		// Token: 0x06003586 RID: 13702 RVA: 0x000E79BC File Offset: 0x000E5BBC
		private void OnHeroCreated(Hero hero, bool bornNaturally)
		{
			if (!bornNaturally)
			{
				DynamicBodyProperties dynamicBodyPropertiesBetweenMinMaxRange = CharacterHelper.GetDynamicBodyPropertiesBetweenMinMaxRange(hero.CharacterObject);
				hero.Weight = dynamicBodyPropertiesBetweenMinMaxRange.Weight;
				hero.Build = dynamicBodyPropertiesBetweenMinMaxRange.Build;
			}
		}

		// Token: 0x06003587 RID: 13703 RVA: 0x000E79F0 File Offset: 0x000E5BF0
		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			this.OnPlayerBodyPropertiesChanged();
		}

		// Token: 0x06003588 RID: 13704 RVA: 0x000E79F8 File Offset: 0x000E5BF8
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

		// Token: 0x06003589 RID: 13705 RVA: 0x000E7AC0 File Offset: 0x000E5CC0
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastSettlementVisitTime", ref this._lastSettlementVisitTime);
			dataStore.SyncData<CampaignTime>("_lastEncounterTime", ref this._lastEncounterTime);
			dataStore.SyncData<float>("_unmodifiedWeight", ref this._unmodifiedWeight);
			dataStore.SyncData<float>("_unmodifiedBuild", ref this._unmodifiedBuild);
		}

		// Token: 0x0400114B RID: 4427
		private const float DailyBuildDecrease = -0.015f;

		// Token: 0x0400114C RID: 4428
		private const float DailyBuildIncrease = 0.025f;

		// Token: 0x0400114D RID: 4429
		private const float DailyWeightDecreaseWhenStarving = -0.1f;

		// Token: 0x0400114E RID: 4430
		private const float DailyWeightDecreaseWhenNotStarving = -0.025f;

		// Token: 0x0400114F RID: 4431
		private const float DailyWeightIncrease = 0.025f;

		// Token: 0x04001150 RID: 4432
		private CampaignTime _lastSettlementVisitTime = CampaignTime.Now;

		// Token: 0x04001151 RID: 4433
		private CampaignTime _lastEncounterTime = CampaignTime.Now;

		// Token: 0x04001152 RID: 4434
		private float _unmodifiedWeight = -1f;

		// Token: 0x04001153 RID: 4435
		private float _unmodifiedBuild = -1f;
	}
}
