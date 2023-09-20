using System;

namespace TaleWorlds.CampaignSystem.Inventory
{
	[Flags]
	public enum InventoryItemType
	{
		None = 0,
		Weapon = 1,
		Shield = 2,
		HeadArmor = 4,
		BodyArmor = 8,
		LegArmor = 16,
		HandArmor = 32,
		Horse = 64,
		HorseHarness = 128,
		Goods = 256,
		Book = 512,
		Animal = 1024,
		Cape = 2048,
		Banner = 4096,
		HorseCategory = 192,
		Armors = 2108,
		Equipable = 6399,
		All = 4095
	}
}
