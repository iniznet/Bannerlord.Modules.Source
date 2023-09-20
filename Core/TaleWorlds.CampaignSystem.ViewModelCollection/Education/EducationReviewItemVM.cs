using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D3 RID: 211
	public class EducationReviewItemVM : ViewModel
	{
		// Token: 0x0600138C RID: 5004 RVA: 0x0004AE17 File Offset: 0x00049017
		public void UpdateWith(string gainText)
		{
			this.GainText = gainText;
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x0600138D RID: 5005 RVA: 0x0004AE20 File Offset: 0x00049020
		// (set) Token: 0x0600138E RID: 5006 RVA: 0x0004AE28 File Offset: 0x00049028
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x0600138F RID: 5007 RVA: 0x0004AE4B File Offset: 0x0004904B
		// (set) Token: 0x06001390 RID: 5008 RVA: 0x0004AE53 File Offset: 0x00049053
		[DataSourceProperty]
		public string GainText
		{
			get
			{
				return this._gainText;
			}
			set
			{
				if (value != this._gainText)
				{
					this._gainText = value;
					base.OnPropertyChangedWithValue<string>(value, "GainText");
				}
			}
		}

		// Token: 0x0400090E RID: 2318
		private string _title;

		// Token: 0x0400090F RID: 2319
		private string _gainText;
	}
}
