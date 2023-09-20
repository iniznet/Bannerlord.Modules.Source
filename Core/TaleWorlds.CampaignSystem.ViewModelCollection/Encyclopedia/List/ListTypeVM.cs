using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class ListTypeVM : ViewModel
	{
		public ListTypeVM(EncyclopediaPage encyclopediaPage)
		{
			this.EncyclopediaPage = encyclopediaPage;
			this.ID = encyclopediaPage.GetIdentifierNames()[0];
			this.ImageID = encyclopediaPage.GetStringID();
			this.Order = encyclopediaPage.HomePageOrderIndex;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.EncyclopediaPage.GetName().ToString();
		}

		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink("ListPage", this.ID);
		}

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

		public readonly EncyclopediaPage EncyclopediaPage;

		private string _name;

		private string _id;

		private string _imageId;

		private int _order;
	}
}
