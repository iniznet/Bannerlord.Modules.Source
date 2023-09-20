using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public class MultiSelectionInquiryData
	{
		public MultiSelectionInquiryData(string titleText, string descriptionText, List<InquiryElement> inquiryElements, bool isExitShown, int minSelectableOptionCount, int maxSelectableOptionCount, string affirmativeText, string negativeText, Action<List<InquiryElement>> affirmativeAction, Action<List<InquiryElement>> negativeAction, string soundEventPath = "")
		{
			this.TitleText = titleText;
			this.DescriptionText = descriptionText;
			this.InquiryElements = inquiryElements;
			this.IsExitShown = isExitShown;
			this.AffirmativeText = affirmativeText;
			this.NegativeText = negativeText;
			this.AffirmativeAction = affirmativeAction;
			this.NegativeAction = negativeAction;
			this.MinSelectableOptionCount = minSelectableOptionCount;
			this.MaxSelectableOptionCount = maxSelectableOptionCount;
			this.SoundEventPath = soundEventPath;
		}

		public bool HasSameContentWith(object other)
		{
			MultiSelectionInquiryData multiSelectionInquiryData;
			if ((multiSelectionInquiryData = other as MultiSelectionInquiryData) != null)
			{
				bool flag = true;
				if (this.InquiryElements.Count == multiSelectionInquiryData.InquiryElements.Count)
				{
					for (int i = 0; i < this.InquiryElements.Count; i++)
					{
						if (!this.InquiryElements[i].HasSameContentWith(multiSelectionInquiryData.InquiryElements[i]))
						{
							flag = false;
						}
					}
				}
				else
				{
					flag = false;
				}
				return this.TitleText == multiSelectionInquiryData.TitleText && this.DescriptionText == multiSelectionInquiryData.DescriptionText && flag && this.IsExitShown == multiSelectionInquiryData.IsExitShown && this.AffirmativeText == multiSelectionInquiryData.AffirmativeText && this.NegativeText == multiSelectionInquiryData.NegativeText && this.AffirmativeAction == multiSelectionInquiryData.AffirmativeAction && this.NegativeAction == multiSelectionInquiryData.NegativeAction && this.MinSelectableOptionCount == multiSelectionInquiryData.MinSelectableOptionCount && this.MaxSelectableOptionCount == multiSelectionInquiryData.MaxSelectableOptionCount && this.SoundEventPath == multiSelectionInquiryData.SoundEventPath;
			}
			return false;
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
