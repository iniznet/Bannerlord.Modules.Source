using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class DefencePoint : ScriptComponentBehavior
	{
		public void AddDefender(Agent defender)
		{
			this.defenders.Add(defender);
		}

		public bool RemoveDefender(Agent defender)
		{
			return this.defenders.Remove(defender);
		}

		public IEnumerable<Agent> Defenders
		{
			get
			{
				return this.defenders;
			}
		}

		public void PurgeInactiveDefenders()
		{
			foreach (Agent agent in this.defenders.Where((Agent d) => !d.IsActive()).ToList<Agent>())
			{
				this.RemoveDefender(agent);
			}
		}

		private MatrixFrame GetPosition(int index)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 f = globalFrame.rotation.f;
			f.Normalize();
			globalFrame.origin -= f * (float)index * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f * 1.5f;
			return globalFrame;
		}

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

		private List<Agent> defenders = new List<Agent>();

		public BattleSideEnum Side;
	}
}
