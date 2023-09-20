using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x0200007C RID: 124
	public class ItemPreviewVM : ViewModel
	{
		// Token: 0x06000B3F RID: 2879 RVA: 0x0002E0A5 File Offset: 0x0002C2A5
		public ItemPreviewVM(Action onClosed)
		{
			this._onClosed = onClosed;
			this.ItemTableau = new ItemCollectionElementViewModel();
			this.RefreshValues();
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0002E0C5 File Offset: 0x0002C2C5
		public override void OnFinalize()
		{
			this.ItemTableau.OnFinalize();
			this.ItemTableau = null;
			base.OnFinalize();
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x0002E0DF File Offset: 0x0002C2DF
		public void Open(EquipmentElement item)
		{
			this.ItemTableau.FillFrom(item, BannerCode.CreateFrom(Clan.PlayerClan.Banner).Code);
			this.ItemName = item.Item.Name.ToString();
			this.IsSelected = true;
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0002E11F File Offset: 0x0002C31F
		public void ExecuteClose()
		{
			this.Close();
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x0002E127 File Offset: 0x0002C327
		public void Close()
		{
			this._onClosed();
			this.IsSelected = false;
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000B44 RID: 2884 RVA: 0x0002E13B File Offset: 0x0002C33B
		// (set) Token: 0x06000B45 RID: 2885 RVA: 0x0002E143 File Offset: 0x0002C343
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

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0002E160 File Offset: 0x0002C360
		// (set) Token: 0x06000B47 RID: 2887 RVA: 0x0002E168 File Offset: 0x0002C368
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

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x0002E18A File Offset: 0x0002C38A
		// (set) Token: 0x06000B49 RID: 2889 RVA: 0x0002E192 File Offset: 0x0002C392
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

		// Token: 0x04000532 RID: 1330
		private Action _onClosed;

		// Token: 0x04000533 RID: 1331
		private bool _isSelected;

		// Token: 0x04000534 RID: 1332
		private string _itemName;

		// Token: 0x04000535 RID: 1333
		private ItemCollectionElementViewModel _itemTableau;
	}
}
