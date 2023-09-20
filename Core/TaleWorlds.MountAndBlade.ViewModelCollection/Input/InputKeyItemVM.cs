using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Input
{
	// Token: 0x020000D5 RID: 213
	public class InputKeyItemVM : ViewModel
	{
		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x00041268 File Offset: 0x0003F468
		// (set) Token: 0x060013D0 RID: 5072 RVA: 0x00041270 File Offset: 0x0003F470
		public GameKey GameKey { get; private set; }

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x060013D1 RID: 5073 RVA: 0x00041279 File Offset: 0x0003F479
		// (set) Token: 0x060013D2 RID: 5074 RVA: 0x00041281 File Offset: 0x0003F481
		public HotKey HotKey { get; private set; }

		// Token: 0x060013D3 RID: 5075 RVA: 0x0004128A File Offset: 0x0003F48A
		private InputKeyItemVM()
		{
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x000412B2 File Offset: 0x0003F4B2
		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x000412DA File Offset: 0x0003F4DA
		private void OnGamepadActiveStateChanged()
		{
			this.ForceRefresh();
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x000412E2 File Offset: 0x0003F4E2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ForceRefresh();
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x000412F0 File Offset: 0x0003F4F0
		public void SetForcedVisibility(bool? isVisible)
		{
			this._forcedVisibility = isVisible;
			this.UpdateVisibility();
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x00041300 File Offset: 0x0003F500
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
				this.KeyName = ((forcedName != null) ? forcedName.ToString() : null) ?? Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.GameKey.GroupId + "_" + this.GameKey.StringId).ToString();
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

		// Token: 0x060013D9 RID: 5081 RVA: 0x0004151C File Offset: 0x0003F71C
		private void UpdateVisibility()
		{
			this.IsVisible = this._forcedVisibility ?? (!this._isVisibleToConsoleOnly || Input.IsGamepadActive);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x00041558 File Offset: 0x0003F758
		public static InputKeyItemVM CreateFromGameKey(GameKey gameKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x00041573 File Offset: 0x0003F773
		public static InputKeyItemVM CreateFromHotKey(HotKey hotKey, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x0004158E File Offset: 0x0003F78E
		public static InputKeyItemVM CreateFromHotKeyWithForcedName(HotKey hotKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.HotKey = hotKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x000415B0 File Offset: 0x0003F7B0
		public static InputKeyItemVM CreateFromGameKeyWithForcedName(GameKey gameKey, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM.GameKey = gameKey;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x000415D2 File Offset: 0x0003F7D2
		public static InputKeyItemVM CreateFromForcedID(string forcedID, TextObject forcedName, bool isConsoleOnly)
		{
			InputKeyItemVM inputKeyItemVM = new InputKeyItemVM();
			inputKeyItemVM._forcedID = forcedID;
			inputKeyItemVM._forcedName = forcedName;
			inputKeyItemVM._isVisibleToConsoleOnly = isConsoleOnly;
			inputKeyItemVM.ForceRefresh();
			return inputKeyItemVM;
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x000415F4 File Offset: 0x0003F7F4
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x000415FC File Offset: 0x0003F7FC
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

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x060013E1 RID: 5089 RVA: 0x0004161F File Offset: 0x0003F81F
		// (set) Token: 0x060013E2 RID: 5090 RVA: 0x00041627 File Offset: 0x0003F827
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

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x060013E3 RID: 5091 RVA: 0x0004164A File Offset: 0x0003F84A
		// (set) Token: 0x060013E4 RID: 5092 RVA: 0x00041652 File Offset: 0x0003F852
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

		// Token: 0x04000985 RID: 2437
		private bool _isVisibleToConsoleOnly;

		// Token: 0x04000986 RID: 2438
		private TextObject _forcedName;

		// Token: 0x04000987 RID: 2439
		private string _forcedID;

		// Token: 0x04000988 RID: 2440
		private bool? _forcedVisibility;

		// Token: 0x04000989 RID: 2441
		private string _keyID;

		// Token: 0x0400098A RID: 2442
		private string _keyName;

		// Token: 0x0400098B RID: 2443
		private bool _isVisible;
	}
}
