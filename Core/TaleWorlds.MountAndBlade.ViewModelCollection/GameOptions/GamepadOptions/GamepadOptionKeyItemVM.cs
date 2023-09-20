using System;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GamepadOptions
{
	public class GamepadOptionKeyItemVM : ViewModel
	{
		public GameKey GamepadKey { get; }

		public HotKey GamepadHotKey { get; }

		public InputKey? Key { get; }

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

		private TextObject _nameObject;

		private string _action;

		private string _keyIdAsString;

		private int _keyId;
	}
}
