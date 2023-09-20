using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GamepadOptions
{
	// Token: 0x020000FE RID: 254
	public class GamepadOptionCategoryVM : GroupedOptionCategoryVM
	{
		// Token: 0x06001685 RID: 5765 RVA: 0x00048CAC File Offset: 0x00046EAC
		public GamepadOptionCategoryVM(OptionsVM options, TextObject name, OptionCategory category, bool isEnabled, bool isResetSupported = false)
			: base(options, name, category, isEnabled, isResetSupported)
		{
			this.OtherKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.DpadKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.LeftAnalogKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.RightAnalogKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.FaceKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.LeftTriggerAndBumperKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.RightTriggerAndBumperKeys = new MBBindingList<GamepadOptionKeyItemVM>();
			this.SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType.Xbox);
			this.Actions = new MBBindingList<SelectorVM<SelectorItemVM>>();
			this._categories = new SelectorVM<SelectorItemVM>(0, null);
			this._categories.AddItem(new SelectorItemVM(new TextObject("{=gamepadActionKeybind}Action", null)));
			this._categories.AddItem(new SelectorItemVM(new TextObject("{=gamepadMapKeybind}Map", null)));
			this._categories.SetOnChangeAction(new Action<SelectorVM<SelectorItemVM>>(this.OnCategoryChange));
			this._categories.SelectedIndex = 0;
			this.Actions.Add(this._categories);
			this.RefreshValues();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			base.IsEnabled = Input.IsGamepadActive;
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x00048DD4 File Offset: 0x00046FD4
		private void OnCategoryChange(SelectorVM<SelectorItemVM> obj)
		{
			if (obj.SelectedIndex >= 0)
			{
				this.OtherKeys.Clear();
				this.DpadKeys.Clear();
				this.LeftAnalogKeys.Clear();
				this.RightAnalogKeys.Clear();
				this.FaceKeys.Clear();
				this.LeftTriggerAndBumperKeys.Clear();
				this.RightTriggerAndBumperKeys.Clear();
				IEnumerable<GamepadOptionKeyItemVM> enumerable = null;
				if (obj.SelectedIndex == 0)
				{
					enumerable = GamepadOptionCategoryVM.GetActionKeys();
				}
				else if (obj.SelectedIndex == 1)
				{
					enumerable = GamepadOptionCategoryVM.GetMapKeys();
				}
				foreach (GamepadOptionKeyItemVM gamepadOptionKeyItemVM in enumerable)
				{
					InputKey inputKey = gamepadOptionKeyItemVM.Key ?? InputKey.Invalid;
					if (Key.IsLeftAnalogInput(inputKey))
					{
						this.LeftAnalogKeys.Add(gamepadOptionKeyItemVM);
					}
					else if (Key.IsRightAnalogInput(inputKey))
					{
						this.RightAnalogKeys.Add(gamepadOptionKeyItemVM);
					}
					else if (Key.IsDpadInput(inputKey))
					{
						this.DpadKeys.Add(gamepadOptionKeyItemVM);
					}
					else if (Key.IsFaceKeyInput(inputKey))
					{
						this.FaceKeys.Add(gamepadOptionKeyItemVM);
					}
					else
					{
						this.OtherKeys.Add(gamepadOptionKeyItemVM);
					}
					if (Key.IsLeftBumperOrTriggerInput(inputKey))
					{
						this.LeftTriggerAndBumperKeys.Add(gamepadOptionKeyItemVM);
					}
					else if (Key.IsRightBumperOrTriggerInput(inputKey))
					{
						this.RightTriggerAndBumperKeys.Add(gamepadOptionKeyItemVM);
					}
				}
			}
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x00048F40 File Offset: 0x00047140
		public override void RefreshValues()
		{
			base.RefreshValues();
			MBBindingList<GamepadOptionKeyItemVM> otherKeys = this.OtherKeys;
			if (otherKeys != null)
			{
				otherKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> leftAnalogKeys = this.LeftAnalogKeys;
			if (leftAnalogKeys != null)
			{
				leftAnalogKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> rightAnalogKeys = this.RightAnalogKeys;
			if (rightAnalogKeys != null)
			{
				rightAnalogKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> faceKeys = this.FaceKeys;
			if (faceKeys != null)
			{
				faceKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> dpadKeys = this.DpadKeys;
			if (dpadKeys != null)
			{
				dpadKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> leftTriggerAndBumperKeys = this.LeftTriggerAndBumperKeys;
			if (leftTriggerAndBumperKeys != null)
			{
				leftTriggerAndBumperKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<GamepadOptionKeyItemVM> rightTriggerAndBumperKeys = this.RightTriggerAndBumperKeys;
			if (rightTriggerAndBumperKeys != null)
			{
				rightTriggerAndBumperKeys.ApplyActionOnAllItems(delegate(GamepadOptionKeyItemVM x)
				{
					x.RefreshValues();
				});
			}
			MBBindingList<SelectorVM<SelectorItemVM>> actions = this.Actions;
			if (actions == null)
			{
				return;
			}
			actions.ApplyActionOnAllItems(delegate(SelectorVM<SelectorItemVM> x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x000490D2 File Offset: 0x000472D2
		private void SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType type)
		{
			this.CurrentGamepadType = (int)type;
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x000490DC File Offset: 0x000472DC
		private void OnGamepadActiveStateChanged()
		{
			base.IsEnabled = Input.IsGamepadActive;
			Debug.Print("GAMEPAD TAB ENABLED: " + base.IsEnabled.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x0004911D File Offset: 0x0004731D
		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00049145 File Offset: 0x00047345
		private static IEnumerable<GamepadOptionKeyItemVM> GetActionKeys()
		{
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLStick, new TextObject("{=i28Kjuay}Move Character", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerRStick, new TextObject("{=1hlaGzGI}Look", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLOption, new TextObject("{=9pgOGq7X}Log", null));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(31));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(33));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(25));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(15));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(13));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetHotKey("HoldShow"));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(16));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(14));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(9));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(10));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(26));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(27));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("Generic").GetGameKey(5));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("CombatHotKeyCategory").GetGameKey(34));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("ToggleEscapeMenu"));
			yield break;
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x0004914E File Offset: 0x0004734E
		private static IEnumerable<GamepadOptionKeyItemVM> GetMapKeys()
		{
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLStick, new TextObject("{=hdGay8xc}Map Cursor Move", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerRStick, new TextObject("{=atUHbDeM}Map Camera Rotate", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLUp, new TextObject("{=u78WUP9W}Fast Cursor Move Up", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLRight, new TextObject("{=bLPSaLNv}Fast Cursor Move Right", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLLeft, new TextObject("{=82LuSDnd}Fast Cursor Move Left", null));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLDown, new TextObject("{=nEpZvaEl}Fast Cursor Move Down", null));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetHotKey("MapChangeCursorMode"));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory").GetGameKey(39));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(62));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(55));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(64));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(56));
			yield return new GamepadOptionKeyItemVM(InputKey.ControllerLBumper, new TextObject("{=mueocuFG}Show Indicators", null));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(63));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetGameKey(65));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("ToggleEscapeMenu"));
			yield return new GamepadOptionKeyItemVM(HotKeyManager.GetCategory("MapHotKeyCategory").GetHotKey("MapClick"));
			yield break;
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x0600168D RID: 5773 RVA: 0x00049157 File Offset: 0x00047357
		// (set) Token: 0x0600168E RID: 5774 RVA: 0x0004915F File Offset: 0x0004735F
		[DataSourceProperty]
		public int CurrentGamepadType
		{
			get
			{
				return this._currentGamepadType;
			}
			set
			{
				if (value != this._currentGamepadType)
				{
					this._currentGamepadType = value;
					base.OnPropertyChangedWithValue(value, "CurrentGamepadType");
				}
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x0600168F RID: 5775 RVA: 0x0004917D File Offset: 0x0004737D
		// (set) Token: 0x06001690 RID: 5776 RVA: 0x00049185 File Offset: 0x00047385
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> OtherKeys
		{
			get
			{
				return this._otherKeys;
			}
			set
			{
				if (value != this._otherKeys)
				{
					this._otherKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "OtherKeys");
				}
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06001691 RID: 5777 RVA: 0x000491A3 File Offset: 0x000473A3
		// (set) Token: 0x06001692 RID: 5778 RVA: 0x000491AB File Offset: 0x000473AB
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> DpadKeys
		{
			get
			{
				return this._dpadKeys;
			}
			set
			{
				if (value != this._dpadKeys)
				{
					this._dpadKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "DpadKeys");
				}
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x000491C9 File Offset: 0x000473C9
		// (set) Token: 0x06001694 RID: 5780 RVA: 0x000491D1 File Offset: 0x000473D1
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> LeftTriggerAndBumperKeys
		{
			get
			{
				return this._leftTriggerAndBumperKeys;
			}
			set
			{
				if (value != this._leftTriggerAndBumperKeys)
				{
					this._leftTriggerAndBumperKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "LeftTriggerAndBumperKeys");
				}
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06001695 RID: 5781 RVA: 0x000491EF File Offset: 0x000473EF
		// (set) Token: 0x06001696 RID: 5782 RVA: 0x000491F7 File Offset: 0x000473F7
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> RightTriggerAndBumperKeys
		{
			get
			{
				return this._rightTriggerAndBumperKeys;
			}
			set
			{
				if (value != this._rightTriggerAndBumperKeys)
				{
					this._rightTriggerAndBumperKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "RightTriggerAndBumperKeys");
				}
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06001697 RID: 5783 RVA: 0x00049215 File Offset: 0x00047415
		// (set) Token: 0x06001698 RID: 5784 RVA: 0x0004921D File Offset: 0x0004741D
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> RightAnalogKeys
		{
			get
			{
				return this._rightAnalogKeys;
			}
			set
			{
				if (value != this._rightAnalogKeys)
				{
					this._rightAnalogKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "RightAnalogKeys");
				}
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06001699 RID: 5785 RVA: 0x0004923B File Offset: 0x0004743B
		// (set) Token: 0x0600169A RID: 5786 RVA: 0x00049243 File Offset: 0x00047443
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> LeftAnalogKeys
		{
			get
			{
				return this._leftAnalogKeys;
			}
			set
			{
				if (value != this._leftAnalogKeys)
				{
					this._leftAnalogKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "LeftAnalogKeys");
				}
			}
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x0600169B RID: 5787 RVA: 0x00049261 File Offset: 0x00047461
		// (set) Token: 0x0600169C RID: 5788 RVA: 0x00049269 File Offset: 0x00047469
		[DataSourceProperty]
		public MBBindingList<GamepadOptionKeyItemVM> FaceKeys
		{
			get
			{
				return this._faceKeys;
			}
			set
			{
				if (value != this._faceKeys)
				{
					this._faceKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GamepadOptionKeyItemVM>>(value, "FaceKeys");
				}
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x0600169D RID: 5789 RVA: 0x00049287 File Offset: 0x00047487
		// (set) Token: 0x0600169E RID: 5790 RVA: 0x0004928F File Offset: 0x0004748F
		[DataSourceProperty]
		public MBBindingList<SelectorVM<SelectorItemVM>> Actions
		{
			get
			{
				return this._actions;
			}
			set
			{
				if (value != this._actions)
				{
					this._actions = value;
					base.OnPropertyChangedWithValue<MBBindingList<SelectorVM<SelectorItemVM>>>(value, "Actions");
				}
			}
		}

		// Token: 0x04000AB3 RID: 2739
		private SelectorVM<SelectorItemVM> _categories;

		// Token: 0x04000AB4 RID: 2740
		private int _currentGamepadType = -1;

		// Token: 0x04000AB5 RID: 2741
		private MBBindingList<GamepadOptionKeyItemVM> _leftAnalogKeys;

		// Token: 0x04000AB6 RID: 2742
		private MBBindingList<GamepadOptionKeyItemVM> _rightAnalogKeys;

		// Token: 0x04000AB7 RID: 2743
		private MBBindingList<GamepadOptionKeyItemVM> _dpadKeys;

		// Token: 0x04000AB8 RID: 2744
		private MBBindingList<GamepadOptionKeyItemVM> _rightTriggerAndBumperKeys;

		// Token: 0x04000AB9 RID: 2745
		private MBBindingList<GamepadOptionKeyItemVM> _leftTriggerAndBumperKeys;

		// Token: 0x04000ABA RID: 2746
		private MBBindingList<GamepadOptionKeyItemVM> _otherKeys;

		// Token: 0x04000ABB RID: 2747
		private MBBindingList<GamepadOptionKeyItemVM> _faceKeys;

		// Token: 0x04000ABC RID: 2748
		private MBBindingList<SelectorVM<SelectorItemVM>> _actions;

		// Token: 0x0200024A RID: 586
		private enum GamepadType
		{
			// Token: 0x04000F1A RID: 3866
			Xbox,
			// Token: 0x04000F1B RID: 3867
			Playstation4,
			// Token: 0x04000F1C RID: 3868
			Playstation5
		}
	}
}
