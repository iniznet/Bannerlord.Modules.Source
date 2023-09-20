using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	[EncyclopediaViewModel(typeof(Concept))]
	public class EncyclopediaConceptPageVM : EncyclopediaContentPageVM
	{
		public EncyclopediaConceptPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this._concept = base.Obj as Concept;
			Concept.SetConceptTextLinks();
			base.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(this._concept);
			this.RefreshValues();
			this.Refresh();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = this._concept.Title.ToString();
			this.DescriptionText = this._concept.Description.ToString();
			base.UpdateBookmarkHintText();
		}

		public override void Refresh()
		{
			base.IsLoadingOver = false;
			base.IsLoadingOver = true;
		}

		public override string GetName()
		{
			return this._concept.Title.ToString();
		}

		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		public override string GetNavigationBarURL()
		{
			return HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ " + HyperlinkTexts.GetGenericHyperlinkText("ListPage-Concept", GameTexts.FindText("str_encyclopedia_concepts", null).ToString()) + " \\ " + this.GetName();
		}

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

		private Concept _concept;

		private string _titleText;

		private string _descriptionText;
	}
}
