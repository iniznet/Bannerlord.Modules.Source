using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001CE RID: 462
	public abstract class EquipmentSelectionModel : GameModel
	{
		// Token: 0x06001B91 RID: 7057
		public abstract MBList<MBEquipmentRoster> GetEquipmentRostersForHeroComeOfAge(Hero hero, bool isCivilian);

		// Token: 0x06001B92 RID: 7058
		public abstract MBList<MBEquipmentRoster> GetEquipmentRostersForHeroReachesTeenAge(Hero hero);

		// Token: 0x06001B93 RID: 7059
		public abstract MBList<MBEquipmentRoster> GetEquipmentRostersForInitialChildrenGeneration(Hero hero);

		// Token: 0x06001B94 RID: 7060
		public abstract MBList<MBEquipmentRoster> GetEquipmentRostersForDeliveredOffspring(Hero hero);

		// Token: 0x06001B95 RID: 7061
		public abstract MBList<MBEquipmentRoster> GetEquipmentRostersForCompanion(Hero companionHero, bool isCivilian);
	}
}
