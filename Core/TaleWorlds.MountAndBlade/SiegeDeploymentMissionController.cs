using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000280 RID: 640
	public class SiegeDeploymentMissionController : DeploymentMissionController
	{
		// Token: 0x060021FD RID: 8701 RVA: 0x0007C583 File Offset: 0x0007A783
		public SiegeDeploymentMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x0007C58C File Offset: 0x0007A78C
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._siegeDeploymentHandler = base.Mission.GetMissionBehavior<SiegeDeploymentHandler>();
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x0007C5A5 File Offset: 0x0007A7A5
		public override void AfterStart()
		{
			base.Mission.GetMissionBehavior<DeploymentHandler>().InitializeDeploymentPoints();
			base.AfterStart();
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x0007C5C0 File Offset: 0x0007A7C0
		protected override void SetupTeamsOfSide(BattleSideEnum side)
		{
			this._siegeDeploymentHandler.RemoveAllBoundaries();
			this._siegeDeploymentHandler.SetDeploymentBoundary(side);
			Team team = ((side == BattleSideEnum.Attacker) ? base.Mission.AttackerTeam : base.Mission.DefenderTeam);
			if (team == base.Mission.PlayerTeam)
			{
				this._siegeDeploymentHandler.RemoveUnavailableDeploymentPoints(side);
				this._siegeDeploymentHandler.UnHideDeploymentPoints(side);
			}
			else
			{
				this._siegeDeploymentHandler.DeployAllSiegeWeaponsOfAi();
			}
			base.SetupTeamsOfSide(side);
			if (team == base.Mission.PlayerTeam)
			{
				foreach (Formation formation in team.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.SetControlledByAI(true, false);
					}
				}
			}
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x0007C69C File Offset: 0x0007A89C
		protected void OnSiegeSideDeploymentFinished(BattleSideEnum side)
		{
			this._siegeDeploymentHandler.RemoveDeploymentPoints(side);
			foreach (SiegeLadder siegeLadder in (from sl in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>()
				where !sl.GameEntity.IsVisibleIncludeParents()
				select sl).ToList<SiegeLadder>())
			{
				siegeLadder.SetDisabledSynched();
			}
			base.OnSideDeploymentFinished(side);
			this._siegeDeploymentHandler.RemoveAllBoundaries();
			this.MissionBoundaryPlacer.AddBoundaries();
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x0007C748 File Offset: 0x0007A948
		public override void OnBeforeDeploymentFinished()
		{
			this.OnSiegeSideDeploymentFinished(base.Mission.PlayerTeam.Side);
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x0007C760 File Offset: 0x0007A960
		public override void OnAfterDeploymentFinished()
		{
			base.Mission.RemoveMissionBehavior(this._siegeDeploymentHandler);
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x0007C774 File Offset: 0x0007A974
		public List<ItemObject> GetSiegeMissiles()
		{
			List<ItemObject> list = new List<ItemObject>();
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("grapeshot_fire_projectile");
			list.Add(@object);
			foreach (GameEntity gameEntity in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<RangedSiegeWeapon>())
			{
				RangedSiegeWeapon firstScriptOfType = gameEntity.GetFirstScriptOfType<RangedSiegeWeapon>();
				if (!string.IsNullOrEmpty(firstScriptOfType.MissileItemID))
				{
					ItemObject object2 = MBObjectManager.Instance.GetObject<ItemObject>(firstScriptOfType.MissileItemID);
					if (!list.Contains(object2))
					{
						list.Add(object2);
					}
				}
			}
			foreach (GameEntity gameEntity2 in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<StonePile>())
			{
				StonePile firstScriptOfType2 = gameEntity2.GetFirstScriptOfType<StonePile>();
				if (!string.IsNullOrEmpty(firstScriptOfType2.GivenItemID))
				{
					ItemObject object3 = MBObjectManager.Instance.GetObject<ItemObject>(firstScriptOfType2.GivenItemID);
					if (!list.Contains(object3))
					{
						list.Add(object3);
					}
				}
			}
			return list;
		}

		// Token: 0x04000CC7 RID: 3271
		private SiegeDeploymentHandler _siegeDeploymentHandler;
	}
}
