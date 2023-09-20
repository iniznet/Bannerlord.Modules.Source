using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000024 RID: 36
	public class PlayerTypeItemVM : SelectorItemVM
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000A53F File Offset: 0x0000873F
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000A547 File Offset: 0x00008747
		public CustomBattlePlayerType PlayerType { get; private set; }

		// Token: 0x060001BC RID: 444 RVA: 0x0000A550 File Offset: 0x00008750
		public PlayerTypeItemVM(string playerTypeName, CustomBattlePlayerType playerType)
			: base(playerTypeName)
		{
			this.PlayerType = playerType;
		}
	}
}
