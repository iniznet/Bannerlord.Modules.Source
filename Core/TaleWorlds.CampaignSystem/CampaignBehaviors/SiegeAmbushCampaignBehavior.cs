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
	public class SiegeAmbushCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.OnPlayerSiegeStartedEvent.AddNonSerializedListener(this, new Action(this.OnPlayerSiegeStarted));
			CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventEnded));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastAmbushTime", ref this._lastAmbushTime);
		}

		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		private void OnPlayerSiegeStarted()
		{
			if (PlayerSiege.PlayerSiegeEvent.BesiegerCamp.IsPreparationComplete)
			{
				this._lastAmbushTime = CampaignTime.Now;
			}
		}

		private void OnSiegeEventEnded(SiegeEvent siegeEvent)
		{
			if (siegeEvent == PlayerSiege.PlayerSiegeEvent)
			{
				this._lastAmbushTime = CampaignTime.Never;
			}
		}

		private void HourlyTick()
		{
			if (PlayerSiege.PlayerSiegeEvent != null && this._lastAmbushTime == CampaignTime.Never && PlayerSiege.PlayerSiegeEvent.BesiegerCamp.IsPreparationComplete)
			{
				this._lastAmbushTime = CampaignTime.Now;
			}
		}

		private void OnMissionEnded(IMission mission)
		{
			PlayerEncounter playerEncounter = PlayerEncounter.Current;
			if (playerEncounter == null)
			{
				return;
			}
			playerEncounter.SetIsSallyOutAmbush(false);
		}

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

		private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("menu_siege_strategies", "menu_siege_strategies_ambush", "{=LEKzuGzi}Ambush", new GameMenuOption.OnConditionDelegate(this.menu_siege_strategies_ambush_condition), new GameMenuOption.OnConsequenceDelegate(this.menu_siege_strategies_ambush_on_consequence), false, -1, false, null);
		}

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

		private const int SiegeAmbushCooldownPeriodAsHours = 24;

		private CampaignTime _lastAmbushTime = CampaignTime.Never;
	}
}
