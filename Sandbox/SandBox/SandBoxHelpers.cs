using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x02000013 RID: 19
	public static class SandBoxHelpers
	{
		// Token: 0x020000E9 RID: 233
		public static class MissionHelper
		{
			// Token: 0x06000C5E RID: 3166 RVA: 0x00061500 File Offset: 0x0005F700
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
