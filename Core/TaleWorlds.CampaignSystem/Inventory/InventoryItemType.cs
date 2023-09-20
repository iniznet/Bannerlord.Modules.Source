using System;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000DA RID: 218
	[Flags]
	public enum InventoryItemType
	{
		// Token: 0x040006C2 RID: 1730
		None = 0,
		// Token: 0x040006C3 RID: 1731
		Weapon = 1,
		// Token: 0x040006C4 RID: 1732
		Shield = 2,
		// Token: 0x040006C5 RID: 1733
		HeadArmor = 4,
		// Token: 0x040006C6 RID: 1734
		BodyArmor = 8,
		// Token: 0x040006C7 RID: 1735
		LegArmor = 16,
		// Token: 0x040006C8 RID: 1736
		HandArmor = 32,
		// Token: 0x040006C9 RID: 1737
		Horse = 64,
		// Token: 0x040006CA RID: 1738
		HorseHarness = 128,
		// Token: 0x040006CB RID: 1739
		Goods = 256,
		// Token: 0x040006CC RID: 1740
		Book = 512,
		// Token: 0x040006CD RID: 1741
		Animal = 1024,
		// Token: 0x040006CE RID: 1742
		Cape = 2048,
		// Token: 0x040006CF RID: 1743
		Banner = 4096,
		// Token: 0x040006D0 RID: 1744
		HorseCategory = 192,
		// Token: 0x040006D1 RID: 1745
		Armors = 2108,
		// Token: 0x040006D2 RID: 1746
		Equipable = 6399,
		// Token: 0x040006D3 RID: 1747
		All = 4095
	}
}
