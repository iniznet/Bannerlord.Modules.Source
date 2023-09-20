using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Siege
{
	public interface ISiegeEventSide
	{
		SiegeEvent SiegeEvent { get; }

		IEnumerable<PartyBase> GetInvolvedPartiesForEventType(MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		PartyBase GetNextInvolvedPartyForEventType(ref int partyIndex, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		bool HasInvolvedPartyForEventType(PartyBase party, MapEvent.BattleTypes mapEventType = MapEvent.BattleTypes.Siege);

		SiegeStrategy SiegeStrategy { get; }

		BattleSideEnum BattleSide { get; }

		void OnTroopsKilledOnSide(int killCount);

		int NumberOfTroopsKilledOnSide { get; }

		SiegeEvent.SiegeEnginesContainer SiegeEngines { get; }

		void AddSiegeEngineMissile(SiegeEvent.SiegeEngineMissile missile);

		void RemoveDeprecatedMissiles();

		MBReadOnlyList<SiegeEvent.SiegeEngineMissile> SiegeEngineMissiles { get; }

		void SetSiegeStrategy(SiegeStrategy strategy);

		void InitializeSiegeEventSide();

		void GetAttackTarget(ISiegeEventSide siegeEventSide, SiegeEngineType siegeEngine, int siegeEngineSlot, out SiegeBombardTargets targetType, out int targetIndex);

		void FinalizeSiegeEvent();
	}
}
