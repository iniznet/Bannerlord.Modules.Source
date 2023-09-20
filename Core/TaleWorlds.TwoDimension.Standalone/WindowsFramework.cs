using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000010 RID: 16
	public class WindowsFramework
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x000049AE File Offset: 0x00002BAE
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x000049B6 File Offset: 0x00002BB6
		public WindowsFrameworkThreadConfig ThreadConfig { get; set; }

		// Token: 0x060000D6 RID: 214 RVA: 0x000049BF File Offset: 0x00002BBF
		public WindowsFramework()
		{
			this._timer = new Stopwatch();
			this._messageCommunicators = new List<IMessageCommunicator>();
			this.IsActive = false;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000049E4 File Offset: 0x00002BE4
		public void Initialize(FrameworkDomain[] frameworkDomains)
		{
			this._frameworkDomains = frameworkDomains;
			this.IsActive = true;
			if (this.ThreadConfig == WindowsFrameworkThreadConfig.SingleThread)
			{
				this._frameworkDomainThreads = new Thread[1];
				this.CreateThread(0);
				return;
			}
			if (this.ThreadConfig == WindowsFrameworkThreadConfig.MultiThread)
			{
				this._frameworkDomainThreads = new Thread[frameworkDomains.Length];
				for (int i = 0; i < frameworkDomains.Length; i++)
				{
					this.CreateThread(i);
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00004A48 File Offset: 0x00002C48
		private void CreateThread(int index)
		{
			Common.SetInvariantCulture();
			this._frameworkDomainThreads[index] = new Thread(new ParameterizedThreadStart(this.MainLoop));
			this._frameworkDomainThreads[index].SetApartmentState(ApartmentState.STA);
			this._frameworkDomainThreads[index].Name = this._frameworkDomains[index].ToString() + " Thread";
			this._frameworkDomainThreads[index].CurrentCulture = CultureInfo.InvariantCulture;
			this._frameworkDomainThreads[index].CurrentUICulture = CultureInfo.InvariantCulture;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00004AC9 File Offset: 0x00002CC9
		public void RegisterMessageCommunicator(IMessageCommunicator communicator)
		{
			this._messageCommunicators.Add(communicator);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00004AD7 File Offset: 0x00002CD7
		public void UnRegisterMessageCommunicator(IMessageCommunicator communicator)
		{
			this._messageCommunicators.Remove(communicator);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00004AE8 File Offset: 0x00002CE8
		private void MessageLoop()
		{
			try
			{
				if (this.ThreadConfig == WindowsFrameworkThreadConfig.NoThread)
				{
					for (int i = 0; i < this._frameworkDomains.Length; i++)
					{
						this._frameworkDomains[i].Update();
					}
				}
				for (int j = 0; j < this._messageCommunicators.Count; j++)
				{
					this._messageCommunicators[j].MessageLoop();
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				throw;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00004B88 File Offset: 0x00002D88
		private void MainLoop(object parameter)
		{
			try
			{
				if (this.ThreadConfig == WindowsFrameworkThreadConfig.SingleThread)
				{
					while (this.IsActive)
					{
						for (int i = 0; i < this._frameworkDomains.Length; i++)
						{
							this._frameworkDomains[i].Update();
						}
					}
				}
				else if (this.ThreadConfig == WindowsFrameworkThreadConfig.MultiThread)
				{
					FrameworkDomain frameworkDomain = parameter as FrameworkDomain;
					while (this.IsActive)
					{
						frameworkDomain.Update();
					}
				}
				Interlocked.Increment(ref this._abortedThreadCount);
				this.OnFinalize();
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				throw;
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00004C40 File Offset: 0x00002E40
		public void Stop()
		{
			this.IsActive = false;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004C4C File Offset: 0x00002E4C
		public void OnFinalize()
		{
			if (this._abortedThreadCount != this._frameworkDomainThreads.Length)
			{
				return;
			}
			this._frameworkDomainThreads = null;
			FrameworkDomain[] frameworkDomains = this._frameworkDomains;
			for (int i = 0; i < frameworkDomains.Length; i++)
			{
				frameworkDomains[i].Destroy();
			}
			this.IsFinalized = true;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00004C98 File Offset: 0x00002E98
		public void Start()
		{
			this._timer.Start();
			this.IsActive = true;
			if (this.ThreadConfig == WindowsFrameworkThreadConfig.SingleThread)
			{
				this._frameworkDomainThreads[0].Start();
			}
			else if (this.ThreadConfig == WindowsFrameworkThreadConfig.MultiThread)
			{
				for (int i = 0; i < this._frameworkDomains.Length; i++)
				{
					this._frameworkDomainThreads[i].Start(this._frameworkDomains[i]);
				}
			}
			NativeMessage nativeMessage = default(NativeMessage);
			if (this.ThreadConfig == WindowsFrameworkThreadConfig.NoThread)
			{
				while (this.IsActive)
				{
					if (User32.PeekMessage(out nativeMessage, IntPtr.Zero, 0U, 0U, 1U))
					{
						User32.TranslateMessage(ref nativeMessage);
						User32.DispatchMessage(ref nativeMessage);
					}
					this.MessageLoop();
				}
				return;
			}
			while (this.IsActive)
			{
				if (User32.PeekMessage(out nativeMessage, IntPtr.Zero, 0U, 0U, 1U))
				{
					if (nativeMessage.msg == WindowMessage.Quit)
					{
						break;
					}
					User32.TranslateMessage(ref nativeMessage);
					User32.DispatchMessage(ref nativeMessage);
				}
				this.MessageLoop();
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00004D7B File Offset: 0x00002F7B
		public long ElapsedTicks
		{
			get
			{
				return this._timer.ElapsedTicks;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00004D88 File Offset: 0x00002F88
		public long TicksPerSecond
		{
			get
			{
				return Stopwatch.Frequency;
			}
		}

		// Token: 0x04000051 RID: 81
		public bool IsActive;

		// Token: 0x04000052 RID: 82
		private FrameworkDomain[] _frameworkDomains;

		// Token: 0x04000053 RID: 83
		private Thread[] _frameworkDomainThreads;

		// Token: 0x04000054 RID: 84
		private Stopwatch _timer;

		// Token: 0x04000056 RID: 86
		private List<IMessageCommunicator> _messageCommunicators;

		// Token: 0x04000057 RID: 87
		public bool IsFinalized;

		// Token: 0x04000058 RID: 88
		private int _abortedThreadCount;
	}
}
