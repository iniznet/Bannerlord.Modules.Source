using System;

namespace TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes
{
	public class SigilCosmeticElement : CosmeticElement
	{
		public SigilCosmeticElement(string id, CosmeticsManager.CosmeticRarity rarity, int cost, string bannerCode)
			: base(id, rarity, cost, CosmeticsManager.CosmeticType.Sigil)
		{
			this.BannerCode = bannerCode;
		}

		public string BannerCode;
	}
}
