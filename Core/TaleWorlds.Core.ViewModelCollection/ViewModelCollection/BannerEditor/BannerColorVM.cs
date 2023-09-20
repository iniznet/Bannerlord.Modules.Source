using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	// Token: 0x02000025 RID: 37
	public class BannerColorVM : ViewModel
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000551B File Offset: 0x0000371B
		public int ColorID { get; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00005523 File Offset: 0x00003723
		public uint Color { get; }

		// Token: 0x060001A3 RID: 419 RVA: 0x0000552C File Offset: 0x0000372C
		public BannerColorVM(int colorID, uint color, Action<BannerColorVM> onSelection)
		{
			this.Color = color;
			this.ColorAsStr = TaleWorlds.Library.Color.FromUint(this.Color).ToString();
			this.ColorID = colorID;
			this._onSelection = onSelection;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00005573 File Offset: 0x00003773
		public void ExecuteSelectIcon()
		{
			this._onSelection(this);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00005581 File Offset: 0x00003781
		public void SetOnSelectionAction(Action<BannerColorVM> onSelection)
		{
			this._onSelection = onSelection;
			this.IsSelected = false;
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00005591 File Offset: 0x00003791
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00005599 File Offset: 0x00003799
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

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x000055BC File Offset: 0x000037BC
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x000055C4 File Offset: 0x000037C4
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

		// Token: 0x040000AD RID: 173
		private Action<BannerColorVM> _onSelection;

		// Token: 0x040000AE RID: 174
		private string _colorAsStr;

		// Token: 0x040000AF RID: 175
		private bool _isSelected;
	}
}
