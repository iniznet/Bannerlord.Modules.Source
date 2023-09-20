using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class TownManagementShopItemVM : ViewModel
	{
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

		public void ExecuteBeginHint()
		{
			if (this._workshop.WorkshopType != null)
			{
				InformationManager.ShowTooltip(typeof(Workshop), new object[] { this._workshop });
			}
		}

		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

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

		private readonly Workshop _workshop;

		private bool _isEmpty;

		private string _shopName;

		private string _shopId;
	}
}
