using System;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GamepadOptions
{
	// Token: 0x020000FF RID: 255
	public class GamepadOptionKeyItemVM : ViewModel
	{
		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x0600169F RID: 5791 RVA: 0x000492AD File Offset: 0x000474AD
		public GameKey GamepadKey { get; }

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x060016A0 RID: 5792 RVA: 0x000492B5 File Offset: 0x000474B5
		public HotKey GamepadHotKey { get; }

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x060016A1 RID: 5793 RVA: 0x000492BD File Offset: 0x000474BD
		public InputKey? Key { get; }

		// Token: 0x060016A2 RID: 5794 RVA: 0x000492C8 File Offset: 0x000474C8
		public GamepadOptionKeyItemVM(GameKey gamepadGameKey)
		{
			this.GamepadKey = gamepadGameKey;
			this.Action = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.GamepadKey.GroupId + "_" + this.GamepadKey.StringId).ToString();
			this.Key = new InputKey?(gamepadGameKey.ControllerKey.InputKey);
			this.KeyId = (int)this.Key.Value;
			string text;
			if (gamepadGameKey == null)
			{
				text = null;
			}
			else
			{
				Key controllerKey = gamepadGameKey.ControllerKey;
				text = ((controllerKey != null) ? controllerKey.InputKey.ToString() : null);
			}
			this.KeyIdAsString = text ?? string.Empty;
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00049380 File Offset: 0x00047580
		public GamepadOptionKeyItemVM(HotKey gamepadHotKey)
		{
			this.GamepadHotKey = gamepadHotKey;
			this.Action = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.GamepadHotKey.GroupId + "_" + this.GamepadHotKey.Id).ToString();
			Key key = gamepadHotKey.Keys.FirstOrDefault((Key k) => k.IsControllerInput);
			InputKey? inputKey;
			InputKey? inputKey2;
			if (key == null)
			{
				inputKey = null;
				inputKey2 = inputKey;
			}
			else
			{
				inputKey2 = new InputKey?(key.InputKey);
			}
			this.Key = inputKey2;
			inputKey = this.Key;
			this.KeyId = (int)inputKey.Value;
			inputKey = this.Key;
			this.KeyIdAsString = ((inputKey != null) ? inputKey.GetValueOrDefault().ToString() : null) ?? string.Empty;
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00049470 File Offset: 0x00047670
		public GamepadOptionKeyItemVM(InputKey key, TextObject name)
		{
			this.Key = new InputKey?(key);
			InputKey? inputKey = this.Key;
			this.KeyId = (int)inputKey.Value;
			inputKey = this.Key;
			this.KeyIdAsString = ((inputKey != null) ? inputKey.GetValueOrDefault().ToString() : null) ?? string.Empty;
			this._nameObject = name;
			this.Action = this._nameObject.ToString();
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x000494F4 File Offset: 0x000476F4
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.GamepadKey != null)
			{
				this.Action = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.GamepadKey.GroupId + "_" + this.GamepadKey.StringId).ToString();
				return;
			}
			if (this.GamepadHotKey != null)
			{
				this.Action = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this.GamepadHotKey.GroupId + "_" + this.GamepadHotKey.Id).ToString();
				return;
			}
			if (this._nameObject != null)
			{
				this.Action = this._nameObject.ToString();
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x060016A6 RID: 5798 RVA: 0x000495B0 File Offset: 0x000477B0
		// (set) Token: 0x060016A7 RID: 5799 RVA: 0x000495B8 File Offset: 0x000477B8
		[DataSourceProperty]
		public string Action
		{
			get
			{
				return this._action;
			}
			set
			{
				if (value != this._action)
				{
					this._action = value;
					base.OnPropertyChangedWithValue<string>(value, "Action");
				}
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x060016A8 RID: 5800 RVA: 0x000495DB File Offset: 0x000477DB
		// (set) Token: 0x060016A9 RID: 5801 RVA: 0x000495E3 File Offset: 0x000477E3
		[DataSourceProperty]
		public int KeyId
		{
			get
			{
				return this._keyId;
			}
			set
			{
				if (value != this._keyId)
				{
					this._keyId = value;
					base.OnPropertyChangedWithValue(value, "KeyId");
				}
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x060016AA RID: 5802 RVA: 0x00049601 File Offset: 0x00047801
		// (set) Token: 0x060016AB RID: 5803 RVA: 0x00049609 File Offset: 0x00047809
		[DataSourceProperty]
		public string KeyIdAsString
		{
			get
			{
				return this._keyIdAsString;
			}
			set
			{
				if (value != this._keyIdAsString)
				{
					this._keyIdAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "KeyIdAsString");
				}
			}
		}

		// Token: 0x04000ABF RID: 2751
		private TextObject _nameObject;

		// Token: 0x04000AC1 RID: 2753
		private string _action;

		// Token: 0x04000AC2 RID: 2754
		private string _keyIdAsString;

		// Token: 0x04000AC3 RID: 2755
		private int _keyId;
	}
}
