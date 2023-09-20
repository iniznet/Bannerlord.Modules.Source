using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public static class GameEntityExtensions
	{
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

		public static void CreateSimpleSkeleton(this GameEntity gameEntity, string skeletonName)
		{
			gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateSimpleSkeleton(skeletonName);
		}

		public static void CreateAgentSkeleton(this GameEntity gameEntity, string skeletonName, bool isHumanoid, MBActionSet actionSet, string monsterUsageSetName, Monster monster)
		{
			AnimationSystemData animationSystemData = monster.FillAnimationSystemData(actionSet, 1f, false);
			gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateAgentSkeleton(skeletonName, isHumanoid, actionSet.Index, monsterUsageSetName, ref animationSystemData);
		}

		public static void CreateSkeletonWithActionSet(this GameEntity gameEntity, ref AnimationSystemData animationSystemData)
		{
			gameEntity.Skeleton = MBSkeletonExtensions.CreateWithActionSet(ref animationSystemData);
		}

		public static void FadeOut(this GameEntity gameEntity, float interval, bool isRemovingFromScene)
		{
			MBAPI.IMBGameEntityExtensions.FadeOut(gameEntity.Pointer, interval, isRemovingFromScene);
		}

		public static void FadeIn(this GameEntity gameEntity, bool resetAlpha = true)
		{
			MBAPI.IMBGameEntityExtensions.FadeIn(gameEntity.Pointer, resetAlpha);
		}

		public static void HideIfNotFadingOut(this GameEntity gameEntity)
		{
			MBAPI.IMBGameEntityExtensions.HideIfNotFadingOut(gameEntity.Pointer);
		}

		public static void UpdateRestrictedTrajectoryBuilder(this GameEntity gameEntity, float maxShootSpeed, float minShootSpeed, float maxAngle, float minAngle, float turnAngle, float frictionCoefficient)
		{
			MBAPI.IMBGameEntityExtensions.UpdateTrajectoryVisualizerForSpawner(gameEntity.Pointer, maxShootSpeed, minShootSpeed, maxAngle, minAngle, turnAngle, frictionCoefficient);
		}
	}
}
