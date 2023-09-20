using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x020000B6 RID: 182
	public class MultiSelectionInquiryData
	{
		// Token: 0x06000940 RID: 2368 RVA: 0x0001ED4C File Offset: 0x0001CF4C
		public MultiSelectionInquiryData(string titleText, string descriptionText, List<InquiryElement> inquiryElements, bool isExitShown, int maxSelectableOptionCount, string affirmativeText, string negativeText, Action<List<InquiryElement>> affirmativeAction, Action<List<InquiryElement>> negativeAction, string soundEventPath = "")
		{
			this.TitleText = titleText;
			this.DescriptionText = descriptionText;
			this.InquiryElements = inquiryElements;
			this.IsExitShown = isExitShown;
			this.AffirmativeText = affirmativeText;
			this.NegativeText = negativeText;
			this.AffirmativeAction = affirmativeAction;
			this.NegativeAction = negativeAction;
			this.MaxSelectableOptionCount = maxSelectableOptionCount;
			this.SoundEventPath = soundEventPath;
		}

		// Token: 0x04000552 RID: 1362
		public readonly string TitleText;

		// Token: 0x04000553 RID: 1363
		public readonly string DescriptionText;

		// Token: 0x04000554 RID: 1364
		public readonly List<InquiryElement> InquiryElements;

		// Token: 0x04000555 RID: 1365
		public readonly bool IsExitShown;

		// Token: 0x04000556 RID: 1366
		public readonly int MaxSelectableOptionCount;

		// Token: 0x04000557 RID: 1367
		public readonly int MinSelectableOptionCount;

		// Token: 0x04000558 RID: 1368
		public readonly string SoundEventPath;

		// Token: 0x04000559 RID: 1369
		public readonly string AffirmativeText;

		// Token: 0x0400055A RID: 1370
		public readonly string NegativeText;

		// Token: 0x0400055B RID: 1371
		public readonly Action<List<InquiryElement>> AffirmativeAction;

		// Token: 0x0400055C RID: 1372
		public readonly Action<List<InquiryElement>> NegativeAction;
	}
}
