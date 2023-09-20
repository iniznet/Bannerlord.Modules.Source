using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000042 RID: 66
	public struct CraftingStatData
	{
		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x000138D1 File Offset: 0x00011AD1
		public bool IsValid
		{
			get
			{
				return this.MaxValue >= 0f;
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x000138E3 File Offset: 0x00011AE3
		public CraftingStatData(TextObject descriptionText, float curValue, float maxValue, CraftingTemplate.CraftingStatTypes type, DamageTypes damageType = DamageTypes.Invalid)
		{
			this.DescriptionText = descriptionText;
			this.CurValue = curValue;
			this.MaxValue = maxValue;
			this.Type = type;
			this.DamageType = damageType;
		}

		// Token: 0x0400026C RID: 620
		public readonly TextObject DescriptionText;

		// Token: 0x0400026D RID: 621
		public readonly float CurValue;

		// Token: 0x0400026E RID: 622
		public readonly float MaxValue;

		// Token: 0x0400026F RID: 623
		public readonly CraftingTemplate.CraftingStatTypes Type;

		// Token: 0x04000270 RID: 624
		public readonly DamageTypes DamageType;
	}
}
