using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x0200028A RID: 650
	public interface ISiegeEventSide
	{
		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06002256 RID: 8790
		SiegeEvent SiegeEvent { get; }

		// Token: 0x06002257 RID: 8791
		IEnumerable<PartyBase> GetInvolvedPartiesForEventType(MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		// Token: 0x06002258 RID: 8792
		PartyBase GetNextInvolvedPartyForEventType(ref int partyIndex, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		// Token: 0x06002259 RID: 8793
		bool HasInvolvedPartyForEventType(PartyBase party, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x0600225A RID: 8794
		SiegeStrategy SiegeStrategy { get; }

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x0600225B RID: 8795
		BattleSideEnum BattleSide { get; }

		// Token: 0x0600225C RID: 8796
		void OnTroopsKilledOnSide(int killCount);

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x0600225D RID: 8797
		int NumberOfTroopsKilledOnSide { get; }

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x0600225E RID: 8798
		SiegeEvent.SiegeEnginesContainer SiegeEngines { get; }

		// Token: 0x0600225F RID: 8799
		void AddSiegeEngineMissile(SiegeEvent.SiegeEngineMissile missile);

		// Token: 0x06002260 RID: 8800
		void RemoveDeprecatedMissiles();

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06002261 RID: 8801
		MBReadOnlyList<SiegeEvent.SiegeEngineMissile> SiegeEngineMissiles { get; }

		// Token: 0x06002262 RID: 8802
		void SetSiegeStrategy(SiegeStrategy strategy);

		// Token: 0x06002263 RID: 8803
		void InitializeSiegeEventSide();

		// Token: 0x06002264 RID: 8804
		void GetAttackTarget(ISiegeEventSide siegeEventSide, SiegeEngineType siegeEngine, int siegeEngineSlot, out SiegeBombardTargets targetType, out int targetIndex);

		// Token: 0x06002265 RID: 8805
		void FinalizeSiegeEvent();
	}
}
