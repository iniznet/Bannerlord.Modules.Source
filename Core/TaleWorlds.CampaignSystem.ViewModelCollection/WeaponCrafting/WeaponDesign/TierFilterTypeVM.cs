using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000DF RID: 223
	public class TierFilterTypeVM : ViewModel
	{
		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x0004E371 File Offset: 0x0004C571
		public WeaponDesignVM.CraftingPieceTierFilter FilterType { get; }

		// Token: 0x060014C5 RID: 5317 RVA: 0x0004E379 File Offset: 0x0004C579
		public TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter filterType, Action<WeaponDesignVM.CraftingPieceTierFilter> onSelect, string tierName)
		{
			this.FilterType = filterType;
			this._onSelect = onSelect;
			this.TierName = tierName;
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x0004E396 File Offset: 0x0004C596
		public void ExecuteSelectTier()
		{
			Action<WeaponDesignVM.CraftingPieceTierFilter> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this.FilterType);
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x060014C7 RID: 5319 RVA: 0x0004E3AE File Offset: 0x0004C5AE
		// (set) Token: 0x060014C8 RID: 5320 RVA: 0x0004E3B6 File Offset: 0x0004C5B6
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

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x060014C9 RID: 5321 RVA: 0x0004E3D4 File Offset: 0x0004C5D4
		// (set) Token: 0x060014CA RID: 5322 RVA: 0x0004E3DC File Offset: 0x0004C5DC
		[DataSourceProperty]
		public string TierName
		{
			get
			{
				return this._tierName;
			}
			set
			{
				if (value != this._tierName)
				{
					this._tierName = value;
					base.OnPropertyChangedWithValue<string>(value, "TierName");
				}
			}
		}

		// Token: 0x040009AF RID: 2479
		private readonly Action<WeaponDesignVM.CraftingPieceTierFilter> _onSelect;

		// Token: 0x040009B0 RID: 2480
		private bool _isSelected;

		// Token: 0x040009B1 RID: 2481
		private string _tierName;
	}
}
