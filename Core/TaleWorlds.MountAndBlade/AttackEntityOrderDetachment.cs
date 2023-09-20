using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class AttackEntityOrderDetachment : IDetachment
	{
		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		public bool IsLoose
		{
			get
			{
				return true;
			}
		}

		public bool IsActive
		{
			get
			{
				return this._targetEntityDestructableComponent != null && !this._targetEntityDestructableComponent.IsDestroyed;
			}
		}

		public AttackEntityOrderDetachment(GameEntity targetEntity)
		{
			this._targetEntity = targetEntity;
			this._targetEntityDestructableComponent = this._targetEntity.GetFirstScriptOfType<DestructableComponent>();
			this._surroundEntity = this._targetEntity.GetFirstScriptOfType<CastleGate>() == null;
			this._agents = new List<Agent>();
			this._userFormations = new MBList<Formation>();
			MatrixFrame globalFrame = this._targetEntity.GetGlobalFrame();
			this._frame = new WorldFrame(globalFrame.rotation, new WorldPosition(targetEntity.GetScenePointer(), UIntPtr.Zero, globalFrame.origin, false));
			this._frame.Rotation.Orthonormalize();
		}

		public Vec3 GetPosition()
		{
			return this._frame.Origin.GetGroundVec3();
		}

		public void AddAgent(Agent agent, int slotIndex)
		{
			this._agents.Add(agent);
			agent.SetScriptedTargetEntityAndPosition(this._targetEntity, new WorldPosition(agent.Mission.Scene, UIntPtr.Zero, this._targetEntity.GlobalPosition, false), this._surroundEntity ? Agent.AISpecialCombatModeFlags.SurroundAttackEntity : Agent.AISpecialCombatModeFlags.None, false);
		}

		public void AddAgentAtSlotIndex(Agent agent, int slotIndex)
		{
			this.AddAgent(agent, slotIndex);
			Formation formation = agent.Formation;
			if (formation != null)
			{
				formation.DetachUnit(agent, true);
			}
			agent.Detachment = this;
			agent.DetachmentWeight = 1f;
		}

		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			return null;
		}

		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
			for (int i = this._agents.Count; i < 8; i++)
			{
				slotIndexWeightTuples.Add(new ValueTuple<int, float>(i, AttackEntityOrderDetachment.CalculateWeight(i)));
			}
		}

		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return slotIndex < 8 && slotIndex >= this._agents.Count && agent.CanBeAssignedForScriptedMovement() && !this.IsAgentOnInconvenientNavmesh(agent);
		}

		private bool IsAgentOnInconvenientNavmesh(Agent agent)
		{
			if (Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege)
			{
				return false;
			}
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			TeamAISiegeComponent teamAISiegeComponent;
			if ((teamAISiegeComponent = agent.Team.TeamAI as TeamAISiegeComponent) != null)
			{
				if (teamAISiegeComponent is TeamAISiegeAttacker && currentNavigationFaceId % 10 == 1)
				{
					return true;
				}
				if (teamAISiegeComponent is TeamAISiegeDefender && currentNavigationFaceId % 10 != 1)
				{
					return true;
				}
				foreach (int num in teamAISiegeComponent.DifficultNavmeshIDs)
				{
					if (currentNavigationFaceId == num)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		bool IDetachment.IsAgentEligible(Agent agent)
		{
			return true;
		}

		void IDetachment.UnmarkDetachment()
		{
		}

		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return false;
		}

		void IDetachment.MarkSlotAtIndex(int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\TeamAI\\AttackEntityOrderDetachment.cs", "MarkSlotAtIndex", 160);
		}

		bool IDetachment.IsAgentUsingOrInterested(Agent agent)
		{
			return this._agents.Contains(agent);
		}

		void IDetachment.OnFormationLeave(Formation formation)
		{
			for (int i = this._agents.Count - 1; i >= 0; i--)
			{
				Agent agent = this._agents[i];
				if (agent.Formation == formation && !agent.IsPlayerControlled)
				{
					((IDetachment)this).RemoveAgent(agent);
					formation.AttachUnit(agent);
				}
			}
		}

		public bool IsStandingPointAvailableForAgent(Agent agent)
		{
			return this._agents.Count < 8;
		}

		public List<float> GetTemplateCostsOfAgent(Agent candidate, List<float> oldValue)
		{
			WorldPosition worldPosition = candidate.GetWorldPosition();
			WorldPosition origin = this._frame.Origin;
			origin.SetVec2(origin.AsVec2 + this._frame.Rotation.f.AsVec2.Normalized() * 0.7f);
			float num2;
			float num = (Mission.Current.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref origin, candidate.Monster.BodyCapsuleRadius, out num2) ? num2 : float.MaxValue);
			num *= MissionGameModels.Current.AgentStatCalculateModel.GetDetachmentCostMultiplierOfAgent(candidate, this);
			List<float> list = oldValue ?? new List<float>(8);
			list.Clear();
			for (int i = 0; i < 8; i++)
			{
				list.Add(num);
			}
			return list;
		}

		float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\TeamAI\\AttackEntityOrderDetachment.cs", "GetExactCostOfAgentAtSlot", 216);
			return 0f;
		}

		public float GetTemplateWeightOfAgent(Agent candidate)
		{
			WorldPosition worldPosition = candidate.GetWorldPosition();
			WorldPosition origin = this._frame.Origin;
			float num;
			if (!Mission.Current.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref origin, candidate.Monster.BodyCapsuleRadius, out num))
			{
				return float.MaxValue;
			}
			return num;
		}

		public float? GetWeightOfAgentAtNextSlot(List<Agent> newAgents, out Agent match)
		{
			float? weightOfNextSlot = this.GetWeightOfNextSlot(newAgents[0].Team.Side);
			if (this._agents.Count < 8)
			{
				Vec3 position = this._targetEntity.GlobalPosition;
				match = newAgents.MinBy((Agent a) => a.Position.DistanceSquared(position));
				return weightOfNextSlot;
			}
			match = null;
			return null;
		}

		public float? GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> agentTemplateScores, out Agent match)
		{
			float? weight = this.GetWeightOfNextSlot(agentTemplateScores[0].Item1.Team.Side);
			if (this._agents.Count < 8)
			{
				IEnumerable<ValueTuple<Agent, float>> enumerable = agentTemplateScores.Where(delegate(ValueTuple<Agent, float> a)
				{
					Agent item = a.Item1;
					if (item.IsDetachedFromFormation)
					{
						float detachmentWeight = item.DetachmentWeight;
						float? num = weight * 0.4f;
						return (detachmentWeight < num.GetValueOrDefault()) & (num != null);
					}
					return true;
				});
				if (enumerable.Any<ValueTuple<Agent, float>>())
				{
					match = enumerable.MinBy((ValueTuple<Agent, float> a) => a.Item2).Item1;
					return weight;
				}
			}
			match = null;
			return null;
		}

		public float? GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> newAgents, out Agent match)
		{
			float weightOfOccupiedSlot = this.GetWeightOfOccupiedSlot(detachedAgent);
			Vec3 position = this._targetEntity.GlobalPosition;
			match = newAgents.MinBy((Agent a) => a.Position.DistanceSquared(position));
			return new float?(weightOfOccupiedSlot * 0.5f);
		}

		public void RemoveAgent(Agent agent)
		{
			this._agents.Remove(agent);
			agent.DisableScriptedMovement();
			agent.DisableScriptedCombatMovement();
		}

		public WorldFrame? GetAgentFrame(Agent agent)
		{
			return null;
		}

		private static float CalculateWeight(int index)
		{
			return 1f + (1f - (float)index / 8f);
		}

		public float? GetWeightOfNextSlot(BattleSideEnum side)
		{
			if (this._agents.Count < 8)
			{
				return new float?(AttackEntityOrderDetachment.CalculateWeight(this._agents.Count));
			}
			return null;
		}

		public float GetWeightOfOccupiedSlot(Agent agent)
		{
			return AttackEntityOrderDetachment.CalculateWeight(this._agents.IndexOf(agent));
		}

		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			if (this._agents.Count < 8)
			{
				return (float)(8 - this._agents.Count) * 1f / 8f;
			}
			return float.MinValue;
		}

		void IDetachment.ResetEvaluation()
		{
			this._isEvaluated = false;
		}

		bool IDetachment.IsEvaluated()
		{
			return this._isEvaluated;
		}

		void IDetachment.SetAsEvaluated()
		{
			this._isEvaluated = true;
		}

		float IDetachment.GetDetachmentWeightFromCache()
		{
			return this._cachedDetachmentWeight;
		}

		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			this._cachedDetachmentWeight = ((IDetachment)this).GetDetachmentWeight(side);
			return this._cachedDetachmentWeight;
		}

		private const int Capacity = 8;

		private readonly List<Agent> _agents;

		private readonly MBList<Formation> _userFormations;

		private WorldFrame _frame;

		private readonly GameEntity _targetEntity;

		private readonly DestructableComponent _targetEntityDestructableComponent;

		private readonly bool _surroundEntity;

		private bool _isEvaluated;

		private float _cachedDetachmentWeight;
	}
}
