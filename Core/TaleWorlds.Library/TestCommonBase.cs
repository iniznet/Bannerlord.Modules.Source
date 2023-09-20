using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x02000088 RID: 136
	public abstract class TestCommonBase
	{
		// Token: 0x060004AA RID: 1194
		public abstract void Tick();

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x0000F087 File Offset: 0x0000D287
		public static TestCommonBase BaseInstance
		{
			get
			{
				return TestCommonBase._baseInstance;
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0000F08E File Offset: 0x0000D28E
		public void StartTimeoutTimer()
		{
			this.timeoutTimerStart = DateTime.Now;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0000F09B File Offset: 0x0000D29B
		public void ToggleTimeoutTimer()
		{
			this.timeoutTimerEnabled = !this.timeoutTimerEnabled;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0000F0AC File Offset: 0x0000D2AC
		public bool CheckTimeoutTimer()
		{
			return this.timeoutTimerEnabled && DateTime.Now.Subtract(this.timeoutTimerStart).TotalSeconds > (double)this.commonWaitTimeoutLimits;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0000F0EA File Offset: 0x0000D2EA
		protected TestCommonBase()
		{
			TestCommonBase._baseInstance = this;
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0000F120 File Offset: 0x0000D320
		public virtual string GetGameStatus()
		{
			return "";
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0000F128 File Offset: 0x0000D328
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

		// Token: 0x060004B2 RID: 1202 RVA: 0x0000F174 File Offset: 0x0000D374
		public virtual async Task WaitUntil(Func<bool> func)
		{
			while (!func())
			{
				await this.WaitForAsync(0.1);
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0000F1C1 File Offset: 0x0000D3C1
		public Task WaitForAsync(double seconds, Random random)
		{
			return Task.Delay((int)(seconds * 1000.0 * random.NextDouble()));
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0000F1DB File Offset: 0x0000D3DB
		public Task WaitForAsync(double seconds)
		{
			return Task.Delay((int)(seconds * 1000.0));
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0000F1EE File Offset: 0x0000D3EE
		public static string GetAttachmentsFolderPath()
		{
			return "..\\..\\..\\Tools\\TestAutomation\\Attachments\\";
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0000F1F5 File Offset: 0x0000D3F5
		public virtual void OnFinalize()
		{
			TestCommonBase._baseInstance = null;
		}

		// Token: 0x04000164 RID: 356
		public int TestRandomSeed;

		// Token: 0x04000165 RID: 357
		public bool IsTestEnabled;

		// Token: 0x04000166 RID: 358
		public bool isParallelThread;

		// Token: 0x04000167 RID: 359
		public string SceneNameToOpenOnStartup;

		// Token: 0x04000168 RID: 360
		public object TestLock = new object();

		// Token: 0x04000169 RID: 361
		private static TestCommonBase _baseInstance;

		// Token: 0x0400016A RID: 362
		private DateTime timeoutTimerStart = DateTime.Now;

		// Token: 0x0400016B RID: 363
		private bool timeoutTimerEnabled = true;

		// Token: 0x0400016C RID: 364
		private int commonWaitTimeoutLimits = 420;
	}
}
