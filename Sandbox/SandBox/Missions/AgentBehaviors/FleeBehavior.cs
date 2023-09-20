using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public class FleeBehavior : AgentBehavior
	{
		private FleeBehavior.FleeTargetType SelectedFleeTargetType
		{
			get
			{
				return this._selectedFleeTargetType;
			}
			set
			{
				if (value != this._selectedFleeTargetType)
				{
					this._selectedFleeTargetType = value;
					MBActionSet actionSet = base.OwnerAgent.ActionSet;
					ActionIndexValueCache currentActionValue = base.OwnerAgent.GetCurrentActionValue(1);
					if (this._selectedFleeTargetType != FleeBehavior.FleeTargetType.Cover && !actionSet.AreActionsAlternatives(currentActionValue, FleeBehavior.act_scared_idle_1) && !actionSet.AreActionsAlternatives(currentActionValue, FleeBehavior.act_scared_reaction_1))
					{
						base.OwnerAgent.SetActionChannel(1, FleeBehavior.act_scared_reaction_1, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					}
					if (this._selectedFleeTargetType == FleeBehavior.FleeTargetType.Cover)
					{
						this.BeAfraid();
					}
					this._selectedGoal.GoToTarget();
				}
			}
		}

		public FleeBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
			this._reconsiderFleeTargetTimer = new BasicMissionTimer();
			this._state = FleeBehavior.State.None;
		}

		public override void Tick(float dt, bool isSimulation)
		{
			switch (this._state)
			{
			case FleeBehavior.State.None:
				base.OwnerAgent.DisableScriptedMovement();
				base.OwnerAgent.SetActionChannel(1, FleeBehavior.act_scared_reaction_1, false, 0UL, 0f, 1f, -0.2f, 0.4f, MBRandom.RandomFloat, false, -0.2f, 0, true);
				this._selectedGoal = new FleeBehavior.FleeCoverTarget(base.Navigator, base.OwnerAgent);
				this.SelectedFleeTargetType = FleeBehavior.FleeTargetType.Cover;
				return;
			case FleeBehavior.State.Afraid:
				if (this._scareTimer.ElapsedTime > this._scareTime)
				{
					this._state = FleeBehavior.State.LookForPlace;
					this._scareTimer = null;
					return;
				}
				break;
			case FleeBehavior.State.LookForPlace:
				this.LookForPlace();
				return;
			case FleeBehavior.State.Flee:
				this.Flee();
				return;
			case FleeBehavior.State.Complain:
				if (this._complainToGuardTimer != null && this._complainToGuardTimer.ElapsedTime > 2f)
				{
					this._complainToGuardTimer = null;
					base.OwnerAgent.SetActionChannel(0, ActionIndexCache.act_none, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					base.OwnerAgent.SetLookAgent(null);
					(this._selectedGoal as FleeBehavior.FleeAgentTarget).Savior.SetLookAgent(null);
					AlarmedBehaviorGroup.AlarmAgent((this._selectedGoal as FleeBehavior.FleeAgentTarget).Savior);
					this._state = FleeBehavior.State.LookForPlace;
				}
				break;
			default:
				return;
			}
		}

		private Vec3 GetDangerPosition()
		{
			Vec3 vec = Vec3.Zero;
			if (this._missionFightHandler != null)
			{
				IEnumerable<Agent> dangerSources = this._missionFightHandler.GetDangerSources(base.OwnerAgent);
				if (dangerSources.Any<Agent>())
				{
					foreach (Agent agent in dangerSources)
					{
						vec += agent.Position;
					}
					vec /= (float)dangerSources.Count<Agent>();
				}
			}
			return vec;
		}

		private bool IsThereDanger()
		{
			return this._missionFightHandler != null && this._missionFightHandler.GetDangerSources(base.OwnerAgent).Any<Agent>();
		}

		private float GetPathScore(WorldPosition startWorldPos, WorldPosition targetWorldPos)
		{
			float num = 1f;
			NavigationPath navigationPath = new NavigationPath();
			base.Mission.Scene.GetPathBetweenAIFaces(startWorldPos.GetNearestNavMesh(), targetWorldPos.GetNearestNavMesh(), startWorldPos.AsVec2, targetWorldPos.AsVec2, 0f, navigationPath);
			Vec2 asVec = this.GetDangerPosition().AsVec2;
			float num2 = MBMath.WrapAngle((asVec - startWorldPos.AsVec2).RotationInRadians);
			float num3 = MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngle((navigationPath.Size > 0) ? (navigationPath.PathPoints[0] - startWorldPos.AsVec2).RotationInRadians : (targetWorldPos.AsVec2 - startWorldPos.AsVec2).RotationInRadians), num2)) / 3.1415927f * 1f;
			float num4 = startWorldPos.AsVec2.DistanceSquared(asVec);
			if (navigationPath.Size > 0)
			{
				float num5 = float.MaxValue;
				Vec2 vec = startWorldPos.AsVec2;
				for (int i = 0; i < navigationPath.Size; i++)
				{
					float num6 = Vec2.DistanceToLineSegmentSquared(navigationPath.PathPoints[i], vec, asVec);
					vec = navigationPath.PathPoints[i];
					if (num6 < num5)
					{
						num5 = num6;
					}
				}
				if (num4 > num5 && num5 < 25f)
				{
					num = 1f * (num5 - num4) / 225f;
				}
				else if (num4 > 4f)
				{
					num = 1f * num5 / 225f;
				}
				else
				{
					num = 1f;
				}
			}
			float num7 = 1f * (225f / startWorldPos.AsVec2.DistanceSquared(targetWorldPos.AsVec2));
			return (1f + num3) * (1f + num3) - 2f + num + num7;
		}

		private void LookForPlace()
		{
			FleeBehavior.FleeGoalBase fleeGoalBase = new FleeBehavior.FleeCoverTarget(base.Navigator, base.OwnerAgent);
			FleeBehavior.FleeTargetType fleeTargetType = FleeBehavior.FleeTargetType.Cover;
			if (this.IsThereDanger())
			{
				List<ValueTuple<float, Agent>> availableGuardScores = this.GetAvailableGuardScores(5);
				List<ValueTuple<float, Passage>> availablePassageScores = this.GetAvailablePassageScores(10);
				float num = float.MinValue;
				foreach (ValueTuple<float, Passage> valueTuple in availablePassageScores)
				{
					float item = valueTuple.Item1;
					if (item > num)
					{
						num = item;
						fleeTargetType = FleeBehavior.FleeTargetType.Indoor;
						fleeGoalBase = new FleeBehavior.FleePassageTarget(base.Navigator, base.OwnerAgent, valueTuple.Item2);
					}
				}
				foreach (ValueTuple<float, Agent> valueTuple2 in availableGuardScores)
				{
					float item2 = valueTuple2.Item1;
					if (item2 > num)
					{
						num = item2;
						fleeTargetType = FleeBehavior.FleeTargetType.Guard;
						fleeGoalBase = new FleeBehavior.FleeAgentTarget(base.Navigator, base.OwnerAgent, valueTuple2.Item2);
					}
				}
			}
			this._selectedGoal = fleeGoalBase;
			this.SelectedFleeTargetType = fleeTargetType;
			this._state = FleeBehavior.State.Flee;
		}

		private bool ShouldChangeTarget()
		{
			if (this._selectedFleeTargetType == FleeBehavior.FleeTargetType.Guard)
			{
				WorldPosition worldPosition = (this._selectedGoal as FleeBehavior.FleeAgentTarget).Savior.GetWorldPosition();
				WorldPosition worldPosition2 = base.OwnerAgent.GetWorldPosition();
				return this.GetPathScore(worldPosition2, worldPosition) <= 1f && this.IsThereASafePlaceToEscape();
			}
			if (this._selectedFleeTargetType != FleeBehavior.FleeTargetType.Indoor)
			{
				return true;
			}
			StandingPoint vacantStandingPointForAI = (this._selectedGoal as FleeBehavior.FleePassageTarget).EscapePortal.GetVacantStandingPointForAI(base.OwnerAgent);
			if (vacantStandingPointForAI == null)
			{
				return true;
			}
			WorldPosition worldPosition3 = base.OwnerAgent.GetWorldPosition();
			WorldPosition origin = vacantStandingPointForAI.GetUserFrameForAgent(base.OwnerAgent).Origin;
			return this.GetPathScore(worldPosition3, origin) <= 1f && this.IsThereASafePlaceToEscape();
		}

		private bool IsThereASafePlaceToEscape()
		{
			if (!this.GetAvailablePassageScores(1).Any((ValueTuple<float, Passage> d) => d.Item1 > 1f))
			{
				return this.GetAvailableGuardScores(1).Any((ValueTuple<float, Agent> d) => d.Item1 > 1f);
			}
			return true;
		}

		private List<ValueTuple<float, Passage>> GetAvailablePassageScores(int maxPaths = 10)
		{
			WorldPosition worldPosition = base.OwnerAgent.GetWorldPosition();
			List<ValueTuple<float, Passage>> list = new List<ValueTuple<float, Passage>>();
			List<ValueTuple<float, Passage>> list2 = new List<ValueTuple<float, Passage>>();
			List<ValueTuple<WorldPosition, Passage>> list3 = new List<ValueTuple<WorldPosition, Passage>>();
			if (this._missionAgentHandler.TownPassageProps != null)
			{
				foreach (UsableMachine usableMachine in this._missionAgentHandler.TownPassageProps)
				{
					StandingPoint vacantStandingPointForAI = usableMachine.GetVacantStandingPointForAI(base.OwnerAgent);
					Passage passage = usableMachine as Passage;
					if (vacantStandingPointForAI != null && passage != null)
					{
						WorldPosition origin = vacantStandingPointForAI.GetUserFrameForAgent(base.OwnerAgent).Origin;
						list3.Add(new ValueTuple<WorldPosition, Passage>(origin, passage));
					}
				}
			}
			list3 = list3.OrderBy((ValueTuple<WorldPosition, Passage> a) => base.OwnerAgent.Position.AsVec2.DistanceSquared(a.Item1.AsVec2)).ToList<ValueTuple<WorldPosition, Passage>>();
			foreach (ValueTuple<WorldPosition, Passage> valueTuple in list3)
			{
				WorldPosition item = valueTuple.Item1;
				if (item.IsValid && !(item.GetNearestNavMesh() == UIntPtr.Zero))
				{
					float pathScore = this.GetPathScore(worldPosition, item);
					ValueTuple<float, Passage> valueTuple2 = new ValueTuple<float, Passage>(pathScore, valueTuple.Item2);
					list.Add(valueTuple2);
					if (pathScore > 1f)
					{
						list2.Add(valueTuple2);
					}
					if (list2.Count >= maxPaths)
					{
						break;
					}
				}
			}
			if (list2.Count > 0)
			{
				return list2;
			}
			return list;
		}

		private List<ValueTuple<float, Agent>> GetAvailableGuardScores(int maxGuards = 5)
		{
			WorldPosition worldPosition = base.OwnerAgent.GetWorldPosition();
			List<ValueTuple<float, Agent>> list = new List<ValueTuple<float, Agent>>();
			List<ValueTuple<float, Agent>> list2 = new List<ValueTuple<float, Agent>>();
			List<Agent> list3 = new List<Agent>();
			foreach (Agent agent in base.OwnerAgent.Team.ActiveAgents)
			{
				CharacterObject characterObject;
				if ((characterObject = agent.Character as CharacterObject) != null && agent.IsAIControlled && agent.CurrentWatchState != 2 && (characterObject.Occupation == 7 || characterObject.Occupation == 24 || characterObject.Occupation == 23))
				{
					list3.Add(agent);
				}
			}
			list3 = list3.OrderBy((Agent a) => base.OwnerAgent.Position.DistanceSquared(a.Position)).ToList<Agent>();
			foreach (Agent agent2 in list3)
			{
				WorldPosition worldPosition2 = agent2.GetWorldPosition();
				if (worldPosition2.IsValid)
				{
					float pathScore = this.GetPathScore(worldPosition, worldPosition2);
					ValueTuple<float, Agent> valueTuple = new ValueTuple<float, Agent>(pathScore, agent2);
					list.Add(valueTuple);
					if (pathScore > 1f)
					{
						list2.Add(valueTuple);
					}
					if (list2.Count >= maxGuards)
					{
						break;
					}
				}
			}
			if (list2.Count > 0)
			{
				return list2;
			}
			return list;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this._state = FleeBehavior.State.None;
		}

		private void Flee()
		{
			if (this._selectedGoal.IsGoalAchievable())
			{
				if (this._selectedGoal.IsGoalAchieved())
				{
					this._selectedGoal.TargetReached();
					FleeBehavior.FleeTargetType selectedFleeTargetType = this.SelectedFleeTargetType;
					if (selectedFleeTargetType == FleeBehavior.FleeTargetType.Guard)
					{
						this._complainToGuardTimer = new BasicMissionTimer();
						this._state = FleeBehavior.State.Complain;
						return;
					}
					if (selectedFleeTargetType == FleeBehavior.FleeTargetType.Cover && this._reconsiderFleeTargetTimer.ElapsedTime > 0.5f)
					{
						this._state = FleeBehavior.State.LookForPlace;
						this._reconsiderFleeTargetTimer.Reset();
						return;
					}
				}
				else
				{
					if (this.SelectedFleeTargetType == FleeBehavior.FleeTargetType.Guard)
					{
						this._selectedGoal.GoToTarget();
					}
					if (this._reconsiderFleeTargetTimer.ElapsedTime > 1f)
					{
						this._reconsiderFleeTargetTimer.Reset();
						if (this.ShouldChangeTarget())
						{
							this._state = FleeBehavior.State.LookForPlace;
							return;
						}
					}
				}
			}
			else
			{
				this._state = FleeBehavior.State.LookForPlace;
			}
		}

		private void BeAfraid()
		{
			this._scareTimer = new BasicMissionTimer();
			this._scareTime = 0.5f + MBRandom.RandomFloat * 0.5f;
			this._state = FleeBehavior.State.Afraid;
		}

		public override string GetDebugInfo()
		{
			return "Flee " + this._state;
		}

		public override float GetAvailability(bool isSimulation)
		{
			if (base.Mission.CurrentTime < 3f)
			{
				return 0f;
			}
			if (!MissionFightHandler.IsAgentAggressive(base.OwnerAgent))
			{
				return 0.9f;
			}
			return 0.1f;
		}

		private static readonly ActionIndexCache act_scared_reaction_1 = ActionIndexCache.Create("act_scared_reaction_1");

		private static readonly ActionIndexCache act_scared_idle_1 = ActionIndexCache.Create("act_scared_idle_1");

		private static readonly ActionIndexCache act_cheer_1 = ActionIndexCache.Create("act_cheer_1");

		public const float ScoreThreshold = 1f;

		public const float DangerDistance = 5f;

		public const float ImmediateDangerDistance = 2f;

		public const float DangerDistanceSquared = 25f;

		public const float ImmediateDangerDistanceSquared = 4f;

		private readonly MissionAgentHandler _missionAgentHandler;

		private readonly MissionFightHandler _missionFightHandler;

		private FleeBehavior.State _state;

		private readonly BasicMissionTimer _reconsiderFleeTargetTimer;

		private const float ReconsiderImmobilizedFleeTargetTime = 0.5f;

		private const float ReconsiderDefaultFleeTargetTime = 1f;

		private FleeBehavior.FleeGoalBase _selectedGoal;

		private BasicMissionTimer _scareTimer;

		private float _scareTime;

		private BasicMissionTimer _complainToGuardTimer;

		private const float ComplainToGuardTime = 2f;

		private FleeBehavior.FleeTargetType _selectedFleeTargetType;

		private abstract class FleeGoalBase
		{
			protected FleeGoalBase(AgentNavigator navigator, Agent ownerAgent)
			{
				this._navigator = navigator;
				this._ownerAgent = ownerAgent;
			}

			public abstract void TargetReached();

			public abstract void GoToTarget();

			public abstract bool IsGoalAchievable();

			public abstract bool IsGoalAchieved();

			protected readonly AgentNavigator _navigator;

			protected readonly Agent _ownerAgent;
		}

		private class FleeAgentTarget : FleeBehavior.FleeGoalBase
		{
			public Agent Savior { get; private set; }

			public FleeAgentTarget(AgentNavigator navigator, Agent ownerAgent, Agent savior)
				: base(navigator, ownerAgent)
			{
				this.Savior = savior;
			}

			public override void GoToTarget()
			{
				this._navigator.SetTargetFrame(this.Savior.GetWorldPosition(), this.Savior.Frame.rotation.f.AsVec2.RotationInRadians, 0.2f, 0.02f, 10, false);
			}

			public override bool IsGoalAchievable()
			{
				return this.Savior.GetWorldPosition().GetNearestNavMesh() != UIntPtr.Zero && this._navigator.TargetPosition.IsValid && this.Savior.IsActive() && this.Savior.CurrentWatchState != 2;
			}

			public override bool IsGoalAchieved()
			{
				return this._navigator.TargetPosition.IsValid && this._navigator.TargetPosition.GetGroundVec3().Distance(this._ownerAgent.Position) <= this._ownerAgent.GetInteractionDistanceToUsable(this.Savior);
			}

			public override void TargetReached()
			{
				this._ownerAgent.SetActionChannel(0, FleeBehavior.act_cheer_1, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this._ownerAgent.SetActionChannel(1, ActionIndexCache.act_none, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this._ownerAgent.DisableScriptedMovement();
				this.Savior.DisableScriptedMovement();
				this.Savior.SetLookAgent(this._ownerAgent);
				this._ownerAgent.SetLookAgent(this.Savior);
			}
		}

		private class FleePassageTarget : FleeBehavior.FleeGoalBase
		{
			public Passage EscapePortal { get; private set; }

			public FleePassageTarget(AgentNavigator navigator, Agent ownerAgent, Passage escapePortal)
				: base(navigator, ownerAgent)
			{
				this.EscapePortal = escapePortal;
			}

			public override void GoToTarget()
			{
				this._navigator.SetTarget(this.EscapePortal, false);
			}

			public override bool IsGoalAchievable()
			{
				return this.EscapePortal.GetVacantStandingPointForAI(this._ownerAgent) != null && !this.EscapePortal.IsDestroyed;
			}

			public override bool IsGoalAchieved()
			{
				StandingPoint vacantStandingPointForAI = this.EscapePortal.GetVacantStandingPointForAI(this._ownerAgent);
				return vacantStandingPointForAI != null && vacantStandingPointForAI.IsUsableByAgent(this._ownerAgent);
			}

			public override void TargetReached()
			{
			}
		}

		private class FleePositionTarget : FleeBehavior.FleeGoalBase
		{
			public Vec3 Position { get; private set; }

			public FleePositionTarget(AgentNavigator navigator, Agent ownerAgent, Vec3 position)
				: base(navigator, ownerAgent)
			{
				this.Position = position;
			}

			public override void GoToTarget()
			{
			}

			public override bool IsGoalAchievable()
			{
				return this._navigator.TargetPosition.IsValid;
			}

			public override bool IsGoalAchieved()
			{
				return this._navigator.TargetPosition.IsValid && this._navigator.IsTargetReached();
			}

			public override void TargetReached()
			{
			}
		}

		private class FleeCoverTarget : FleeBehavior.FleeGoalBase
		{
			public FleeCoverTarget(AgentNavigator navigator, Agent ownerAgent)
				: base(navigator, ownerAgent)
			{
			}

			public override void GoToTarget()
			{
				this._ownerAgent.DisableScriptedMovement();
			}

			public override bool IsGoalAchievable()
			{
				return true;
			}

			public override bool IsGoalAchieved()
			{
				return true;
			}

			public override void TargetReached()
			{
			}
		}

		private enum State
		{
			None,
			Afraid,
			LookForPlace,
			Flee,
			Complain
		}

		private enum FleeTargetType
		{
			Indoor,
			Guard,
			Cover
		}
	}
}
