using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderColorItemVM : ViewModel
	{
		public int ColorID { get; private set; }

		public BannerColor BannerColor { get; private set; }

		public BannerBuilderColorItemVM(Action<BannerBuilderColorItemVM> onItemSelection, int key, BannerColor value)
		{
			this._onItemSelection = onItemSelection;
			this.ColorID = key;
			this.BannerColor = value;
			this.ColorAsStr = Color.FromUint(value.Color).ToString();
		}

		public void ExecuteSelection()
		{
			Action<BannerBuilderColorItemVM> onItemSelection = this._onItemSelection;
			if (onItemSelection == null)
			{
				return;
			}
			onItemSelection(this);
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
		public string ColorAsStr
		{
			get
			{
				return this._colorAsStr;
			}
			set
			{
				if (value != this._colorAsStr)
				{
					this._colorAsStr = value;
					base.OnPropertyChangedWithValue<string>(value, "ColorAsStr");
				}
			}
		}

		private readonly Action<BannerBuilderColorItemVM> _onItemSelection;

		private bool _isSelected;

		private string _colorAsStr;
	}
}
