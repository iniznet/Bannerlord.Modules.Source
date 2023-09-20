using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	// Token: 0x02000026 RID: 38
	public class BannerIconVM : ViewModel
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001AA RID: 426 RVA: 0x000055E2 File Offset: 0x000037E2
		public int IconID { get; }

		// Token: 0x060001AB RID: 427 RVA: 0x000055EA File Offset: 0x000037EA
		public BannerIconVM(int iconID, Action<BannerIconVM> onSelection)
		{
			this.IconPath = iconID.ToString();
			this.IconID = iconID;
			this._onSelection = onSelection;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000560D File Offset: 0x0000380D
		public void ExecuteSelectIcon()
		{
			this._onSelection(this);
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000561B File Offset: 0x0000381B
		// (set) Token: 0x060001AE RID: 430 RVA: 0x00005623 File Offset: 0x00003823
		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00005646 File Offset: 0x00003846
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000564E File Offset: 0x0000384E
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

		// Token: 0x040000B1 RID: 177
		private readonly Action<BannerIconVM> _onSelection;

		// Token: 0x040000B2 RID: 178
		private string _iconPath;

		// Token: 0x040000B3 RID: 179
		private bool _isSelected;
	}
}
