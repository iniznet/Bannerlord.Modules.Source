using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000185 RID: 389
	public abstract class EmissaryModel : GameModel
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x060019A7 RID: 6567
		public abstract int EmissaryRelationBonusForMainClan { get; }

		// Token: 0x060019A8 RID: 6568
		public abstract bool IsEmissary(Hero hero);
	}
}
