using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000282 RID: 642
	public class SiegeMissionPreparationHandler : MissionLogic
	{
		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06002209 RID: 8713 RVA: 0x0007C884 File Offset: 0x0007AA84
		private Scene MissionScene
		{
			get
			{
				return Mission.Current.Scene;
			}
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x0007C890 File Offset: 0x0007AA90
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

		// Token: 0x0600220B RID: 8715 RVA: 0x0007C8C6 File Offset: 0x0007AAC6
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.SetUpScene();
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x0007C8D4 File Offset: 0x0007AAD4
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

		// Token: 0x0600220D RID: 8717 RVA: 0x0007C8F8 File Offset: 0x0007AAF8
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

		// Token: 0x0600220E RID: 8718 RVA: 0x0007C96C File Offset: 0x0007AB6C
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

		// Token: 0x0600220F RID: 8719 RVA: 0x0007CA24 File Offset: 0x0007AC24
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

		// Token: 0x06002210 RID: 8720 RVA: 0x0007CDF0 File Offset: 0x0007AFF0
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

		// Token: 0x06002211 RID: 8721 RVA: 0x0007CE90 File Offset: 0x0007B090
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

		// Token: 0x04000CC8 RID: 3272
		private const string SallyOutTag = "sally_out";

		// Token: 0x04000CC9 RID: 3273
		private const string AssaultTag = "siege_assault";

		// Token: 0x04000CCA RID: 3274
		private const string DamageDecalTag = "damage_decal";

		// Token: 0x04000CCB RID: 3275
		private float[] _wallHitPointPercentages;

		// Token: 0x04000CCC RID: 3276
		private bool _hasAnySiegeTower;

		// Token: 0x04000CCD RID: 3277
		private SiegeMissionPreparationHandler.SiegeMissionType _siegeMissionType;

		// Token: 0x0200058D RID: 1421
		private enum SiegeMissionType
		{
			// Token: 0x04001D8C RID: 7564
			Assault,
			// Token: 0x04001D8D RID: 7565
			SallyOut,
			// Token: 0x04001D8E RID: 7566
			ReliefForce
		}
	}
}
