using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeDeploymentMissionController : DeploymentMissionController
	{
		public SiegeDeploymentMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._siegeDeploymentHandler = base.Mission.GetMissionBehavior<SiegeDeploymentHandler>();
		}

		public override void AfterStart()
		{
			base.Mission.GetMissionBehavior<DeploymentHandler>().InitializeDeploymentPoints();
			base.AfterStart();
		}

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

		public override void OnBeforeDeploymentFinished()
		{
			this.OnSiegeSideDeploymentFinished(base.Mission.PlayerTeam.Side);
		}

		public override void OnAfterDeploymentFinished()
		{
			base.Mission.RemoveMissionBehavior(this._siegeDeploymentHandler);
		}

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

		private SiegeDeploymentHandler _siegeDeploymentHandler;
	}
}
