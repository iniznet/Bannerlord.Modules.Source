using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000094 RID: 148
	public class TownManagementShopItemVM : ViewModel
	{
		// Token: 0x06000E4D RID: 3661 RVA: 0x00038F5C File Offset: 0x0003715C
		public TownManagementShopItemVM(Workshop workshop)
		{
			this._workshop = workshop;
			this.IsEmpty = this._workshop.WorkshopType == null;
			if (!this.IsEmpty)
			{
				this.ShopId = this._workshop.WorkshopType.StringId;
			}
			else
			{
				this.ShopId = "empty";
			}
			this.RefreshValues();
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x00038FBC File Offset: 0x000371BC
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (!this.IsEmpty)
			{
				this.ShopName = this._workshop.WorkshopType.Name.ToString();
				return;
			}
			this.ShopName = GameTexts.FindText("str_empty", null).ToString();
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00039009 File Offset: 0x00037209
		public void ExecuteBeginHint()
		{
			if (this._workshop.WorkshopType != null)
			{
				InformationManager.ShowTooltip(typeof(Workshop), new object[] { this._workshop });
			}
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00039036 File Offset: 0x00037236
		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x0003903D File Offset: 0x0003723D
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x00039045 File Offset: 0x00037245
		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
			set
			{
				if (value != this._isEmpty)
				{
					this._isEmpty = value;
					base.OnPropertyChangedWithValue(value, "IsEmpty");
				}
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06000E53 RID: 3667 RVA: 0x00039063 File Offset: 0x00037263
		// (set) Token: 0x06000E54 RID: 3668 RVA: 0x0003906B File Offset: 0x0003726B
		[DataSourceProperty]
		public string ShopName
		{
			get
			{
				return this._shopName;
			}
			set
			{
				if (value != this._shopName)
				{
					this._shopName = value;
					base.OnPropertyChangedWithValue<string>(value, "ShopName");
				}
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0003908E File Offset: 0x0003728E
		// (set) Token: 0x06000E56 RID: 3670 RVA: 0x00039096 File Offset: 0x00037296
		[DataSourceProperty]
		public string ShopId
		{
			get
			{
				return this._shopId;
			}
			set
			{
				if (value != this._shopId)
				{
					this._shopId = value;
					base.OnPropertyChangedWithValue<string>(value, "ShopId");
				}
			}
		}

		// Token: 0x040006A6 RID: 1702
		private readonly Workshop _workshop;

		// Token: 0x040006A7 RID: 1703
		private bool _isEmpty;

		// Token: 0x040006A8 RID: 1704
		private string _shopName;

		// Token: 0x040006A9 RID: 1705
		private string _shopId;
	}
}
