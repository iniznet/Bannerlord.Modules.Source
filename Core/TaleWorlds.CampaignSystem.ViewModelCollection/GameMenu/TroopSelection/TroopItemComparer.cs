using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection
{
	// Token: 0x0200008B RID: 139
	public class TroopItemComparer : IComparer<TroopSelectionItemVM>
	{
		// Token: 0x06000DBC RID: 3516 RVA: 0x000376D0 File Offset: 0x000358D0
		public int Compare(TroopSelectionItemVM x, TroopSelectionItemVM y)
		{
			int num;
			if (y.Troop.Character.IsPlayerCharacter)
			{
				num = 1;
			}
			else if (y.Troop.Character.IsHero)
			{
				if (x.Troop.Character.IsPlayerCharacter)
				{
					num = -1;
				}
				else if (x.Troop.Character.IsHero)
				{
					num = y.Troop.Character.Level - x.Troop.Character.Level;
				}
				else
				{
					num = 1;
				}
			}
			else if (x.Troop.Character.IsPlayerCharacter || x.Troop.Character.IsHero)
			{
				num = -1;
			}
			else
			{
				num = y.Troop.Character.Level - x.Troop.Character.Level;
			}
			return num;
		}
	}
}
