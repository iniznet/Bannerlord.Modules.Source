using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000021 RID: 33
	public class GameTypeItemVM : SelectorItemVM
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001AC RID: 428 RVA: 0x0000A404 File Offset: 0x00008604
		// (set) Token: 0x060001AD RID: 429 RVA: 0x0000A40C File Offset: 0x0000860C
		public CustomBattleGameType GameType { get; private set; }

		// Token: 0x060001AE RID: 430 RVA: 0x0000A415 File Offset: 0x00008615
		public GameTypeItemVM(string gameTypeName, CustomBattleGameType gameType)
			: base(gameTypeName)
		{
			this.GameType = gameType;
		}
	}
}
