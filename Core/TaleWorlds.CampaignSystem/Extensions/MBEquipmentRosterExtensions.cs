using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000158 RID: 344
	public static class MBEquipmentRosterExtensions
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x0600183C RID: 6204 RVA: 0x0007B03F File Offset: 0x0007923F
		public static MBReadOnlyList<MBEquipmentRoster> All
		{
			get
			{
				return Campaign.Current.AllEquipmentRosters;
			}
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0007B04B File Offset: 0x0007924B
		public static IEnumerable<Equipment> GetCivilianEquipments(this MBEquipmentRoster instance)
		{
			return instance.AllEquipments.Where((Equipment x) => x.IsCivilian);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x0007B077 File Offset: 0x00079277
		public static IEnumerable<Equipment> GetBattleEquipments(this MBEquipmentRoster instance)
		{
			return instance.AllEquipments.Where((Equipment x) => !x.IsCivilian);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0007B0A3 File Offset: 0x000792A3
		public static Equipment GetRandomCivilianEquipment(this MBEquipmentRoster instance)
		{
			return instance.AllEquipments.GetRandomElementWithPredicate((Equipment x) => x.IsCivilian);
		}
	}
}
