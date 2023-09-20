using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000096 RID: 150
	[Flags]
	public enum EquipmentFlags : uint
	{
		// Token: 0x04000485 RID: 1157
		None = 0U,
		// Token: 0x04000486 RID: 1158
		IsWandererEquipment = 1U,
		// Token: 0x04000487 RID: 1159
		IsGentryEquipment = 2U,
		// Token: 0x04000488 RID: 1160
		IsRebelHeroEquipment = 4U,
		// Token: 0x04000489 RID: 1161
		IsNoncombatantTemplate = 8U,
		// Token: 0x0400048A RID: 1162
		IsCombatantTemplate = 16U,
		// Token: 0x0400048B RID: 1163
		IsCivilianTemplate = 32U,
		// Token: 0x0400048C RID: 1164
		IsNobleTemplate = 64U,
		// Token: 0x0400048D RID: 1165
		IsFemaleTemplate = 128U,
		// Token: 0x0400048E RID: 1166
		IsMediumTemplate = 256U,
		// Token: 0x0400048F RID: 1167
		IsHeavyTemplate = 512U,
		// Token: 0x04000490 RID: 1168
		IsFlamboyantTemplate = 1024U,
		// Token: 0x04000491 RID: 1169
		IsStoicTemplate = 2048U,
		// Token: 0x04000492 RID: 1170
		IsNomadTemplate = 4096U,
		// Token: 0x04000493 RID: 1171
		IsWoodlandTemplate = 8192U,
		// Token: 0x04000494 RID: 1172
		IsChildEquipmentTemplate = 16384U,
		// Token: 0x04000495 RID: 1173
		IsTeenagerEquipmentTemplate = 32768U
	}
}
