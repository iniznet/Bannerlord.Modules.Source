using System;
using System.Collections.Generic;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	public class DetachmentData
	{
		public int AgentCount
		{
			get
			{
				return this.joinedFormations.SumQ((Formation f) => f.CountOfDetachableNonplayerUnits) + this.MovingAgentCount + this.DefendingAgentCount;
			}
		}

		public bool IsPrecalculated()
		{
			int count = this.agentScores.Count;
			return count > 0 && count >= this.AgentCount;
		}

		public DetachmentData()
		{
			this.firstTime = MBCommon.GetTotalMissionTime();
		}

		public void RemoveScoreOfAgent(Agent agent)
		{
			for (int i = this.agentScores.Count - 1; i >= 0; i--)
			{
				if (this.agentScores[i].Item1 == agent)
				{
					this.agentScores.RemoveAt(i);
					return;
				}
			}
		}

		public List<Formation> joinedFormations = new List<Formation>();

		public List<ValueTuple<Agent, List<float>>> agentScores = new List<ValueTuple<Agent, List<float>>>();

		public int MovingAgentCount;

		public int DefendingAgentCount;

		public float firstTime;
	}
}
