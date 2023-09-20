using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MPCultureItemVM : ViewModel
	{
		public BasicCultureObject Culture { get; private set; }

		public MPCultureItemVM(string cultureCode, Action<MPCultureItemVM> onSelection)
		{
			this._onSelection = onSelection;
			this.CultureCode = cultureCode;
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(cultureCode);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(this.Culture.Name, null);
		}

		private void ExecuteSelection()
		{
			this._onSelection(this);
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
					base.OnPropertyChanged("IsSelected");
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
					base.OnPropertyChanged("CultureCode");
				}
			}
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
					base.OnPropertyChanged("Hint");
				}
			}
		}

		private Action<MPCultureItemVM> _onSelection;

		private bool _isSelected;

		private string _cultureCode;

		private HintViewModel _hint;
	}
}
