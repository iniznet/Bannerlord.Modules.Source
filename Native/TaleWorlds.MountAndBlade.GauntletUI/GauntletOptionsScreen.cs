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
	[OverrideView(typeof(OptionsScreen))]
	public class GauntletOptionsScreen : ScreenBase
	{
		public GauntletOptionsScreen(bool isFromMainMenu)
		{
			this._isFromMainMenu = isFromMainMenu;
		}

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
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
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

		protected override void OnDeactivate()
		{
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._gauntletLayer != null && !this._keybindingPopup.IsActive)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					if (this._dataSource.ExposurePopUp.Visible)
					{
						this._dataSource.ExposurePopUp.ExecuteCancel();
					}
					else if (this._dataSource.BrightnessPopUp.Visible)
					{
						this._dataSource.BrightnessPopUp.ExecuteCancel();
					}
					else
					{
						this._dataSource.ExecuteCancel();
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					if (this._dataSource.ExposurePopUp.Visible)
					{
						this._dataSource.ExposurePopUp.ExecuteConfirm();
					}
					else if (this._dataSource.BrightnessPopUp.Visible)
					{
						this._dataSource.BrightnessPopUp.ExecuteConfirm();
					}
					else
					{
						this._dataSource.ExecuteDone();
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					UISoundsHelper.PlayUISound("event:/ui/tab");
					this._dataSource.SelectPreviousCategory();
				}
				else if (this._gauntletLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					UISoundsHelper.PlayUISound("event:/ui/tab");
					this._dataSource.SelectNextCategory();
				}
			}
			this._keybindingPopup.Tick();
		}

		private void OnKeybindRequest(KeyOptionVM requestedHotKeyToChange)
		{
			this._currentKey = requestedHotKeyToChange;
			this._keybindingPopup.OnToggle(true);
		}

		private void SetHotKey(Key key)
		{
			if (key.IsControllerInput)
			{
				Debug.FailedAssert("Trying to use SetHotKey with a controller input", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletOptionsScreen.cs", "SetHotKey", 161);
				MBInformationManager.AddQuickInformation(new TextObject("{=*}Invalid key", null), 0, null, "");
				this._keybindingPopup.OnToggle(false);
				return;
			}
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

		private GauntletLayer _gauntletLayer;

		private OptionsVM _dataSource;

		private IGauntletMovie _gauntletMovie;

		private KeybindingPopup _keybindingPopup;

		private KeyOptionVM _currentKey;

		private SpriteCategory _optionsSpriteCategory;

		private bool _isFromMainMenu;
	}
}
