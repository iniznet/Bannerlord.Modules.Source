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
	// Token: 0x02000072 RID: 114
	public class FleeBehavior : AgentBehavior
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x00023295 File Offset: 0x00021495
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x000232A0 File Offset: 0x000214A0
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

		// Token: 0x060004F0 RID: 1264 RVA: 0x00023350 File Offset: 0x00021550
		public FleeBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			this._missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
			this._reconsiderFleeTargetTimer = new BasicMissionTimer();
			this._state = FleeBehavior.State.None;
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00023390 File Offset: 0x00021590
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

		// Token: 0x060004F2 RID: 1266 RVA: 0x000234EC File Offset: 0x000216EC
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

		// Token: 0x060004F3 RID: 1267 RVA: 0x00023574 File Offset: 0x00021774
		private bool IsThereDanger()
		{
			return this._missionFightHandler != null && this._missionFightHandler.GetDangerSources(base.OwnerAgent).Any<Agent>();
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00023598 File Offset: 0x00021798
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

		// Token: 0x060004F5 RID: 1269 RVA: 0x0002377C File Offset: 0x0002197C
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

		// Token: 0x060004F6 RID: 1270 RVA: 0x000238A0 File Offset: 0x00021AA0
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

		// Token: 0x060004F7 RID: 1271 RVA: 0x00023954 File Offset: 0x00021B54
		private bool IsThereASafePlaceToEscape()
		{
			if (!this.GetAvailablePassageScores(1).Any((ValueTuple<float, Passage> d) => d.Item1 > 1f))
			{
				return this.GetAvailableGuardScores(1).Any((ValueTuple<float, Agent> d) => d.Item1 > 1f);
			}
			return true;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x000239BC File Offset: 0x00021BBC
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

		// Token: 0x060004F9 RID: 1273 RVA: 0x00023B38 File Offset: 0x00021D38
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

		// Token: 0x060004FA RID: 1274 RVA: 0x00023CA4 File Offset: 0x00021EA4
		protected override void OnActivate()
		{
			base.OnActivate();
			this._state = FleeBehavior.State.None;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00023CB4 File Offset: 0x00021EB4
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

		// Token: 0x060004FC RID: 1276 RVA: 0x00023D7B File Offset: 0x00021F7B
		private void BeAfraid()
		{
			this._scareTimer = new BasicMissionTimer();
			this._scareTime = 0.5f + MBRandom.RandomFloat * 0.5f;
			this._state = FleeBehavior.State.Afraid;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00023DA6 File Offset: 0x00021FA6
		public override string GetDebugInfo()
		{
			return "Flee " + this._state;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00023DBD File Offset: 0x00021FBD
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

		// Token: 0x0400025A RID: 602
		private static readonly ActionIndexCache act_scared_reaction_1 = ActionIndexCache.Create("act_scared_reaction_1");

		// Token: 0x0400025B RID: 603
		private static readonly ActionIndexCache act_scared_idle_1 = ActionIndexCache.Create("act_scared_idle_1");

		// Token: 0x0400025C RID: 604
		private static readonly ActionIndexCache act_cheer_1 = ActionIndexCache.Create("act_cheer_1");

		// Token: 0x0400025D RID: 605
		public const float ScoreThreshold = 1f;

		// Token: 0x0400025E RID: 606
		public const float DangerDistance = 5f;

		// Token: 0x0400025F RID: 607
		public const float ImmediateDangerDistance = 2f;

		// Token: 0x04000260 RID: 608
		public const float DangerDistanceSquared = 25f;

		// Token: 0x04000261 RID: 609
		public const float ImmediateDangerDistanceSquared = 4f;

		// Token: 0x04000262 RID: 610
		private readonly MissionAgentHandler _missionAgentHandler;

		// Token: 0x04000263 RID: 611
		private readonly MissionFightHandler _missionFightHandler;

		// Token: 0x04000264 RID: 612
		private FleeBehavior.State _state;

		// Token: 0x04000265 RID: 613
		private readonly BasicMissionTimer _reconsiderFleeTargetTimer;

		// Token: 0x04000266 RID: 614
		private const float ReconsiderImmobilizedFleeTargetTime = 0.5f;

		// Token: 0x04000267 RID: 615
		private const float ReconsiderDefaultFleeTargetTime = 1f;

		// Token: 0x04000268 RID: 616
		private FleeBehavior.FleeGoalBase _selectedGoal;

		// Token: 0x04000269 RID: 617
		private BasicMissionTimer _scareTimer;

		// Token: 0x0400026A RID: 618
		private float _scareTime;

		// Token: 0x0400026B RID: 619
		private BasicMissionTimer _complainToGuardTimer;

		// Token: 0x0400026C RID: 620
		private const float ComplainToGuardTime = 2f;

		// Token: 0x0400026D RID: 621
		private FleeBehavior.FleeTargetType _selectedFleeTargetType;

		// Token: 0x0200013B RID: 315
		private abstract class FleeGoalBase
		{
			// Token: 0x06000D4E RID: 3406 RVA: 0x00063626 File Offset: 0x00061826
			protected FleeGoalBase(AgentNavigator navigator, Agent ownerAgent)
			{
				this._navigator = navigator;
				this._ownerAgent = ownerAgent;
			}

			// Token: 0x06000D4F RID: 3407
			public abstract void TargetReached();

			// Token: 0x06000D50 RID: 3408
			public abstract void GoToTarget();

			// Token: 0x06000D51 RID: 3409
			public abstract bool IsGoalAchievable();

			// Token: 0x06000D52 RID: 3410
			public abstract bool IsGoalAchieved();

			// Token: 0x040005DB RID: 1499
			protected readonly AgentNavigator _navigator;

			// Token: 0x040005DC RID: 1500
			protected readonly Agent _ownerAgent;
		}

		// Token: 0x0200013C RID: 316
		private class FleeAgentTarget : FleeBehavior.FleeGoalBase
		{
			// Token: 0x170000EF RID: 239
			// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0006363C File Offset: 0x0006183C
			// (set) Token: 0x06000D54 RID: 3412 RVA: 0x00063644 File Offset: 0x00061844
			public Agent Savior { get; private set; }

			// Token: 0x06000D55 RID: 3413 RVA: 0x0006364D File Offset: 0x0006184D
			public FleeAgentTarget(AgentNavigator navigator, Agent ownerAgent, Agent savior)
				: base(navigator, ownerAgent)
			{
				this.Savior = savior;
			}

			// Token: 0x06000D56 RID: 3414 RVA: 0x00063660 File Offset: 0x00061860
			public override void GoToTarget()
			{
				this._navigator.SetTargetFrame(this.Savior.GetWorldPosition(), this.Savior.Frame.rotation.f.AsVec2.RotationInRadians, 0.2f, 0.02f, 10, false);
			}

			// Token: 0x06000D57 RID: 3415 RVA: 0x000636B8 File Offset: 0x000618B8
			public override bool IsGoalAchievable()
			{
				return this.Savior.GetWorldPosition().GetNearestNavMesh() != UIntPtr.Zero && this._navigator.TargetPosition.IsValid && this.Savior.IsActive() && this.Savior.CurrentWatchState != 2;
			}

			// Token: 0x06000D58 RID: 3416 RVA: 0x0006371C File Offset: 0x0006191C
			public override bool IsGoalAchieved()
			{
				return this._navigator.TargetPosition.IsValid && this._navigator.TargetPosition.GetGroundVec3().Distance(this._ownerAgent.Position) <= this._ownerAgent.GetInteractionDistanceToUsable(this.Savior);
			}

			// Token: 0x06000D59 RID: 3417 RVA: 0x0006377C File Offset: 0x0006197C
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

		// Token: 0x0200013D RID: 317
		private class FleePassageTarget : FleeBehavior.FleeGoalBase
		{
			// Token: 0x170000F0 RID: 240
			// (get) Token: 0x06000D5A RID: 3418 RVA: 0x0006382D File Offset: 0x00061A2D
			// (set) Token: 0x06000D5B RID: 3419 RVA: 0x00063835 File Offset: 0x00061A35
			public Passage EscapePortal { get; private set; }

			// Token: 0x06000D5C RID: 3420 RVA: 0x0006383E File Offset: 0x00061A3E
			public FleePassageTarget(AgentNavigator navigator, Agent ownerAgent, Passage escapePortal)
				: base(navigator, ownerAgent)
			{
				this.EscapePortal = escapePortal;
			}

			// Token: 0x06000D5D RID: 3421 RVA: 0x0006384F File Offset: 0x00061A4F
			public override void GoToTarget()
			{
				this._navigator.SetTarget(this.EscapePortal, false);
			}

			// Token: 0x06000D5E RID: 3422 RVA: 0x00063863 File Offset: 0x00061A63
			public override bool IsGoalAchievable()
			{
				return this.EscapePortal.GetVacantStandingPointForAI(this._ownerAgent) != null && !this.EscapePortal.IsDestroyed;
			}

			// Token: 0x06000D5F RID: 3423 RVA: 0x00063888 File Offset: 0x00061A88
			public override bool IsGoalAchieved()
			{
				StandingPoint vacantStandingPointForAI = this.EscapePortal.GetVacantStandingPointForAI(this._ownerAgent);
				return vacantStandingPointForAI != null && vacantStandingPointForAI.IsUsableByAgent(this._ownerAgent);
			}

			// Token: 0x06000D60 RID: 3424 RVA: 0x000638B8 File Offset: 0x00061AB8
			public override void TargetReached()
			{
			}
		}

		// Token: 0x0200013E RID: 318
		private class FleePositionTarget : FleeBehavior.FleeGoalBase
		{
			// Token: 0x170000F1 RID: 241
			// (get) Token: 0x06000D61 RID: 3425 RVA: 0x000638BA File Offset: 0x00061ABA
			// (set) Token: 0x06000D62 RID: 3426 RVA: 0x000638C2 File Offset: 0x00061AC2
			public Vec3 Position { get; private set; }

			// Token: 0x06000D63 RID: 3427 RVA: 0x000638CB File Offset: 0x00061ACB
			public FleePositionTarget(AgentNavigator navigator, Agent ownerAgent, Vec3 position)
				: base(navigator, ownerAgent)
			{
				this.Position = position;
			}

			// Token: 0x06000D64 RID: 3428 RVA: 0x000638DC File Offset: 0x00061ADC
			public override void GoToTarget()
			{
			}

			// Token: 0x06000D65 RID: 3429 RVA: 0x000638E0 File Offset: 0x00061AE0
			public override bool IsGoalAchievable()
			{
				return this._navigator.TargetPosition.IsValid;
			}

			// Token: 0x06000D66 RID: 3430 RVA: 0x00063900 File Offset: 0x00061B00
			public override bool IsGoalAchieved()
			{
				return this._navigator.TargetPosition.IsValid && this._navigator.IsTargetReached();
			}

			// Token: 0x06000D67 RID: 3431 RVA: 0x0006392F File Offset: 0x00061B2F
			public override void TargetReached()
			{
			}
		}

		// Token: 0x0200013F RID: 319
		private class FleeCoverTarget : FleeBehavior.FleeGoalBase
		{
			// Token: 0x06000D68 RID: 3432 RVA: 0x00063931 File Offset: 0x00061B31
			public FleeCoverTarget(AgentNavigator navigator, Agent ownerAgent)
				: base(navigator, ownerAgent)
			{
			}

			// Token: 0x06000D69 RID: 3433 RVA: 0x0006393B File Offset: 0x00061B3B
			public override void GoToTarget()
			{
				this._ownerAgent.DisableScriptedMovement();
			}

			// Token: 0x06000D6A RID: 3434 RVA: 0x00063948 File Offset: 0x00061B48
			public override bool IsGoalAchievable()
			{
				return true;
			}

			// Token: 0x06000D6B RID: 3435 RVA: 0x0006394B File Offset: 0x00061B4B
			public override bool IsGoalAchieved()
			{
				return true;
			}

			// Token: 0x06000D6C RID: 3436 RVA: 0x0006394E File Offset: 0x00061B4E
			public override void TargetReached()
			{
			}
		}

		// Token: 0x02000140 RID: 320
		private enum State
		{
			// Token: 0x040005E1 RID: 1505
			None,
			// Token: 0x040005E2 RID: 1506
			Afraid,
			// Token: 0x040005E3 RID: 1507
			LookForPlace,
			// Token: 0x040005E4 RID: 1508
			Flee,
			// Token: 0x040005E5 RID: 1509
			Complain
		}

		// Token: 0x02000141 RID: 321
		private enum FleeTargetType
		{
			// Token: 0x040005E7 RID: 1511
			Indoor,
			// Token: 0x040005E8 RID: 1512
			Guard,
			// Token: 0x040005E9 RID: 1513
			Cover
		}
	}
}
