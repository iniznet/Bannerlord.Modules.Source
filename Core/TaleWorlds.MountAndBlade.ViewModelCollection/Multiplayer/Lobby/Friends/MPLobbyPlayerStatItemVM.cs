using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyPlayerStatItemVM : ViewModel
	{
		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, string value)
		{
			this.GameMode = gameMode;
			this._descriptionText = description;
			this.Value = value;
			this.RefreshValues();
		}

		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, float value)
			: this(gameMode, description, value.ToString("0.00"))
		{
		}

		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, int value)
			: this(gameMode, description, value.ToString())
		{
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject descriptionText = this._descriptionText;
			this.Description = ((descriptionText != null) ? descriptionText.ToString() : null) ?? "";
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

		[DataSourceProperty]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		public readonly string GameMode;

		private readonly TextObject _descriptionText;

		private string _description;

		private string _value;
	}
}
