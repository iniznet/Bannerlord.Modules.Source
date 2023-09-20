using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200016D RID: 365
	public class AttackEntityOrderDetachment : IDetachment
	{
		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060012C5 RID: 4805 RVA: 0x00048FFA File Offset: 0x000471FA
		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00049002 File Offset: 0x00047202
		public bool IsLoose
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x00049005 File Offset: 0x00047205
		public bool IsActive
		{
			get
			{
				return this._targetEntityDestructableComponent != null && !this._targetEntityDestructableComponent.IsDestroyed;
			}
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x00049020 File Offset: 0x00047220
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

		// Token: 0x060012C9 RID: 4809 RVA: 0x000490B9 File Offset: 0x000472B9
		public Vec3 GetPosition()
		{
			return this._frame.Origin.GetGroundVec3();
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x000490CC File Offset: 0x000472CC
		public void AddAgent(Agent agent, int slotIndex)
		{
			this._agents.Add(agent);
			agent.SetScriptedTargetEntityAndPosition(this._targetEntity, new WorldPosition(agent.Mission.Scene, UIntPtr.Zero, this._targetEntity.GlobalPosition, false), this._surroundEntity ? Agent.AISpecialCombatModeFlags.SurroundAttackEntity : Agent.AISpecialCombatModeFlags.None, false);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0004911F File Offset: 0x0004731F
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

		// Token: 0x060012CC RID: 4812 RVA: 0x0004914E File Offset: 0x0004734E
		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x0004915C File Offset: 0x0004735C
		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0004916B File Offset: 0x0004736B
		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x00049179 File Offset: 0x00047379
		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			return null;
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0004917C File Offset: 0x0004737C
		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
			for (int i = this._agents.Count; i < 8; i++)
			{
				slotIndexWeightTuples.Add(new ValueTuple<int, float>(i, AttackEntityOrderDetachment.CalculateWeight(i)));
			}
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x000491B1 File Offset: 0x000473B1
		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return slotIndex < 8 && slotIndex >= this._agents.Count && agent.CanBeAssignedForScriptedMovement() && !this.IsAgentOnInconvenientNavmesh(agent);
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x000491DC File Offset: 0x000473DC
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

		// Token: 0x060012D3 RID: 4819 RVA: 0x00049284 File Offset: 0x00047484
		bool IDetachment.IsAgentEligible(Agent agent)
		{
			return true;
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x00049287 File Offset: 0x00047487
		void IDetachment.UnmarkDetachment()
		{
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x00049289 File Offset: 0x00047489
		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return false;
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0004928C File Offset: 0x0004748C
		void IDetachment.MarkSlotAtIndex(int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\TeamAI\\AttackEntityOrderDetachment.cs", "MarkSlotAtIndex", 160);
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x000492A7 File Offset: 0x000474A7
		bool IDetachment.IsAgentUsingOrInterested(Agent agent)
		{
			return this._agents.Contains(agent);
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x000492B8 File Offset: 0x000474B8
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

		// Token: 0x060012D9 RID: 4825 RVA: 0x00049309 File Offset: 0x00047509
		public bool IsStandingPointAvailableForAgent(Agent agent)
		{
			return this._agents.Count < 8;
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0004931C File Offset: 0x0004751C
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

		// Token: 0x060012DB RID: 4827 RVA: 0x000493E4 File Offset: 0x000475E4
		float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\TeamAI\\AttackEntityOrderDetachment.cs", "GetExactCostOfAgentAtSlot", 216);
			return 0f;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x00049404 File Offset: 0x00047604
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

		// Token: 0x060012DD RID: 4829 RVA: 0x00049450 File Offset: 0x00047650
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

		// Token: 0x060012DE RID: 4830 RVA: 0x000494BC File Offset: 0x000476BC
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

		// Token: 0x060012DF RID: 4831 RVA: 0x0004955C File Offset: 0x0004775C
		public float? GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> newAgents, out Agent match)
		{
			float weightOfOccupiedSlot = this.GetWeightOfOccupiedSlot(detachedAgent);
			Vec3 position = this._targetEntity.GlobalPosition;
			match = newAgents.MinBy((Agent a) => a.Position.DistanceSquared(position));
			return new float?(weightOfOccupiedSlot * 0.5f);
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x000495A6 File Offset: 0x000477A6
		public void RemoveAgent(Agent agent)
		{
			this._agents.Remove(agent);
			agent.DisableScriptedMovement();
			agent.DisableScriptedCombatMovement();
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x000495C4 File Offset: 0x000477C4
		public WorldFrame? GetAgentFrame(Agent agent)
		{
			return null;
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x000495DA File Offset: 0x000477DA
		private static float CalculateWeight(int index)
		{
			return 1f + (1f - (float)index / 8f);
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x000495F0 File Offset: 0x000477F0
		public float? GetWeightOfNextSlot(BattleSideEnum side)
		{
			if (this._agents.Count < 8)
			{
				return new float?(AttackEntityOrderDetachment.CalculateWeight(this._agents.Count));
			}
			return null;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0004962A File Offset: 0x0004782A
		public float GetWeightOfOccupiedSlot(Agent agent)
		{
			return AttackEntityOrderDetachment.CalculateWeight(this._agents.IndexOf(agent));
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0004963D File Offset: 0x0004783D
		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			if (this._agents.Count < 8)
			{
				return (float)(8 - this._agents.Count) * 1f / 8f;
			}
			return float.MinValue;
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0004966D File Offset: 0x0004786D
		void IDetachment.ResetEvaluation()
		{
			this._isEvaluated = false;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x00049676 File Offset: 0x00047876
		bool IDetachment.IsEvaluated()
		{
			return this._isEvaluated;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0004967E File Offset: 0x0004787E
		void IDetachment.SetAsEvaluated()
		{
			this._isEvaluated = true;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x00049687 File Offset: 0x00047887
		float IDetachment.GetDetachmentWeightFromCache()
		{
			return this._cachedDetachmentWeight;
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0004968F File Offset: 0x0004788F
		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			this._cachedDetachmentWeight = ((IDetachment)this).GetDetachmentWeight(side);
			return this._cachedDetachmentWeight;
		}

		// Token: 0x04000540 RID: 1344
		private const int Capacity = 8;

		// Token: 0x04000541 RID: 1345
		private readonly List<Agent> _agents;

		// Token: 0x04000542 RID: 1346
		private readonly MBList<Formation> _userFormations;

		// Token: 0x04000543 RID: 1347
		private WorldFrame _frame;

		// Token: 0x04000544 RID: 1348
		private readonly GameEntity _targetEntity;

		// Token: 0x04000545 RID: 1349
		private readonly DestructableComponent _targetEntityDestructableComponent;

		// Token: 0x04000546 RID: 1350
		private readonly bool _surroundEntity;

		// Token: 0x04000547 RID: 1351
		private bool _isEvaluated;

		// Token: 0x04000548 RID: 1352
		private float _cachedDetachmentWeight;
	}
}
