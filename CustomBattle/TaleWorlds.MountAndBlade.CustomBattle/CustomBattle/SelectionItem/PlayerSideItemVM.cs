using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000023 RID: 35
	public class PlayerSideItemVM : SelectorItemVM
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000A51E File Offset: 0x0000871E
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000A526 File Offset: 0x00008726
		public CustomBattlePlayerSide PlayerSide { get; private set; }

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A52F File Offset: 0x0000872F
		public PlayerSideItemVM(string playerSideName, CustomBattlePlayerSide playerSide)
			: base(playerSideName)
		{
			this.PlayerSide = playerSide;
		}
	}
}
