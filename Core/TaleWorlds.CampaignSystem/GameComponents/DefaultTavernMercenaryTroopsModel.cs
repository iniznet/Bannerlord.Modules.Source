using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000144 RID: 324
	public class DefaultTavernMercenaryTroopsModel : TavernMercenaryTroopsModel
	{
		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x060017E5 RID: 6117 RVA: 0x00078604 File Offset: 0x00076804
		public override float RegularMercenariesSpawnChance
		{
			get
			{
				return 0.7f;
			}
		}
	}
}
