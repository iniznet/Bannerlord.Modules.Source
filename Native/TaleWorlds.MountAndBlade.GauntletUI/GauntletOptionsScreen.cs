using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x0200000C RID: 12
	[OverrideView(typeof(OptionsScreen))]
	public class GauntletOptionsScreen : ScreenBase
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00003671 File Offset: 0x00001871
		public GauntletOptionsScreen(bool isFromMainMenu)
		{
			this._isFromMainMenu = isFromMainMenu;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003680 File Offset: 0x00001880
		protected override void OnInitialize()
		{
			base.OnInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._optionsSpriteCategory = spriteData.SpriteCategories["ui_options"];
			this._optionsSpriteCategory.Load(resourceContext, uiresourceDepot);
			OptionsVM.OptionsMode optionsMode = (this._isFromMainMenu ? 0 : 1);
			this._dataSource = new OptionsVM(true, optionsMode, new Action<KeyOptionVM>(this.OnKeybindRequest), null, null);
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			this._dataSource.ExposurePopUp.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.ExposurePopUp.SetConfirmInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.BrightnessPopUp.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.BrightnessPopUp.SetConfirmInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._gauntletLayer = new GauntletLayer(4000, "GauntletLayer", false);
			this._gauntletMovie = this._gauntletLayer.LoadMovie("Options", this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			this._keybindingPopup = new KeybindingPopup(new Action<Key>(this.SetHotKey), this);
			base.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			if (BannerlordConfig.ForceVSyncInMenus)
			{
				Utilities.SetForceVsync(true);
			}
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(13));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000038D7 File Offset: 0x00001AD7
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			this._optionsSpriteCategory.Unload();
			Utilities.SetForceVsync(false);
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003915 File Offset: 0x00001B15
		protected override void OnDeactivate()
		{
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000391C File Offset: 0x00001B1C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer != null && !this._keybindingPopup.IsActive)
			{
				if (this._dataSource.ExposurePopUp.Visible)
				{
					if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
					{
						this._dataSource.ExposurePopUp.ExecuteConfirm();
					}
					else if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
					{
						this._dataSource.ExposurePopUp.ExecuteCancel();
					}
				}
				else if (this._dataSource.BrightnessPopUp.Visible)
				{
					if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
					{
						this._dataSource.BrightnessPopUp.ExecuteConfirm();
					}
					else if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
					{
						this._dataSource.BrightnessPopUp.ExecuteCancel();
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					this._dataSource.ExecuteCancel();
				}
				else if (Input.IsGamepadActive && this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					this._dataSource.ExecuteDone();
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					this._dataSource.SelectPreviousCategory();
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					this._dataSource.SelectNextCategory();
				}
			}
			this._keybindingPopup.Tick();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003AC5 File Offset: 0x00001CC5
		private void OnKeybindRequest(KeyOptionVM requestedHotKeyToChange)
		{
			this._currentKey = requestedHotKeyToChange;
			this._keybindingPopup.OnToggle(true);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003ADC File Offset: 0x00001CDC
		private void SetHotKey(Key key)
		{
			GameKeyOptionVM gameKey;
			if ((gameKey = this._currentKey as GameKeyOptionVM) == null)
			{
				AuxiliaryKeyOptionVM auxiliaryKey;
				if ((auxiliaryKey = this._currentKey as AuxiliaryKeyOptionVM) != null)
				{
					AuxiliaryKeyGroupVM auxiliaryKeyGroupVM = this._dataSource.GameKeyOptionGroups.AuxiliaryKeyGroups.First((AuxiliaryKeyGroupVM g) => g.HotKeys.Contains(auxiliaryKey));
					if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
					{
						this._keybindingPopup.OnToggle(false);
						return;
					}
					if (auxiliaryKeyGroupVM.HotKeys.Any((AuxiliaryKeyOptionVM k) => k.CurrentKey.InputKey == key.InputKey && k.CurrentHotKey.HasSameModifiers(auxiliaryKey.CurrentHotKey)))
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=n4UUrd1p}Already in use", null), 0, null, "");
						return;
					}
					AuxiliaryKeyOptionVM auxiliaryKey2 = auxiliaryKey;
					if (auxiliaryKey2 != null)
					{
						auxiliaryKey2.Set(key.InputKey);
					}
					auxiliaryKey = null;
					this._keybindingPopup.OnToggle(false);
				}
				return;
			}
			GameKeyGroupVM gameKeyGroupVM = this._dataSource.GameKeyOptionGroups.GameKeyGroups.First((GameKeyGroupVM g) => g.GameKeys.Contains(gameKey));
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._keybindingPopup.OnToggle(false);
				return;
			}
			if (gameKeyGroupVM.GameKeys.Any((GameKeyOptionVM k) => k.CurrentKey.InputKey == key.InputKey))
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=n4UUrd1p}Already in use", null), 0, null, "");
				return;
			}
			GameKeyOptionVM gameKey2 = gameKey;
			if (gameKey2 != null)
			{
				gameKey2.Set(key.InputKey);
			}
			gameKey = null;
			this._keybindingPopup.OnToggle(false);
		}

		// Token: 0x0400002E RID: 46
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400002F RID: 47
		private OptionsVM _dataSource;

		// Token: 0x04000030 RID: 48
		private IGauntletMovie _gauntletMovie;

		// Token: 0x04000031 RID: 49
		private KeybindingPopup _keybindingPopup;

		// Token: 0x04000032 RID: 50
		private KeyOptionVM _currentKey;

		// Token: 0x04000033 RID: 51
		private SpriteCategory _optionsSpriteCategory;

		// Token: 0x04000034 RID: 52
		private bool _isFromMainMenu;
	}
}
