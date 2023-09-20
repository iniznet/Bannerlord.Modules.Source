﻿using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[ScriptingInterfaceBase]
	internal interface IMBGameEntityExtensions
	{
		[EngineMethod("create_from_weapon", false)]
		GameEntity CreateFromWeapon(UIntPtr scenePointer, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, int ammoWeaponStatsDataLength, bool showHolsterWithWeapon);

		[EngineMethod("fade_out", false)]
		void FadeOut(UIntPtr entityPointer, float interval, bool isRemovingFromScene);

		[EngineMethod("fade_in", false)]
		void FadeIn(UIntPtr entityPointer, bool resetAlpha);

		[EngineMethod("hide_if_not_fading_out", false)]
		void HideIfNotFadingOut(UIntPtr entityPointer);
	}
}
