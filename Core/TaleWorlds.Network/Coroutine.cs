using System;
using System.Collections;

namespace TaleWorlds.Network
{
	// Token: 0x02000005 RID: 5
	public class Coroutine
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002628 File Offset: 0x00000828
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002630 File Offset: 0x00000830
		public bool IsStarted { get; internal set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002639 File Offset: 0x00000839
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002641 File Offset: 0x00000841
		internal CoroutineDelegate CoroutineMethod { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000264A File Offset: 0x0000084A
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002652 File Offset: 0x00000852
		internal IEnumerator Enumerator { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000265B File Offset: 0x0000085B
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002663 File Offset: 0x00000863
		internal CoroutineState CurrentState { get; set; }
	}
}
