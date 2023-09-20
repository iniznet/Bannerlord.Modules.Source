using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FactionBanVote
{
	public class MultiplayerFactionBanVoteVM : ViewModel
	{
		public MultiplayerFactionBanVoteVM(BasicCultureObject culture, Action<MultiplayerFactionBanVoteVM> onSelect)
		{
			this.Culture = culture;
			this._onSelect = onSelect;
			this._isEnabled = true;
			this._name = culture.Name.ToString();
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
					if (value)
					{
						this._onSelect(this);
					}
				}
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

		private readonly Action<MultiplayerFactionBanVoteVM> _onSelect;

		public readonly BasicCultureObject Culture;

		private string _name;

		private bool _isEnabled;

		private bool _isSelected;
	}
}
