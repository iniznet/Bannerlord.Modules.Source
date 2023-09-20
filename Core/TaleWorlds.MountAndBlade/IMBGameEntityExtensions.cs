using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019A RID: 410
	[ScriptingInterfaceBase]
	internal interface IMBGameEntityExtensions
	{
		// Token: 0x060016D6 RID: 5846
		[EngineMethod("create_from_weapon", false)]
		GameEntity CreateFromWeapon(UIntPtr scenePointer, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, int weaponStatsDataLength, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, int ammoWeaponStatsDataLength, bool showHolsterWithWeapon);

		// Token: 0x060016D7 RID: 5847
		[EngineMethod("fade_out", false)]
		void FadeOut(UIntPtr entityPointer, float interval, bool isRemovingFromScene);

		// Token: 0x060016D8 RID: 5848
		[EngineMethod("fade_in", false)]
		void FadeIn(UIntPtr entityPointer, bool resetAlpha);

		// Token: 0x060016D9 RID: 5849
		[EngineMethod("hide_if_not_fading_out", false)]
		void HideIfNotFadingOut(UIntPtr entityPointer);

		// Token: 0x060016DA RID: 5850
		[EngineMethod("update_trajectory_visualizer_for_spawner", false)]
		void UpdateTrajectoryVisualizerForSpawner(UIntPtr gameEntity, float maxShootSpeed, float minShootSpeed, float maxAngle, float minAngle, float turnAngle, float frictionCoefficient);
	}
}
