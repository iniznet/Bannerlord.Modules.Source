using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x02000082 RID: 130
	public class InventoryCharacterSelectorItemVM : SelectorItemVM
	{
		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000CCA RID: 3274 RVA: 0x0003418E File Offset: 0x0003238E
		// (set) Token: 0x06000CCB RID: 3275 RVA: 0x00034196 File Offset: 0x00032396
		public string CharacterID { get; private set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0003419F File Offset: 0x0003239F
		// (set) Token: 0x06000CCD RID: 3277 RVA: 0x000341A7 File Offset: 0x000323A7
		public Hero Hero { get; private set; }

		// Token: 0x06000CCE RID: 3278 RVA: 0x000341B0 File Offset: 0x000323B0
		public InventoryCharacterSelectorItemVM(string characterID, Hero hero, TextObject characterName)
			: base(characterName)
		{
			this.Hero = hero;
			this.CharacterID = characterID;
		}
	}
}
