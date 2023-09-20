using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Objects.Siege
{
	public class AgentPathNavMeshChecker
	{
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

		public void TickOccasionally(float dt)
		{
			this._nearbyAgents = this._mission.GetNearbyAgents(this._pathFrameToCheck.origin.AsVec2, this._maxDistanceCheck, this._nearbyAgents);
		}

		public bool HasAgentsUsingPath()
		{
			return this._isBeingUsed;
		}

		private BattleSideEnum _teamToCollect;

		private AgentPathNavMeshChecker.Direction _directionToCollect;

		private MatrixFrame _pathFrameToCheck;

		private float _radiusToCheck;

		private Mission _mission;

		private int _navMeshId;

		private Timer _tickOccasionallyTimer;

		private MBList<Agent> _nearbyAgents = new MBList<Agent>();

		private bool _isBeingUsed;

		private Timer _setBeingUsedToFalseTimer;

		private float _maxDistanceCheck;

		private float _agentMoveTime;

		public enum Direction
		{
			ForwardOnly,
			BackwardOnly,
			BothDirections
		}
	}
}
