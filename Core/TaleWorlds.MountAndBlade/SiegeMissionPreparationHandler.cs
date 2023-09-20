using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeMissionPreparationHandler : MissionLogic
	{
		private Scene MissionScene
		{
			get
			{
				return Mission.Current.Scene;
			}
		}

		public SiegeMissionPreparationHandler(bool isSallyOut, bool isReliefForceAttack, float[] wallHitPointPercentages, bool hasAnySiegeTower)
		{
			if (isSallyOut)
			{
				this._siegeMissionType = SiegeMissionPreparationHandler.SiegeMissionType.SallyOut;
			}
			else if (isReliefForceAttack)
			{
				this._siegeMissionType = SiegeMissionPreparationHandler.SiegeMissionType.ReliefForce;
			}
			else
			{
				this._siegeMissionType = SiegeMissionPreparationHandler.SiegeMissionType.Assault;
			}
			this._wallHitPointPercentages = wallHitPointPercentages;
			this._hasAnySiegeTower = hasAnySiegeTower;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.SetUpScene();
		}

		private void SetUpScene()
		{
			this.ArrangeBesiegerDeploymentPointsAndMachines();
			this.ArrangeEntitiesForMissionType();
			this.ArrangeDestructedMeshes();
			if (this._siegeMissionType != SiegeMissionPreparationHandler.SiegeMissionType.Assault)
			{
				this.ArrangeSiegeMachinesForNonAssaultMission();
			}
		}

		private void ArrangeBesiegerDeploymentPointsAndMachines()
		{
			bool flag = this._siegeMissionType == SiegeMissionPreparationHandler.SiegeMissionType.Assault;
			Debug.Print("{SIEGE} ArrangeBesiegerDeploymentPointsAndMachines", 0, Debug.DebugColor.DarkCyan, 64UL);
			Debug.Print("{SIEGE} MissionType: " + this._siegeMissionType, 0, Debug.DebugColor.DarkCyan, 64UL);
			if (!flag)
			{
				SiegeLadder[] array = base.Mission.ActiveMissionObjects.FindAllWithType<SiegeLadder>().ToArray<SiegeLadder>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetDisabledSynched();
				}
			}
		}

		private void ArrangeEntitiesForMissionType()
		{
			string text = ((this._siegeMissionType == SiegeMissionPreparationHandler.SiegeMissionType.Assault) ? "sally_out" : "siege_assault");
			Debug.Print("{SIEGE} ArrangeEntitiesForMissionType", 0, Debug.DebugColor.DarkCyan, 64UL);
			Debug.Print("{SIEGE} MissionType: " + this._siegeMissionType, 0, Debug.DebugColor.DarkCyan, 64UL);
			Debug.Print("{SIEGE} TagToBeRemoved: " + text, 0, Debug.DebugColor.DarkCyan, 64UL);
			foreach (GameEntity gameEntity in this.MissionScene.FindEntitiesWithTag(text).ToList<GameEntity>())
			{
				gameEntity.Remove(77);
			}
		}

		private void ArrangeDestructedMeshes()
		{
			float num = 0f;
			foreach (float num2 in this._wallHitPointPercentages)
			{
				num += num2;
			}
			if (!this._wallHitPointPercentages.IsEmpty<float>())
			{
				num /= (float)this._wallHitPointPercentages.Length;
			}
			float num3 = MBMath.Lerp(0f, 0.7f, 1f - num, 1E-05f);
			IEnumerable<SynchedMissionObject> enumerable = base.Mission.MissionObjects.OfType<SynchedMissionObject>();
			IEnumerable<DestructableComponent> enumerable2 = enumerable.OfType<DestructableComponent>();
			foreach (StrategicArea strategicArea in base.Mission.ActiveMissionObjects.OfType<StrategicArea>().ToList<StrategicArea>())
			{
				strategicArea.DetermineAssociatedDestructibleComponents(enumerable2);
			}
			foreach (SynchedMissionObject synchedMissionObject in enumerable)
			{
				if (this._hasAnySiegeTower && synchedMissionObject.GameEntity.HasTag("tower_merlon"))
				{
					synchedMissionObject.SetVisibleSynched(false, true);
				}
				else
				{
					DestructableComponent firstScriptOfType = synchedMissionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>();
					if (firstScriptOfType != null && firstScriptOfType.CanBeDestroyedInitially && num3 > 0f && MBRandom.RandomFloat <= num3)
					{
						firstScriptOfType.PreDestroy();
					}
				}
			}
			if (num3 >= 0.1f)
			{
				List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("damage_decal").ToList<GameEntity>();
				foreach (GameEntity gameEntity in list)
				{
					gameEntity.GetFirstScriptOfType<SynchedMissionObject>().SetVisibleSynched(false, false);
				}
				for (int j = MathF.Floor((float)list.Count * num3); j > 0; j--)
				{
					GameEntity gameEntity2 = list[MBRandom.RandomInt(list.Count)];
					list.Remove(gameEntity2);
					gameEntity2.GetFirstScriptOfType<SynchedMissionObject>().SetVisibleSynched(true, false);
				}
			}
			List<WallSegment> list2 = new List<WallSegment>();
			List<WallSegment> list3 = base.Mission.ActiveMissionObjects.FindAllWithType<WallSegment>().Where(delegate(WallSegment ws)
			{
				if (ws.DefenseSide != FormationAI.BehaviorSide.BehaviorSideNotSet)
				{
					return ws.GameEntity.GetChildren().Any((GameEntity ge) => ge.HasTag("broken_child"));
				}
				return false;
			}).ToList<WallSegment>();
			foreach (float num4 in this._wallHitPointPercentages)
			{
				WallSegment wallSegment = this.FindRightMostWall(list3);
				if (MathF.Abs(num4) < 1E-05f)
				{
					wallSegment.OnChooseUsedWallSegment(true);
					list2.Add(wallSegment);
				}
				else
				{
					wallSegment.OnChooseUsedWallSegment(false);
				}
				list3.Remove(wallSegment);
			}
			foreach (WallSegment wallSegment2 in list3)
			{
				wallSegment2.OnChooseUsedWallSegment(false);
			}
			if (num3 >= 0.1f)
			{
				List<SiegeWeapon> list4 = new List<SiegeWeapon>();
				using (IEnumerator<SiegeWeapon> enumerator5 = (from sw in base.Mission.ActiveMissionObjects.FindAllWithType<SiegeWeapon>()
					where sw is IPrimarySiegeWeapon
					select sw).GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						SiegeWeapon primarySiegeWeapon = enumerator5.Current;
						if (list2.Any((WallSegment b) => b.DefenseSide == ((IPrimarySiegeWeapon)primarySiegeWeapon).WeaponSide))
						{
							list4.Add(primarySiegeWeapon);
						}
					}
				}
				list4.ForEach(delegate(SiegeWeapon siegeWeaponToRemove)
				{
					siegeWeaponToRemove.SetDisabledSynched();
				});
			}
		}

		private WallSegment FindRightMostWall(List<WallSegment> wallList)
		{
			int count = wallList.Count;
			if (count == 1)
			{
				return wallList[0];
			}
			BatteringRam batteringRam = base.Mission.ActiveMissionObjects.FindAllWithType<BatteringRam>().First<BatteringRam>();
			if (count != 2)
			{
				return null;
			}
			if (Vec3.CrossProduct(wallList[0].GameEntity.GlobalPosition - batteringRam.GameEntity.GlobalPosition, wallList[1].GameEntity.GlobalPosition - batteringRam.GameEntity.GlobalPosition).z < 0f)
			{
				return wallList[1];
			}
			return wallList[0];
		}

		private void ArrangeSiegeMachinesForNonAssaultMission()
		{
			foreach (GameEntity gameEntity in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<SiegeWeapon>())
			{
				SiegeWeapon firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeWeapon>();
				if (!(firstScriptOfType is RangedSiegeWeapon))
				{
					firstScriptOfType.Deactivate();
				}
			}
		}

		private const string SallyOutTag = "sally_out";

		private const string AssaultTag = "siege_assault";

		private const string DamageDecalTag = "damage_decal";

		private float[] _wallHitPointPercentages;

		private bool _hasAnySiegeTower;

		private SiegeMissionPreparationHandler.SiegeMissionType _siegeMissionType;

		private enum SiegeMissionType
		{
			Assault,
			SallyOut,
			ReliefForce
		}
	}
}
