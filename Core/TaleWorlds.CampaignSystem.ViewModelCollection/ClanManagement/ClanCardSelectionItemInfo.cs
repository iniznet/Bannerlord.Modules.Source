using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000FA RID: 250
	public readonly struct ClanCardSelectionItemInfo
	{
		// Token: 0x0600175C RID: 5980 RVA: 0x0005659C File Offset: 0x0005479C
		public ClanCardSelectionItemInfo(object identifier, TextObject title, ImageIdentifier image, CardSelectionItemSpriteType spriteType, string spriteName, string spriteLabel, IEnumerable<ClanCardSelectionItemPropertyInfo> properties, bool isDisabled, TextObject disabledReason, TextObject actionResult)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.Image = image;
			this.SpriteType = spriteType;
			this.SpriteName = spriteName;
			this.SpriteLabel = spriteLabel;
			this.Properties = properties;
			this.IsSpecialActionItem = false;
			this.SpecialActionText = null;
			this.IsDisabled = isDisabled;
			this.DisabledReason = disabledReason;
			this.ActionResult = actionResult;
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x00056624 File Offset: 0x00054824
		public ClanCardSelectionItemInfo(TextObject specialActionText, bool isDisabled, TextObject disabledReason, TextObject actionResult)
		{
			this.Identifier = null;
			this.Title = null;
			this.Image = null;
			this.SpriteType = CardSelectionItemSpriteType.None;
			this.SpriteName = null;
			this.SpriteLabel = null;
			this.Properties = null;
			this.IsSpecialActionItem = true;
			this.SpecialActionText = specialActionText;
			this.IsDisabled = isDisabled;
			this.DisabledReason = disabledReason;
			this.ActionResult = actionResult;
		}

		// Token: 0x04000AF9 RID: 2809
		public readonly object Identifier;

		// Token: 0x04000AFA RID: 2810
		public readonly TextObject Title;

		// Token: 0x04000AFB RID: 2811
		public readonly ImageIdentifier Image;

		// Token: 0x04000AFC RID: 2812
		public readonly CardSelectionItemSpriteType SpriteType;

		// Token: 0x04000AFD RID: 2813
		public readonly string SpriteName;

		// Token: 0x04000AFE RID: 2814
		public readonly string SpriteLabel;

		// Token: 0x04000AFF RID: 2815
		public readonly IEnumerable<ClanCardSelectionItemPropertyInfo> Properties;

		// Token: 0x04000B00 RID: 2816
		public readonly bool IsSpecialActionItem;

		// Token: 0x04000B01 RID: 2817
		public readonly TextObject SpecialActionText;

		// Token: 0x04000B02 RID: 2818
		public readonly bool IsDisabled;

		// Token: 0x04000B03 RID: 2819
		public readonly TextObject DisabledReason;

		// Token: 0x04000B04 RID: 2820
		public readonly TextObject ActionResult;
	}
}
