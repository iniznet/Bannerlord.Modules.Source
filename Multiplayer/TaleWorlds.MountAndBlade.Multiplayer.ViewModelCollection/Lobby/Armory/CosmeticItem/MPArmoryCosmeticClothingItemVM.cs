using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem
{
	public class MPArmoryCosmeticClothingItemVM : MPArmoryCosmeticItemBaseVM
	{
		public EquipmentElement EquipmentElement { get; }

		public MPArmoryCosmeticsVM.ClothingCategory ClothingCategory { get; }

		public ClothingCosmeticElement ClothingCosmeticElement { get; }

		public MPArmoryCosmeticClothingItemVM(CosmeticElement cosmetic, string cosmeticID)
			: base(cosmetic, cosmeticID, 0)
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(cosmetic.Id);
			this.EquipmentElement = new EquipmentElement(@object, null, null, false);
			base.Icon = new ImageIdentifierVM(@object, "");
			this.ClothingCategory = this.GetCosmeticCategory();
			this.ClothingCosmeticElement = cosmetic as ClothingCosmeticElement;
			base.ItemType = this.EquipmentElement.Item.ItemType;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			ItemObject item = this.EquipmentElement.Item;
			base.Name = ((item != null) ? item.Name.ToString() : null);
		}

		private MPArmoryCosmeticsVM.ClothingCategory GetCosmeticCategory()
		{
			ItemObject.ItemTypeEnum type = this.EquipmentElement.Item.Type;
			switch (type)
			{
			case 12:
				return MPArmoryCosmeticsVM.ClothingCategory.HeadArmor;
			case 13:
				return MPArmoryCosmeticsVM.ClothingCategory.BodyArmor;
			case 14:
				return MPArmoryCosmeticsVM.ClothingCategory.LegArmor;
			case 15:
				return MPArmoryCosmeticsVM.ClothingCategory.HandArmor;
			default:
				if (type != 22)
				{
					return MPArmoryCosmeticsVM.ClothingCategory.Invalid;
				}
				return MPArmoryCosmeticsVM.ClothingCategory.Cape;
			}
		}
	}
}
