using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000352 RID: 850
	public class LadderQueueManager : MissionObject
	{
		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x06002DA2 RID: 11682 RVA: 0x000B2E3B File Offset: 0x000B103B
		// (set) Token: 0x06002DA3 RID: 11683 RVA: 0x000B2E43 File Offset: 0x000B1043
		public bool IsDeactivated { get; set; }

		// Token: 0x06002DA4 RID: 11684 RVA: 0x000B2E4C File Offset: 0x000B104C
		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002DA5 RID: 11685 RVA: 0x000B2E60 File Offset: 0x000B1060
		public void Initialize(int managedNavigationFaceId, MatrixFrame managedFrame, Vec3 managedDirection, BattleSideEnum managedSide, int maxUserCount, float arcAngle, float queueBeginDistance, float queueRowSize, float costPerRow, float baseCost, bool blockUsage, float agentSpacing, float zDifferenceToStopUsing, float distanceToStopUsing2d, bool doesManageMultipleIDs, int managedNavigationFaceAlternateID1, int managedNavigationFaceAlternateID2, int maxClimberCount, int maxRunnerCount)
		{
			this.ManagedNavigationFaceId = managedNavigationFaceId;
			this._managedEntitialFrame = managedFrame;
			this._managedEntitialDirection = managedDirection.AsVec2.Normalized();
			MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
			this._managedGlobalFrame = matrixFrame.TransformToParent(managedFrame);
			this._managedGlobalWorldPosition = new WorldPosition(base.GameEntity.GetScenePointer(), UIntPtr.Zero, this._managedGlobalFrame.origin, false);
			this._managedGlobalWorldPosition.GetGroundVec3();
			matrixFrame = base.GameEntity.GetGlobalFrame();
			this._managedGlobalDirection = matrixFrame.rotation.TransformToParent(managedDirection).AsVec2.Normalized();
			this._lastCachedGameEntityGlobalPosition = base.GameEntity.GetGlobalFrame().origin;
			this._managedSide = managedSide;
			this._maxUserCount = maxUserCount;
			this._arcAngle = arcAngle;
			this._queueBeginDistance = queueBeginDistance;
			this._queueRowSize = queueRowSize;
			this._costPerRow = costPerRow;
			this._baseCost = baseCost;
			this._blockUsage = blockUsage;
			this._agentSpacing = agentSpacing;
			this._zDifferenceToStopUsing = zDifferenceToStopUsing;
			this._distanceToStopUsing2d = distanceToStopUsing2d;
			this._doesManageMultipleIDs = doesManageMultipleIDs;
			this.ManagedNavigationFaceAlternateID1 = managedNavigationFaceAlternateID1;
			this.ManagedNavigationFaceAlternateID2 = managedNavigationFaceAlternateID2;
			this._maxClimberCount = maxClimberCount;
			this._maxRunnerCount = maxRunnerCount;
			this._lastUserCountPerLadder = new ValueTuple<int, bool>[3];
		}

		// Token: 0x06002DA6 RID: 11686 RVA: 0x000B2FAE File Offset: 0x000B11AE
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002DA7 RID: 11687 RVA: 0x000B2FC8 File Offset: 0x000B11C8
		private void UpdateGlobalFrameCache()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			if (this._lastCachedGameEntityGlobalPosition != globalFrame.origin)
			{
				this._lastCachedGameEntityGlobalPosition = globalFrame.origin;
				this._managedGlobalFrame = globalFrame.TransformToParent(this._managedEntitialFrame);
				this._managedGlobalWorldPosition = new WorldPosition(base.GameEntity.GetScenePointer(), UIntPtr.Zero, this._managedGlobalFrame.origin, false);
				this._managedGlobalWorldPosition.GetGroundVec3MT();
				this._managedGlobalDirection = globalFrame.rotation.TransformToParent(new Vec3(this._managedEntitialDirection, 0f, -1f)).AsVec2.Normalized();
			}
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x000B3080 File Offset: 0x000B1280
		private void OnTickParallelAux(float dt)
		{
			if (this.IsDeactivated)
			{
				this._userAgents.Clear();
				for (int i = 0; i < this._queuedAgents.Count; i++)
				{
					if (this._queuedAgents[i] != null && this._queuedAgents[i].IsActive())
					{
						this.RemoveAgentFromQueueAtIndex(i);
					}
				}
				this._queuedAgents.Clear();
				return;
			}
			this.UpdateGlobalFrameCache();
			Vec3 groundVec = this._managedGlobalWorldPosition.GetGroundVec3();
			this._timeSinceLastUpdate += dt;
			if (this._timeSinceLastUpdate < this._updatePeriod)
			{
				return;
			}
			if (this._neighborLadderQueueManager != null && (float)this._neighborLadderQueueManager._queuedAgentCount < (float)this._queuedAgentCount * 0.4f && this._neighborLadderQueueManager.CostAddition < this.CostAddition * 0.6667f)
			{
				this.FlushQueueManager();
			}
			this._usingAgentResetTime -= this._timeSinceLastUpdate;
			this._timeSinceLastUpdate = 0f;
			this._updatePeriod = 0.2f + MBRandom.RandomFloat * 0.1f;
			StackArray.StackArray3Int stackArray3Int = default(StackArray.StackArray3Int);
			int num = 0;
			for (int j = this._userAgents.Count - 1; j >= 0; j--)
			{
				Agent agent = this._userAgents[j];
				bool flag = false;
				int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
				if (!agent.IsActive())
				{
					flag = true;
				}
				else if (this._usingAgentResetTime < 0f && (agent.Position.z - groundVec.z > this._zDifferenceToStopUsing || (agent.Position.AsVec2 - groundVec.AsVec2).LengthSquared > this._distanceToStopUsing2d * this._distanceToStopUsing2d))
				{
					if (currentNavigationFaceId == this.ManagedNavigationFaceId || (this._doesManageMultipleIDs && (currentNavigationFaceId == this.ManagedNavigationFaceAlternateID1 || currentNavigationFaceId == this.ManagedNavigationFaceAlternateID2)))
					{
						flag = true;
					}
					else if (this._doesManageMultipleIDs ? (!agent.HasPathThroughNavigationFacesIDFromDirectionMT(this.ManagedNavigationFaceId, this.ManagedNavigationFaceAlternateID1, this.ManagedNavigationFaceAlternateID2, this._managedGlobalDirection)) : (!agent.HasPathThroughNavigationFaceIdFromDirectionMT(this.ManagedNavigationFaceId, this._managedGlobalDirection)))
					{
						flag = true;
					}
				}
				if (flag)
				{
					this._userAgents[j].HumanAIComponent.AdjustSpeedLimit(this._userAgents[j], -1f, false);
					this._userAgents.RemoveAt(j);
				}
				else
				{
					bool flag2 = false;
					if (currentNavigationFaceId == this.ManagedNavigationFaceId)
					{
						ref StackArray.StackArray3Int ptr = ref stackArray3Int;
						ptr[0] = ptr[0] + 1;
						num++;
						flag2 = true;
					}
					else if (this._doesManageMultipleIDs)
					{
						if (currentNavigationFaceId == this.ManagedNavigationFaceAlternateID1)
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[1] = ptr[1] + 1;
							num++;
							flag2 = true;
						}
						else if (currentNavigationFaceId == this.ManagedNavigationFaceAlternateID2)
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[2] = ptr[2] + 1;
							num++;
							flag2 = true;
						}
					}
					if (!flag2)
					{
						if (this._userAgents[j].HasPathThroughNavigationFaceIdFromDirectionMT(this.ManagedNavigationFaceId, this._managedGlobalDirection))
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[0] = ptr[0] + 1;
						}
						else if (this._userAgents[j].HasPathThroughNavigationFaceIdFromDirectionMT(this.ManagedNavigationFaceAlternateID1, this._managedGlobalDirection))
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[1] = ptr[1] + 1;
						}
						else if (this._userAgents[j].HasPathThroughNavigationFaceIdFromDirectionMT(this.ManagedNavigationFaceAlternateID2, this._managedGlobalDirection))
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[2] = ptr[2] + 1;
						}
					}
				}
			}
			if (this._neighborLadderQueueManager != null)
			{
				for (int k = this._neighborLadderQueueManager._userAgents.Count - 1; k >= 0; k--)
				{
					int currentNavigationFaceId2 = this._neighborLadderQueueManager._userAgents[k].GetCurrentNavigationFaceId();
					if (currentNavigationFaceId2 == this.ManagedNavigationFaceId)
					{
						ref StackArray.StackArray3Int ptr = ref stackArray3Int;
						ptr[0] = ptr[0] + 1;
						num++;
					}
					else if (this._doesManageMultipleIDs)
					{
						if (currentNavigationFaceId2 == this.ManagedNavigationFaceAlternateID1)
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[1] = ptr[1] + 1;
							num++;
						}
						else if (currentNavigationFaceId2 == this.ManagedNavigationFaceAlternateID2)
						{
							ref StackArray.StackArray3Int ptr = ref stackArray3Int;
							ptr[2] = ptr[2] + 1;
							num++;
						}
					}
				}
			}
			for (int l = 0; l < 3; l++)
			{
				if (this._lastUserCountPerLadder[l].Item1 != stackArray3Int[l])
				{
					this._lastUserCountPerLadder[l].Item1 = stackArray3Int[l];
					this._lastUserCountPerLadder[l].Item2 = true;
				}
			}
			for (int m = this._queuedAgents.Count - 1; m >= 0; m--)
			{
				if (this._queuedAgents[m] != null)
				{
					if (!this.ConditionsAreMet(this._queuedAgents[m], Agent.AIScriptedFrameFlags.GoToPosition))
					{
						this.RemoveAgentFromQueueAtIndex(m);
					}
					else
					{
						float num2 = MBRandom.RandomFloat * (float)this._maxUserCount;
						if (num2 > 0.7f)
						{
							int num3;
							int num4;
							this.GetParentIndicesForQueueIndex(m, out num3, out num4);
							if (num3 >= 0 && this._queuedAgents[num3] == null && num2 > ((num4 >= 0) ? 0.85f : 0.7f))
							{
								this.MoveAgentFromQueueIndexToQueueIndex(m, num3);
							}
							else if (num4 >= 0 && this._queuedAgents[num4] == null)
							{
								this.MoveAgentFromQueueIndexToQueueIndex(m, num4);
							}
						}
					}
				}
			}
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, groundVec.AsVec2, 30f, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (this.ConditionsAreMet(lastFoundAgent, Agent.AIScriptedFrameFlags.None) && lastFoundAgent.Position.DistanceSquared(groundVec) < 900f && !this._queuedAgents.Contains(lastFoundAgent) && lastFoundAgent.HasPathThroughNavigationFacesIDFromDirectionMT(this.ManagedNavigationFaceId, this.ManagedNavigationFaceAlternateID1, this.ManagedNavigationFaceAlternateID2, Vec2.Zero))
				{
					if (this._neighborLadderQueueManager == null)
					{
						this.AddAgentToQueue(lastFoundAgent);
					}
					else if (!this._neighborLadderQueueManager._userAgents.Contains(lastFoundAgent))
					{
						this.AddAgentToQueue(lastFoundAgent);
					}
					else
					{
						this._userAgents.Add(lastFoundAgent);
					}
				}
				AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
			}
			int num5 = this._userAgents.Count - num;
			int num6 = Math.Min(this._maxClimberCount - num, this._maxRunnerCount - num5);
			if (!this._blockUsage && num6 > 0)
			{
				float num7 = float.MaxValue;
				int num8 = -1;
				for (int n = 0; n < this._queuedAgents.Count; n++)
				{
					if (this._queuedAgents[n] != null)
					{
						float lengthSquared = (this._queuedAgents[n].Position - groundVec).LengthSquared;
						if (lengthSquared < num7)
						{
							num7 = lengthSquared;
							num8 = n;
						}
					}
				}
				if (num8 >= 0)
				{
					this._userAgents.Add(this._queuedAgents[num8]);
					this._queuedAgents[num8].HumanAIComponent.AdjustSpeedLimit(this._queuedAgents[num8], 0.2f, true);
					this._usingAgentResetTime = 2f;
					this.RemoveAgentFromQueueAtIndex(num8);
				}
			}
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x000B37DC File Offset: 0x000B19DC
		protected internal override void OnTickParallel(float dt)
		{
			if (this._neighborLadderQueueManager == null)
			{
				this.OnTickParallelAux(dt);
				return;
			}
			LadderQueueManager ladderQueueManager = ((base.Id.Id < this._neighborLadderQueueManager.Id.Id) ? this : this._neighborLadderQueueManager);
			lock (ladderQueueManager)
			{
				this.OnTickParallelAux(dt);
			}
		}

		// Token: 0x06002DAA RID: 11690 RVA: 0x000B3850 File Offset: 0x000B1A50
		protected internal override void OnTick(float dt)
		{
			if (!GameNetwork.IsClientOrReplay && !this.IsDeactivated && !this._blockUsage)
			{
				if (this.ManagedNavigationFaceId > 1 && this._lastUserCountPerLadder[0].Item2)
				{
					this._lastUserCountPerLadder[0].Item2 = false;
					this.CostAddition = this.GetNavigationFaceCostPerClimber(this._lastUserCountPerLadder[0].Item1);
					Mission.Current.SetNavigationFaceCostWithIdAroundPosition(this.ManagedNavigationFaceId, this._managedGlobalWorldPosition.GetGroundVec3(), this.CostAddition);
				}
				if (this.ManagedNavigationFaceAlternateID1 > 1 && this._lastUserCountPerLadder[1].Item2)
				{
					this._lastUserCountPerLadder[1].Item2 = false;
					Mission.Current.SetNavigationFaceCostWithIdAroundPosition(this.ManagedNavigationFaceAlternateID1, this._managedGlobalWorldPosition.GetGroundVec3(), this.GetNavigationFaceCostPerClimber(this._lastUserCountPerLadder[1].Item1));
				}
				if (this.ManagedNavigationFaceAlternateID2 > 1 && this._lastUserCountPerLadder[2].Item2)
				{
					this._lastUserCountPerLadder[2].Item2 = false;
					Mission.Current.SetNavigationFaceCostWithIdAroundPosition(this.ManagedNavigationFaceAlternateID2, this._managedGlobalWorldPosition.GetGroundVec3(), this.GetNavigationFaceCostPerClimber(this._lastUserCountPerLadder[2].Item1));
				}
			}
		}

		// Token: 0x06002DAB RID: 11691 RVA: 0x000B39AC File Offset: 0x000B1BAC
		private bool ConditionsAreMet(Agent agent, Agent.AIScriptedFrameFlags flags)
		{
			return agent.IsAIControlled && agent.IsActive() && agent.Team != null && agent.Team.Side == this._managedSide && agent.MovementLockedState == AgentMovementLockedState.None && !agent.IsUsingGameObject && !agent.InteractingWithAnyGameObject() && !agent.IsDetachedFromFormation && agent.Position.z - this._managedGlobalWorldPosition.GetGroundZ() < this._zDifferenceToStopUsing && !this._userAgents.Contains(agent) && agent.GetScriptedFlags() == flags && agent.GetCurrentNavigationFaceId() != this.ManagedNavigationFaceId && (!this._doesManageMultipleIDs || (agent.GetCurrentNavigationFaceId() != this.ManagedNavigationFaceAlternateID1 && agent.GetCurrentNavigationFaceId() != this.ManagedNavigationFaceAlternateID2));
		}

		// Token: 0x06002DAC RID: 11692 RVA: 0x000B3A87 File Offset: 0x000B1C87
		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this._userAgents.Clear();
			this.IsDeactivated = true;
			this._queuedAgents.Clear();
			this._queuedAgentCount = 0;
		}

		// Token: 0x06002DAD RID: 11693 RVA: 0x000B3AB4 File Offset: 0x000B1CB4
		private void GetParentIndicesForQueueIndex(int queueIndex, out int parentIndex1, out int parentIndex2)
		{
			parentIndex1 = -1;
			parentIndex2 = -1;
			Vec2i coordinatesForQueueIndex = this.GetCoordinatesForQueueIndex(queueIndex);
			int num = coordinatesForQueueIndex.Y - 1;
			if (num >= 0)
			{
				int num2 = MathF.Max(this.GetRowSize(num) - 1, 1);
				int num3 = MathF.Max(this.GetRowSize(coordinatesForQueueIndex.Y) - 1, 1);
				float num4 = (float)coordinatesForQueueIndex.X * (float)num2 / (float)num3;
				parentIndex1 = (int)num4;
				float num5 = MathF.Abs(num4 - (float)parentIndex1);
				if (num5 > 0.2f)
				{
					if (num5 > 0.8f)
					{
						parentIndex1++;
					}
					else
					{
						parentIndex2 = parentIndex1 + 1;
					}
				}
				parentIndex1 = this.GetQueueIndexForCoordinates(new Vec2i(parentIndex1, num));
				if (parentIndex2 >= 0)
				{
					parentIndex2 = this.GetQueueIndexForCoordinates(new Vec2i(parentIndex2, num));
				}
			}
		}

		// Token: 0x06002DAE RID: 11694 RVA: 0x000B3B6C File Offset: 0x000B1D6C
		private float GetScoreForAddingAgentToQueueIndex(Vec3 agentPosition, int queueIndex, out int scoreOfQueueIndex)
		{
			scoreOfQueueIndex = queueIndex;
			float num = float.MinValue;
			if (this._queuedAgents.Count <= queueIndex || this._queuedAgents[queueIndex] == null)
			{
				int num2;
				int num3;
				this.GetParentIndicesForQueueIndex(queueIndex, out num2, out num3);
				if (num2 < 0 || (this._queuedAgents.Count > num2 && this._queuedAgents[num2] != null) || (num3 >= 0 && this._queuedAgents.Count > num3 && this._queuedAgents[num3] != null))
				{
					Vec2i coordinatesForQueueIndex = this.GetCoordinatesForQueueIndex(queueIndex);
					num = (float)coordinatesForQueueIndex.Y * this._queueRowSize * -3f;
					num -= (agentPosition.AsVec2 - this.GetQueuePositionForCoordinates(coordinatesForQueueIndex, -1).AsVec2).Length;
				}
				if (num2 >= 0 && (this._queuedAgents.Count <= num2 || this._queuedAgents[num2] == null))
				{
					int num4;
					float scoreForAddingAgentToQueueIndex = this.GetScoreForAddingAgentToQueueIndex(agentPosition, num2, out num4);
					if (num < scoreForAddingAgentToQueueIndex)
					{
						scoreOfQueueIndex = num4;
						num = scoreForAddingAgentToQueueIndex;
					}
				}
				if (num3 >= 0 && (this._queuedAgents.Count <= num3 || this._queuedAgents[num3] == null))
				{
					int num5;
					float scoreForAddingAgentToQueueIndex2 = this.GetScoreForAddingAgentToQueueIndex(agentPosition, num3, out num5);
					if (num < scoreForAddingAgentToQueueIndex2)
					{
						scoreOfQueueIndex = num5;
						num = scoreForAddingAgentToQueueIndex2;
					}
				}
			}
			return num;
		}

		// Token: 0x06002DAF RID: 11695 RVA: 0x000B3CA4 File Offset: 0x000B1EA4
		private void AddAgentToQueue(Agent agent)
		{
			int y = this.GetCoordinatesForQueueIndex(this._queuedAgents.Count).Y;
			int rowSize = this.GetRowSize(y);
			Vec3 position = agent.Position;
			int num = -1;
			float num2 = float.MinValue;
			for (int i = 0; i < rowSize; i++)
			{
				int num3;
				float scoreForAddingAgentToQueueIndex = this.GetScoreForAddingAgentToQueueIndex(position, this.GetQueueIndexForCoordinates(new Vec2i(i, y)), out num3);
				if (scoreForAddingAgentToQueueIndex > num2)
				{
					num2 = scoreForAddingAgentToQueueIndex;
					num = num3;
				}
			}
			while (this._queuedAgents.Count <= num)
			{
				this._queuedAgents.Add(null);
			}
			this._queuedAgents[num] = agent;
			WorldPosition queuePositionForIndex = this.GetQueuePositionForIndex(num, agent.Index);
			agent.SetScriptedPosition(ref queuePositionForIndex, false, Agent.AIScriptedFrameFlags.None);
			this._queuedAgentCount++;
		}

		// Token: 0x06002DB0 RID: 11696 RVA: 0x000B3D68 File Offset: 0x000B1F68
		private void RemoveAgentFromQueueAtIndex(int queueIndex)
		{
			this._queuedAgentCount--;
			if (!this._queuedAgents[queueIndex].IsUsingGameObject && (!this._queuedAgents[queueIndex].IsAIControlled || !this._queuedAgents[queueIndex].AIMoveToGameObjectIsEnabled()))
			{
				this._queuedAgents[queueIndex].DisableScriptedMovement();
			}
			this._queuedAgents[queueIndex] = null;
		}

		// Token: 0x06002DB1 RID: 11697 RVA: 0x000B3DDA File Offset: 0x000B1FDA
		private float GetNavigationFaceCost(int rowIndex)
		{
			return this._baseCost + (float)MathF.Max(rowIndex - 1, 0) * this._costPerRow;
		}

		// Token: 0x06002DB2 RID: 11698 RVA: 0x000B3DF4 File Offset: 0x000B1FF4
		private float GetNavigationFaceCostPerClimber(int count)
		{
			return this._baseCost + (float)count * this._costPerRow;
		}

		// Token: 0x06002DB3 RID: 11699 RVA: 0x000B3E08 File Offset: 0x000B2008
		private void MoveAgentFromQueueIndexToQueueIndex(int fromQueueIndex, int toQueueIndex)
		{
			this._queuedAgents[toQueueIndex] = this._queuedAgents[fromQueueIndex];
			this._queuedAgents[fromQueueIndex] = null;
			WorldPosition queuePositionForIndex = this.GetQueuePositionForIndex(toQueueIndex, this._queuedAgents[toQueueIndex].Index);
			this._queuedAgents[toQueueIndex].SetScriptedPosition(ref queuePositionForIndex, false, Agent.AIScriptedFrameFlags.None);
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x000B3E68 File Offset: 0x000B2068
		private int GetRowSize(int rowIndex)
		{
			float num = this._arcAngle * (this._queueBeginDistance + this._queueRowSize * (float)rowIndex);
			return 1 + (int)(num / this._agentSpacing);
		}

		// Token: 0x06002DB5 RID: 11701 RVA: 0x000B3E98 File Offset: 0x000B2098
		private int GetQueueIndexForCoordinates(Vec2i coordinates)
		{
			int num = coordinates.X;
			for (int i = 0; i < coordinates.Y; i++)
			{
				num += this.GetRowSize(i);
			}
			return num;
		}

		// Token: 0x06002DB6 RID: 11702 RVA: 0x000B3EC8 File Offset: 0x000B20C8
		private Vec2i GetCoordinatesForQueueIndex(int queueIndex)
		{
			Vec2i vec2i = default(Vec2i);
			for (;;)
			{
				int rowSize = this.GetRowSize(vec2i.Y);
				if (rowSize > queueIndex)
				{
					break;
				}
				queueIndex -= rowSize;
				vec2i.Y++;
			}
			vec2i.X = queueIndex;
			return vec2i;
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x000B3F0C File Offset: 0x000B210C
		private WorldPosition GetQueuePositionForCoordinates(Vec2i coordinates, int randomSeed)
		{
			MatrixFrame managedGlobalFrame = this._managedGlobalFrame;
			WorldPosition managedGlobalWorldPosition = this._managedGlobalWorldPosition;
			float num = 0f;
			int rowSize = this.GetRowSize(coordinates.Y);
			if (rowSize > 1)
			{
				num = this._arcAngle * ((float)coordinates.X / (float)(rowSize - 1) - 0.5f);
			}
			managedGlobalFrame.rotation.RotateAboutForward(num);
			managedGlobalFrame.origin += managedGlobalFrame.rotation.u * (this._queueBeginDistance + this._queueRowSize * (float)coordinates.Y);
			if (randomSeed >= 0)
			{
				Random random = new Random(coordinates.X * 100000 + coordinates.Y * 10000000 + randomSeed);
				managedGlobalFrame.rotation.RotateAboutForward(random.NextFloat() * 3.1415927f * 2f);
				managedGlobalFrame.origin += managedGlobalFrame.rotation.u * random.NextFloat() * 0.3f;
			}
			managedGlobalWorldPosition.SetVec2(managedGlobalFrame.origin.AsVec2);
			return managedGlobalWorldPosition;
		}

		// Token: 0x06002DB8 RID: 11704 RVA: 0x000B4032 File Offset: 0x000B2232
		private WorldPosition GetQueuePositionForIndex(int queueIndex, int randomSeed)
		{
			return this.GetQueuePositionForCoordinates(this.GetCoordinatesForQueueIndex(queueIndex), randomSeed);
		}

		// Token: 0x06002DB9 RID: 11705 RVA: 0x000B4044 File Offset: 0x000B2244
		public void FlushQueueManager()
		{
			int num = this._queuedAgentCount / 2;
			for (int i = this._queuedAgents.Count - 1; i >= num; i--)
			{
				if (this._queuedAgents[i] != null)
				{
					this.RemoveAgentFromQueueAtIndex(i);
				}
			}
		}

		// Token: 0x06002DBA RID: 11706 RVA: 0x000B4087 File Offset: 0x000B2287
		public void AssignNeighborQueueManager(LadderQueueManager neighborLadderQueueManager)
		{
			this._neighborLadderQueueManager = neighborLadderQueueManager;
		}

		// Token: 0x040011D8 RID: 4568
		public int ManagedNavigationFaceId;

		// Token: 0x040011D9 RID: 4569
		public int ManagedNavigationFaceAlternateID1;

		// Token: 0x040011DA RID: 4570
		public int ManagedNavigationFaceAlternateID2;

		// Token: 0x040011DB RID: 4571
		private MatrixFrame _managedEntitialFrame;

		// Token: 0x040011DC RID: 4572
		private Vec2 _managedEntitialDirection;

		// Token: 0x040011DD RID: 4573
		private Vec3 _lastCachedGameEntityGlobalPosition;

		// Token: 0x040011DE RID: 4574
		private MatrixFrame _managedGlobalFrame;

		// Token: 0x040011DF RID: 4575
		private WorldPosition _managedGlobalWorldPosition;

		// Token: 0x040011E0 RID: 4576
		private Vec2 _managedGlobalDirection;

		// Token: 0x040011E1 RID: 4577
		private BattleSideEnum _managedSide;

		// Token: 0x040011E2 RID: 4578
		private bool _blockUsage;

		// Token: 0x040011E4 RID: 4580
		private readonly List<Agent> _userAgents = new List<Agent>();

		// Token: 0x040011E5 RID: 4581
		private int _maxUserCount;

		// Token: 0x040011E6 RID: 4582
		private int _queuedAgentCount;

		// Token: 0x040011E7 RID: 4583
		private readonly List<Agent> _queuedAgents = new List<Agent>();

		// Token: 0x040011E8 RID: 4584
		private float _arcAngle = 2.3561945f;

		// Token: 0x040011E9 RID: 4585
		private float _queueBeginDistance = 1f;

		// Token: 0x040011EA RID: 4586
		private float _queueRowSize = 0.8f;

		// Token: 0x040011EB RID: 4587
		private float _agentSpacing = 1f;

		// Token: 0x040011EC RID: 4588
		private float _timeSinceLastUpdate;

		// Token: 0x040011ED RID: 4589
		private float _updatePeriod;

		// Token: 0x040011EE RID: 4590
		private float _usingAgentResetTime;

		// Token: 0x040011EF RID: 4591
		private float _costPerRow;

		// Token: 0x040011F0 RID: 4592
		private float _baseCost;

		// Token: 0x040011F1 RID: 4593
		private float _zDifferenceToStopUsing = 2f;

		// Token: 0x040011F2 RID: 4594
		private float _distanceToStopUsing2d = 5f;

		// Token: 0x040011F3 RID: 4595
		private bool _doesManageMultipleIDs;

		// Token: 0x040011F4 RID: 4596
		private int _maxClimberCount = 18;

		// Token: 0x040011F5 RID: 4597
		private int _maxRunnerCount = 6;

		// Token: 0x040011F6 RID: 4598
		public float CostAddition;

		// Token: 0x040011F7 RID: 4599
		private LadderQueueManager _neighborLadderQueueManager;

		// Token: 0x040011F8 RID: 4600
		private ValueTuple<int, bool>[] _lastUserCountPerLadder;
	}
}
