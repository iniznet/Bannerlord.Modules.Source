using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018C RID: 396
	public abstract class RomanceModel : GameModel
	{
		// Token: 0x060019CE RID: 6606
		public abstract int GetAttractionValuePercentage(Hero potentiallyInterestedCharacter, Hero heroOfInterest);
	}
}
