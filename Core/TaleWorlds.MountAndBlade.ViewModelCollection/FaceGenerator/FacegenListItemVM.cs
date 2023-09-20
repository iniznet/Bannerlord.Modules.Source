using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator
{
	// Token: 0x02000105 RID: 261
	public class FacegenListItemVM : ViewModel
	{
		// Token: 0x060016F3 RID: 5875 RVA: 0x0004AC3A File Offset: 0x00048E3A
		public void ExecuteAction()
		{
			this._setSelected(this, true);
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x0004AC49 File Offset: 0x00048E49
		public FacegenListItemVM(string imagePath, int index, Action<FacegenListItemVM, bool> setSelected)
		{
			this.ImagePath = imagePath;
			this.Index = index;
			this.IsSelected = false;
			this._setSelected = setSelected;
		}

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x060016F5 RID: 5877 RVA: 0x0004AC7B File Offset: 0x00048E7B
		// (set) Token: 0x060016F6 RID: 5878 RVA: 0x0004AC83 File Offset: 0x00048E83
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

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x0004ACA6 File Offset: 0x00048EA6
		// (set) Token: 0x060016F8 RID: 5880 RVA: 0x0004ACAE File Offset: 0x00048EAE
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

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x0004ACCC File Offset: 0x00048ECC
		// (set) Token: 0x060016FA RID: 5882 RVA: 0x0004ACD4 File Offset: 0x00048ED4
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

		// Token: 0x04000ADD RID: 2781
		private readonly Action<FacegenListItemVM, bool> _setSelected;

		// Token: 0x04000ADE RID: 2782
		private string _imagePath;

		// Token: 0x04000ADF RID: 2783
		private bool _isSelected = true;

		// Token: 0x04000AE0 RID: 2784
		private int _index = -1;
	}
}
