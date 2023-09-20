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
	[GameStateScreen(typeof(InventoryState))]
	public class GauntletInventoryScreen : ScreenBase, IInventoryStateHandler, IGameStateListener, IChangeableScreen
	{
		public InventoryState InventoryState { get; private set; }

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

		public GauntletInventoryScreen(InventoryState inventoryState)
		{
			this.InventoryState = inventoryState;
			this.InventoryState.Handler = this;
		}

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

		private string GetFiveStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anyshift").ToString();
			}
			return string.Empty;
		}

		private string GetEntireStackShortcutkeyText()
		{
			if (!Input.IsControllerConnected || Input.IsMouseActive)
			{
				return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", "anycontrol").ToString();
			}
			return null;
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._closed = true;
			MBInformationManager.HideInformations();
		}

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

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._gauntletMovie = null;
			this._inventoryCategory.Unload();
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		void IGameStateListener.OnActivate()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(2));
		}

		void IGameStateListener.OnDeactivate()
		{
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

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

		public void ExecuteLootingScript()
		{
			this._dataSource.ExecuteBuyAllItems();
		}

		public void ExecuteSellAllLoot()
		{
			this._dataSource.ExecuteSellAllItems();
		}

		private void HandleResetInput()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteResetTranstactions();
			}
		}

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

		public void ExecuteConfirm()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteCompleteTranstactions();
			}
		}

		public void ExecuteSwitchToPreviousTab()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.CharacterList.ExecuteSelectPreviousItem();
			}
		}

		public void ExecuteSwitchToNextTab()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.CharacterList.ExecuteSelectNextItem();
			}
		}

		public void ExecuteTakeAll()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteBuyAllItems();
			}
		}

		public void ExecuteGiveAll()
		{
			if (!this._dataSource.ItemPreview.IsSelected)
			{
				this._dataSource.ExecuteSellAllItems();
			}
		}

		public void ExecuteBuyConsumableItem()
		{
			this._dataSource.ExecuteBuyItemTest();
		}

		private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
		{
			if (!string.IsNullOrEmpty(item.ItemUsage))
			{
				return MBItem.GetItemUsageSetFlags(item.ItemUsage);
			}
			return 0;
		}

		private void CloseInventoryScreen()
		{
			InventoryManager.Instance.CloseInventoryPresentation(false);
		}

		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this.InventoryState.InventoryLogic.IsThereAnyChanges();
		}

		bool IChangeableScreen.CanChangesBeApplied()
		{
			return this.InventoryState.InventoryLogic.CanPlayerCompleteTransaction();
		}

		void IChangeableScreen.ApplyChanges()
		{
			this._dataSource.ItemPreview.Close();
			this.InventoryState.InventoryLogic.DoneLogic();
		}

		void IChangeableScreen.ResetChanges()
		{
			this.InventoryState.InventoryLogic.Reset(true);
		}

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

		private const string _panelOpenSound = "event:/ui/panels/panel_inventory_open";

		private IGauntletMovie _gauntletMovie;

		private SPInventoryVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private bool _closed;

		private bool _openedFromMission;

		private SpriteCategory _inventoryCategory;
	}
}
