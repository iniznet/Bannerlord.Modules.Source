using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000004 RID: 4
	public class ApplicationHealthChecker
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public void Start()
		{
			Debug.Print("Starting ApplicationHealthChecker", 0, Debug.DebugColor.White, 17592186044416UL);
			try
			{
				File.WriteAllText(BasePath.Name + "Application.HealthCheckerStarted", "...");
			}
			catch (Exception ex)
			{
				ApplicationHealthChecker.Print("Blocked main thread file create e: " + ex);
			}
			this._isRunning = true;
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
			this._thread = new Thread(new ThreadStart(this.ThreadUpdate));
			this._thread.IsBackground = true;
			this._thread.Start();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002104 File Offset: 0x00000304
		public void Stop()
		{
			this._thread = null;
			this._stopwatch = null;
			this._isRunning = false;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000211B File Offset: 0x0000031B
		public void Update()
		{
			if (this._isRunning)
			{
				this._stopwatch.Restart();
				this._mainThread = Thread.CurrentThread;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000213B File Offset: 0x0000033B
		private static void Print(string log)
		{
			Debug.Print(log, 0, Debug.DebugColor.White, 17592186044416UL);
			Console.WriteLine(log);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002158 File Offset: 0x00000358
		private void ThreadUpdate()
		{
			while (this._isRunning)
			{
				long num = this._stopwatch.ElapsedMilliseconds / 1000L;
				if (num > 180L)
				{
					ApplicationHealthChecker.Print("Main thread is blocked for " + num + " seconds");
					try
					{
						File.WriteAllText(BasePath.Name + "Application.Blocked", num.ToString());
					}
					catch (Exception ex)
					{
						ApplicationHealthChecker.Print("Blocked main thread file create e: " + ex);
					}
					try
					{
						ApplicationHealthChecker.Print("Blocked main thread IsAlive: " + this._mainThread.IsAlive.ToString());
						ApplicationHealthChecker.Print("Blocked main thread ThreadState: " + this._mainThread.ThreadState);
					}
					catch (Exception ex2)
					{
						ApplicationHealthChecker.Print("Blocked main thread e: " + ex2);
					}
					Utilities.ExitProcess(1453);
				}
				else
				{
					try
					{
						if (File.Exists(BasePath.Name + "Application.Blocked"))
						{
							File.Delete(BasePath.Name + "Application.Blocked");
						}
					}
					catch (Exception ex3)
					{
						ApplicationHealthChecker.Print("Blocked main thread file delete e: " + ex3);
					}
				}
				Thread.Sleep(10000);
			}
		}

		// Token: 0x04000001 RID: 1
		private Thread _thread;

		// Token: 0x04000002 RID: 2
		private bool _isRunning;

		// Token: 0x04000003 RID: 3
		private Stopwatch _stopwatch;

		// Token: 0x04000004 RID: 4
		private Thread _mainThread;
	}
}
