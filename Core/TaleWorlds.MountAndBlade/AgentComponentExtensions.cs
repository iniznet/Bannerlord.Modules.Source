using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class AgentComponentExtensions
	{
		public static float GetMorale(this Agent agent)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				return commonAIComponent.Morale;
			}
			return -1f;
		}

		public static void SetMorale(this Agent agent, float morale)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				commonAIComponent.Morale = morale;
			}
		}

		public static void ChangeMorale(this Agent agent, float delta)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent != null)
			{
				commonAIComponent.Morale += delta;
			}
		}

		public static bool IsRetreating(this Agent agent, bool isComponentAssured = true)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			return commonAIComponent != null && commonAIComponent.IsRetreating;
		}

		public static void Retreat(this Agent agent, bool useCachingSystem = false)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent == null)
			{
				return;
			}
			commonAIComponent.Retreat(useCachingSystem);
		}

		public static void StopRetreatingMoraleComponent(this Agent agent)
		{
			CommonAIComponent commonAIComponent = agent.CommonAIComponent;
			if (commonAIComponent == null)
			{
				return;
			}
			commonAIComponent.StopRetreating();
		}

		public static void SetBehaviorValueSet(this Agent agent, HumanAIComponent.BehaviorValueSet behaviorValueSet)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return;
			}
			humanAIComponent.SetBehaviorValueSet(behaviorValueSet);
		}

		public static void RefreshBehaviorValues(this Agent agent, MovementOrder.MovementOrderEnum movementOrder, ArrangementOrder.ArrangementOrderEnum arrangementOrder)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return;
			}
			humanAIComponent.RefreshBehaviorValues(movementOrder, arrangementOrder);
		}

		public static void SetAIBehaviorValues(this Agent agent, HumanAIComponent.AISimpleBehaviorKind behavior, float y1, float x2, float y2, float x3, float y3)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return;
			}
			humanAIComponent.SetBehaviorParams(behavior, y1, x2, y2, x3, y3);
		}

		public static void AIMoveToGameObjectEnable(this Agent agent, UsableMissionObject usedObject, IDetachment detachment, Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
		{
			agent.HumanAIComponent.MoveToUsableGameObject(usedObject, detachment, scriptedFrameFlags);
		}

		public static void AIMoveToGameObjectDisable(this Agent agent)
		{
			agent.HumanAIComponent.MoveToClear();
		}

		public static bool AIMoveToGameObjectIsEnabled(this Agent agent)
		{
			return agent.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.UseObjectMoving);
		}

		public static void AIDefendGameObjectEnable(this Agent agent, UsableMissionObject usedObject, IDetachment detachment)
		{
			agent.HumanAIComponent.StartDefendingGameObject(usedObject, detachment);
		}

		public static void AIDefendGameObjectDisable(this Agent agent)
		{
			agent.HumanAIComponent.StopDefendingGameObject();
		}

		public static bool AIDefendGameObjectIsEnabled(this Agent agent)
		{
			return agent.HumanAIComponent.IsDefending;
		}

		public static bool AIInterestedInAnyGameObject(this Agent agent)
		{
			return agent.HumanAIComponent.IsInterestedInAnyGameObject();
		}

		public static bool AIInterestedInGameObject(this Agent agent, UsableMissionObject usableMissionObject)
		{
			return agent.HumanAIComponent.IsInterestedInGameObject(usableMissionObject);
		}

		public static void AIUseGameObjectEnable(this Agent agent)
		{
			agent.AIStateFlags |= Agent.AIStateFlag.UseObjectUsing;
		}

		public static void AIUseGameObjectDisable(this Agent agent)
		{
			agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectUsing;
		}

		public static bool AIUseGameObjectIsEnabled(this Agent agent)
		{
			return agent.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.UseObjectUsing);
		}

		public static Agent GetFollowedUnit(this Agent agent)
		{
			HumanAIComponent humanAIComponent = agent.HumanAIComponent;
			if (humanAIComponent == null)
			{
				return null;
			}
			return humanAIComponent.FollowedAgent;
		}

		public static void SetFollowedUnit(this Agent agent, Agent followedUnit)
		{
			agent.HumanAIComponent.FollowAgent(followedUnit);
		}
	}
}
