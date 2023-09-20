using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200008E RID: 142
	public class InquiryElement
	{
		// Token: 0x060007CE RID: 1998 RVA: 0x0001AF19 File Offset: 0x00019119
		public InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.ImageIdentifier = imageIdentifier;
			this.IsEnabled = true;
			this.Hint = null;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0001AF44 File Offset: 0x00019144
		public InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier, bool isEnabled, string hint)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.ImageIdentifier = imageIdentifier;
			this.IsEnabled = isEnabled;
			this.Hint = hint;
		}

		// Token: 0x040003F9 RID: 1017
		public readonly string Title;

		// Token: 0x040003FA RID: 1018
		public readonly ImageIdentifier ImageIdentifier;

		// Token: 0x040003FB RID: 1019
		public readonly object Identifier;

		// Token: 0x040003FC RID: 1020
		public readonly bool IsEnabled;

		// Token: 0x040003FD RID: 1021
		public readonly string Hint;
	}
}
