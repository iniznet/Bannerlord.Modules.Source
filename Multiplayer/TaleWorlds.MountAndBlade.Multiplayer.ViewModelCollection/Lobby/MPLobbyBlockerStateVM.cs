using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyBlockerStateVM : ViewModel
	{
		public MPLobbyBlockerStateVM(Action<bool> setNavigationRestriction)
		{
			this._setNavigationRestriction = setNavigationRestriction;
		}

		public void OnLobbyStateIsBlocker(TextObject description)
		{
			this._descriptionObj = description;
			this.IsEnabled = true;
			this.Description = this._descriptionObj.ToString();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject descriptionObj = this._descriptionObj;
			this.Description = ((descriptionObj != null) ? descriptionObj.ToString() : null);
		}

		public void OnLobbyStateNotBlocker()
		{
			this.IsEnabled = false;
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this._setNavigationRestriction(value);
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

		private Action<bool> _setNavigationRestriction;

		private TextObject _descriptionObj;

		private bool _isEnabled;

		private string _description;
	}
}
