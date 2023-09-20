using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class ItemCollectionElementViewModel : ViewModel
	{
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

		public void FillFrom(EquipmentElement item, string bannerCode = "")
		{
			this.StringId = ((item.Item != null) ? item.Item.StringId : "");
			this.Ammo = (int)((!item.IsEmpty && item.Item.PrimaryWeapon != null && item.Item.PrimaryWeapon.IsConsumable) ? item.GetModifiedStackCountForUsage(0) : 0);
			this.AverageUnitCost = item.ItemValue;
			this.ItemModifierId = ((item.ItemModifier != null) ? item.ItemModifier.StringId : "");
			this.BannerCode = bannerCode;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.StringId = "";
			this.ItemModifierId = "";
		}

		private string _stringId;

		private int _ammo;

		private int _averageUnitCost;

		private string _itemModifierId;

		private string _bannerCode;

		private float _initialPanRotation;
	}
}
