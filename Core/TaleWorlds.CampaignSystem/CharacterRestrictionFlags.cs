using System;

namespace TaleWorlds.CampaignSystem
{
	[Flags]
	public enum CharacterRestrictionFlags : uint
	{
		None = 0U,
		NotTransferableInPartyScreen = 1U,
		CanNotGoInHideout = 2U
	}
}
