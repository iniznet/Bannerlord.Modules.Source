using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017E RID: 382
	public abstract class GenericXpModel : GameModel
	{
		// Token: 0x06001950 RID: 6480
		public abstract float GetXpMultiplier(Hero hero);
	}
}
