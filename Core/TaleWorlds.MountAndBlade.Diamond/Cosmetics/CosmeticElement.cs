using System;

namespace TaleWorlds.MountAndBlade.Diamond.Cosmetics
{
	public class CosmeticElement
	{
		public bool IsFree
		{
			get
			{
				return this.Cost <= 0;
			}
		}

		public CosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, CosmeticsManager.CosmeticType type)
		{
			this.UsageIndex = -1;
			this.Id = id;
			this.Rarity = rarity;
			this.Cost = cost;
			this.Type = type;
		}

		public int UsageIndex;

		public string Id;

		public CosmeticsManager.CosmeticRarity Rarity;

		public int Cost;

		public CosmeticsManager.CosmeticType Type;
	}
}
