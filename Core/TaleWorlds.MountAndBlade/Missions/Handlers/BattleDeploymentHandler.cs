using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Missions.Handlers
{
	// Token: 0x02000401 RID: 1025
	public class BattleDeploymentHandler : DeploymentHandler
	{
		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06003524 RID: 13604 RVA: 0x000DDA20 File Offset: 0x000DBC20
		// (remove) Token: 0x06003525 RID: 13605 RVA: 0x000DDA58 File Offset: 0x000DBC58
		public event Action OnDeploymentReady;

		// Token: 0x06003526 RID: 13606 RVA: 0x000DDA8D File Offset: 0x000DBC8D
		public BattleDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06003527 RID: 13607 RVA: 0x000DDA96 File Offset: 0x000DBC96
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

		// Token: 0x06003528 RID: 13608 RVA: 0x000DDAB0 File Offset: 0x000DBCB0
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

		// Token: 0x06003529 RID: 13609 RVA: 0x000DDB06 File Offset: 0x000DBD06
		public override void FinishDeployment()
		{
			base.FinishDeployment();
			Mission mission = base.Mission ?? Mission.Current;
			mission.GetMissionBehavior<DeploymentMissionController>().FinishDeployment();
			mission.IsTeleportingAgents = false;
		}

		// Token: 0x0600352A RID: 13610 RVA: 0x000DDB30 File Offset: 0x000DBD30
		public Vec2 GetEstimatedAverageDefenderPosition()
		{
			WorldPosition worldPosition;
			Vec2 vec;
			base.Mission.GetFormationSpawnFrame(BattleSideEnum.Defender, FormationClass.Infantry, false, out worldPosition, out vec);
			return worldPosition.AsVec2;
		}

		// Token: 0x0600352B RID: 13611 RVA: 0x000DDB56 File Offset: 0x000DBD56
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			base.Mission.IsFormationUnitPositionAvailable_AdditionalCondition -= this.Mission_IsFormationUnitPositionAvailable_AdditionalCondition;
		}
	}
}
