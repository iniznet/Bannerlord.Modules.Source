using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001BB RID: 443
	public abstract class SiegeEventModel : GameModel
	{
		// Token: 0x06001B08 RID: 6920
		public abstract int GetSiegeEngineDestructionCasualties(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType destroyedSiegeEngine);

		// Token: 0x06001B09 RID: 6921
		public abstract float GetCasualtyChance(MobileParty siegeParty, SiegeEvent siegeEvent, BattleSideEnum side);

		// Token: 0x06001B0A RID: 6922
		public abstract int GetColleteralDamageCasualties(SiegeEngineType attackerSiegeEngine, MobileParty party);

		// Token: 0x06001B0B RID: 6923
		public abstract float GetSiegeEngineHitChance(SiegeEngineType siegeEngineType, BattleSideEnum battleSide, SiegeBombardTargets target, Town town);

		// Token: 0x06001B0C RID: 6924
		public abstract string GetSiegeEngineMapPrefabName(SiegeEngineType siegeEngineType, int wallLevel, BattleSideEnum side);

		// Token: 0x06001B0D RID: 6925
		public abstract string GetSiegeEngineMapProjectilePrefabName(SiegeEngineType siegeEngineType);

		// Token: 0x06001B0E RID: 6926
		public abstract string GetSiegeEngineMapReloadAnimationName(SiegeEngineType siegeEngineType, BattleSideEnum side);

		// Token: 0x06001B0F RID: 6927
		public abstract string GetSiegeEngineMapFireAnimationName(SiegeEngineType siegeEngineType, BattleSideEnum side);

		// Token: 0x06001B10 RID: 6928
		public abstract sbyte GetSiegeEngineMapProjectileBoneIndex(SiegeEngineType siegeEngineType, BattleSideEnum side);

		// Token: 0x06001B11 RID: 6929
		public abstract float GetSiegeStrategyScore(SiegeEvent siege, BattleSideEnum side, SiegeStrategy strategy);

		// Token: 0x06001B12 RID: 6930
		public abstract float GetConstructionProgressPerHour(SiegeEngineType type, SiegeEvent siegeEvent, ISiegeEventSide side);

		// Token: 0x06001B13 RID: 6931
		public abstract MobileParty GetEffectiveSiegePartyForSide(SiegeEvent siegeEvent, BattleSideEnum side);

		// Token: 0x06001B14 RID: 6932
		public abstract float GetAvailableManDayPower(ISiegeEventSide side);

		// Token: 0x06001B15 RID: 6933
		public abstract IEnumerable<SiegeEngineType> GetAvailableAttackerRangedSiegeEngines(PartyBase party);

		// Token: 0x06001B16 RID: 6934
		public abstract IEnumerable<SiegeEngineType> GetAvailableDefenderSiegeEngines(PartyBase party);

		// Token: 0x06001B17 RID: 6935
		public abstract IEnumerable<SiegeEngineType> GetAvailableAttackerRamSiegeEngines(PartyBase party);

		// Token: 0x06001B18 RID: 6936
		public abstract IEnumerable<SiegeEngineType> GetAvailableAttackerTowerSiegeEngines(PartyBase party);

		// Token: 0x06001B19 RID: 6937
		public abstract IEnumerable<SiegeEngineType> GetPrebuiltSiegeEnginesOfSettlement(Settlement settlement);

		// Token: 0x06001B1A RID: 6938
		public abstract IEnumerable<SiegeEngineType> GetPrebuiltSiegeEnginesOfSiegeCamp(BesiegerCamp camp);

		// Token: 0x06001B1B RID: 6939
		public abstract float GetSiegeEngineHitPoints(SiegeEvent siegeEvent, SiegeEngineType siegeEngine, BattleSideEnum battleSide);

		// Token: 0x06001B1C RID: 6940
		public abstract int GetRangedSiegeEngineReloadTime(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngine);

		// Token: 0x06001B1D RID: 6941
		public abstract float GetSiegeEngineDamage(SiegeEvent siegeEvent, BattleSideEnum battleSide, SiegeEngineType siegeEngine, SiegeBombardTargets target);

		// Token: 0x06001B1E RID: 6942
		public abstract FlattenedTroopRoster GetPriorityTroopsForSallyOutAmbush();
	}
}
