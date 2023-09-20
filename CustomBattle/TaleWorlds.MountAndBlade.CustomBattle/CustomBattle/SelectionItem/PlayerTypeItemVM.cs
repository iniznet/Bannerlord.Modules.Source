using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	public class PlayerTypeItemVM : SelectorItemVM
	{
		public CustomBattlePlayerType PlayerType { get; private set; }

		public PlayerTypeItemVM(string playerTypeName, CustomBattlePlayerType playerType)
			: base(playerTypeName)
		{
			this.PlayerType = playerType;
		}
	}
}
