using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017D RID: 381
	public abstract class SmithingModel : GameModel
	{
		// Token: 0x0600193D RID: 6461
		public abstract int GetCraftingPartDifficulty(CraftingPiece craftingPiece);

		// Token: 0x0600193E RID: 6462
		public abstract int CalculateWeaponDesignDifficulty(WeaponDesign weaponDesign);

		// Token: 0x0600193F RID: 6463
		public abstract int GetModifierTierForSmithedWeapon(WeaponDesign weaponDesign, Hero weaponsmith);

		// Token: 0x06001940 RID: 6464
		public abstract Crafting.OverrideData GetModifierChanges(int modifierTier, Hero hero, WeaponComponentData weapon);

		// Token: 0x06001941 RID: 6465
		public abstract IEnumerable<Crafting.RefiningFormula> GetRefiningFormulas(Hero weaponsmith);

		// Token: 0x06001942 RID: 6466
		public abstract ItemObject GetCraftingMaterialItem(CraftingMaterials craftingMaterial);

		// Token: 0x06001943 RID: 6467
		public abstract int[] GetSmeltingOutputForItem(ItemObject item);

		// Token: 0x06001944 RID: 6468
		public abstract int GetSkillXpForRefining(ref Crafting.RefiningFormula refineFormula);

		// Token: 0x06001945 RID: 6469
		public abstract int GetSkillXpForSmelting(ItemObject item);

		// Token: 0x06001946 RID: 6470
		public abstract int GetSkillXpForSmithingInFreeBuildMode(ItemObject item);

		// Token: 0x06001947 RID: 6471
		public abstract int GetSkillXpForSmithingInCraftingOrderMode(ItemObject item);

		// Token: 0x06001948 RID: 6472
		public abstract int[] GetSmithingCostsForWeaponDesign(WeaponDesign weaponDesign);

		// Token: 0x06001949 RID: 6473
		public abstract int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero);

		// Token: 0x0600194A RID: 6474
		public abstract int GetEnergyCostForSmithing(ItemObject item, Hero hero);

		// Token: 0x0600194B RID: 6475
		public abstract int GetEnergyCostForSmelting(ItemObject item, Hero hero);

		// Token: 0x0600194C RID: 6476
		public abstract float ResearchPointsNeedForNewPart(int totalPartCount, int openedPartCount);

		// Token: 0x0600194D RID: 6477
		public abstract int GetPartResearchGainForSmeltingItem(ItemObject item, Hero hero);

		// Token: 0x0600194E RID: 6478
		public abstract int GetPartResearchGainForSmithingItem(ItemObject item, Hero hero, bool isFreeBuildMode);
	}
}
