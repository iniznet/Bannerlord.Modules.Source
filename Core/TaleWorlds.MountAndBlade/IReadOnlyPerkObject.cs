using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public interface IReadOnlyPerkObject
	{
		TextObject Name { get; }

		TextObject Description { get; }

		List<string> GameModes { get; }

		int PerkListIndex { get; }

		string IconId { get; }

		string HeroIdleAnimOverride { get; }

		string HeroMountIdleAnimOverride { get; }

		string TroopIdleAnimOverride { get; }

		string TroopMountIdleAnimOverride { get; }

		float GetTroopCountMultiplier(bool isWarmup);

		float GetExtraTroopCount(bool isWarmup);

		List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isWarmup, bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAllEquipments = false);

		float GetDrivenPropertyBonusOnSpawn(bool isWarmup, bool isPlayer, DrivenProperty drivenProperty, float baseValue);

		float GetHitpoints(bool isWarmup, bool isPlayer);

		MPPerkObject Clone(MissionPeer peer);
	}
}
