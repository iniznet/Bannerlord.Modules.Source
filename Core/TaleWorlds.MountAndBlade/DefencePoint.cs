using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000337 RID: 823
	public class DefencePoint : ScriptComponentBehavior
	{
		// Token: 0x06002C45 RID: 11333 RVA: 0x000ABA5E File Offset: 0x000A9C5E
		public void AddDefender(Agent defender)
		{
			this.defenders.Add(defender);
		}

		// Token: 0x06002C46 RID: 11334 RVA: 0x000ABA6C File Offset: 0x000A9C6C
		public bool RemoveDefender(Agent defender)
		{
			return this.defenders.Remove(defender);
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x06002C47 RID: 11335 RVA: 0x000ABA7A File Offset: 0x000A9C7A
		public IEnumerable<Agent> Defenders
		{
			get
			{
				return this.defenders;
			}
		}

		// Token: 0x06002C48 RID: 11336 RVA: 0x000ABA84 File Offset: 0x000A9C84
		public void PurgeInactiveDefenders()
		{
			foreach (Agent agent in this.defenders.Where((Agent d) => !d.IsActive()).ToList<Agent>())
			{
				this.RemoveDefender(agent);
			}
		}

		// Token: 0x06002C49 RID: 11337 RVA: 0x000ABB04 File Offset: 0x000A9D04
		private MatrixFrame GetPosition(int index)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 f = globalFrame.rotation.f;
			f.Normalize();
			globalFrame.origin -= f * (float)index * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f * 1.5f;
			return globalFrame;
		}

		// Token: 0x06002C4A RID: 11338 RVA: 0x000ABB70 File Offset: 0x000A9D70
		public MatrixFrame GetVacantPosition(Agent a)
		{
			Mission mission = Mission.Current;
			Team team = mission.Teams.First((Team t) => t.Side == this.Side);
			for (int i = 0; i < 100; i++)
			{
				MatrixFrame position = this.GetPosition(i);
				Agent closestAllyAgent = mission.GetClosestAllyAgent(team, position.origin, ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius));
				if (closestAllyAgent == null || closestAllyAgent == a)
				{
					return position;
				}
			}
			Debug.FailedAssert("Couldn't find a vacant position", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\DefencePoint.cs", "GetVacantPosition", 73);
			return MatrixFrame.Identity;
		}

		// Token: 0x06002C4B RID: 11339 RVA: 0x000ABBF0 File Offset: 0x000A9DF0
		public int CountOccupiedDefenderPositions()
		{
			Mission mission = Mission.Current;
			Team team = mission.Teams.First((Team t) => t.Side == this.Side);
			for (int i = 0; i < 100; i++)
			{
				MatrixFrame position = this.GetPosition(i);
				if (mission.GetClosestAllyAgent(team, position.origin, ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius)) == null)
				{
					return i;
				}
			}
			return 100;
		}

		// Token: 0x040010D6 RID: 4310
		private List<Agent> defenders = new List<Agent>();

		// Token: 0x040010D7 RID: 4311
		public BattleSideEnum Side;
	}
}
