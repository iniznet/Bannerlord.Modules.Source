using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200039B RID: 923
	public interface ICraftingCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x17000CCD RID: 3277
		// (get) Token: 0x06003720 RID: 14112
		IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> CraftingOrders { get; }

		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x06003721 RID: 14113
		IReadOnlyCollection<WeaponDesign> CraftingHistory { get; }

		// Token: 0x06003722 RID: 14114
		void CompleteOrder(Town town, CraftingOrder craftingOrder, ItemObject craftedItem, Hero completerHero);

		// Token: 0x06003723 RID: 14115
		void GetOrderResult(CraftingOrder craftingOrder, ItemObject craftedItem, out bool isSucceed, out TextObject orderRemark, out TextObject orderResult, out int finalPrice);

		// Token: 0x06003724 RID: 14116
		int GetCraftingDifficulty(WeaponDesign weaponDesign);

		// Token: 0x06003725 RID: 14117
		bool CanHeroUsePart(Hero hero, CraftingPiece craftingPiece);

		// Token: 0x06003726 RID: 14118
		int GetHeroCraftingStamina(Hero hero);

		// Token: 0x06003727 RID: 14119
		void SetHeroCraftingStamina(Hero hero, int value);

		// Token: 0x06003728 RID: 14120
		int GetMaxHeroCraftingStamina(Hero hero);

		// Token: 0x06003729 RID: 14121
		void DoRefinement(Hero hero, Crafting.RefiningFormula refineFormula);

		// Token: 0x0600372A RID: 14122
		void DoSmelting(Hero currentCraftingHero, EquipmentElement equipmentElement);

		// Token: 0x0600372B RID: 14123
		ItemObject CreateCraftedWeaponInFreeBuildMode(Hero hero, WeaponDesign currentWeaponDesign, int modifierTier, Crafting.OverrideData overrideData);

		// Token: 0x0600372C RID: 14124
		ItemObject CreateCraftedWeaponInCraftingOrderMode(Hero crafterHero, CraftingOrder craftingOrder, WeaponDesign weaponDesign, int modifierTier, Crafting.OverrideData overrideData);

		// Token: 0x0600372D RID: 14125
		bool IsOpened(CraftingPiece craftingPiece, CraftingTemplate craftingTemplate);

		// Token: 0x0600372E RID: 14126
		void InitializeCraftingElements();
	}
}
