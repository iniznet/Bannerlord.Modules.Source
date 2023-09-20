using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory
{
	public static class LobbyCosmeticExtensions
	{
		public static ItemObject.ItemTypeEnum ToItemTypeEnum(this MPArmoryCosmeticsVM.ClothingCategory cosmeticCategory)
		{
			switch (cosmeticCategory)
			{
			case MPArmoryCosmeticsVM.ClothingCategory.ClothingCategoriesBegin:
				return 0;
			case MPArmoryCosmeticsVM.ClothingCategory.HeadArmor:
				return 12;
			case MPArmoryCosmeticsVM.ClothingCategory.Cape:
				return 22;
			case MPArmoryCosmeticsVM.ClothingCategory.BodyArmor:
				return 13;
			case MPArmoryCosmeticsVM.ClothingCategory.HandArmor:
				return 15;
			case MPArmoryCosmeticsVM.ClothingCategory.LegArmor:
				return 14;
			default:
				return 0;
			}
		}

		public static EquipmentIndex GetCosmeticEquipmentIndex(this ItemObject itemObject)
		{
			if (itemObject == null)
			{
				return -1;
			}
			ItemObject.ItemTypeEnum type = itemObject.Type;
			if (type <= 15)
			{
				if (type == 1)
				{
					return 10;
				}
				switch (type)
				{
				case 12:
					return 5;
				case 13:
					return 6;
				case 14:
					return 7;
				case 15:
					return 8;
				}
			}
			else
			{
				if (type == 22)
				{
					return 9;
				}
				if (type == 23)
				{
					return 11;
				}
			}
			return -1;
		}
	}
}
