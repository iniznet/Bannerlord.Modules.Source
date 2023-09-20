using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.MapSiege;
using SandBox.ViewModelCollection.Missions.NameMarker;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200003E RID: 62
	public abstract class TutorialItemBase
	{
		// Token: 0x06000187 RID: 391
		public abstract bool IsConditionsMetForCompletion();

		// Token: 0x06000188 RID: 392
		public abstract bool IsConditionsMetForActivation();

		// Token: 0x06000189 RID: 393
		public abstract TutorialContexts GetTutorialsRelevantContext();

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00006AE2 File Offset: 0x00004CE2
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00006AD9 File Offset: 0x00004CD9
		public TutorialItemVM.ItemPlacements Placement { get; protected set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00006AF3 File Offset: 0x00004CF3
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00006AEA File Offset: 0x00004CEA
		public bool MouseRequired { get; protected set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00006B04 File Offset: 0x00004D04
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00006AFB File Offset: 0x00004CFB
		public string HighlightedVisualElementID { get; protected set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00006B15 File Offset: 0x00004D15
		// (set) Token: 0x06000190 RID: 400 RVA: 0x00006B0C File Offset: 0x00004D0C
		public string Type { get; protected set; }

		// Token: 0x06000192 RID: 402 RVA: 0x00006B1D File Offset: 0x00004D1D
		protected virtual string GetCustomTutorialElementHighlightID()
		{
			return "";
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00006B24 File Offset: 0x00004D24
		public virtual void OnDeactivate()
		{
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00006B26 File Offset: 0x00004D26
		public virtual bool IsConditionsMetForVisibility()
		{
			return this.GetTutorialsRelevantContext() != 8 || !BannerlordConfig.HideBattleUI;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00006B3B File Offset: 0x00004D3B
		public virtual void OnInventoryTransferItem(InventoryTransferItemEvent obj)
		{
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00006B3D File Offset: 0x00004D3D
		public virtual void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00006B3F File Offset: 0x00004D3F
		public virtual void OnInventoryFilterChanged(InventoryFilterChangedEvent obj)
		{
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00006B41 File Offset: 0x00004D41
		public virtual void OnPerkSelectedByPlayer(PerkSelectedByPlayerEvent obj)
		{
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00006B43 File Offset: 0x00004D43
		public virtual void OnFocusAddedByPlayer(FocusAddedByPlayerEvent obj)
		{
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00006B45 File Offset: 0x00004D45
		public virtual void OnGameMenuOpened(MenuCallbackArgs obj)
		{
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00006B47 File Offset: 0x00004D47
		public virtual void OnMainMapCameraMove(MainMapCameraMoveEvent obj)
		{
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00006B49 File Offset: 0x00004D49
		public virtual void OnCharacterPortraitPopUpOpened(CharacterObject obj)
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00006B4B File Offset: 0x00004D4B
		public virtual void OnPlayerStartTalkFromMenuOverlay(Hero obj)
		{
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00006B4D File Offset: 0x00004D4D
		public virtual void OnGameMenuOptionSelected(GameMenuOption obj)
		{
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00006B4F File Offset: 0x00004D4F
		public virtual void OnPlayerStartRecruitment(CharacterObject obj)
		{
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00006B51 File Offset: 0x00004D51
		public virtual void OnNewCompanionAdded(Hero obj)
		{
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00006B53 File Offset: 0x00004D53
		public virtual void OnMissionNameMarkerToggled(MissionNameMarkerToggleEvent obj)
		{
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00006B55 File Offset: 0x00004D55
		public virtual void OnPlayerToggleTrackSettlementFromEncyclopedia(PlayerToggleTrackSettlementFromEncyclopediaEvent obj)
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00006B57 File Offset: 0x00004D57
		public virtual void OnInventoryEquipmentTypeChange(InventoryEquipmentTypeChangedEvent obj)
		{
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00006B59 File Offset: 0x00004D59
		public virtual void OnArmyCohesionByPlayerBoosted(ArmyCohesionBoostedByPlayerEvent obj)
		{
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00006B5B File Offset: 0x00004D5B
		public virtual void OnPartyAddedToArmyByPlayer(PartyAddedToArmyByPlayerEvent obj)
		{
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00006B5D File Offset: 0x00004D5D
		public virtual void OnPlayerStartEngineConstruction(PlayerStartEngineConstructionEvent obj)
		{
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00006B5F File Offset: 0x00004D5F
		public virtual void OnPlayerUpgradeTroop(CharacterObject arg1, CharacterObject arg2, int arg3)
		{
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00006B61 File Offset: 0x00004D61
		public virtual void OnPlayerMoveTroop(PlayerMoveTroopEvent obj)
		{
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00006B63 File Offset: 0x00004D63
		public virtual void OnPerkSelectionToggle(PerkSelectionToggleEvent obj)
		{
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00006B65 File Offset: 0x00004D65
		public virtual void OnPlayerInspectedPartySpeed(PlayerInspectedPartySpeedEvent obj)
		{
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00006B67 File Offset: 0x00004D67
		public virtual void OnPlayerMovementFlagChanged(MissionPlayerMovementFlagsChangeEvent obj)
		{
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00006B69 File Offset: 0x00004D69
		public virtual void OnPlayerToggledUpgradePopup(PlayerToggledUpgradePopupEvent obj)
		{
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00006B6B File Offset: 0x00004D6B
		public virtual void OnOrderOfBattleHeroAssignedToFormation(OrderOfBattleHeroAssignedToFormationEvent obj)
		{
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00006B6D File Offset: 0x00004D6D
		public virtual void OnOrderOfBattleFormationClassChanged(OrderOfBattleFormationClassChangedEvent obj)
		{
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00006B6F File Offset: 0x00004D6F
		public virtual void OnOrderOfBattleFormationWeightChanged(OrderOfBattleFormationWeightChangedEvent obj)
		{
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00006B71 File Offset: 0x00004D71
		public virtual void OnCraftingWeaponClassSelectionOpened(CraftingWeaponClassSelectionOpenedEvent obj)
		{
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00006B73 File Offset: 0x00004D73
		public virtual void OnCraftingOnWeaponResultPopupOpened(CraftingWeaponResultPopupToggledEvent obj)
		{
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00006B75 File Offset: 0x00004D75
		public virtual void OnCraftingOrderSelectionOpened(CraftingOrderSelectionOpenedEvent obj)
		{
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00006B77 File Offset: 0x00004D77
		public virtual void OnInventoryItemInspected(InventoryItemInspectedEvent obj)
		{
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00006B79 File Offset: 0x00004D79
		public virtual void OnCrimeValueInspectedInSettlementOverlay(SettlementMenuOverlayVM.CrimeValueInspectedInSettlementOverlayEvent obj)
		{
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00006B7B File Offset: 0x00004D7B
		public virtual void OnClanRoleAssignedThroughClanScreen(ClanRoleAssignedThroughClanScreenEvent obj)
		{
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00006B7D File Offset: 0x00004D7D
		public virtual void OnPlayerSelectedAKingdomDecisionOption(PlayerSelectedAKingdomDecisionOptionEvent obj)
		{
		}
	}
}
