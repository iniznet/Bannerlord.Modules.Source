using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x0200000B RID: 11
	public class CraftingItemViewModel : ViewModel
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000055 RID: 85 RVA: 0x0000283D File Offset: 0x00000A3D
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002845 File Offset: 0x00000A45
		[DataSourceProperty]
		public string UsedPieces
		{
			get
			{
				return this._usedPieces;
			}
			set
			{
				this._usedPieces = value;
				base.OnPropertyChangedWithValue<string>(value, "UsedPieces");
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000057 RID: 87 RVA: 0x0000285A File Offset: 0x00000A5A
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002862 File Offset: 0x00000A62
		[DataSourceProperty]
		public int WeaponClass
		{
			get
			{
				return this._weaponClass;
			}
			set
			{
				if (value != this._weaponClass)
				{
					this._weaponClass = value;
					base.OnPropertyChangedWithValue(value, "WeaponClass");
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002880 File Offset: 0x00000A80
		public WeaponClass GetWeaponClass()
		{
			return (WeaponClass)this.WeaponClass;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002888 File Offset: 0x00000A88
		public void SetCraftingData(WeaponClass weaponClass, WeaponDesignElement[] craftingPieces)
		{
			this.WeaponClass = (int)weaponClass;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002891 File Offset: 0x00000A91
		public CraftingItemViewModel()
		{
			this.WeaponClass = -1;
		}

		// Token: 0x04000019 RID: 25
		private string _usedPieces;

		// Token: 0x0400001A RID: 26
		private int _weaponClass;
	}
}
