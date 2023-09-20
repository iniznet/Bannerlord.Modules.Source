using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000005 RID: 5
	[EngineClass("rglManaged_concurrent_task")]
	public sealed class AsyncTask : NativeObject, ITask
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000022C0 File Offset: 0x000004C0
		internal AsyncTask(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022CF File Offset: 0x000004CF
		public static AsyncTask CreateWithDelegate(ManagedDelegate function, bool isBackground)
		{
			return EngineApplicationInterface.IAsyncTask.CreateWithDelegate(function, isBackground);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022DD File Offset: 0x000004DD
		void ITask.Invoke()
		{
			EngineApplicationInterface.IAsyncTask.Invoke(base.Pointer);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022EF File Offset: 0x000004EF
		void ITask.Wait()
		{
			EngineApplicationInterface.IAsyncTask.Wait(base.Pointer);
		}
	}
}
