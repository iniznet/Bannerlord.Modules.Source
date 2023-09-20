using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	public abstract class TestCommonBase
	{
		public abstract void Tick();

		public static TestCommonBase BaseInstance
		{
			get
			{
				return TestCommonBase._baseInstance;
			}
		}

		public void StartTimeoutTimer()
		{
			this.timeoutTimerStart = DateTime.Now;
		}

		public void ToggleTimeoutTimer()
		{
			this.timeoutTimerEnabled = !this.timeoutTimerEnabled;
		}

		public bool CheckTimeoutTimer()
		{
			return this.timeoutTimerEnabled && DateTime.Now.Subtract(this.timeoutTimerStart).TotalSeconds > (double)this.commonWaitTimeoutLimits;
		}

		protected TestCommonBase()
		{
			TestCommonBase._baseInstance = this;
		}

		public virtual string GetGameStatus()
		{
			return "";
		}

		public void WaitFor(double seconds)
		{
			if (!this.isParallelThread)
			{
				DateTime now = DateTime.Now;
				while ((DateTime.Now - now).TotalSeconds < seconds)
				{
					Monitor.Pulse(this.TestLock);
					Monitor.Wait(this.TestLock);
				}
			}
		}

		public virtual async Task WaitUntil(Func<bool> func)
		{
			while (!func())
			{
				await this.WaitForAsync(0.1);
			}
		}

		public Task WaitForAsync(double seconds, Random random)
		{
			return Task.Delay((int)(seconds * 1000.0 * random.NextDouble()));
		}

		public Task WaitForAsync(double seconds)
		{
			return Task.Delay((int)(seconds * 1000.0));
		}

		public static string GetAttachmentsFolderPath()
		{
			return "..\\..\\..\\Tools\\TestAutomation\\Attachments\\";
		}

		public virtual void OnFinalize()
		{
			TestCommonBase._baseInstance = null;
		}

		public int TestRandomSeed;

		public bool IsTestEnabled;

		public bool isParallelThread;

		public string SceneNameToOpenOnStartup;

		public object TestLock = new object();

		private static TestCommonBase _baseInstance;

		private DateTime timeoutTimerStart = DateTime.Now;

		private bool timeoutTimerEnabled = true;

		private int commonWaitTimeoutLimits = 420;
	}
}
