using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Input
{
	// Token: 0x02000034 RID: 52
	public class InputKeyItemVM : ViewModel
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x0001211B File Offset: 0x0001031B
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x00012123 File Offset: 0x00010323
		public GameKey GameKey { get; private set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0001212C File Offset: 0x0001032C
		// (set) Token: 0x060003EE RID: 1006 RVA: 0x00012134 File Offset: 0x00010334
		public HotKey HotKey { get; private set; }

		// Token: 0x060003EF RID: 1007 RVA: 0x0001213D File Offset: 0x0001033D
		private InputKeyItemVM()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.RegisterEvent<GamepadActiveStateChangedEvent>(new Action<GamepadActiveStateChangedEvent>(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0001216A File Offset: 0x0001036A
		public override void OnFinalize()
		{
			base.OnFinalize();
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.UnregisterEvent<GamepadActiveStateChangedEvent>(new Action<GamepadActiveStateChangedEvent>(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00012197 File Offset: 0x00010397
		private void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj)
		{
			this.ForceRefresh();
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0001219F File Offset: 0x0001039F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ForceRefresh();
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000121AD File Offset: 0x000103AD
		public void SetForcedVisibility(bool? isVisible)
		{
			this._forcedVisibility = isVisible;
			this.UpdateVisibility();
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000121BC File Offset: 0x000103BC
		private void ForceRefresh()
		{
			this.UpdateVisibility();
			if (this.GameKey != null)
			{
				string text;
				if (!Input.IsGamepadActive)
				{
					Key keyboardKey = this.GameKey.KeyboardKey;
					text = ((keyboardKey != null) ? keyboardKey.InputKey.ToString() : null);
				}
				else
				{
					Key controllerKey = this.GameKey.ControllerKey;
					text = ((controllerKey != null) ? controllerKey.InputKey.ToString() : null);
				}
				this.KeyID = text;
				TextObject forcedName = this._forcedName;
				this.KeyName = ((forcedName != null) ? forcedName.ToString() : null) ?? Game.Current.GameTextManager.FindText("str_key_name", this.GameKey.GroupId + "_" + this.GameKey.StringId).ToString();
				return;
			}
			if (this.HotKey != null)
			{
				string text2;
				if (!Input.IsGamepadActive)
				{
					Key key = this.HotKey.Keys.Find((Key k) => !k.IsControllerInput);
					text2 = ((key != null) ? key.InputKey.ToString() : null);
				}
				else
				{
					Key key2 = this.HotKey.Keys.Find((Key k) => k.IsControllerInput);
					text2 = ((key2 != null) ? key2.InputKey.ToString() : null);
				}
				this.KeyID = text2;
				TextObject forcedName2 = this._forcedName;
				this.KeyName = ((forcedName2 != null) ? forcedName2.ToString() : null) ?? Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.HotKey.GroupId + "_" + this.HotKey.Id).ToString();
				return;
			}
			if (this._forcedID != null)
			{
				this.KeyID = this._forcedID;
				TextObject forcedName3 = this._forcedName;
				this.KeyName = ((forcedName3 != null) ? forcedName3.ToString() : null) ?? string.Empty;
				return;
			}
			this.KeyID = string.Empty;
			this.KeyName = string.Empty;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000123D8 File Offset: 0x000105D8
		private void UpdateVisibility()
		{
			this.IsVisible = this._forcedVisibility ?? (!this._isVisibleToConsoleOnly || Input.IsGamepadActive);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00012414 File Offset: 0x00010614
		public static InputKeyItemVM CreateFromGameKey(GameKey gameKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0001242F File Offset: 0x0001062F
		public static InputKeyItemVM CreateFromHotKey(HotKey hotKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001244A File Offset: 0x0001064A
		public static InputKeyItemVM CreateFromHotKeyWithForcedName(HotKey hotKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001246C File Offset: 0x0001066C
		public static InputKeyItemVM CreateFromGameKeyWithForcedName(GameKey gameKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0001248E File Offset: 0x0001068E
		public static InputKeyItemVM CreateFromForcedID(string forcedID, TextObject forcedName)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM._forcedID = forcedID;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x000124A9 File Offset: 0x000106A9
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x000124B1 File Offset: 0x000106B1
		[DataSourceProperty]
		public string KeyID
		{
			get
			{
				return this._keyID;
			}
			set
			{
				if (value != this._keyID)
				{
					this._keyID = value;
					base.OnPropertyChangedWithValue<string>(value, "KeyID");
				}
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x000124D4 File Offset: 0x000106D4
		// (set) Token: 0x060003FE RID: 1022 RVA: 0x000124DC File Offset: 0x000106DC
		[DataSourceProperty]
		public string KeyName
		{
			get
			{
				return this._keyName;
			}
			set
			{
				if (value != this._keyName)
				{
					this._keyName = value;
					base.OnPropertyChangedWithValue<string>(value, "KeyName");
				}
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x000124FF File Offset: 0x000106FF
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x00012507 File Offset: 0x00010707
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x0400020C RID: 524
		private bool _isVisibleToConsoleOnly;

		// Token: 0x0400020D RID: 525
		private TextObject _forcedName;

		// Token: 0x0400020E RID: 526
		private string _forcedID;

		// Token: 0x0400020F RID: 527
		private bool? _forcedVisibility;

		// Token: 0x04000210 RID: 528
		private string _keyID;

		// Token: 0x04000211 RID: 529
		private string _keyName;

		// Token: 0x04000212 RID: 530
		private bool _isVisible;
	}
}
