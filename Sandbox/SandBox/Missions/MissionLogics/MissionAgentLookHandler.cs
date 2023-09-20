using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class MissionAgentLookHandler : MissionLogic
	{
		public MissionAgentLookHandler()
		{
			this._staticPointList = new List<MissionAgentLookHandler.PointOfInterest>();
			this._checklist = new List<MissionAgentLookHandler.LookInfo>();
			this._selectionDelegate = new MissionAgentLookHandler.SelectionDelegate(this.SelectRandomAccordingToScore);
		}

		public override void AfterStart()
		{
			this.AddStablePointsOfInterest();
		}

		private void AddStablePointsOfInterest()
		{
			foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("point_of_interest"))
			{
				this._staticPointList.Add(new MissionAgentLookHandler.PointOfInterest(gameEntity.GetGlobalFrame()));
			}
		}

		private void DebugTick()
		{
		}

		public override void OnMissionTick(float dt)
		{
			if (Game.Current.IsDevelopmentMode)
			{
				this.DebugTick();
			}
			float currentTime = base.Mission.CurrentTime;
			foreach (MissionAgentLookHandler.LookInfo lookInfo in this._checklist)
			{
				if (lookInfo.Agent.IsActive() && !ConversationMission.ConversationAgents.Contains(lookInfo.Agent) && (!ConversationMission.ConversationAgents.Any<Agent>() || !lookInfo.Agent.IsPlayerControlled))
				{
					if (lookInfo.CheckTimer.Check(currentTime))
					{
						MissionAgentLookHandler.PointOfInterest pointOfInterest = this._selectionDelegate(lookInfo.Agent);
						if (pointOfInterest != null)
						{
							lookInfo.Reset(pointOfInterest, 5f);
						}
						else
						{
							lookInfo.Reset(null, 1f + MBRandom.RandomFloat);
						}
					}
					else if (lookInfo.PointOfInterest != null && (!lookInfo.PointOfInterest.IsActive || !lookInfo.PointOfInterest.IsVisibleFor(lookInfo.Agent)))
					{
						MissionAgentLookHandler.PointOfInterest pointOfInterest2 = this._selectionDelegate(lookInfo.Agent);
						if (pointOfInterest2 != null)
						{
							lookInfo.Reset(pointOfInterest2, 5f + MBRandom.RandomFloat);
						}
						else
						{
							lookInfo.Reset(null, MBRandom.RandomFloat * 5f + 5f);
						}
					}
					else if (lookInfo.PointOfInterest != null)
					{
						Vec3 targetPosition = lookInfo.PointOfInterest.GetTargetPosition();
						lookInfo.Agent.SetLookToPointOfInterest(targetPosition);
					}
				}
			}
		}

		private MissionAgentLookHandler.PointOfInterest SelectFirstNonAgent(Agent agent)
		{
			if (agent.IsAIControlled)
			{
				int num = MBRandom.RandomInt(this._staticPointList.Count);
				int num2 = num;
				MissionAgentLookHandler.PointOfInterest pointOfInterest;
				for (;;)
				{
					pointOfInterest = this._staticPointList[num2];
					if (pointOfInterest.GetScore(agent) > 0f)
					{
						break;
					}
					num2 = ((num2 + 1 == this._staticPointList.Count) ? 0 : (num2 + 1));
					if (num2 == num)
					{
						goto IL_53;
					}
				}
				return pointOfInterest;
			}
			IL_53:
			return null;
		}

		private MissionAgentLookHandler.PointOfInterest SelectBestOfLimitedNonAgent(Agent agent)
		{
			int num = 3;
			MissionAgentLookHandler.PointOfInterest pointOfInterest = null;
			float num2 = -1f;
			if (agent.IsAIControlled)
			{
				int num3 = MBRandom.RandomInt(this._staticPointList.Count);
				int num4 = num3;
				do
				{
					MissionAgentLookHandler.PointOfInterest pointOfInterest2 = this._staticPointList[num4];
					float score = pointOfInterest2.GetScore(agent);
					if (score > 0f)
					{
						if (score > num2)
						{
							num2 = score;
							pointOfInterest = pointOfInterest2;
						}
						num--;
					}
					num4 = ((num4 + 1 == this._staticPointList.Count) ? 0 : (num4 + 1));
				}
				while (num4 != num3 && num > 0);
			}
			return pointOfInterest;
		}

		private MissionAgentLookHandler.PointOfInterest SelectBest(Agent agent)
		{
			MissionAgentLookHandler.PointOfInterest pointOfInterest = null;
			float num = -1f;
			if (agent.IsAIControlled)
			{
				foreach (MissionAgentLookHandler.PointOfInterest pointOfInterest2 in this._staticPointList)
				{
					float score = pointOfInterest2.GetScore(agent);
					if (score > 0f && score > num)
					{
						num = score;
						pointOfInterest = pointOfInterest2;
					}
				}
				AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(base.Mission, agent.Position.AsVec2, 5f, false);
				while (proximityMapSearchStruct.LastFoundAgent != null)
				{
					MissionAgentLookHandler.PointOfInterest pointOfInterest3 = new MissionAgentLookHandler.PointOfInterest(proximityMapSearchStruct.LastFoundAgent);
					float score2 = pointOfInterest3.GetScore(agent);
					if (score2 > 0f && score2 > num)
					{
						num = score2;
						pointOfInterest = pointOfInterest3;
					}
					AgentProximityMap.FindNext(base.Mission, ref proximityMapSearchStruct);
				}
			}
			return pointOfInterest;
		}

		private MissionAgentLookHandler.PointOfInterest SelectRandomAccordingToScore(Agent agent)
		{
			float num = 0f;
			List<KeyValuePair<float, MissionAgentLookHandler.PointOfInterest>> list = new List<KeyValuePair<float, MissionAgentLookHandler.PointOfInterest>>();
			if (agent.IsAIControlled)
			{
				foreach (MissionAgentLookHandler.PointOfInterest pointOfInterest in this._staticPointList)
				{
					float score = pointOfInterest.GetScore(agent);
					if (score > 0f)
					{
						list.Add(new KeyValuePair<float, MissionAgentLookHandler.PointOfInterest>(score, pointOfInterest));
						num += score;
					}
				}
				AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, agent.Position.AsVec2, 5f, false);
				while (proximityMapSearchStruct.LastFoundAgent != null)
				{
					MissionAgentLookHandler.PointOfInterest pointOfInterest2 = new MissionAgentLookHandler.PointOfInterest(proximityMapSearchStruct.LastFoundAgent);
					float score2 = pointOfInterest2.GetScore(agent);
					if (score2 > 0f)
					{
						list.Add(new KeyValuePair<float, MissionAgentLookHandler.PointOfInterest>(score2, pointOfInterest2));
						num += score2;
					}
					AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			float num2 = MBRandom.RandomFloat * num;
			MissionAgentLookHandler.PointOfInterest pointOfInterest3 = list[list.Count - 1].Value;
			foreach (KeyValuePair<float, MissionAgentLookHandler.PointOfInterest> keyValuePair in list)
			{
				num2 -= keyValuePair.Key;
				if (num2 <= 0f)
				{
					pointOfInterest3 = keyValuePair.Value;
					break;
				}
			}
			return pointOfInterest3;
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsHuman)
			{
				this._checklist.Add(new MissionAgentLookHandler.LookInfo(agent, MBRandom.RandomFloat));
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			for (int i = 0; i < this._checklist.Count; i++)
			{
				MissionAgentLookHandler.LookInfo lookInfo = this._checklist[i];
				if (lookInfo.Agent == affectedAgent)
				{
					this._checklist.RemoveAt(i);
					i--;
				}
				else if (lookInfo.PointOfInterest != null && lookInfo.PointOfInterest.IsRelevant(affectedAgent))
				{
					lookInfo.Reset(null, MBRandom.RandomFloat * 2f + 2f);
				}
			}
		}

		private readonly List<MissionAgentLookHandler.PointOfInterest> _staticPointList;

		private readonly List<MissionAgentLookHandler.LookInfo> _checklist;

		private MissionAgentLookHandler.SelectionDelegate _selectionDelegate;

		private class PointOfInterest
		{
			public bool IsActive
			{
				get
				{
					return this._agent == null || this._agent.IsActive();
				}
			}

			public PointOfInterest(Agent agent)
			{
				this._agent = agent;
				this._selectDistance = 5;
				this._releaseDistanceSquare = 36;
				this._ignoreDirection = false;
				CharacterObject characterObject = (CharacterObject)agent.Character;
				if (!agent.IsHuman)
				{
					this._priority = 1;
					return;
				}
				if (characterObject.IsHero)
				{
					this._priority = 5;
					return;
				}
				if (characterObject.Occupation == 12 || characterObject.Occupation == 10 || characterObject.Occupation == 4 || characterObject.Occupation == 11 || characterObject.Occupation == 28)
				{
					this._priority = 3;
					return;
				}
				this._priority = 1;
			}

			public PointOfInterest(MatrixFrame frame)
			{
				this._frame = frame;
				this._selectDistance = 4;
				this._releaseDistanceSquare = 25;
				this._ignoreDirection = true;
				this._priority = 2;
			}

			public float GetScore(Agent agent)
			{
				if (agent == this._agent || this.GetBasicPosition().DistanceSquared(agent.Position) > (float)(this._selectDistance * this._selectDistance))
				{
					return -1f;
				}
				Vec3 vec = this.GetTargetPosition() - agent.GetEyeGlobalPosition();
				float num = vec.Normalize();
				if (Vec2.DotProduct(vec.AsVec2, agent.GetMovementDirection()) < 0.7f)
				{
					return -1f;
				}
				float num2 = (float)(this._priority * this._selectDistance) / num;
				if (this.IsMoving())
				{
					num2 *= 5f;
				}
				if (!this._ignoreDirection)
				{
					MatrixFrame matrixFrame = this.GetTargetFrame();
					Vec2 asVec = matrixFrame.rotation.f.AsVec2;
					matrixFrame = agent.Frame;
					float num3 = Vec2.DotProduct(asVec, matrixFrame.rotation.f.AsVec2);
					if (num3 < -0.7f)
					{
						num2 *= 2f;
					}
					else if (MathF.Abs(num3) < 0.1f)
					{
						num2 *= 2f;
					}
				}
				return num2;
			}

			public Vec3 GetTargetPosition()
			{
				Agent agent = this._agent;
				if (agent == null)
				{
					return this._frame.origin;
				}
				return agent.GetEyeGlobalPosition();
			}

			public Vec3 GetBasicPosition()
			{
				if (this._agent == null)
				{
					return this._frame.origin;
				}
				return this._agent.Position;
			}

			private bool IsMoving()
			{
				return this._agent == null || this._agent.GetCurrentVelocity().LengthSquared > 0.040000003f;
			}

			private MatrixFrame GetTargetFrame()
			{
				if (this._agent == null)
				{
					return this._frame;
				}
				return this._agent.Frame;
			}

			public bool IsVisibleFor(Agent agent)
			{
				Vec3 basicPosition = this.GetBasicPosition();
				Vec3 position = agent.Position;
				if (agent == this._agent || position.DistanceSquared(basicPosition) > (float)this._releaseDistanceSquare)
				{
					return false;
				}
				Vec3 vec = basicPosition - position;
				vec.Normalize();
				return Vec2.DotProduct(vec.AsVec2, agent.GetMovementDirection()) > 0.4f;
			}

			public bool IsRelevant(Agent agent)
			{
				return agent == this._agent;
			}

			public const int MaxSelectDistanceForAgent = 5;

			public const int MaxSelectDistanceForFrame = 4;

			private readonly int _selectDistance;

			private readonly int _releaseDistanceSquare;

			private readonly Agent _agent;

			private readonly MatrixFrame _frame;

			private readonly bool _ignoreDirection;

			private readonly int _priority;
		}

		private class LookInfo
		{
			public LookInfo(Agent agent, float checkTime)
			{
				this.Agent = agent;
				this.CheckTimer = new Timer(Mission.Current.CurrentTime, checkTime, true);
			}

			public void Reset(MissionAgentLookHandler.PointOfInterest pointOfInterest, float duration)
			{
				if (this.PointOfInterest != pointOfInterest)
				{
					this.PointOfInterest = pointOfInterest;
					if (this.PointOfInterest != null)
					{
						this.Agent.SetLookToPointOfInterest(this.PointOfInterest.GetTargetPosition());
					}
					else if (this.Agent.IsActive())
					{
						this.Agent.DisableLookToPointOfInterest();
					}
				}
				this.CheckTimer.Reset(Mission.Current.CurrentTime, duration);
			}

			public readonly Agent Agent;

			public MissionAgentLookHandler.PointOfInterest PointOfInterest;

			public readonly Timer CheckTimer;
		}

		private delegate MissionAgentLookHandler.PointOfInterest SelectionDelegate(Agent agent);
	}
}
