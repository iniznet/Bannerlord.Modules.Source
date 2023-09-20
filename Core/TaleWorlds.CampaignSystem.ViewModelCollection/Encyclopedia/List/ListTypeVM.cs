using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000C3 RID: 195
	public class ListTypeVM : ViewModel
	{
		// Token: 0x060012F2 RID: 4850 RVA: 0x00049408 File Offset: 0x00047608
		public ListTypeVM(EncyclopediaPage encyclopediaPage)
		{
			this.EncyclopediaPage = encyclopediaPage;
			this.ID = encyclopediaPage.GetIdentifierNames()[0];
			this.ImageID = encyclopediaPage.GetStringID();
			this.Order = encyclopediaPage.HomePageOrderIndex;
			this.RefreshValues();
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x00049443 File Offset: 0x00047643
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EncyclopediaPage.GetName().ToString();
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x00049461 File Offset: 0x00047661
		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink("ListPage", this.ID);
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x060012F5 RID: 4853 RVA: 0x0004947D File Offset: 0x0004767D
		// (set) Token: 0x060012F6 RID: 4854 RVA: 0x00049485 File Offset: 0x00047685
		[DataSourceProperty]
		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value != this._id)
				{
					this._id = value;
					base.OnPropertyChangedWithValue<string>(value, "ID");
				}
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x060012F7 RID: 4855 RVA: 0x000494A8 File Offset: 0x000476A8
		// (set) Token: 0x060012F8 RID: 4856 RVA: 0x000494B0 File Offset: 0x000476B0
		[DataSourceProperty]
		public int Order
		{
			get
			{
				return this._order;
			}
			set
			{
				if (value != this._order)
				{
					this._order = value;
					base.OnPropertyChangedWithValue(value, "Order");
				}
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x000494CE File Offset: 0x000476CE
		// (set) Token: 0x060012FA RID: 4858 RVA: 0x000494D6 File Offset: 0x000476D6
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x000494F9 File Offset: 0x000476F9
		// (set) Token: 0x060012FC RID: 4860 RVA: 0x00049501 File Offset: 0x00047701
		[DataSourceProperty]
		public string ImageID
		{
			get
			{
				return this._imageId;
			}
			set
			{
				if (value != this._imageId)
				{
					this._imageId = value;
					base.OnPropertyChangedWithValue<string>(value, "ImageID");
				}
			}
		}

		// Token: 0x040008C8 RID: 2248
		public readonly EncyclopediaPage EncyclopediaPage;

		// Token: 0x040008C9 RID: 2249
		private string _name;

		// Token: 0x040008CA RID: 2250
		private string _id;

		// Token: 0x040008CB RID: 2251
		private string _imageId;

		// Token: 0x040008CC RID: 2252
		private int _order;
	}
}
