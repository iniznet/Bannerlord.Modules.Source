using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Missions.Handlers
{
	public class BattleDeploymentHandler : DeploymentHandler
	{
		public event Action OnDeploymentReady;

		public BattleDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		public override void OnTeamDeployed(Team team)
		{
			if (team.IsPlayerTeam)
			{
				Action onDeploymentReady = this.OnDeploymentReady;
				if (onDeploymentReady == null)
				{
					return;
				}
				onDeploymentReady();
			}
		}

		protected bool Mission_IsFormationUnitPositionAvailable_AdditionalCondition(WorldPosition position, Team team)
		{
			if (team != null && team.IsPlayerTeam && team.Side == BattleSideEnum.Defender)
			{
				Scene scene = base.Mission.Scene;
				Vec3 globalPosition = scene.FindEntityWithTag("defender_infantry").GlobalPosition;
				WorldPosition worldPosition = new WorldPosition(scene, UIntPtr.Zero, globalPosition, false);
				return scene.DoesPathExistBetweenPositions(worldPosition, position);
			}
			return true;
		}

		public override void FinishDeployment()
		{
			base.FinishDeployment();
			Mission mission = base.Mission ?? Mission.Current;
			mission.GetMissionBehavior<DeploymentMissionController>().FinishDeployment();
			mission.IsTeleportingAgents = false;
		}

		public Vec2 GetEstimatedAverageDefenderPosition()
		{
			WorldPosition worldPosition;
			Vec2 vec;
			base.Mission.GetFormationSpawnFrame(BattleSideEnum.Defender, FormationClass.Infantry, false, out worldPosition, out vec);
			return worldPosition.AsVec2;
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.Mission.IsFormationUnitPositionAvailable_AdditionalCondition -= this.Mission_IsFormationUnitPositionAvailable_AdditionalCondition;
		}
	}
}
