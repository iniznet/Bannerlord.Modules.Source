using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000AC RID: 172
	public class EncyclopediaHomeVM : EncyclopediaPageVM
	{
		// Token: 0x060010E0 RID: 4320 RVA: 0x00043140 File Offset: 0x00041340
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

		// Token: 0x060010E1 RID: 4321 RVA: 0x000431DC File Offset: 0x000413DC
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshValues();
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x000431EC File Offset: 0x000413EC
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

		// Token: 0x060010E3 RID: 4323 RVA: 0x00043255 File Offset: 0x00041455
		public override string GetNavigationBarURL()
		{
			return GameTexts.FindText("str_encyclopedia_home", null).ToString() + " \\";
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00043271 File Offset: 0x00041471
		public override string GetName()
		{
			return this._baseName;
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x00043279 File Offset: 0x00041479
		// (set) Token: 0x060010E6 RID: 4326 RVA: 0x00043281 File Offset: 0x00041481
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

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x0004329F File Offset: 0x0004149F
		// (set) Token: 0x060010E8 RID: 4328 RVA: 0x000432A7 File Offset: 0x000414A7
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

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x000432CA File Offset: 0x000414CA
		// (set) Token: 0x060010EA RID: 4330 RVA: 0x000432D2 File Offset: 0x000414D2
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

		// Token: 0x040007E0 RID: 2016
		private string _baseName;

		// Token: 0x040007E1 RID: 2017
		private MBBindingList<ListTypeVM> _lists;

		// Token: 0x040007E2 RID: 2018
		private bool _isListActive;

		// Token: 0x040007E3 RID: 2019
		private string _homeTitleText;
	}
}
