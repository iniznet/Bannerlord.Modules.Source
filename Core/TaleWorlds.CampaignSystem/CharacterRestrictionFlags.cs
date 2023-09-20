using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000075 RID: 117
	[Flags]
	public enum CharacterRestrictionFlags : uint
	{
		// Token: 0x040004BF RID: 1215
		None = 0U,
		// Token: 0x040004C0 RID: 1216
		NotTransferableInPartyScreen = 1U,
		// Token: 0x040004C1 RID: 1217
		CanNotGoInHideout = 2U
	}
}
