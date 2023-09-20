using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticCategory
{
	public class MPArmoryClothingCosmeticCategoryVM : MPArmoryCosmeticCategoryBaseVM
	{
		public static event Action<MPArmoryClothingCosmeticCategoryVM> OnSelected;

		public MPArmoryClothingCosmeticCategoryVM(MPArmoryCosmeticsVM.ClothingCategory clothingCategory)
			: base(0)
		{
			this._defaultCosmeticIDs = new List<string>();
			this.ClothingCategory = clothingCategory;
			base.CosmeticCategoryName = clothingCategory.ToString();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.AvailableCosmetics.ApplyActionOnAllItems(delegate(MPArmoryCosmeticItemBaseVM c)
			{
				c.RefreshValues();
			});
		}

		protected override void ExecuteSelectCategory()
		{
			Action<MPArmoryClothingCosmeticCategoryVM> onSelected = MPArmoryClothingCosmeticCategoryVM.OnSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		private void AddDefaultItem(ItemObject item)
		{
			MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM = new MPArmoryCosmeticClothingItemVM(new ClothingCosmeticElement(item.StringId, 0, 0, new List<string>(), new List<Tuple<string, string>>()), string.Empty)
			{
				IsUnlocked = true,
				IsUnequippable = false
			};
			ItemObject.ItemTypeEnum itemTypeEnum = this.ClothingCategory.ToItemTypeEnum();
			if (itemTypeEnum == null || itemTypeEnum == item.ItemType)
			{
				base.AvailableCosmetics.Add(mparmoryCosmeticClothingItemVM);
			}
		}

		public void SetDefaultEquipments(Equipment equipment)
		{
			base.AvailableCosmetics.Clear();
			this._defaultCosmeticIDs.Clear();
			if (equipment != null)
			{
				for (EquipmentIndex equipmentIndex = 5; equipmentIndex < 10; equipmentIndex++)
				{
					ItemObject item = equipment[equipmentIndex].Item;
					if (item != null)
					{
						this._defaultCosmeticIDs.Add(equipment[equipmentIndex].Item.StringId);
						this.AddDefaultItem(item);
					}
				}
			}
		}

		public void ReplaceCosmeticWithDefaultItem(MPArmoryCosmeticClothingItemVM cosmetic, MPArmoryCosmeticsVM.ClothingCategory clothingCategory, MultiplayerClassDivisions.MPHeroClass selectedClass, List<string> ownedCosmetics)
		{
			bool flag = cosmetic.ClothingCategory == clothingCategory || clothingCategory == MPArmoryCosmeticsVM.ClothingCategory.ClothingCategoriesBegin;
			ClothingCosmeticElement clothingCosmeticElement;
			bool flag2 = (clothingCosmeticElement = cosmetic.Cosmetic as ClothingCosmeticElement) != null && (clothingCosmeticElement.ReplaceItemsId.Any((string c) => this._defaultCosmeticIDs.Contains(c)) || clothingCosmeticElement.ReplaceItemless.Any((Tuple<string, string> r) => r.Item1 == selectedClass.StringId)) && !base.AvailableCosmetics.Contains(cosmetic);
			if (flag && flag2)
			{
				base.AvailableCosmetics.Add(cosmetic);
				cosmetic.IsUnlocked = (ownedCosmetics != null && ownedCosmetics.Contains(cosmetic.CosmeticID)) || cosmetic.Cosmetic.IsFree;
			}
		}

		public void OnEquipmentRefreshed(EquipmentIndex equipmentIndex)
		{
			foreach (MPArmoryCosmeticItemBaseVM mparmoryCosmeticItemBaseVM in base.AvailableCosmetics)
			{
				MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
				if ((mparmoryCosmeticClothingItemVM = mparmoryCosmeticItemBaseVM as MPArmoryCosmeticClothingItemVM) != null && mparmoryCosmeticClothingItemVM.EquipmentElement.Item.GetCosmeticEquipmentIndex() == equipmentIndex)
				{
					mparmoryCosmeticItemBaseVM.IsUsed = false;
				}
			}
		}

		public readonly MPArmoryCosmeticsVM.ClothingCategory ClothingCategory;

		private List<string> _defaultCosmeticIDs;
	}
}
