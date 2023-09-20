using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.AfterBattle
{
	public class MPAfterBattleLootRewardItemVM : MPAfterBattleRewardItemVM
	{
		public MPAfterBattleLootRewardItemVM(int lootGained, int additionalLootFromBadges)
		{
			base.Type = 0;
			GameTexts.SetVariable("LOOT", lootGained);
			string text = new TextObject("{=JYIURZLb}+{LOOT} from match", null).ToString();
			if (additionalLootFromBadges > 0)
			{
				GameTexts.SetVariable("LOOT", additionalLootFromBadges);
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", new TextObject("{=erp8X0KD}+{LOOT} from badges", null));
				GameTexts.SetVariable("newline", "\n");
				base.Name = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			else
			{
				base.Name = text;
			}
			this.RefreshValues();
		}
	}
}
