using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public abstract class KeyOptionVM : ViewModel
	{
		public Key CurrentKey { get; protected set; }

		public Key Key { get; protected set; }

		public KeyOptionVM(string groupId, string id, Action<KeyOptionVM> onKeybindRequest)
		{
			this._groupId = groupId;
			this._id = id;
			this._onKeybindRequest = onKeybindRequest;
		}

		public abstract void Set(InputKey newKey);

		public abstract void Update();

		public abstract void OnDone();

		internal abstract bool IsChanged();

		internal abstract void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj);

		[DataSourceProperty]
		public string OptionValueText
		{
			get
			{
				return this._optionValueText;
			}
			set
			{
				if (value != this._optionValueText)
				{
					this._optionValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "OptionValueText");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		protected readonly string _groupId;

		protected readonly string _id;

		protected readonly Action<KeyOptionVM> _onKeybindRequest;

		private string _optionValueText;

		private string _name;

		private string _description;
	}
}
