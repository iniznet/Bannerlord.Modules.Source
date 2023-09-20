using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000006 RID: 6
	public abstract class CoroutineState
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002674 File Offset: 0x00000874
		// (set) Token: 0x06000029 RID: 41 RVA: 0x0000267C File Offset: 0x0000087C
		private protected CoroutineManager CoroutineManager { protected get; private set; }

		// Token: 0x0600002A RID: 42 RVA: 0x00002685 File Offset: 0x00000885
		protected internal virtual void Initialize(CoroutineManager coroutineManager)
		{
			this.CoroutineManager = coroutineManager;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43
		protected internal abstract bool IsFinished { get; }
	}
}
