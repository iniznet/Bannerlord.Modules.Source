using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	public class EncyclopediaHomeVM : EncyclopediaPageVM
	{
		public EncyclopediaHomeVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this.Lists = new MBBindingList<ListTypeVM>();
			foreach (EncyclopediaPage encyclopediaPage in from p in Campaign.Current.EncyclopediaManager.GetEncyclopediaPages()
				orderby p.HomePageOrderIndex
				select p)
			{
				this.Lists.Add(new ListTypeVM(encyclopediaPage));
			}
			this.RefreshValues();
		}

		public override void Refresh()
		{
			base.Refresh();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._baseName = GameTexts.FindText("str_encyclopedia_name", null).ToString();
			this.HomeTitleText = GameTexts.FindText("str_encyclopedia_name", null).ToString();
			this.Lists.ApplyActionOnAllItems(delegate(ListTypeVM x)
			{
				x.RefreshValues();
			});
		}

		public override string GetNavigationBarURL()
		{
			return GameTexts.FindText("str_encyclopedia_home", null).ToString() + " \\";
		}

		public override string GetName()
		{
			return this._baseName;
		}

		[DataSourceProperty]
		public bool IsListActive
		{
			get
			{
				return this._isListActive;
			}
			set
			{
				if (value != this._isListActive)
				{
					this._isListActive = value;
					base.OnPropertyChangedWithValue(value, "IsListActive");
				}
			}
		}

		[DataSourceProperty]
		public string HomeTitleText
		{
			get
			{
				return this._homeTitleText;
			}
			set
			{
				if (value != this._homeTitleText)
				{
					this._homeTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "HomeTitleText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<ListTypeVM> Lists
		{
			get
			{
				return this._lists;
			}
			set
			{
				if (value != this._lists)
				{
					this._lists = value;
					base.OnPropertyChangedWithValue<MBBindingList<ListTypeVM>>(value, "Lists");
				}
			}
		}

		private string _baseName;

		private MBBindingList<ListTypeVM> _lists;

		private bool _isListActive;

		private string _homeTitleText;
	}
}
