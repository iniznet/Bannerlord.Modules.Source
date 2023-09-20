using System;
using Helpers;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D4 RID: 980
	public class SiegeAmbushCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003B14 RID: 15124 RVA: 0x00115D2C File Offset: 0x00113F2C
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.OnPlayerSiegeStartedEvent.AddNonSerializedListener(this, new Action(this.OnPlayerSiegeStarted));
			CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventEnded));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x00115DAC File Offset: 0x00113FAC
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastAmbushTime", ref this._lastAmbushTime);
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x00115DC0 File Offset: 0x00113FC0
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x06003B17 RID: 15127 RVA: 0x00115DC9 File Offset: 0x00113FC9
		private void OnPlayerSiegeStarted()
		{
			if (PlayerSiege.PlayerSiegeEvent.BesiegerCamp.IsPreparationComplete)
			{
				this._lastAmbushTime = CampaignTime.Now;
			}
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x00115DE7 File Offset: 0x00113FE7
		private void OnSiegeEventEnded(SiegeEvent siegeEvent)
		{
			if (siegeEvent == PlayerSiege.PlayerSiegeEvent)
			{
				this._lastAmbushTime = CampaignTime.Never;
			}
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x00115DFC File Offset: 0x00113FFC
		private void HourlyTick()
		{
			if (PlayerSiege.PlayerSiegeEvent != null && this._lastAmbushTime == CampaignTime.Never && PlayerSiege.PlayerSiegeEvent.BesiegerCamp.IsPreparationComplete)
			{
				this._lastAmbushTime = CampaignTime.Now;
			}
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x00115E33 File Offset: 0x00114033
		private void OnMissionEnded(IMission mission)
		{
			PlayerEncounter playerEncounter = PlayerEncounter.Current;
			if (playerEncounter == null)
			{
				return;
			}
			playerEncounter.SetIsSallyOutAmbush(false);
		}

		// Token: 0x06003B1B RID: 15131 RVA: 0x00115E48 File Offset: 0x00114048
		private bool CanMainHeroAmbush(out TextObject reason)
		{
			if (this._lastAmbushTime.ElapsedHoursUntilNow < 24f)
			{
				reason = new TextObject("{=lCYPxuWN}The enemy is alert, you cannot ambush right now.", null);
				return false;
			}
			if (Hero.MainHero.IsWounded)
			{
				reason = new TextObject("{=pQaQW1As}You cannot ambush right now due to your wounds.", null);
				return false;
			}
			SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
			if (playerSiegeEvent.BesiegerCamp.BesiegerParty.MapEvent != null && MobileParty.MainParty.MapEvent == null)
			{
				reason = new TextObject("{=GAh1gNYn}Enemies are already engaged in battle.", null);
				return false;
			}
			if (playerSiegeEvent.GetPreparedSiegeEnginesAsDictionary(playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker)).Count <= 0)
			{
				reason = new TextObject("{=f4g7r0xF}The enemy does not have any vulnerabilities.", null);
				return false;
			}
			if (playerSiegeEvent.BesiegedSettlement.SettlementWallSectionHitPointsRatioList.AnyQ((float r) => r <= 0f))
			{
				reason = new TextObject("{=Nzt8Xkro}You cannot ambush because the settlement walls are breached.", null);
				return false;
			}
			reason = TextObject.Empty;
			return true;
		}

		// Token: 0x06003B1C RID: 15132 RVA: 0x00115F30 File Offset: 0x00114130
		private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_ambush", "{=LEKzuGzi}Ambush", new GameMenuOption.OnConditionDelegate(this.menu_siege_strategies_ambush_condition), new GameMenuOption.OnConsequenceDelegate(this.menu_siege_strategies_ambush_on_consequence), false, -1, false, null);
		}

		// Token: 0x06003B1D RID: 15133 RVA: 0x00115F70 File Offset: 0x00114170
		private bool menu_siege_strategies_ambush_condition(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSide != BattleSideEnum.Defender)
			{
				return false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.SiegeAmbush;
			TextObject textObject;
			if (!this.CanMainHeroAmbush(out textObject))
			{
				args.IsEnabled = false;
				args.Tooltip = textObject;
			}
			return true;
		}

		// Token: 0x06003B1E RID: 15134 RVA: 0x00115FB0 File Offset: 0x001141B0
		private void menu_siege_strategies_ambush_on_consequence(MenuCallbackArgs args)
		{
			this._lastAmbushTime = CampaignTime.Now;
			if (PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.SiegeEvent != null && !PlayerEncounter.EncounterSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
			{
				PlayerEncounter.RestartPlayerEncounter(PartyBase.MainParty, PlayerEncounter.EncounterSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Party, false);
			}
			PlayerEncounter.Current.SetIsSallyOutAmbush(true);
			PlayerEncounter.StartBattle();
			MenuHelper.EncounterAttackConsequence(args);
		}

		// Token: 0x04001217 RID: 4631
		private const int SiegeAmbushCooldownPeriodAsHours = 24;

		// Token: 0x04001218 RID: 4632
		private CampaignTime _lastAmbushTime = CampaignTime.Never;
	}
}
