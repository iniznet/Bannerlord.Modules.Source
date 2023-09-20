using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x0200002C RID: 44
	[OverrideView(typeof(MissionOptionsUIHandler))]
	public class MissionGauntletOptionsUIHandler : MissionView
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000B748 File Offset: 0x00009948
		// (set) Token: 0x0600021C RID: 540 RVA: 0x0000B750 File Offset: 0x00009950
		public bool IsEnabled { get; private set; }

		// Token: 0x0600021D RID: 541 RVA: 0x0000B759 File Offset: 0x00009959
		public MissionGauntletOptionsUIHandler()
		{
			this.ViewOrderPriority = 49;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000B76C File Offset: 0x0000996C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.Mission.GetMissionBehavior<MissionOptionsComponent>().OnOptionsAdded += new OnMissionAddOptionsDelegate(this.OnShowOptions);
			this._keybindingPopup = new KeybindingPopup(new Action<Key>(this.SetHotKey), base.MissionScreen);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000B7B8 File Offset: 0x000099B8
		public override void OnMissionScreenFinalize()
		{
			base.Mission.GetMissionBehavior<MissionOptionsComponent>().OnOptionsAdded -= new OnMissionAddOptionsDelegate(this.OnShowOptions);
			base.OnMissionScreenFinalize();
			OptionsVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this._movie = null;
			KeybindingPopup keybindingPopup = this._keybindingPopup;
			if (keybindingPopup != null)
			{
				keybindingPopup.OnToggle(false);
			}
			this._keybindingPopup = null;
			this._gauntletLayer = null;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000B828 File Offset: 0x00009A28
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._gauntletLayer != null && !this._keybindingPopup.IsActive)
			{
				if (this._dataSource.ExposurePopUp.Visible)
				{
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
					{
						this._dataSource.ExposurePopUp.ExecuteConfirm();
					}
					else if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
					{
						this._dataSource.ExposurePopUp.ExecuteCancel();
					}
				}
				else if (this._dataSource.BrightnessPopUp.Visible)
				{
					if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
					{
						this._dataSource.BrightnessPopUp.ExecuteConfirm();
					}
					else if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
					{
						this._dataSource.BrightnessPopUp.ExecuteCancel();
					}
				}
				else if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
				{
					this._dataSource.ExecuteCancel();
				}
				else if (TaleWorlds.InputSystem.Input.IsGamepadActive && this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
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
			KeybindingPopup keybindingPopup = this._keybindingPopup;
			if (keybindingPopup == null)
			{
				return;
			}
			keybindingPopup.Tick();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000B9D6 File Offset: 0x00009BD6
		public override bool OnEscape()
		{
			if (this._dataSource != null)
			{
				this._dataSource.ExecuteCloseOptions();
				return true;
			}
			return base.OnEscape();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000B9F3 File Offset: 0x00009BF3
		private void OnShowOptions()
		{
			this.IsEnabled = true;
			this.OnEscapeMenuToggled(true);
			this._initialClothSimValue = NativeOptions.GetConfig(49) == 0f;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000BA18 File Offset: 0x00009C18
		private void OnCloseOptions()
		{
			this.IsEnabled = false;
			this.OnEscapeMenuToggled(false);
			bool flag = NativeOptions.GetConfig(49) == 0f;
			if (this._initialClothSimValue != flag)
			{
				InformationManager.ShowInquiry(new InquiryData(Module.CurrentModule.GlobalTextManager.FindText("str_option_wont_take_effect_mission_title", null).ToString(), Module.CurrentModule.GlobalTextManager.FindText("str_option_wont_take_effect_mission_desc", null).ToString(), true, false, Module.CurrentModule.GlobalTextManager.FindText("str_ok", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), true, false);
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000BAC0 File Offset: 0x00009CC0
		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return this._gauntletLayer == null;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000BACC File Offset: 0x00009CCC
		private void OnEscapeMenuToggled(bool isOpened)
		{
			if (isOpened)
			{
				if (!GameNetwork.IsMultiplayer)
				{
					MBCommon.PauseGameEngine();
				}
			}
			else
			{
				MBCommon.UnPauseGameEngine();
			}
			if (isOpened)
			{
				OptionsVM.OptionsMode optionsMode = (GameNetwork.IsMultiplayer ? 2 : 1);
				this._dataSource = new OptionsVM(optionsMode, new Action(this.OnCloseOptions), new Action<KeyOptionVM>(this.OnKeybindRequest), null, null);
				this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
				this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
				this._dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
				this._dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
				int num = this.ViewOrderPriority + 1;
				this.ViewOrderPriority = num;
				this._gauntletLayer = new GauntletLayer(num, "GauntletLayer", false);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._optionsSpriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_options"];
				this._optionsSpriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
				this._fullScreensSpriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_fullscreens"];
				this._fullScreensSpriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
				this._movie = this._gauntletLayer.LoadMovie("Options", this._dataSource);
				base.MissionScreen.AddLayer(this._gauntletLayer);
				this._gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._gauntletLayer);
				Game game = Game.Current;
				if (game == null)
				{
					return;
				}
				game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(13));
				return;
			}
			else
			{
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				KeybindingPopup keybindingPopup = this._keybindingPopup;
				if (keybindingPopup != null)
				{
					keybindingPopup.OnToggle(false);
				}
				this._optionsSpriteCategory.Unload();
				this._fullScreensSpriteCategory.Unload();
				this._gauntletLayer = null;
				this._dataSource.OnFinalize();
				this._dataSource = null;
				this._gauntletLayer = null;
				Game game2 = Game.Current;
				if (game2 == null)
				{
					return;
				}
				game2.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(8));
				return;
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000BD49 File Offset: 0x00009F49
		private void OnKeybindRequest(KeyOptionVM requestedHotKeyToChange)
		{
			this._currentKey = requestedHotKeyToChange;
			this._keybindingPopup.OnToggle(true);
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000BD60 File Offset: 0x00009F60
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

		// Token: 0x0400010A RID: 266
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400010B RID: 267
		private OptionsVM _dataSource;

		// Token: 0x0400010C RID: 268
		private IGauntletMovie _movie;

		// Token: 0x0400010D RID: 269
		private KeybindingPopup _keybindingPopup;

		// Token: 0x0400010E RID: 270
		private KeyOptionVM _currentKey;

		// Token: 0x0400010F RID: 271
		private SpriteCategory _optionsSpriteCategory;

		// Token: 0x04000110 RID: 272
		private SpriteCategory _fullScreensSpriteCategory;

		// Token: 0x04000111 RID: 273
		private bool _initialClothSimValue;
	}
}
