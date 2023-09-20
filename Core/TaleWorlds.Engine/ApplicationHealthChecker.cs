using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public class ApplicationHealthChecker
	{
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

		public void Stop()
		{
			this._thread = null;
			this._stopwatch = null;
			this._isRunning = false;
		}

		public void Update()
		{
			if (this._isRunning)
			{
				this._stopwatch.Restart();
				this._mainThread = Thread.CurrentThread;
			}
		}

		private static void Print(string log)
		{
			Debug.Print(log, 0, Debug.DebugColor.White, 17592186044416UL);
			Console.WriteLine(log);
		}

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

		private Thread _thread;

		private bool _isRunning;

		private Stopwatch _stopwatch;

		private Thread _mainThread;
	}
}
