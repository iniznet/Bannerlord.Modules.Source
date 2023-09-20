using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	public class FactionItemVM : ViewModel
	{
		public BasicCultureObject Faction { get; private set; }

		public FactionItemVM(BasicCultureObject faction, Action<FactionItemVM> onSelected)
		{
			this.Faction = faction;
			this._onSelected = onSelected;
			this.CultureCode = faction.GetCultureCode().ToString().ToLower();
			this.Hint = new HintViewModel(faction.Name, null);
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
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
						this._onSelected(this);
					}
				}
			}
		}

		private Action<FactionItemVM> _onSelected;

		private HintViewModel _hint;

		private string _cultureCode;

		private bool _isSelected;
	}
}
