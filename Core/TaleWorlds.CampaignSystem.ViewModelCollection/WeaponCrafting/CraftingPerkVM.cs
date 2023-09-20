using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	public class CraftingPerkVM : ViewModel
	{
		public CraftingPerkVM(PerkObject perk)
		{
			this.Perk = perk;
			this.Name = this.Perk.Name.ToString();
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		public readonly PerkObject Perk;

		private string _name;
	}
}
