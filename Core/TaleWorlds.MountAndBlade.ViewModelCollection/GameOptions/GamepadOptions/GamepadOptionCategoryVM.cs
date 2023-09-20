using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GamepadOptions
{
	public class GamepadOptionCategoryVM : GroupedOptionCategoryVM
	{
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
			if (Input.ControllerType == Input.ControllerTypes.PlayStationDualSense)
			{
				this.SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType.Playstation5);
			}
			else if (Input.ControllerType == Input.ControllerTypes.PlayStationDualShock)
			{
				this.SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType.Playstation4);
			}
			else
			{
				this.SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType.Xbox);
			}
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

		private void SetCurrentGamepadType(GamepadOptionCategoryVM.GamepadType type)
		{
			this.CurrentGamepadType = (int)type;
		}

		private void OnGamepadActiveStateChanged()
		{
			base.IsEnabled = Input.IsGamepadActive;
			Debug.Print("GAMEPAD TAB ENABLED: " + base.IsEnabled.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

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

		private SelectorVM<SelectorItemVM> _categories;

		private int _currentGamepadType = -1;

		private MBBindingList<GamepadOptionKeyItemVM> _leftAnalogKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _rightAnalogKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _dpadKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _rightTriggerAndBumperKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _leftTriggerAndBumperKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _otherKeys;

		private MBBindingList<GamepadOptionKeyItemVM> _faceKeys;

		private MBBindingList<SelectorVM<SelectorItemVM>> _actions;

		private enum GamepadType
		{
			Xbox,
			Playstation4,
			Playstation5
		}
	}
}
