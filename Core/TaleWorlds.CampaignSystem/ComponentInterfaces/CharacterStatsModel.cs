using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200016B RID: 363
	public abstract class CharacterStatsModel : GameModel
	{
		// Token: 0x060018C2 RID: 6338
		public abstract ExplainedNumber MaxHitpoints(CharacterObject character, bool includeDescriptions = false);

		// Token: 0x060018C3 RID: 6339
		public abstract int GetTier(CharacterObject character);

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x060018C4 RID: 6340
		public abstract int MaxCharacterTier { get; }
	}
}
