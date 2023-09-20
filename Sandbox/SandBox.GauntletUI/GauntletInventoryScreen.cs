using System;
using System.Collections.Generic;
using SandBox.View;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x0200000A RID: 10
	[GameStateScreen(typeof(InventoryState))]
	public class GauntletInventoryScreen : ScreenBase, IInventoryStateHandler, IGameStateListener, IChangeableScreen
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00004E1C File Offset: 0x0000301C
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00004E24 File Offset: 0x00003024
		public InventoryState InventoryState { get; private set; }

		// Token: 0x06000061 RID: 97 RVA: 0x00004E30 File Offset: 0x00003030
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (!this._closed)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			this._dataSource.IsFiveStackModifierActive = this._gauntletLayer.Input.IsHotKeyDown("FiveStackModifier");
			this._dataSource.IsEntireStackModifierActive = this._gauntletLayer.Input.IsHotKeyDown("EntireStackModifier");
			if (!this._dataSource.IsSearchAvailable || !this._gauntletLayer.IsFocusedOnInput())
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchAlternative") && this._dataSource != null)
				{
					this._dataSource.CompareNextItem();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || this._gauntletLayer.Input.IsGameKeyDownAndReleased(38))
				{
					this.ExecuteCancel();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
				{
					this.ExecuteConfirm();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Reset"))
				{
					this.HandleResetInput();
					return;
				}
				if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					if (!this._dataSource.IsFocusedOnItemList || !Input.IsGamepadActive)
					{
						this.ExecuteSwitchToPreviousTab();
						return;
					}
					if (this._dataSource.CurrentFocusedItem != null && this._dataSource.CurrentFocusedItem.IsTransferable && this._dataSource.CurrentFocusedItem.InventorySide == null)
					{
						this._dataSource.CurrentFocusedItem.ExecuteBuySingle();
						return;
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					if (!this._dataSource.IsFocusedOnItemList || !Input.IsGamepadActive)
					{
						this.ExecuteSwitchToNextTab();
						return;
					}
					if (this._dataSource.CurrentFocusedItem != null && this._dataSource.CurrentFocusedItem.IsTransferable && this._dataSource.CurrentFocusedItem.InventorySide == 1)
					{
						this._dataSource.CurrentFocusedItem.ExecuteSellSingle();
						return;
					}
				}
				else
				{
					if (this._gauntletLayer.Input.IsHotKeyPressed("TakeAll"))
					{
						this.ExecuteTakeAll();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyPressed("GiveAll"))
					{
						this.ExecuteGiveAll();
					}
				}
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005074 File Offset: 0x00003274
		public GauntletInventoryScreen(InventoryState inventoryState)
		{
			this.InventoryState = inventoryState;
			this.InventoryState.Handler = this;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005090 File Offset: 0x00003290
		protected override void OnInitialize()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._inventoryCategory = spriteData.SpriteCategories["ui_inventory"];
			this._inventoryCategory.Load(resourceContext, uiresourceDepot);
			InventoryLogic inventoryLogic = this.InventoryState.InventoryLogic;
			Mission mission = Mission.Current;
			this._dataSource = new SPInventoryVM(inventoryLogic, mission != null && mission.DoesMissionRequireCivilianEquipment, new Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>(this.GetItemUsageSetFlag), this.GetFiveStackShortcutkeyText(), this.GetEntireStackShortcutkeyText());
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<string, TextObject>(Game.Current.GameTextManager.GetHotKeyGameTextFromKeyID));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetPreviousCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextCharacterInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			this._dataSource.SetBuyAllInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("TakeAll"));
			this._dataSource.SetSellAllInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("GiveAll"));
			this._gauntletLayer = new GauntletLayer(15, "GauntletLayer", true)
			{
				IsFocusLayer = true
			};
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			base.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("InventoryHotKeyCategory"));
			this._gauntletMovie = this._gauntletLayer.LoadMovie("Inventory", this._dataSource);
			this._openedFromMission = this.InventoryState.Predecessor is MissionState;
			InformationManager.ClearAllMessages();
			SoundEvent.PlaySound2D("event:/ui/panels/panel_inventory_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000052F7 File Offset: 0x000034F7
		private string GetFiveStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anyshift").ToString();
			}
			return string.Empty;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005331 File Offset: 0x00003531
		private string GetEntireStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anycontrol").ToString();
			}
			return null;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005367 File Offset: 0x00003567
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._closed = true;
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000537B File Offset: 0x0000357B
		protected override void OnActivate()
		{
			base.OnActivate();
			SPInventoryVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.RefreshCallbacks();
			}
			if (this._gauntletLayer != null)
			{
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000053A7 File Offset: 0x000035A7
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._gauntletMovie = null;
			this._inventoryCategory.Unload();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000053DA File Offset: 0x000035DA
		void IGameStateListener.OnActivate()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(2));
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000053F1 File Offset: 0x000035F1
		void IGameStateListener.OnDeactivate()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005408 File Offset: 0x00003608
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000540A File Offset: 0x0000360A
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000540C File Offset: 0x0000360C
		void IInventoryStateHandler.FilterInventoryAtOpening(InventoryManager.InventoryCategoryType inventoryCategoryType)
		{
			switch (inventoryCategoryType)
			{
			case 1:
				this._dataSource.ExecuteFilterArmors();
				return;
			case 2:
				this._dataSource.ExecuteFilterWeapons();
				return;
			case 3:
				break;
			case 4:
				this._dataSource.ExecuteFilterMounts();
				return;
			case 5:
				this._dataSource.ExecuteFilterMisc();
				break;
			default:
				return;
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005465 File Offset: 0x00003665
		public void ExecuteLootingScript()
		{
			this._dataSource.ExecuteBuyAllItems();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005472 File Offset: 0x00003672
		public void ExecuteSellAllLoot()
		{
			this._dataSource.ExecuteSellAllItems();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000547F File Offset: 0x0000367F
		private void HandleResetInput()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteResetTranstactions();
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000054A0 File Offset: 0x000036A0
		public void ExecuteCancel()
		{
			if (this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ClosePreview();
				return;
			}
			if (this._dataSource.IsExtendedEquipmentControlsEnabled)
			{
				this._dataSource.IsExtendedEquipmentControlsEnabled = false;
				return;
			}
			this._dataSource.ExecuteResetAndCompleteTranstactions();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000054F0 File Offset: 0x000036F0
		public void ExecuteConfirm()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteCompleteTranstactions();
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000550F File Offset: 0x0000370F
		public void ExecuteSwitchToPreviousTab()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.CharacterList.ExecuteSelectPreviousItem();
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005533 File Offset: 0x00003733
		public void ExecuteSwitchToNextTab()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.CharacterList.ExecuteSelectNextItem();
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005557 File Offset: 0x00003757
		public void ExecuteTakeAll()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteBuyAllItems();
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005576 File Offset: 0x00003776
		public void ExecuteGiveAll()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteSellAllItems();
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005595 File Offset: 0x00003795
		public void ExecuteBuyConsumableItem()
		{
			this._dataSource.ExecuteBuyItemTest();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000055A2 File Offset: 0x000037A2
		private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
		{
			if (!string.IsNullOrEmpty(item.ItemUsage))
			{
				return MBItem.GetItemUsageSetFlags(item.ItemUsage);
			}
			return 0;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000055BE File Offset: 0x000037BE
		private void CloseInventoryScreen()
		{
			InventoryManager.Instance.CloseInventoryPresentation(false);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000055CB File Offset: 0x000037CB
		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this.InventoryState.InventoryLogic.IsThereAnyChanges();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000055DD File Offset: 0x000037DD
		bool IChangeableScreen.CanChangesBeApplied()
		{
			return this.InventoryState.InventoryLogic.CanPlayerCompleteTransaction();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000055EF File Offset: 0x000037EF
		void IChangeableScreen.ApplyChanges()
		{
			this._dataSource.ItemPreview.Close();
			this.InventoryState.InventoryLogic.DoneLogic();
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005612 File Offset: 0x00003812
		void IChangeableScreen.ResetChanges()
		{
			this.InventoryState.InventoryLogic.Reset(true);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005628 File Offset: 0x00003828
		[CommandLineFunctionality.CommandLineArgumentFunction("set_inventory_search_enabled", "ui")]
		public static string SetInventorySearchEnabled(List<string> args)
		{
			string text = "Format is \"ui.set_inventory_search_enabled [1/0]\".";
			GauntletInventoryScreen gauntletInventoryScreen;
			if ((gauntletInventoryScreen = ScreenManager.TopScreen as GauntletInventoryScreen) == null)
			{
				return "Inventory screen is not open!";
			}
			if (args.Count != 1)
			{
				return text;
			}
			int num;
			if (int.TryParse(args[0], out num) && (num == 1 || num == 0))
			{
				gauntletInventoryScreen._dataSource.IsSearchAvailable = num == 1;
				return "Success.";
			}
			return text;
		}

		// Token: 0x04000036 RID: 54
		private const string _panelOpenSound = "event:/ui/panels/panel_inventory_open";

		// Token: 0x04000037 RID: 55
		private IGauntletMovie _gauntletMovie;

		// Token: 0x04000038 RID: 56
		private SPInventoryVM _dataSource;

		// Token: 0x04000039 RID: 57
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400003A RID: 58
		private bool _closed;

		// Token: 0x0400003B RID: 59
		private bool _openedFromMission;

		// Token: 0x0400003C RID: 60
		private SpriteCategory _inventoryCategory;
	}
}
