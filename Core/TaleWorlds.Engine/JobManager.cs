using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine
{
	// Token: 0x0200004E RID: 78
	public class JobManager
	{
		// Token: 0x060006CF RID: 1743 RVA: 0x00004F13 File Offset: 0x00003113
		public JobManager()
		{
			this._jobs = new List<Job>();
			this._locker = new object();
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x00004F34 File Offset: 0x00003134
		public void AddJob(Job job)
		{
			object locker = this._locker;
			lock (locker)
			{
				this._jobs.Add(job);
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00004F7C File Offset: 0x0000317C
		internal void OnTick(float dt)
		{
			object locker = this._locker;
			lock (locker)
			{
				for (int i = 0; i < this._jobs.Count; i++)
				{
					Job job = this._jobs[i];
					job.DoJob(dt);
					if (job.Finished)
					{
						this._jobs.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x0400009D RID: 157
		private List<Job> _jobs;

		// Token: 0x0400009E RID: 158
		private object _locker;
	}
}
