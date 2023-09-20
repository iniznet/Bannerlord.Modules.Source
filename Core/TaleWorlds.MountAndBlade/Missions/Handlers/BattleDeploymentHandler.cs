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
	}
}
