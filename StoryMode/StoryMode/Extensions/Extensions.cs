using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.Extensions
{
	// Token: 0x02000053 RID: 83
	public static class Extensions
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x0001BCD5 File Offset: 0x00019ED5
		public static bool IsTrainingField(this Settlement settlement)
		{
			return settlement.SettlementComponent is TrainingField;
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001BCE5 File Offset: 0x00019EE5
		public static TrainingField TrainingField(this Settlement settlement)
		{
			return settlement.SettlementComponent as TrainingField;
		}
	}
}
