using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes
{
	public class ClothingCosmeticElement : CosmeticElement
	{
		public ClothingCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, List<string> replaceItemsId, List<Tuple<string, string>> replaceItemless)
			: base(id, rarity, cost, CosmeticsManager.CosmeticType.Clothing)
		{
			this.ReplaceItemsId = replaceItemsId;
			this.ReplaceItemless = replaceItemless;
		}

		public readonly List<string> ReplaceItemsId;

		public readonly List<Tuple<string, string>> ReplaceItemless;
	}
}
