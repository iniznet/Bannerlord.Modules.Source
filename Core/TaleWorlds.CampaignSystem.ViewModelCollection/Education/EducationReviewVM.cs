using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationReviewVM : ViewModel
	{
		public EducationReviewVM(int pageCount)
		{
			this._pageCount = pageCount;
			this.ReviewList = new MBBindingList<EducationReviewItemVM>();
			for (int i = 0; i < this._pageCount - 1; i++)
			{
				this.ReviewList.Add(new EducationReviewItemVM());
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			for (int i = 0; i < this.ReviewList.Count; i++)
			{
				this._educationPageTitle.SetTextVariable("NUMBER", i + 1);
				this.ReviewList[i].Title = this._educationPageTitle.ToString();
			}
			this.StageCompleteText = this._stageCompleteTextObject.ToString();
		}

		public void SetGainForStage(int pageIndex, string gainText)
		{
			if (pageIndex >= 0 && pageIndex < this._pageCount)
			{
				this.ReviewList[pageIndex].UpdateWith(gainText);
			}
		}

		public void SetCurrentPage(int currentPageIndex)
		{
			this.IsEnabled = currentPageIndex == this._pageCount - 1;
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string StageCompleteText
		{
			get
			{
				return this._stageCompleteText;
			}
			set
			{
				if (value != this._stageCompleteText)
				{
					this._stageCompleteText = value;
					base.OnPropertyChangedWithValue<string>(value, "StageCompleteText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EducationReviewItemVM> ReviewList
		{
			get
			{
				return this._reviewList;
			}
			set
			{
				if (value != this._reviewList)
				{
					this._reviewList = value;
					base.OnPropertyChangedWithValue<MBBindingList<EducationReviewItemVM>>(value, "ReviewList");
				}
			}
		}

		private readonly int _pageCount;

		private readonly TextObject _educationPageTitle = new TextObject("{=m1Yynagz}Page {NUMBER}", null);

		private readonly TextObject _stageCompleteTextObject = new TextObject("{=flxDkoMh}Stage Complete", null);

		private MBBindingList<EducationReviewItemVM> _reviewList;

		private bool _isEnabled;

		private string _stageCompleteText;
	}
}
