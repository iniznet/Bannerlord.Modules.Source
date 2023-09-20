using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	// Token: 0x020000B2 RID: 178
	public class EncyclopediaContentPageVM : EncyclopediaPageVM
	{
		// Token: 0x0600116F RID: 4463 RVA: 0x00044E1E File Offset: 0x0004301E
		public EncyclopediaContentPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00044E49 File Offset: 0x00043049
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PreviousButtonLabel = this._previousButtonLabelText.ToString();
			this.NextButtonLabel = this._nextButtonLabelText.ToString();
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00044E74 File Offset: 0x00043074
		public void InitializeQuickNavigation(EncyclopediaListVM list)
		{
			if (list != null && list.Items != null)
			{
				List<EncyclopediaListItemVM> list2 = list.Items.Where((EncyclopediaListItemVM x) => !x.IsFiltered).ToList<EncyclopediaListItemVM>();
				int count = list2.Count;
				int num = list2.FindIndex((EncyclopediaListItemVM x) => x.Object == base.Obj);
				if (count > 1 && num > -1)
				{
					if (num > 0)
					{
						this._previousItem = list2[num - 1];
						this.PreviousButtonHint = new HintViewModel(new TextObject(this._previousItem.Name, null), null);
						this.IsPreviousButtonEnabled = true;
					}
					if (num < count - 1)
					{
						this._nextItem = list2[num + 1];
						this.NextButtonHint = new HintViewModel(new TextObject(this._nextItem.Name, null), null);
						this.IsNextButtonEnabled = true;
					}
				}
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00044F54 File Offset: 0x00043154
		public void ExecuteGoToNextItem()
		{
			if (this._nextItem != null)
			{
				this._nextItem.Execute();
				return;
			}
			Debug.FailedAssert("If the next button is enabled then next item should not be null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Encyclopedia\\Pages\\EncyclopediaContentPageVM.cs", "ExecuteGoToNextItem", 66);
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x00044F80 File Offset: 0x00043180
		public void ExecuteGoToPreviousItem()
		{
			if (this._previousItem != null)
			{
				this._previousItem.Execute();
				return;
			}
			Debug.FailedAssert("If the previous button is enabled then previous item should not be null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Encyclopedia\\Pages\\EncyclopediaContentPageVM.cs", "ExecuteGoToPreviousItem", 78);
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06001174 RID: 4468 RVA: 0x00044FAC File Offset: 0x000431AC
		// (set) Token: 0x06001175 RID: 4469 RVA: 0x00044FB4 File Offset: 0x000431B4
		[DataSourceProperty]
		public bool IsPreviousButtonEnabled
		{
			get
			{
				return this._isPreviousButtonEnabled;
			}
			set
			{
				if (value != this._isPreviousButtonEnabled)
				{
					this._isPreviousButtonEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPreviousButtonEnabled");
				}
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001176 RID: 4470 RVA: 0x00044FD2 File Offset: 0x000431D2
		// (set) Token: 0x06001177 RID: 4471 RVA: 0x00044FDA File Offset: 0x000431DA
		[DataSourceProperty]
		public bool IsNextButtonEnabled
		{
			get
			{
				return this._isNextButtonEnabled;
			}
			set
			{
				if (value != this._isNextButtonEnabled)
				{
					this._isNextButtonEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsNextButtonEnabled");
				}
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x00044FF8 File Offset: 0x000431F8
		// (set) Token: 0x06001179 RID: 4473 RVA: 0x00045000 File Offset: 0x00043200
		[DataSourceProperty]
		public string PreviousButtonLabel
		{
			get
			{
				return this._previousButtonLabel;
			}
			set
			{
				if (value != this._previousButtonLabel)
				{
					this._previousButtonLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviousButtonLabel");
				}
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x0600117A RID: 4474 RVA: 0x00045023 File Offset: 0x00043223
		// (set) Token: 0x0600117B RID: 4475 RVA: 0x0004502B File Offset: 0x0004322B
		[DataSourceProperty]
		public string NextButtonLabel
		{
			get
			{
				return this._nextButtonLabel;
			}
			set
			{
				if (value != this._nextButtonLabel)
				{
					this._nextButtonLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "NextButtonLabel");
				}
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x0600117C RID: 4476 RVA: 0x0004504E File Offset: 0x0004324E
		// (set) Token: 0x0600117D RID: 4477 RVA: 0x00045056 File Offset: 0x00043256
		[DataSourceProperty]
		public HintViewModel PreviousButtonHint
		{
			get
			{
				return this._previousButtonHint;
			}
			set
			{
				if (value != this._previousButtonHint)
				{
					this._previousButtonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PreviousButtonHint");
				}
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x0600117E RID: 4478 RVA: 0x00045074 File Offset: 0x00043274
		// (set) Token: 0x0600117F RID: 4479 RVA: 0x0004507C File Offset: 0x0004327C
		[DataSourceProperty]
		public HintViewModel NextButtonHint
		{
			get
			{
				return this._nextButtonHint;
			}
			set
			{
				if (value != this._nextButtonHint)
				{
					this._nextButtonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NextButtonHint");
				}
			}
		}

		// Token: 0x0400081E RID: 2078
		private EncyclopediaListItemVM _previousItem;

		// Token: 0x0400081F RID: 2079
		private EncyclopediaListItemVM _nextItem;

		// Token: 0x04000820 RID: 2080
		private TextObject _previousButtonLabelText = new TextObject("{=zlcMGAbn}Previous Page", null);

		// Token: 0x04000821 RID: 2081
		private TextObject _nextButtonLabelText = new TextObject("{=QFfMd5q3}Next Page", null);

		// Token: 0x04000822 RID: 2082
		private bool _isPreviousButtonEnabled;

		// Token: 0x04000823 RID: 2083
		private bool _isNextButtonEnabled;

		// Token: 0x04000824 RID: 2084
		private string _previousButtonLabel;

		// Token: 0x04000825 RID: 2085
		private string _nextButtonLabel;

		// Token: 0x04000826 RID: 2086
		private HintViewModel _previousButtonHint;

		// Token: 0x04000827 RID: 2087
		private HintViewModel _nextButtonHint;
	}
}
