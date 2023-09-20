using System;
using System.Collections.Generic;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000BA RID: 186
	public class SaveableCoreTypeDefiner : SaveableTypeDefiner
	{
		// Token: 0x06000951 RID: 2385 RVA: 0x0001EE2C File Offset: 0x0001D02C
		public SaveableCoreTypeDefiner()
			: base(10000)
		{
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0001EE3C File Offset: 0x0001D03C
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(ArmorComponent), 2, null);
			base.AddClassDefinition(typeof(Banner), 3, null);
			base.AddClassDefinition(typeof(BannerData), 4, null);
			base.AddClassDefinition(typeof(BasicCharacterObject), 5, null);
			base.AddClassDefinition(typeof(CharacterAttribute), 6, null);
			base.AddClassDefinition(typeof(CharacterSkills), 8, null);
			base.AddClassDefinition(typeof(WeaponDesign), 9, null);
			base.AddClassDefinition(typeof(CraftingPiece), 10, null);
			base.AddClassDefinition(typeof(CraftingTemplate), 11, null);
			base.AddClassDefinition(typeof(EntitySystem<>), 15, null);
			base.AddClassDefinition(typeof(Equipment), 16, null);
			base.AddClassDefinition(typeof(TradeItemComponent), 18, null);
			base.AddClassDefinition(typeof(GameType), 26, null);
			base.AddClassDefinition(typeof(HorseComponent), 27, null);
			base.AddClassDefinition(typeof(ItemCategory), 28, null);
			base.AddClassDefinition(typeof(ItemComponent), 29, null);
			base.AddClassDefinition(typeof(ItemModifier), 30, null);
			base.AddClassDefinition(typeof(ItemModifierGroup), 31, null);
			base.AddClassDefinition(typeof(ItemObject), 32, null);
			base.AddClassDefinition(typeof(MissionResult), 36, null);
			base.AddClassDefinition(typeof(PropertyObject), 38, null);
			base.AddClassDefinition(typeof(SkillObject), 39, null);
			base.AddClassDefinition(typeof(PropertyOwner<>), 40, null);
			base.AddClassDefinition(typeof(PropertyOwnerF<>), 41, null);
			base.AddClassDefinition(typeof(SiegeEngineType), 42, null);
			base.AddClassDefinition(typeof(WeaponDesignElement), 44, null);
			base.AddClassDefinition(typeof(WeaponComponent), 45, null);
			base.AddClassDefinition(typeof(WeaponComponentData), 46, null);
			base.AddClassDefinition(typeof(InformationData), 50, null);
			base.AddClassDefinition(typeof(Crafting.OverrideData), 51, null);
			base.AddClassDefinition(typeof(MBFastRandom), 52, null);
			base.AddClassDefinition(typeof(BannerComponent), 53, null);
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0001F0A4 File Offset: 0x0001D2A4
		protected override void DefineStructTypes()
		{
			base.AddStructDefinition(typeof(ItemRosterElement), 1004, null);
			base.AddStructDefinition(typeof(UniqueTroopDescriptor), 1006, null);
			base.AddStructDefinition(typeof(StaticBodyProperties), 1009, null);
			base.AddStructDefinition(typeof(EquipmentElement), 1011, null);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0001F10C File Offset: 0x0001D30C
		protected override void DefineEnumTypes()
		{
			base.AddEnumDefinition(typeof(BattleSideEnum), 2001, null);
			base.AddEnumDefinition(typeof(Equipment.EquipmentType), 2006, null);
			base.AddEnumDefinition(typeof(WeaponFlags), 2007, null);
			base.AddEnumDefinition(typeof(FormationClass), 2008, null);
			base.AddEnumDefinition(typeof(BattleState), 2009, null);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0001F187 File Offset: 0x0001D387
		protected override void DefineInterfaceTypes()
		{
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0001F189 File Offset: 0x0001D389
		protected override void DefineRootClassTypes()
		{
			base.AddRootClassDefinition(typeof(Game), 4001, null);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0001F1A1 File Offset: 0x0001D3A1
		protected override void DefineGenericClassDefinitions()
		{
			base.ConstructGenericClassDefinition(typeof(Tuple<int, int>));
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0001F1B3 File Offset: 0x0001D3B3
		protected override void DefineGenericStructDefinitions()
		{
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0001F1B8 File Offset: 0x0001D3B8
		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(ItemRosterElement[]));
			base.ConstructContainerDefinition(typeof(EquipmentElement[]));
			base.ConstructContainerDefinition(typeof(Equipment[]));
			base.ConstructContainerDefinition(typeof(WeaponDesignElement[]));
			base.ConstructContainerDefinition(typeof(List<ItemObject>));
			base.ConstructContainerDefinition(typeof(List<ItemComponent>));
			base.ConstructContainerDefinition(typeof(List<ItemModifier>));
			base.ConstructContainerDefinition(typeof(List<ItemModifierGroup>));
			base.ConstructContainerDefinition(typeof(List<CharacterAttribute>));
			base.ConstructContainerDefinition(typeof(List<SkillObject>));
			base.ConstructContainerDefinition(typeof(List<ItemCategory>));
			base.ConstructContainerDefinition(typeof(List<CraftingPiece>));
			base.ConstructContainerDefinition(typeof(List<CraftingTemplate>));
			base.ConstructContainerDefinition(typeof(List<SiegeEngineType>));
			base.ConstructContainerDefinition(typeof(List<PropertyObject>));
			base.ConstructContainerDefinition(typeof(List<UniqueTroopDescriptor>));
			base.ConstructContainerDefinition(typeof(List<Equipment>));
			base.ConstructContainerDefinition(typeof(List<BannerData>));
			base.ConstructContainerDefinition(typeof(List<EquipmentElement>));
			base.ConstructContainerDefinition(typeof(List<WeaponDesign>));
			base.ConstructContainerDefinition(typeof(List<ItemRosterElement>));
			base.ConstructContainerDefinition(typeof(List<InformationData>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, ItemCategory>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, CraftingPiece>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, CraftingTemplate>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, SiegeEngineType>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, PropertyObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, SkillObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, CharacterAttribute>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, ItemModifierGroup>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, ItemComponent>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, ItemObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, ItemModifier>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, ItemCategory>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, CraftingPiece>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, CraftingTemplate>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, SiegeEngineType>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, PropertyObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, SkillObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, CharacterAttribute>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, ItemModifierGroup>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, ItemObject>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, ItemComponent>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, ItemModifier>));
			base.ConstructContainerDefinition(typeof(Dictionary<ItemCategory, float>));
			base.ConstructContainerDefinition(typeof(Dictionary<ItemCategory, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<SiegeEngineType, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<SkillObject, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<PropertyObject, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<PropertyObject, float>));
			base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, float>));
			base.ConstructContainerDefinition(typeof(Dictionary<CharacterAttribute, int>));
			base.ConstructContainerDefinition(typeof(Dictionary<CraftingTemplate, List<CraftingPiece>>));
			base.ConstructContainerDefinition(typeof(Dictionary<CraftingTemplate, float>));
			base.ConstructContainerDefinition(typeof(Dictionary<long, Dictionary<long, int>>));
			base.ConstructContainerDefinition(typeof(Dictionary<int, Tuple<int, int>>));
			base.ConstructContainerDefinition(typeof(Dictionary<EquipmentElement, int>));
		}
	}
}
