using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000119 RID: 281
	[Flags]
	internal enum InventoryItemType
	{
		// Token: 0x04000287 RID: 647
		None = 0,
		// Token: 0x04000288 RID: 648
		Weapon = 1,
		// Token: 0x04000289 RID: 649
		Shield = 2,
		// Token: 0x0400028A RID: 650
		HeadArmor = 4,
		// Token: 0x0400028B RID: 651
		BodyArmor = 8,
		// Token: 0x0400028C RID: 652
		LegArmor = 16,
		// Token: 0x0400028D RID: 653
		HandArmor = 32,
		// Token: 0x0400028E RID: 654
		Horse = 64,
		// Token: 0x0400028F RID: 655
		HorseHarness = 128,
		// Token: 0x04000290 RID: 656
		Goods = 256,
		// Token: 0x04000291 RID: 657
		Book = 512,
		// Token: 0x04000292 RID: 658
		Animal = 1024,
		// Token: 0x04000293 RID: 659
		Cape = 2048,
		// Token: 0x04000294 RID: 660
		HorseCategory = 192,
		// Token: 0x04000295 RID: 661
		Armors = 2108,
		// Token: 0x04000296 RID: 662
		Equipable = 2303,
		// Token: 0x04000297 RID: 663
		All = 4095
	}
}
