using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public class MultiSelectionInquiryData
	{
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

		public readonly string TitleText;

		public readonly string DescriptionText;

		public readonly List<InquiryElement> InquiryElements;

		public readonly bool IsExitShown;

		public readonly int MaxSelectableOptionCount;

		public readonly int MinSelectableOptionCount;

		public readonly string SoundEventPath;

		public readonly string AffirmativeText;

		public readonly string NegativeText;

		public readonly Action<List<InquiryElement>> AffirmativeAction;

		public readonly Action<List<InquiryElement>> NegativeAction;
	}
}
