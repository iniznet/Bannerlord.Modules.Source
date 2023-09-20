using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	public class GameTypeItemVM : SelectorItemVM
	{
		public CustomBattleGameType GameType { get; private set; }

		public GameTypeItemVM(string gameTypeName, CustomBattleGameType gameType)
			: base(gameTypeName)
		{
			this.GameType = gameType;
		}
	}
}
