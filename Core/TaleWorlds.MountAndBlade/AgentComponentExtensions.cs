using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F6 RID: 246
	public static class AgentComponentExtensions
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x00016A4C File Offset: 0x00014C4C
		public static float GetMorale(this Agent agent)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				return commonAIComponent.Morale;
			}
			return -1f;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00016A70 File Offset: 0x00014C70
		public static void SetMorale(this Agent agent, float morale)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				commonAIComponent.Morale = morale;
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00016A90 File Offset: 0x00014C90
		public static void ChangeMorale(this Agent agent, float delta)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				commonAIComponent.Morale += delta;
			}
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00016AB8 File Offset: 0x00014CB8
		public static bool IsRetreating(this Agent agent, bool isComponentAssured = true)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			return commonAIComponent != null && commonAIComponent.IsRetreating;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x00016AD7 File Offset: 0x00014CD7
		public static void Retreat(this Agent agent)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent == null)
			{
				return;
			}
			commonAIComponent.Retreat();
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00016AE9 File Offset: 0x00014CE9
		public static void StopRetreatingMoraleComponent(this Agent agent)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent == null)
			{
				return;
			}
			commonAIComponent.StopRetreating();
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00016AFB File Offset: 0x00014CFB
		public static void SetAIBehaviorValues(this Agent agent, HumanAIComponent.AISimpleBehaviorKind behavior, float y1, float x2, float y2, float x3, float y3)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return;
			}
			humanAIComponent.SetBehaviorParams(behavior, y1, x2, y2, x3, y3);
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x00016B16 File Offset: 0x00014D16
		public static void AIMoveToGameObjectEnable(this Agent agent, UsableMissionObject usedObject, IDetachment detachment, Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
		{
			agent.HumanAIComponent.MoveToUsableGameObject(usedObject, detachment, scriptedFrameFlags);
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x00016B26 File Offset: 0x00014D26
		public static void AIMoveToGameObjectDisable(this Agent agent)
		{
			agent.HumanAIComponent.MoveToClear();
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00016B33 File Offset: 0x00014D33
		public static bool AIMoveToGameObjectIsEnabled(this Agent agent)
		{
			return agent.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.UseObjectMoving);
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00016B41 File Offset: 0x00014D41
		public static void AIDefendGameObjectEnable(this Agent agent, UsableMissionObject usedObject, IDetachment detachment, Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
		{
			agent.HumanAIComponent.StartDefendingGameObject(usedObject, detachment);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x00016B50 File Offset: 0x00014D50
		public static void AIDefendGameObjectDisable(this Agent agent)
		{
			agent.HumanAIComponent.StopDefendingGameObject();
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x00016B5D File Offset: 0x00014D5D
		public static bool AIDefendGameObjectIsEnabled(this Agent agent)
		{
			return agent.HumanAIComponent.IsDefending;
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00016B6A File Offset: 0x00014D6A
		public static bool AIInterestedInAnyGameObject(this Agent agent)
		{
			return agent.HumanAIComponent.IsInterestedInAnyGameObject();
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x00016B77 File Offset: 0x00014D77
		public static bool AIInterestedInGameObject(this Agent agent, UsableMissionObject usableMissionObject)
		{
			return agent.HumanAIComponent.IsInterestedInGameObject(usableMissionObject);
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00016B85 File Offset: 0x00014D85
		public static void AIUseGameObjectEnable(this Agent agent)
		{
			agent.AIStateFlags |= Agent.AIStateFlag.UseObjectUsing;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00016B96 File Offset: 0x00014D96
		public static void AIUseGameObjectDisable(this Agent agent)
		{
			agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectUsing;
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x00016BA7 File Offset: 0x00014DA7
		public static bool AIUseGameObjectIsEnabled(this Agent agent)
		{
			return agent.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.UseObjectUsing);
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x00016BB6 File Offset: 0x00014DB6
		public static Agent GetFollowedUnit(this Agent agent)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return null;
			}
			return humanAIComponent.FollowedAgent;
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x00016BC9 File Offset: 0x00014DC9
		public static void SetFollowedUnit(this Agent agent, Agent followedUnit)
		{
			agent.HumanAIComponent.FollowAgent(followedUnit);
		}
	}
}
