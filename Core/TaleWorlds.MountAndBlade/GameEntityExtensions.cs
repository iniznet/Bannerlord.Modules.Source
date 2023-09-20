using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020A RID: 522
	public static class GameEntityExtensions
	{
		// Token: 0x06001DA0 RID: 7584 RVA: 0x0006A91C File Offset: 0x00068B1C
		public static GameEntity Instantiate(Scene scene, MissionWeapon weapon, bool showHolsterWithWeapon, bool needBatchedVersion)
		{
			WeaponData weaponData = weapon.GetWeaponData(needBatchedVersion);
			WeaponStatsData[] weaponStatsData = weapon.GetWeaponStatsData();
			WeaponData ammoWeaponData = weapon.GetAmmoWeaponData(needBatchedVersion);
			WeaponStatsData[] ammoWeaponStatsData = weapon.GetAmmoWeaponStatsData();
			GameEntity gameEntity = MBAPI.IMBGameEntityExtensions.CreateFromWeapon(scene.Pointer, weaponData, weaponStatsData, weaponStatsData.Length, ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData.Length, showHolsterWithWeapon);
			weaponData.DeinitializeManagedPointers();
			return gameEntity;
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x0006A96F File Offset: 0x00068B6F
		public static void CreateSimpleSkeleton(this GameEntity gameEntity, string skeletonName)
		{
			gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateSimpleSkeleton(skeletonName);
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x0006A984 File Offset: 0x00068B84
		public static void CreateAgentSkeleton(this GameEntity gameEntity, string skeletonName, bool isHumanoid, MBActionSet actionSet, string monsterUsageSetName, Monster monster)
		{
			AnimationSystemData animationSystemData = monster.FillAnimationSystemData(actionSet, 1f, false);
			gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateAgentSkeleton(skeletonName, isHumanoid, actionSet.Index, monsterUsageSetName, ref animationSystemData);
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x0006A9BC File Offset: 0x00068BBC
		public static void CreateSkeletonWithActionSet(this GameEntity gameEntity, ref AnimationSystemData animationSystemData)
		{
			gameEntity.Skeleton = MBSkeletonExtensions.CreateWithActionSet(ref animationSystemData);
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x0006A9CA File Offset: 0x00068BCA
		public static void FadeOut(this GameEntity gameEntity, float interval, bool isRemovingFromScene)
		{
			MBAPI.IMBGameEntityExtensions.FadeOut(gameEntity.Pointer, interval, isRemovingFromScene);
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x0006A9DE File Offset: 0x00068BDE
		public static void FadeIn(this GameEntity gameEntity, bool resetAlpha = true)
		{
			MBAPI.IMBGameEntityExtensions.FadeIn(gameEntity.Pointer, resetAlpha);
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0006A9F1 File Offset: 0x00068BF1
		public static void HideIfNotFadingOut(this GameEntity gameEntity)
		{
			MBAPI.IMBGameEntityExtensions.HideIfNotFadingOut(gameEntity.Pointer);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0006AA03 File Offset: 0x00068C03
		public static void UpdateRestrictedTrajectoryBuilder(this GameEntity gameEntity, float maxShootSpeed, float minShootSpeed, float maxAngle, float minAngle, float turnAngle, float frictionCoefficient)
		{
			MBAPI.IMBGameEntityExtensions.UpdateTrajectoryVisualizerForSpawner(gameEntity.Pointer, maxShootSpeed, minShootSpeed, maxAngle, minAngle, turnAngle, frictionCoefficient);
		}
	}
}
