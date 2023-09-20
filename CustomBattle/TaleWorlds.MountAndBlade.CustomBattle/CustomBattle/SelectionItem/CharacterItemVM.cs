using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001F RID: 31
	public class CharacterItemVM : SelectorItemVM
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000A2EF File Offset: 0x000084EF
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x0000A2F7 File Offset: 0x000084F7
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x060001A2 RID: 418 RVA: 0x0000A300 File Offset: 0x00008500
		public CharacterItemVM(BasicCharacterObject character)
			: base(character.Name.ToString())
		{
			this.Character = character;
		}
	}
}
