using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Objects.Siege
{
	// Token: 0x020003ED RID: 1005
	public class AgentPathNavMeshChecker
	{
		// Token: 0x060034A4 RID: 13476 RVA: 0x000DA948 File Offset: 0x000D8B48
		public AgentPathNavMeshChecker(Mission mission, MatrixFrame pathFrameToCheck, float radiusToCheck, int navMeshId, BattleSideEnum teamToCollect, AgentPathNavMeshChecker.Direction directionToCollect, float maxDistanceCheck, float agentMoveTime)
		{
			this._mission = mission;
			this._pathFrameToCheck = pathFrameToCheck;
			this._radiusToCheck = radiusToCheck;
			this._navMeshId = navMeshId;
			this._teamToCollect = teamToCollect;
			this._directionToCollect = directionToCollect;
			this._maxDistanceCheck = maxDistanceCheck;
			this._agentMoveTime = agentMoveTime;
		}

		// Token: 0x060034A5 RID: 13477 RVA: 0x000DA9A4 File Offset: 0x000D8BA4
		public void Tick(float dt)
		{
			float currentTime = this._mission.CurrentTime;
			if (this._tickOccasionallyTimer == null || this._tickOccasionallyTimer.Check(currentTime))
			{
				float num = dt;
				if (this._tickOccasionallyTimer != null)
				{
					num = this._tickOccasionallyTimer.ElapsedTime();
				}
				this._tickOccasionallyTimer = new Timer(currentTime, 0.1f + MBRandom.RandomFloat * 0.1f, true);
				this.TickOccasionally(num);
			}
			bool flag = false;
			foreach (Agent agent in this._nearbyAgents)
			{
				Vec3 position = agent.Position;
				if ((this._teamToCollect == BattleSideEnum.None || (agent.Team != null && agent.Team.Side == this._teamToCollect)) && agent.IsAIControlled)
				{
					if (agent.GetCurrentNavigationFaceId() == this._navMeshId)
					{
						flag = true;
						break;
					}
					if (this._isBeingUsed && position.DistanceSquared(this._pathFrameToCheck.origin) < this._radiusToCheck * this._radiusToCheck)
					{
						flag = true;
						break;
					}
					if (agent.MovementVelocity.LengthSquared > 0.01f)
					{
						Vec2 vec;
						if (this._directionToCollect == AgentPathNavMeshChecker.Direction.ForwardOnly)
						{
							vec = this._pathFrameToCheck.rotation.f.AsVec2;
						}
						else if (this._directionToCollect == AgentPathNavMeshChecker.Direction.BackwardOnly)
						{
							vec = -this._pathFrameToCheck.rotation.f.AsVec2;
						}
						else
						{
							vec = Vec2.Zero;
						}
						if (agent.HasPathThroughNavigationFaceIdFromDirection(this._navMeshId, vec))
						{
							float num2 = agent.GetPathDistanceToPoint(ref this._pathFrameToCheck.origin);
							if (num2 >= 100000f)
							{
								num2 = agent.Position.Distance(this._pathFrameToCheck.origin);
							}
							float maximumForwardUnlimitedSpeed = agent.MaximumForwardUnlimitedSpeed;
							if (num2 < this._radiusToCheck * 2f || num2 / maximumForwardUnlimitedSpeed < this._agentMoveTime)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (flag)
			{
				this._isBeingUsed = true;
				this._setBeingUsedToFalseTimer = null;
			}
			else if (this._setBeingUsedToFalseTimer == null)
			{
				this._setBeingUsedToFalseTimer = new Timer(currentTime, 1f, true);
			}
			if (this._setBeingUsedToFalseTimer != null && this._setBeingUsedToFalseTimer.Check(currentTime))
			{
				this._setBeingUsedToFalseTimer = null;
				this._isBeingUsed = false;
			}
		}

		// Token: 0x060034A6 RID: 13478 RVA: 0x000DAC18 File Offset: 0x000D8E18
		public void TickOccasionally(float dt)
		{
			this._nearbyAgents = this._mission.GetNearbyAgents(this._pathFrameToCheck.origin.AsVec2, this._maxDistanceCheck, this._nearbyAgents);
		}

		// Token: 0x060034A7 RID: 13479 RVA: 0x000DAC47 File Offset: 0x000D8E47
		public bool HasAgentsUsingPath()
		{
			return this._isBeingUsed;
		}

		// Token: 0x04001681 RID: 5761
		private BattleSideEnum _teamToCollect;

		// Token: 0x04001682 RID: 5762
		private AgentPathNavMeshChecker.Direction _directionToCollect;

		// Token: 0x04001683 RID: 5763
		private MatrixFrame _pathFrameToCheck;

		// Token: 0x04001684 RID: 5764
		private float _radiusToCheck;

		// Token: 0x04001685 RID: 5765
		private Mission _mission;

		// Token: 0x04001686 RID: 5766
		private int _navMeshId;

		// Token: 0x04001687 RID: 5767
		private Timer _tickOccasionallyTimer;

		// Token: 0x04001688 RID: 5768
		private MBList<Agent> _nearbyAgents = new MBList<Agent>();

		// Token: 0x04001689 RID: 5769
		private bool _isBeingUsed;

		// Token: 0x0400168A RID: 5770
		private Timer _setBeingUsedToFalseTimer;

		// Token: 0x0400168B RID: 5771
		private float _maxDistanceCheck;

		// Token: 0x0400168C RID: 5772
		private float _agentMoveTime;

		// Token: 0x020006D0 RID: 1744
		public enum Direction
		{
			// Token: 0x040022C8 RID: 8904
			ForwardOnly,
			// Token: 0x040022C9 RID: 8905
			BackwardOnly,
			// Token: 0x040022CA RID: 8906
			BothDirections
		}
	}
}
