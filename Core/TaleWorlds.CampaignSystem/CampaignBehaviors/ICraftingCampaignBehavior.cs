using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public interface ICraftingCampaignBehavior : ICampaignBehavior
	{
		IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> CraftingOrders { get; }

		IReadOnlyCollection<WeaponDesign> CraftingHistory { get; }

		void CompleteOrder(Town town, CraftingOrder craftingOrder, ItemObject craftedItem, Hero completerHero);

		void GetOrderResult(CraftingOrder craftingOrder, ItemObject craftedItem, out bool isSucceed, out TextObject orderRemark, out TextObject orderResult, out int finalPrice);

		int GetCraftingDifficulty(WeaponDesign weaponDesign);

		bool CanHeroUsePart(Hero hero, CraftingPiece craftingPiece);

		int GetHeroCraftingStamina(Hero hero);

		void SetHeroCraftingStamina(Hero hero, int value);

		int GetMaxHeroCraftingStamina(Hero hero);

		void DoRefinement(Hero hero, Crafting.RefiningFormula refineFormula);

		void DoSmelting(Hero currentCraftingHero, EquipmentElement equipmentElement);

		ItemObject CreateCraftedWeaponInFreeBuildMode(Hero hero, WeaponDesign currentWeaponDesign, int modifierTier, Crafting.OverrideData overrideData);

		ItemObject CreateCraftedWeaponInCraftingOrderMode(Hero crafterHero, CraftingOrder craftingOrder, WeaponDesign weaponDesign, int modifierTier, Crafting.OverrideData overrideData);

		bool IsOpened(CraftingPiece craftingPiece, CraftingTemplate craftingTemplate);

		void InitializeCraftingElements();
	}
}
