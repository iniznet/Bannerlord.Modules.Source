using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public static class SandBoxHelpers
	{
		public static class MissionHelper
		{
			public static void FadeOutAgents(IEnumerable<Agent> agents, bool hideInstantly, bool hideMount)
			{
				if (agents != null)
				{
					Agent[] array = agents.ToArray<Agent>();
					foreach (Agent agent in array)
					{
						if (!agent.IsMount)
						{
							agent.FadeOut(hideInstantly, hideMount);
						}
					}
					foreach (Agent agent2 in array)
					{
						if (agent2.State != 2)
						{
							agent2.FadeOut(hideInstantly, hideMount);
						}
					}
				}
			}
		}
	}
}
