using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B1 RID: 177
	[EncyclopediaViewModel(typeof(Concept))]
	public class EncyclopediaConceptPageVM : EncyclopediaContentPageVM
	{
		// Token: 0x06001164 RID: 4452 RVA: 0x00044C4C File Offset: 0x00042E4C
		public EncyclopediaConceptPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._concept = base.Obj as Concept;
			Concept.SetConceptTextLinks();
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._concept);
			this.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x00044CA2 File Offset: 0x00042EA2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = this._concept.Title.ToString();
			this.DescriptionText = this._concept.Description.ToString();
			base.UpdateBookmarkHintText();
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00044CDC File Offset: 0x00042EDC
		public override void Refresh()
		{
			base.IsLoadingOver = false;
			base.IsLoadingOver = true;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00044CEC File Offset: 0x00042EEC
		public override string GetName()
		{
			return this._concept.Title.ToString();
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x00044CFE File Offset: 0x00042EFE
		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00044D10 File Offset: 0x00042F10
		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Concept", GameTexts.FindText("str_encyclopedia_concepts", null).ToString()) + " \\ " + this.GetName();
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x00044D78 File Offset: 0x00042F78
		public override void ExecuteSwitchBookmarkedState()
		{
			base.ExecuteSwitchBookmarkedState();
			if (base.IsBookmarked)
			{
				Campaign.Current.EncyclopediaManager.ViewDataTracker.AddEncyclopediaBookmarkToItem(this._concept);
				return;
			}
			Campaign.Current.EncyclopediaManager.ViewDataTracker.RemoveEncyclopediaBookmarkFromItem(this._concept);
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x0600116B RID: 4459 RVA: 0x00044DC8 File Offset: 0x00042FC8
		// (set) Token: 0x0600116C RID: 4460 RVA: 0x00044DD0 File Offset: 0x00042FD0
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x0600116D RID: 4461 RVA: 0x00044DF3 File Offset: 0x00042FF3
		// (set) Token: 0x0600116E RID: 4462 RVA: 0x00044DFB File Offset: 0x00042FFB
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x0400081B RID: 2075
		private Concept _concept;

		// Token: 0x0400081C RID: 2076
		private string _titleText;

		// Token: 0x0400081D RID: 2077
		private string _descriptionText;
	}
}
