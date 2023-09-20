using System;
using System.Collections.Generic;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000139 RID: 313
	public class DetachmentData
	{
		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x0002EF0B File Offset: 0x0002D10B
		public int AgentCount
		{
			get
			{
				return this.joinedFormations.SumQ((Formation f) => f.CountOfDetachableNonplayerUnits) + this.MovingAgentCount + this.DefendingAgentCount;
			}
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x0002EF48 File Offset: 0x0002D148
		public bool IsPrecalculated()
		{
			int count = this.agentScores.Count;
			return count > 0 && count >= this.AgentCount;
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x0002EF73 File Offset: 0x0002D173
		public DetachmentData()
		{
			this.firstTime = MBCommon.GetTotalMissionTime();
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x0002EF9C File Offset: 0x0002D19C
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

		// Token: 0x040003AB RID: 939
		public List<Formation> joinedFormations = new List<Formation>();

		// Token: 0x040003AC RID: 940
		public List<ValueTuple<Agent, List<float>>> agentScores = new List<ValueTuple<Agent, List<float>>>();

		// Token: 0x040003AD RID: 941
		public int MovingAgentCount;

		// Token: 0x040003AE RID: 942
		public int DefendingAgentCount;

		// Token: 0x040003AF RID: 943
		public float firstTime;
	}
}
