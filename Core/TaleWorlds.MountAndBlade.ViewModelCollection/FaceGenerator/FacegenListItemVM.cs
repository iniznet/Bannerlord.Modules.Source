using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator
{
	public class FacegenListItemVM : ViewModel
	{
		public void ExecuteAction()
		{
			this._setSelected(this, true);
		}

		public FacegenListItemVM(string imagePath, int index, Action<FacegenListItemVM, bool> setSelected)
		{
			this.ImagePath = imagePath;
			this.Index = index;
			this.IsSelected = false;
			this._setSelected = setSelected;
		}

		[DataSourceProperty]
		public string ImagePath
		{
			get
			{
				return this._imagePath;
			}
			set
			{
				if (value != this._imagePath)
				{
					this._imagePath = value;
					base.OnPropertyChangedWithValue<string>(value, "ImagePath");
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
		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (value != this._index)
				{
					this._index = value;
					base.OnPropertyChangedWithValue(value, "Index");
				}
			}
		}

		private readonly Action<FacegenListItemVM, bool> _setSelected;

		private string _imagePath;

		private bool _isSelected = true;

		private int _index = -1;
	}
}
