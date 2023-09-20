using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine
{
	public class JobManager
	{
		public JobManager()
		{
			this._jobs = new List<Job>();
			this._locker = new object();
		}

		public void AddJob(Job job)
		{
			object locker = this._locker;
			lock (locker)
			{
				this._jobs.Add(job);
			}
		}

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

		private List<Job> _jobs;

		private object _locker;
	}
}
