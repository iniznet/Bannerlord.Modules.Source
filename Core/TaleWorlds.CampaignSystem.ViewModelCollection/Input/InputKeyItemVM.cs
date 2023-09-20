using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Input
{
	// Token: 0x02000084 RID: 132
	public class InputKeyItemVM : ViewModel
	{
		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000D13 RID: 3347 RVA: 0x0003525C File Offset: 0x0003345C
		// (set) Token: 0x06000D14 RID: 3348 RVA: 0x00035264 File Offset: 0x00033464
		public GameKey GameKey { get; private set; }

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000D15 RID: 3349 RVA: 0x0003526D File Offset: 0x0003346D
		// (set) Token: 0x06000D16 RID: 3350 RVA: 0x00035275 File Offset: 0x00033475
		public HotKey HotKey { get; private set; }

		// Token: 0x06000D17 RID: 3351 RVA: 0x0003527E File Offset: 0x0003347E
		private InputKeyItemVM()
		{
			EventManager eventManager = Game.Current.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.RegisterEvent<GamepadActiveStateChangedEvent>(new Action<GamepadActiveStateChangedEvent>(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x000352A6 File Offset: 0x000334A6
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

		// Token: 0x06000D19 RID: 3353 RVA: 0x000352D3 File Offset: 0x000334D3
		private void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj)
		{
			this.ForceRefresh();
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000352DB File Offset: 0x000334DB
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ForceRefresh();
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x000352E9 File Offset: 0x000334E9
		public void SetForcedVisibility(bool? isVisible)
		{
			this._forcedVisibility = isVisible;
			this.UpdateVisibility();
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x000352F8 File Offset: 0x000334F8
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
				this.KeyName = ((forcedName2 != null) ? forcedName2.ToString() : null) ?? Game.Current.GameTextManager.FindText("str_key_name", this.HotKey.GroupId + "_" + this.HotKey.Id).ToString();
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

		// Token: 0x06000D1D RID: 3357 RVA: 0x00035514 File Offset: 0x00033714
		private void UpdateVisibility()
		{
			this.IsVisible = this._forcedVisibility ?? (!this._isVisibleToConsoleOnly || Input.IsGamepadActive);
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x00035550 File Offset: 0x00033750
		public static InputKeyItemVM CreateFromGameKey(GameKey gameKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0003556B File Offset: 0x0003376B
		public static InputKeyItemVM CreateFromHotKey(HotKey hotKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x00035586 File Offset: 0x00033786
		public static InputKeyItemVM CreateFromHotKeyWithForcedName(HotKey hotKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000355A8 File Offset: 0x000337A8
		public static InputKeyItemVM CreateFromGameKeyWithForcedName(GameKey gameKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x000355CA File Offset: 0x000337CA
		public static InputKeyItemVM CreateFromForcedID(string forcedID, TextObject forcedName)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM._forcedID = forcedID;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000D23 RID: 3363 RVA: 0x000355E5 File Offset: 0x000337E5
		// (set) Token: 0x06000D24 RID: 3364 RVA: 0x000355ED File Offset: 0x000337ED
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

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000D25 RID: 3365 RVA: 0x00035610 File Offset: 0x00033810
		// (set) Token: 0x06000D26 RID: 3366 RVA: 0x00035618 File Offset: 0x00033818
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

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000D27 RID: 3367 RVA: 0x0003563B File Offset: 0x0003383B
		// (set) Token: 0x06000D28 RID: 3368 RVA: 0x00035643 File Offset: 0x00033843
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

		// Token: 0x04000604 RID: 1540
		private bool _isVisibleToConsoleOnly;

		// Token: 0x04000605 RID: 1541
		private TextObject _forcedName;

		// Token: 0x04000606 RID: 1542
		private string _forcedID;

		// Token: 0x04000607 RID: 1543
		private bool? _forcedVisibility;

		// Token: 0x04000608 RID: 1544
		private string _keyID;

		// Token: 0x04000609 RID: 1545
		private string _keyName;

		// Token: 0x0400060A RID: 1546
		private bool _isVisible;
	}
}
