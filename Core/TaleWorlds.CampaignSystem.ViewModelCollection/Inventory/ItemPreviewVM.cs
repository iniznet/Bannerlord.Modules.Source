using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class ItemPreviewVM : ViewModel
	{
		public ItemPreviewVM(Action onClosed)
		{
			this._onClosed = onClosed;
			this.ItemTableau = new ItemCollectionElementViewModel();
			this.RefreshValues();
		}

		public override void OnFinalize()
		{
			this.ItemTableau.OnFinalize();
			this.ItemTableau = null;
			base.OnFinalize();
		}

		public void Open(EquipmentElement item)
		{
			this.ItemTableau.FillFrom(item, BannerCode.CreateFrom(Clan.PlayerClan.Banner).Code);
			this.ItemName = item.Item.Name.ToString();
			this.IsSelected = true;
		}

		public void ExecuteClose()
		{
			this.Close();
		}

		public void Close()
		{
			this._onClosed();
			this.IsSelected = false;
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
					base.OnPropertyChanged("IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string ItemName
		{
			get
			{
				return this._itemName;
			}
			set
			{
				if (value != this._itemName)
				{
					this._itemName = value;
					base.OnPropertyChanged("ItemName");
				}
			}
		}

		[DataSourceProperty]
		public ItemCollectionElementViewModel ItemTableau
		{
			get
			{
				return this._itemTableau;
			}
			set
			{
				if (value != this._itemTableau)
				{
					this._itemTableau = value;
					base.OnPropertyChangedWithValue<ItemCollectionElementViewModel>(value, "ItemTableau");
				}
			}
		}

		private Action _onClosed;

		private bool _isSelected;

		private string _itemName;

		private ItemCollectionElementViewModel _itemTableau;
	}
}
