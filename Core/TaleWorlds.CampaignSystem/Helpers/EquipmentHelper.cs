using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Helpers
{
	// Token: 0x0200000B RID: 11
	public static class EquipmentHelper
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public static void AssignHeroEquipmentFromEquipment(Hero hero, Equipment equipment)
		{
			Equipment equipment2 = (equipment.IsCivilian ? hero.CivilianEquipment : hero.BattleEquipment);
			for (int i = 0; i < 12; i++)
			{
				equipment2[i] = new EquipmentElement(equipment[i].Item, equipment[i].ItemModifier, null, false);
			}
		}
	}
}
