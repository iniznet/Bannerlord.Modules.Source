using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class WoundAllEnemiesCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			this.KillAllEnemies();
		}

		private void KillAllEnemies()
		{
			Mission mission = Mission.Current;
			MBReadOnlyList<Agent> mbreadOnlyList = ((mission != null) ? mission.Agents : null);
			Mission mission2 = Mission.Current;
			Agent agent = ((mission2 != null) ? mission2.MainAgent : null);
			Mission mission3 = Mission.Current;
			Team team = ((mission3 != null) ? mission3.PlayerTeam : null);
			if (mbreadOnlyList == null || team == null)
			{
				return;
			}
			for (int i = mbreadOnlyList.Count - 1; i >= 0; i--)
			{
				Agent agent2 = mbreadOnlyList[i];
				if (agent2 != agent && Extensions.HasAnyFlag<AgentFlag>(agent2.GetAgentFlags(), 8) && team != null && agent2.Team.IsValid && team.IsEnemyOf(agent2.Team))
				{
					this.KillAgent(agent2);
				}
			}
		}

		private void KillAgent(Agent agent)
		{
			Mission mission = Mission.Current;
			Agent agent2 = ((mission != null) ? mission.MainAgent : null) ?? agent;
			Blow blow;
			blow..ctor(agent2.Index);
			blow.DamageType = 2;
			blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
			blow.GlobalPosition = agent.Position;
			blow.GlobalPosition.z = blow.GlobalPosition.z + agent.GetEyeGlobalHeight();
			blow.BaseMagnitude = 2000f;
			blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
			blow.InflictedDamage = 2000;
			blow.SwingDirection = agent.LookDirection;
			blow.Direction = blow.SwingDirection;
			blow.DamageCalculated = true;
			sbyte mainHandItemBoneIndex = agent2.Monster.MainHandItemBoneIndex;
			AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, 1, -1, 0, 2, blow.BoneIndex, 0, mainHandItemBoneIndex, 2, -1, 0, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
			agent.RegisterBlow(blow, ref attackCollisionDataForDebugPurpose);
		}

		public override TextObject GetName()
		{
			return new TextObject("{=FJ93PXVa}Wound All Enemies", null);
		}
	}
}
