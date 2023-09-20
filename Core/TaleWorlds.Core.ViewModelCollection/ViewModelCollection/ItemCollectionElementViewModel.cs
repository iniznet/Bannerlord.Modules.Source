using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x0200000C RID: 12
	public class ItemCollectionElementViewModel : ViewModel
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000028A0 File Offset: 0x00000AA0
		// (set) Token: 0x0600005D RID: 93 RVA: 0x000028A8 File Offset: 0x00000AA8
		[DataSourceProperty]
		public string StringId
		{
			get
			{
				return this._stringId;
			}
			set
			{
				if (this._stringId != value)
				{
					this._stringId = value;
					base.OnPropertyChangedWithValue<string>(value, "StringId");
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000028CB File Offset: 0x00000ACB
		// (set) Token: 0x0600005F RID: 95 RVA: 0x000028D3 File Offset: 0x00000AD3
		[DataSourceProperty]
		public int Ammo
		{
			get
			{
				return this._ammo;
			}
			set
			{
				if (this._ammo != value)
				{
					this._ammo = value;
					base.OnPropertyChangedWithValue(value, "Ammo");
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000028F1 File Offset: 0x00000AF1
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000028F9 File Offset: 0x00000AF9
		[DataSourceProperty]
		public int AverageUnitCost
		{
			get
			{
				return this._averageUnitCost;
			}
			set
			{
				if (this._averageUnitCost != value)
				{
					this._averageUnitCost = value;
					base.OnPropertyChangedWithValue(value, "AverageUnitCost");
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002917 File Offset: 0x00000B17
		// (set) Token: 0x06000063 RID: 99 RVA: 0x0000291F File Offset: 0x00000B1F
		[DataSourceProperty]
		public string ItemModifierId
		{
			get
			{
				return this._itemModifierId;
			}
			set
			{
				if (this._itemModifierId != value)
				{
					this._itemModifierId = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemModifierId");
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002942 File Offset: 0x00000B42
		// (set) Token: 0x06000065 RID: 101 RVA: 0x0000294A File Offset: 0x00000B4A
		[DataSourceProperty]
		public string BannerCode
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerCode");
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000296D File Offset: 0x00000B6D
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002975 File Offset: 0x00000B75
		[DataSourceProperty]
		public float InitialPanRotation
		{
			get
			{
				return this._initialPanRotation;
			}
			set
			{
				if (value != this._initialPanRotation)
				{
					this._initialPanRotation = value;
					base.OnPropertyChangedWithValue(value, "InitialPanRotation");
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002994 File Offset: 0x00000B94
		public void FillFrom(EquipmentElement item, string bannerCode = "")
		{
			this.StringId = ((item.Item != null) ? item.Item.StringId : "");
			this.Ammo = (int)((!item.IsEmpty && item.Item.PrimaryWeapon != null && item.Item.PrimaryWeapon.IsConsumable) ? item.GetModifiedStackCountForUsage(0) : 0);
			this.AverageUnitCost = item.ItemValue;
			this.ItemModifierId = ((item.ItemModifier != null) ? item.ItemModifier.StringId : "");
			this.BannerCode = bannerCode;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002A34 File Offset: 0x00000C34
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.StringId = "";
			this.ItemModifierId = "";
		}

		// Token: 0x0400001B RID: 27
		private string _stringId;

		// Token: 0x0400001C RID: 28
		private int _ammo;

		// Token: 0x0400001D RID: 29
		private int _averageUnitCost;

		// Token: 0x0400001E RID: 30
		private string _itemModifierId;

		// Token: 0x0400001F RID: 31
		private string _bannerCode;

		// Token: 0x04000020 RID: 32
		private float _initialPanRotation;
	}
}
