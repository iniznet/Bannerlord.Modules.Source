using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AB RID: 427
	public abstract class HeirSelectionCalculationModel : GameModel
	{
		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06001AB8 RID: 6840
		public abstract int HighestSkillPoint { get; }

		// Token: 0x06001AB9 RID: 6841
		public abstract int CalculateHeirSelectionPoint(Hero candidateHeir, Hero deadHero, ref Hero maxSkillHero);
	}
}
