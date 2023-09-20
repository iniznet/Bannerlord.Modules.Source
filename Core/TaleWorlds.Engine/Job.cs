using System;

namespace TaleWorlds.Engine
{
	// Token: 0x0200004F RID: 79
	public class Job
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00004FF8 File Offset: 0x000031F8
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x00005000 File Offset: 0x00003200
		public bool Finished { get; protected set; }

		// Token: 0x060006D4 RID: 1748 RVA: 0x00005009 File Offset: 0x00003209
		public virtual void DoJob(float dt)
		{
		}
	}
}
