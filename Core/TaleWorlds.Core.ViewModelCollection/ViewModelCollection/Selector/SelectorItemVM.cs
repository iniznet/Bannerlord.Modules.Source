using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Selector
{
	public class SelectorItemVM : ViewModel
	{
		public SelectorItemVM(TextObject s)
		{
			this._s = s;
			this.RefreshValues();
		}

		public SelectorItemVM(string s)
		{
			this._stringItem = s;
			this.RefreshValues();
		}

		public SelectorItemVM(TextObject s, TextObject hint)
		{
			this._s = s;
			this._hintObj = hint;
			this.RefreshValues();
		}

		public SelectorItemVM(string s, TextObject hint)
		{
			this._stringItem = s;
			this._hintObj = hint;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._s != null)
			{
				this._stringItem = this._s.ToString();
			}
			if (this._hintObj != null)
			{
				if (this._hint == null)
				{
					this._hint = new HintViewModel(this._hintObj, null);
					return;
				}
				this._hint.HintText = this._hintObj;
			}
		}

		[DataSourceProperty]
		public string StringItem
		{
			get
			{
				return this._stringItem;
			}
			set
			{
				if (value != this._stringItem)
				{
					this._stringItem = value;
					base.OnPropertyChangedWithValue<string>(value, "StringItem");
				}
			}
		}

		[DataSourceProperty]
		public bool CanBeSelected
		{
			get
			{
				return this._canBeSelected;
			}
			set
			{
				if (value != this._canBeSelected)
				{
					this._canBeSelected = value;
					base.OnPropertyChangedWithValue(value, "CanBeSelected");
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
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
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

		private TextObject _s;

		private TextObject _hintObj;

		private string _stringItem;

		private HintViewModel _hint;

		private bool _canBeSelected = true;

		private bool _isSelected;
	}
}
