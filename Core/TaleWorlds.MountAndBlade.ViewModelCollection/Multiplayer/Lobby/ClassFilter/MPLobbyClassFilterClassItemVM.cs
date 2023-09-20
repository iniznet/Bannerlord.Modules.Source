using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter
{
	public class MPLobbyClassFilterClassItemVM : ViewModel
	{
		public MultiplayerClassDivisions.MPHeroClass HeroClass { get; private set; }

		public MPLobbyClassFilterClassItemVM(string cultureCode, MultiplayerClassDivisions.MPHeroClass heroClass, Action<MPLobbyClassFilterClassItemVM> onSelect)
		{
			this.HeroClass = heroClass;
			this._onSelect = onSelect;
			this.CultureCode = cultureCode;
			this.IconType = this.HeroClass.IconType.ToString();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.HeroClass.HeroName.ToString();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroClass = null;
		}

		private void ExecuteSelect()
		{
			if (this._onSelect != null)
			{
				this._onSelect(this);
			}
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
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureCode");
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
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
				}
			}
		}

		private Action<MPLobbyClassFilterClassItemVM> _onSelect;

		private bool _isEnabled;

		private bool _isSelected;

		private string _cultureCode;

		private string _name;

		private string _iconType;
	}
}
