using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D2 RID: 210
	public class EducationReviewVM : ViewModel
	{
		// Token: 0x06001381 RID: 4993 RVA: 0x0004AC90 File Offset: 0x00048E90
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

		// Token: 0x06001382 RID: 4994 RVA: 0x0004AD00 File Offset: 0x00048F00
		public override void RefreshValues()
		{
			for (int i = 0; i < this.ReviewList.Count; i++)
			{
				this._educationPageTitle.SetTextVariable("NUMBER", i + 1);
				this.ReviewList[i].Title = this._educationPageTitle.ToString();
			}
			this.StageCompleteText = this._stageCompleteTextObject.ToString();
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x0004AD64 File Offset: 0x00048F64
		public void SetGainForStage(int pageIndex, string gainText)
		{
			if (pageIndex >= 0 && pageIndex < this._pageCount)
			{
				this.ReviewList[pageIndex].UpdateWith(gainText);
			}
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0004AD85 File Offset: 0x00048F85
		public void SetCurrentPage(int currentPageIndex)
		{
			this.IsEnabled = currentPageIndex == this._pageCount - 1;
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x0004AD98 File Offset: 0x00048F98
		// (set) Token: 0x06001386 RID: 4998 RVA: 0x0004ADA0 File Offset: 0x00048FA0
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

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001387 RID: 4999 RVA: 0x0004ADBE File Offset: 0x00048FBE
		// (set) Token: 0x06001388 RID: 5000 RVA: 0x0004ADC6 File Offset: 0x00048FC6
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

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06001389 RID: 5001 RVA: 0x0004ADE9 File Offset: 0x00048FE9
		// (set) Token: 0x0600138A RID: 5002 RVA: 0x0004ADF1 File Offset: 0x00048FF1
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

		// Token: 0x04000908 RID: 2312
		private readonly int _pageCount;

		// Token: 0x04000909 RID: 2313
		private readonly TextObject _educationPageTitle = new TextObject("{=m1Yynagz}Page {NUMBER}", null);

		// Token: 0x0400090A RID: 2314
		private readonly TextObject _stageCompleteTextObject = new TextObject("{=flxDkoMh}Stage Complete", null);

		// Token: 0x0400090B RID: 2315
		private MBBindingList<EducationReviewItemVM> _reviewList;

		// Token: 0x0400090C RID: 2316
		private bool _isEnabled;

		// Token: 0x0400090D RID: 2317
		private string _stageCompleteText;
	}
}
