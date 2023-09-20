using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum EquipmentFlags : uint
	{
		None = 0U,
		IsWandererEquipment = 1U,
		IsGentryEquipment = 2U,
		IsRebelHeroEquipment = 4U,
		IsNoncombatantTemplate = 8U,
		IsCombatantTemplate = 16U,
		IsCivilianTemplate = 32U,
		IsNobleTemplate = 64U,
		IsFemaleTemplate = 128U,
		IsMediumTemplate = 256U,
		IsHeavyTemplate = 512U,
		IsFlamboyantTemplate = 1024U,
		IsStoicTemplate = 2048U,
		IsNomadTemplate = 4096U,
		IsWoodlandTemplate = 8192U,
		IsChildEquipmentTemplate = 16384U,
		IsTeenagerEquipmentTemplate = 32768U
	}
}
