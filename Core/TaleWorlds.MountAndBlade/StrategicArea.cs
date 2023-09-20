using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000154 RID: 340
	public class StrategicArea : MissionObject, IDetachment
	{
		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x0003840B File Offset: 0x0003660B
		public bool IsLoose
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x0003840E File Offset: 0x0003660E
		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001112 RID: 4370 RVA: 0x00038416 File Offset: 0x00036616
		public float DistanceToCheck
		{
			get
			{
				return this._distanceToCheck;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x0003841E File Offset: 0x0003661E
		public bool IgnoreHeight
		{
			get
			{
				return this._ignoreHeight;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x00038426 File Offset: 0x00036626
		// (set) Token: 0x06001115 RID: 4373 RVA: 0x00038430 File Offset: 0x00036630
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					List<Team> list = Mission.Current.Teams.Where((Team t) => this.IsUsableBy(t.Side)).ToList<Team>();
					this._isActive = value;
					foreach (Team team in list)
					{
						if (team.TeamAI != null)
						{
							if (this._isActive)
							{
								team.TeamAI.AddStrategicArea(this);
							}
							else
							{
								team.TeamAI.RemoveStrategicArea(this);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x000384D0 File Offset: 0x000366D0
		protected internal override void OnInit()
		{
			base.OnInit();
			this._agents = new List<Agent>();
			this._userFormations = new MBList<Formation>();
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			this._frame = new WorldFrame(globalFrame.rotation, new WorldPosition(base.Scene, UIntPtr.Zero, globalFrame.origin, false));
			this._frame.Rotation.Orthonormalize();
			this._unitSpacing = ArrangementOrder.GetUnitSpacingOf(ArrangementOrder.ArrangementOrderEnum.Line);
			this._capacity = this.CalculateCapacity();
			this._simulationFormations = new Dictionary<Formation, Formation>();
			this._isActive = true;
			for (int i = 0; i < 5; i++)
			{
				this._strategicAreaSidesScoreTally[i] = new StrategicArea.StrategicAreaMutableTuple(0, 0);
			}
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00038582 File Offset: 0x00036782
		private int CalculateCapacity()
		{
			return MathF.Max(1, MathF.Ceiling(MathF.Max(1f, this._width) * MathF.Max(1f, this._depth)));
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x000385B0 File Offset: 0x000367B0
		public Vec3 GetGroundPosition()
		{
			return this._frame.Origin.GetGroundVec3();
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x000385C4 File Offset: 0x000367C4
		public void DetermineAssociatedDestructibleComponents(IEnumerable<DestructableComponent> destructibleComponents)
		{
			this._nearbyDestructibleObjects = new List<DestructableComponent>();
			foreach (DestructableComponent destructableComponent in destructibleComponents)
			{
				destructableComponent.GameEntity.GetGlobalFrame();
				Vec3 vec;
				Vec3 vec2;
				destructableComponent.GameEntity.GetPhysicsMinMax(true, out vec, out vec2, false);
				if (((vec2 + vec) * 0.5f).DistanceSquared(base.GameEntity.GlobalPosition) <= 9f)
				{
					this._nearbyDestructibleObjects.Add(destructableComponent);
				}
			}
			foreach (DestructableComponent destructableComponent2 in this._nearbyDestructibleObjects)
			{
				destructableComponent2.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnCoveringDestructibleObjectDestroyed);
			}
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x000386B4 File Offset: 0x000368B4
		public void OnParentGameEntityVisibilityChanged(bool isVisible)
		{
			this.IsActive = isVisible;
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x000386BD File Offset: 0x000368BD
		private void OnCoveringDestructibleObjectDestroyed(DestructableComponent destroyedComponent, Agent destroyerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			this.IsActive = false;
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x000386C8 File Offset: 0x000368C8
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			foreach (DestructableComponent destructableComponent in this._nearbyDestructibleObjects)
			{
				destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnCoveringDestructibleObjectDestroyed);
			}
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x0003872C File Offset: 0x0003692C
		public void InitializeAutogenerated(float width, int capacity, BattleSideEnum side)
		{
			this._width = width;
			this._capacity = capacity;
			this._side = side;
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x00038744 File Offset: 0x00036944
		public void AddAgent(Agent agent, int slotIndex)
		{
			this._agents.Add(agent);
			if (this._capacity == 1 && this._centerPosition == null)
			{
				this._centerPosition = new WorldPosition?(this._frame.Origin);
				Mat3 identity = Mat3.Identity;
				identity.f = base.GameEntity.GetGlobalFrame().rotation.f;
				identity.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				this._cachedWorldFrame = new WorldFrame(identity, this._centerPosition.Value);
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x000387C9 File Offset: 0x000369C9
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

		// Token: 0x06001120 RID: 4384 RVA: 0x000387F8 File Offset: 0x000369F8
		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00038806 File Offset: 0x00036A06
		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00038815 File Offset: 0x00036A15
		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00038823 File Offset: 0x00036A23
		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			return null;
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00038828 File Offset: 0x00036A28
		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
			for (int i = this._agents.Count; i < this._capacity; i++)
			{
				slotIndexWeightTuples.Add(new ValueTuple<int, float>(i, StrategicArea.CalculateWeight(this._capacity, i)));
			}
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00038868 File Offset: 0x00036A68
		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return agent.CanBeAssignedForScriptedMovement() && slotIndex < this._capacity && slotIndex >= this._agents.Count && this.IsAgentEligible(agent) && !this.IsAgentOnInconvenientNavmesh(agent);
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x000388A0 File Offset: 0x00036AA0
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

		// Token: 0x06001127 RID: 4391 RVA: 0x00038948 File Offset: 0x00036B48
		public bool IsAgentEligible(Agent agent)
		{
			return agent.IsRangedCached;
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00038950 File Offset: 0x00036B50
		void IDetachment.UnmarkDetachment()
		{
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00038952 File Offset: 0x00036B52
		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return false;
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00038955 File Offset: 0x00036B55
		void IDetachment.MarkSlotAtIndex(int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\StrategicArea.cs", "MarkSlotAtIndex", 322);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00038970 File Offset: 0x00036B70
		bool IDetachment.IsAgentUsingOrInterested(Agent agent)
		{
			return this._agents.Contains(agent);
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00038980 File Offset: 0x00036B80
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

		// Token: 0x0600112D RID: 4397 RVA: 0x000389D1 File Offset: 0x00036BD1
		public bool IsStandingPointAvailableForAgent(Agent agent)
		{
			return this._agents.Count < this._capacity;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x000389E8 File Offset: 0x00036BE8
		public List<float> GetTemplateCostsOfAgent(Agent candidate, List<float> oldValue)
		{
			WorldPosition worldPosition = candidate.GetWorldPosition();
			float num = (candidate.Mission.Scene.DoesPathExistBetweenPositions(worldPosition, this._frame.Origin) ? worldPosition.GetNavMeshVec3().DistanceSquared(this._frame.Origin.GetNavMeshVec3()) : float.MaxValue);
			num *= MissionGameModels.Current.AgentStatCalculateModel.GetDetachmentCostMultiplierOfAgent(candidate, this);
			List<float> list = oldValue ?? new List<float>(this._capacity);
			list.Clear();
			for (int i = 0; i < this._capacity; i++)
			{
				list.Add(num);
			}
			return list;
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00038A89 File Offset: 0x00036C89
		float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
		{
			Debug.FailedAssert("This should never have been called because this detachment does not seek to replace moving agents.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\StrategicArea.cs", "GetExactCostOfAgentAtSlot", 372);
			return 0f;
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x00038AAC File Offset: 0x00036CAC
		public float GetTemplateWeightOfAgent(Agent candidate)
		{
			WorldPosition worldPosition = candidate.GetWorldPosition();
			WorldPosition origin = this._frame.Origin;
			if (!candidate.Mission.Scene.DoesPathExistBetweenPositions(worldPosition, origin))
			{
				return float.MaxValue;
			}
			return worldPosition.GetNavMeshVec3().DistanceSquared(origin.GetNavMeshVec3());
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00038AFC File Offset: 0x00036CFC
		public float? GetWeightOfAgentAtNextSlot(List<Agent> newAgents, out Agent match)
		{
			float? weightOfNextSlot = this.GetWeightOfNextSlot(newAgents[0].Team.Side);
			if (this._agents.Count < this._capacity)
			{
				Vec3 position = base.GameEntity.GlobalPosition;
				match = newAgents.MinBy((Agent a) => a.Position.DistanceSquared(position));
				return weightOfNextSlot;
			}
			match = null;
			return null;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00038B70 File Offset: 0x00036D70
		public float? GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> agentTemplateScores, out Agent match)
		{
			float? weight = this.GetWeightOfNextSlot(agentTemplateScores[0].Item1.Team.Side);
			if (this._agents.Count < this._capacity)
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

		// Token: 0x06001133 RID: 4403 RVA: 0x00038C14 File Offset: 0x00036E14
		public float? GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> newAgents, out Agent match)
		{
			float weightOfOccupiedSlot = this.GetWeightOfOccupiedSlot(detachedAgent);
			Vec3 position = base.GameEntity.GlobalPosition;
			match = newAgents.MinBy((Agent a) => a.Position.DistanceSquared(position));
			return new float?(weightOfOccupiedSlot * 0.5f);
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00038C5E File Offset: 0x00036E5E
		public void RemoveAgent(Agent agent)
		{
			this._agents.Remove(agent);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00038C6D File Offset: 0x00036E6D
		private Formation GetSimulationFormation(Formation formation)
		{
			if (!this._simulationFormations.ContainsKey(formation))
			{
				this._simulationFormations[formation] = new Formation(null, -1);
			}
			return this._simulationFormations[formation];
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x00038C9C File Offset: 0x00036E9C
		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if (base.GameEntity.IsVisibleIncludeParents() && this.CalculateCapacity() == 1)
			{
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				WorldFrame worldFrame = new WorldFrame(globalFrame.rotation, new WorldPosition(base.Scene, globalFrame.origin));
				if (worldFrame.Origin.GetNavMesh() == UIntPtr.Zero)
				{
					uint upgradeLevelMaskCumulative = (uint)base.GameEntity.GetUpgradeLevelMaskCumulative();
					int upgradeLevelCount = base.Scene.GetUpgradeLevelCount();
					string text = "";
					for (int i = 0; i < upgradeLevelCount; i++)
					{
						if (((ulong)upgradeLevelMaskCumulative & (ulong)(1L << (i & 31))) != 0UL)
						{
							text = text + base.Scene.GetUpgradeLevelNameOfIndex(i) + ",";
						}
					}
					MBEditor.AddEntityWarning(base.GameEntity, string.Concat(new object[]
					{
						"Strategic archer position at position at X=",
						globalFrame.origin.X,
						" Y=",
						globalFrame.origin.Y,
						" Z=",
						globalFrame.origin.Z,
						"doesn't yield a viable frame. It may be in the air, underground or off the navmesh, please check. Scene: ",
						base.Scene.GetName(),
						"Upgrade Mask: ",
						upgradeLevelMaskCumulative,
						", Upgrade Level Names: ",
						text
					}));
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00038E10 File Offset: 0x00037010
		public WorldFrame? GetAgentFrame(Agent agent)
		{
			if (this._capacity > 1)
			{
				int num = this._agents.IndexOf(agent);
				Formation formation = agent.Formation;
				Formation simulationFormation = this.GetSimulationFormation(formation);
				Formation formation2 = formation;
				Formation formation3 = simulationFormation;
				int num2 = num;
				Vec2 vec = this._frame.Rotation.f.AsVec2;
				vec = vec.Normalized();
				WorldPosition? worldPosition;
				Vec2? vec2;
				formation2.GetUnitPositionWithIndexAccordingToNewOrder(formation3, num2, this._frame.Origin, vec, this._width, this._unitSpacing, this._agents.Count, out worldPosition, out vec2);
				if (worldPosition != null)
				{
					return new WorldFrame?(new WorldFrame(this._frame.Rotation, worldPosition.Value));
				}
				if (this._centerPosition == null)
				{
					MBDebug.ShowWarning(string.Concat(new object[]
					{
						"Strategic archer position at position at X=",
						this._frame.Origin.GetGroundVec3().x,
						" Y=",
						this._frame.Origin.GetGroundVec3().y,
						" Z=",
						this._frame.Origin.GetGroundVec3().z,
						"doesn't yield a viable frame. It may be in the air, underground or off the navmesh, please check. Scene: ",
						base.Scene.GetName()
					}));
				}
				return new WorldFrame?(agent.GetWorldFrame());
			}
			else
			{
				float totalMissionTime = MBCommon.GetTotalMissionTime();
				StrategicArea.ShimmyDirection shimmyDirection = this._shimmyDirection;
				int num3 = 0;
				StrategicArea.StrategicAreaMutableTuple[] strategicAreaSidesScoreTally = this._strategicAreaSidesScoreTally;
				for (int i = 0; i < strategicAreaSidesScoreTally.Length; i++)
				{
					if (strategicAreaSidesScoreTally[i] != null)
					{
						num3++;
					}
				}
				bool flag = num3 > 1;
				if (flag && this._lastShootTime < agent.LastRangedAttackTime)
				{
					this._lastShootTime = agent.LastRangedAttackTime;
					StrategicArea.StrategicAreaMutableTuple strategicAreaMutableTuple = this._strategicAreaSidesScoreTally[(int)this._shimmyDirection];
					if (strategicAreaMutableTuple != null)
					{
						strategicAreaMutableTuple.RangedHitScoredCount++;
					}
					else
					{
						this._strategicAreaSidesScoreTally[(int)this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(0, 1);
					}
				}
				bool flag2 = false;
				if (flag && this._lastShimmyTime < agent.LastRangedHitTime)
				{
					StrategicArea.StrategicAreaMutableTuple strategicAreaMutableTuple2 = this._strategicAreaSidesScoreTally[(int)this._shimmyDirection];
					if (strategicAreaMutableTuple2 != null)
					{
						strategicAreaMutableTuple2.RangedHitReceivedCount++;
					}
					else
					{
						this._strategicAreaSidesScoreTally[(int)this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(1, 0);
					}
					flag2 = true;
				}
				bool flag3 = false;
				if (flag && !flag2 && totalMissionTime - MathF.Max(agent.LastRangedAttackTime, this._lastShimmyTime) > 8f)
				{
					StrategicArea.StrategicAreaMutableTuple strategicAreaMutableTuple3 = this._strategicAreaSidesScoreTally[(int)this._shimmyDirection];
					if (strategicAreaMutableTuple3 != null)
					{
						strategicAreaMutableTuple3.RangedHitScoredCount--;
					}
					else
					{
						this._strategicAreaSidesScoreTally[(int)this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(0, -1);
					}
					flag3 = true;
				}
				if (flag2 || flag3)
				{
					int num4 = int.MinValue;
					int num5 = 0;
					for (int j = 0; j < 5; j++)
					{
						if (j != (int)this._shimmyDirection && this._strategicAreaSidesScoreTally[j] != null)
						{
							int num6 = this._strategicAreaSidesScoreTally[j].RangedHitScoredCount - this._strategicAreaSidesScoreTally[j].RangedHitReceivedCount;
							if (num6 > num4)
							{
								num4 = num6;
								num5 = 1;
							}
							else if (num6 == num4)
							{
								num5++;
							}
						}
					}
					int num7 = MBRandom.RandomInt(num5 - 1);
					for (int k = 0; k < 5; k++)
					{
						if (k != (int)this._shimmyDirection && this._strategicAreaSidesScoreTally[k] != null && this._strategicAreaSidesScoreTally[k].RangedHitScoredCount - this._strategicAreaSidesScoreTally[k].RangedHitReceivedCount == num4 && --num7 < 0)
						{
							shimmyDirection = (StrategicArea.ShimmyDirection)k;
						}
					}
					this._doesFrameNeedUpdate = true;
				}
				if (!this._doesFrameNeedUpdate)
				{
					return new WorldFrame?(this._cachedWorldFrame);
				}
				if (this._centerPosition != null)
				{
					WorldPosition value = this._centerPosition.Value;
					Vec2 vec = this._frame.Rotation.f.AsVec2;
					Vec2 vec3 = vec.Normalized();
					Vec2 vec4;
					switch (shimmyDirection)
					{
					case StrategicArea.ShimmyDirection.Center:
						vec4 = Vec2.Zero;
						break;
					case StrategicArea.ShimmyDirection.Left:
						vec4 = vec3.RightVec();
						break;
					case StrategicArea.ShimmyDirection.Forward:
						vec4 = vec3;
						break;
					case StrategicArea.ShimmyDirection.Right:
						vec4 = vec3.LeftVec();
						break;
					case StrategicArea.ShimmyDirection.Back:
						vec4 = -vec3;
						break;
					default:
						vec4 = Vec2.Zero;
						break;
					}
					WorldPosition worldPosition2 = value;
					int num8 = 8;
					bool flag4 = false;
					while (num8-- > 0)
					{
						value.SetVec2(worldPosition2.AsVec2 + (0.6f + 0.05f * (float)num8) * vec4);
						if (value.GetNavMesh() != UIntPtr.Zero)
						{
							flag4 = true;
							break;
						}
					}
					this._doesFrameNeedUpdate = false;
					if (!flag4)
					{
						this._strategicAreaSidesScoreTally[(int)shimmyDirection] = null;
					}
					else
					{
						this._shimmyDirection = shimmyDirection;
						this._lastShimmyTime = totalMissionTime;
						Mat3 identity = Mat3.Identity;
						identity.f = new Vec3(vec3, 0f, -1f);
						identity.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						this._cachedWorldFrame = new WorldFrame(identity, value);
					}
					return new WorldFrame?(this._cachedWorldFrame);
				}
				MBDebug.ShowWarning(string.Concat(new object[]
				{
					"Strategic archer position at position at X=",
					this._frame.Origin.GetGroundVec3().x,
					" Y=",
					this._frame.Origin.GetGroundVec3().y,
					" Z=",
					this._frame.Origin.GetGroundVec3().z,
					"doesn't yield a viable frame. It may be in the air, underground or off the navmesh, please check. Scene: ",
					base.Scene.GetName()
				}));
				return new WorldFrame?(agent.GetWorldFrame());
			}
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x0003938B File Offset: 0x0003758B
		private static float CalculateWeight(int capacity, int index)
		{
			return (float)(capacity - index) * 1f / (float)capacity * 0.5f;
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x000393A0 File Offset: 0x000375A0
		public float? GetWeightOfNextSlot(BattleSideEnum side)
		{
			if (this._agents.Count < this._capacity)
			{
				return new float?(StrategicArea.CalculateWeight(this._capacity, this._agents.Count));
			}
			return null;
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x000393E5 File Offset: 0x000375E5
		public float GetWeightOfOccupiedSlot(Agent agent)
		{
			return StrategicArea.CalculateWeight(this._capacity, this._agents.IndexOf(agent));
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x000393FE File Offset: 0x000375FE
		public bool IsUsableBy(BattleSideEnum side)
		{
			return this._side == side || this._side == BattleSideEnum.None;
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00039417 File Offset: 0x00037617
		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			if (this._agents.Count < this._capacity)
			{
				return (float)(this._capacity - this._agents.Count) * 1f / (float)this._capacity;
			}
			return float.MinValue;
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00039453 File Offset: 0x00037653
		void IDetachment.ResetEvaluation()
		{
			this._isEvaluated = false;
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0003945C File Offset: 0x0003765C
		bool IDetachment.IsEvaluated()
		{
			return this._isEvaluated;
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00039464 File Offset: 0x00037664
		void IDetachment.SetAsEvaluated()
		{
			this._isEvaluated = true;
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0003946D File Offset: 0x0003766D
		float IDetachment.GetDetachmentWeightFromCache()
		{
			return this._cachedDetachmentWeight;
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x00039475 File Offset: 0x00037675
		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			this._cachedDetachmentWeight = ((IDetachment)this).GetDetachmentWeight(side);
			return this._cachedDetachmentWeight;
		}

		// Token: 0x0400045E RID: 1118
		private List<Agent> _agents;

		// Token: 0x0400045F RID: 1119
		private WorldFrame _frame;

		// Token: 0x04000460 RID: 1120
		[EditableScriptComponentVariable(true)]
		private float _width;

		// Token: 0x04000461 RID: 1121
		private int _unitSpacing;

		// Token: 0x04000462 RID: 1122
		private int _capacity;

		// Token: 0x04000463 RID: 1123
		private MBList<Formation> _userFormations;

		// Token: 0x04000464 RID: 1124
		private Dictionary<Formation, Formation> _simulationFormations;

		// Token: 0x04000465 RID: 1125
		[EditableScriptComponentVariable(true)]
		private BattleSideEnum _side;

		// Token: 0x04000466 RID: 1126
		[EditableScriptComponentVariable(true)]
		private float _depth = 1f;

		// Token: 0x04000467 RID: 1127
		[EditableScriptComponentVariable(true)]
		private float _distanceToCheck = 10f;

		// Token: 0x04000468 RID: 1128
		[EditableScriptComponentVariable(true)]
		private bool _ignoreHeight = true;

		// Token: 0x04000469 RID: 1129
		private List<DestructableComponent> _nearbyDestructibleObjects = new List<DestructableComponent>();

		// Token: 0x0400046A RID: 1130
		private bool _isActive;

		// Token: 0x0400046B RID: 1131
		private float _lastShimmyTime;

		// Token: 0x0400046C RID: 1132
		private float _lastShootTime;

		// Token: 0x0400046D RID: 1133
		private StrategicArea.ShimmyDirection _shimmyDirection;

		// Token: 0x0400046E RID: 1134
		private bool _doesFrameNeedUpdate = true;

		// Token: 0x0400046F RID: 1135
		private readonly StrategicArea.StrategicAreaMutableTuple[] _strategicAreaSidesScoreTally = new StrategicArea.StrategicAreaMutableTuple[5];

		// Token: 0x04000470 RID: 1136
		private WorldPosition? _centerPosition;

		// Token: 0x04000471 RID: 1137
		private WorldFrame _cachedWorldFrame;

		// Token: 0x04000472 RID: 1138
		private bool _isEvaluated;

		// Token: 0x04000473 RID: 1139
		private float _cachedDetachmentWeight;

		// Token: 0x02000498 RID: 1176
		private class StrategicAreaMutableTuple
		{
			// Token: 0x06003708 RID: 14088 RVA: 0x000E21B8 File Offset: 0x000E03B8
			public StrategicAreaMutableTuple(int rangedHitReceivedCount, int rangedHitScoredCount)
			{
				this.RangedHitReceivedCount = rangedHitReceivedCount;
				this.RangedHitScoredCount = rangedHitScoredCount;
			}

			// Token: 0x040019BD RID: 6589
			public int RangedHitReceivedCount;

			// Token: 0x040019BE RID: 6590
			public int RangedHitScoredCount;
		}

		// Token: 0x02000499 RID: 1177
		private enum ShimmyDirection
		{
			// Token: 0x040019C0 RID: 6592
			Center,
			// Token: 0x040019C1 RID: 6593
			Left,
			// Token: 0x040019C2 RID: 6594
			Forward,
			// Token: 0x040019C3 RID: 6595
			Right,
			// Token: 0x040019C4 RID: 6596
			Back,
			// Token: 0x040019C5 RID: 6597
			NumDirections
		}
	}
}
