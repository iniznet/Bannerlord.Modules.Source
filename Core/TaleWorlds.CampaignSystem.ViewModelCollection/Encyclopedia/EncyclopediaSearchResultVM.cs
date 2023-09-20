using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000AF RID: 175
	public class EncyclopediaSearchResultVM : ViewModel
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x00043DF1 File Offset: 0x00041FF1
		// (set) Token: 0x06001122 RID: 4386 RVA: 0x00043DF9 File Offset: 0x00041FF9
		public string OrgNameText { get; private set; }

		// Token: 0x06001123 RID: 4387 RVA: 0x00043E04 File Offset: 0x00042004
		public EncyclopediaSearchResultVM(EncyclopediaListItem source, string searchedText, int matchStartIndex)
		{
			this.MatchStartIndex = matchStartIndex;
			this.LinkId = source.Id;
			this.PageType = source.TypeName;
			this.OrgNameText = source.Name;
			this._nameText = source.Name;
			this.UpdateSearchedText(searchedText);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00043E60 File Offset: 0x00042060
		public void UpdateSearchedText(string searchedText)
		{
			this._searchedText = searchedText;
			string text = this.OrgNameText.Substring(this.OrgNameText.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase), this._searchedText.Length);
			if (!string.IsNullOrEmpty(text))
			{
				this.NameText = this.OrgNameText.Replace(text, "<a>" + text + "</a>");
			}
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00043EC7 File Offset: 0x000420C7
		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.PageType, this.LinkId);
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001126 RID: 4390 RVA: 0x00043EE4 File Offset: 0x000420E4
		// (set) Token: 0x06001127 RID: 4391 RVA: 0x00043EEC File Offset: 0x000420EC
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (this._nameText != value)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x040007F8 RID: 2040
		private string _searchedText;

		// Token: 0x040007FA RID: 2042
		public readonly int MatchStartIndex;

		// Token: 0x040007FB RID: 2043
		public string LinkId = "";

		// Token: 0x040007FC RID: 2044
		public string PageType;

		// Token: 0x040007FD RID: 2045
		public string _nameText;
	}
}
