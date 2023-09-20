using System;

namespace TaleWorlds.Core
{
	public class InquiryElement
	{
		public InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.ImageIdentifier = imageIdentifier;
			this.IsEnabled = true;
			this.Hint = null;
		}

		public InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier, bool isEnabled, string hint)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.ImageIdentifier = imageIdentifier;
			this.IsEnabled = isEnabled;
			this.Hint = hint;
		}

		public bool HasSameContentWith(object other)
		{
			InquiryElement inquiryElement;
			return (inquiryElement = other as InquiryElement) != null && (this.Title == inquiryElement.Title && this.ImageIdentifier.Equals(inquiryElement.ImageIdentifier) && this.Identifier == inquiryElement.Identifier && this.IsEnabled == inquiryElement.IsEnabled) && this.Hint == inquiryElement.Hint;
		}

		public readonly string Title;

		public readonly ImageIdentifier ImageIdentifier;

		public readonly object Identifier;

		public readonly bool IsEnabled;

		public readonly string Hint;
	}
}
