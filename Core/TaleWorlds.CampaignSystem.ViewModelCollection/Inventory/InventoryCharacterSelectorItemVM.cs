using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class InventoryCharacterSelectorItemVM : SelectorItemVM
	{
		public string CharacterID { get; private set; }

		public Hero Hero { get; private set; }

		public InventoryCharacterSelectorItemVM(string characterID, Hero hero, TextObject characterName)
			: base(characterName)
		{
			this.Hero = hero;
			this.CharacterID = characterID;
		}
	}
}
