using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	public class EncyclopediaSearchResultVM : ViewModel
	{
		public string OrgNameText { get; private set; }

		public EncyclopediaSearchResultVM(EncyclopediaListItem source, string searchedText, int matchStartIndex)
		{
			this.MatchStartIndex = matchStartIndex;
			this.LinkId = source.Id;
			this.PageType = source.TypeName;
			this.OrgNameText = source.Name;
			this._nameText = source.Name;
			this.UpdateSearchedText(searchedText);
		}

		public void UpdateSearchedText(string searchedText)
		{
			this._searchedText = searchedText;
			string text = this.OrgNameText.Substring(this.OrgNameText.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase), this._searchedText.Length);
			if (!string.IsNullOrEmpty(text))
			{
				this.NameText = this.OrgNameText.Replace(text, "<a>" + text + "</a>");
			}
		}

		public void Execute()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.PageType, this.LinkId);
		}

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

		private string _searchedText;

		public readonly int MatchStartIndex;

		public string LinkId = "";

		public string PageType;

		public string _nameText;
	}
}
