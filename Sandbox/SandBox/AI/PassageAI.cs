using System;
using System.Diagnostics;
using SandBox.Objects;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.AI
{
	// Token: 0x020000CB RID: 203
	public class PassageAI : UsableMachineAIBase
	{
		// Token: 0x06000C0F RID: 3087 RVA: 0x0005F138 File Offset: 0x0005D338
		public PassageAI(UsableMachine usableMachine)
			: base(usableMachine)
		{
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0005F141 File Offset: 0x0005D341
		protected override Agent.AIScriptedFrameFlags GetScriptedFrameFlags(Agent agent)
		{
			if (agent.CurrentWatchState != 2)
			{
				return 18;
			}
			return 10;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0005F154 File Offset: 0x0005D354
		protected override void OnTick(Agent agentToCompareTo, Formation formationToCompareTo, Team potentialUsersTeam, float dt)
		{
			foreach (StandingPoint standingPoint in this.UsableMachine.StandingPoints)
			{
				PassageUsePoint passageUsePoint = (PassageUsePoint)standingPoint;
				if ((agentToCompareTo == null || passageUsePoint.UserAgent == agentToCompareTo) && (formationToCompareTo == null || (passageUsePoint.UserAgent != null && passageUsePoint.UserAgent.IsAIControlled && passageUsePoint.UserAgent.Formation == formationToCompareTo)))
				{
					Debug.FailedAssert("isAgentManagedByThisMachineAI(standingPoint.UserAgent)", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\AI\\PassageAI.cs", "OnTick", 41);
					Agent userAgent = passageUsePoint.UserAgent;
					if (this.HasActionCompleted || (potentialUsersTeam != null && this.UsableMachine.IsDisabledForBattleSideAI(potentialUsersTeam.Side)) || userAgent.IsRunningAway)
					{
						this.HandleAgentStopUsingStandingPoint(userAgent, passageUsePoint);
					}
				}
				for (int i = passageUsePoint.MovingAgents.Count - 1; i >= 0; i--)
				{
					Agent agent = passageUsePoint.MovingAgents[i];
					if ((agentToCompareTo == null || agent == agentToCompareTo) && (formationToCompareTo == null || (agent != null && agent.IsAIControlled && agent.Formation == formationToCompareTo)))
					{
						if (this.HasActionCompleted || (potentialUsersTeam != null && this.UsableMachine.IsDisabledForBattleSideAI(potentialUsersTeam.Side)) || agent.IsRunningAway)
						{
							Debug.FailedAssert("HasActionCompleted || (potentialUsersTeam != null && UsableMachine.IsDisabledForBattleSideAI(potentialUsersTeam.Side)) || agent.IsRunningAway", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\AI\\PassageAI.cs", "OnTick", 69);
							this.HandleAgentStopUsingStandingPoint(agent, passageUsePoint);
						}
						else if (!passageUsePoint.IsDisabled && !agent.IsPaused && agent.CanReachAndUseObject(passageUsePoint, passageUsePoint.GetUserFrameForAgent(agent).Origin.GetGroundVec3().DistanceSquared(agent.Position)))
						{
							agent.UseGameObject(passageUsePoint, -1);
							agent.SetScriptedFlags(agent.GetScriptedFlags() & ~passageUsePoint.DisableScriptedFrameFlags);
						}
					}
				}
			}
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0005F33C File Offset: 0x0005D53C
		[Conditional("DEBUG")]
		private void TickForDebug()
		{
			if (Input.DebugInput.IsHotKeyDown("UsableMachineAiBaseHotkeyShowMachineUsers"))
			{
				foreach (StandingPoint standingPoint in this.UsableMachine.StandingPoints)
				{
					PassageUsePoint passageUsePoint = (PassageUsePoint)standingPoint;
					foreach (Agent agent in passageUsePoint.MovingAgents)
					{
					}
					Agent userAgent = passageUsePoint.UserAgent;
				}
			}
		}
	}
}
