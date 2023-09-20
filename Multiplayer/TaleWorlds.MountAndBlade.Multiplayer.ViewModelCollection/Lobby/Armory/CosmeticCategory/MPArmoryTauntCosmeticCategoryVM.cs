using System;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticCategory
{
	public class MPArmoryTauntCosmeticCategoryVM : MPArmoryCosmeticCategoryBaseVM
	{
		public static event Action<MPArmoryTauntCosmeticCategoryVM> OnSelected;

		public MPArmoryTauntCosmeticCategoryVM(MPArmoryCosmeticsVM.TauntCategoryFlag tauntCategory)
			: base(3)
		{
			this.TauntCategory = tauntCategory;
			base.CosmeticCategoryName = tauntCategory.ToString();
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
			Action<MPArmoryTauntCosmeticCategoryVM> onSelected = MPArmoryTauntCosmeticCategoryVM.OnSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		public readonly MPArmoryCosmeticsVM.TauntCategoryFlag TauntCategory;
	}
}
