using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000023 RID: 35
	public class ManagedToUnmanagedScopedCallCounter : IDisposable
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x000048DC File Offset: 0x00002ADC
		public ManagedToUnmanagedScopedCallCounter()
		{
			if (!ManagedToUnmanagedScopedCallCounter._table.IsValueCreated)
			{
				ManagedToUnmanagedScopedCallCounter._table.Value = new Dictionary<int, List<StackTrace>>();
			}
			ThreadLocal<int> depth = ManagedToUnmanagedScopedCallCounter._depth;
			int value = depth.Value;
			depth.Value = value + 1;
			if (ManagedToUnmanagedScopedCallCounter._depth.Value < ManagedToUnmanagedScopedCallCounter._depthThreshold)
			{
				return;
			}
			this._st = new StackTrace(true);
			List<StackTrace> list;
			if (ManagedToUnmanagedScopedCallCounter._table.Value.TryGetValue(ManagedToUnmanagedScopedCallCounter._depth.Value, out list))
			{
				list.Add(this._st);
				return;
			}
			ManagedToUnmanagedScopedCallCounter._table.Value.Add(ManagedToUnmanagedScopedCallCounter._depth.Value, new List<StackTrace> { this._st });
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004990 File Offset: 0x00002B90
		public void Dispose()
		{
			ThreadLocal<int> depth = ManagedToUnmanagedScopedCallCounter._depth;
			int value = depth.Value;
			depth.Value = value - 1;
		}

		// Token: 0x04000050 RID: 80
		private static ThreadLocal<Dictionary<int, List<StackTrace>>> _table = new ThreadLocal<Dictionary<int, List<StackTrace>>>();

		// Token: 0x04000051 RID: 81
		private static ThreadLocal<int> _depth = new ThreadLocal<int>();

		// Token: 0x04000052 RID: 82
		private static int _depthThreshold = 4;

		// Token: 0x04000053 RID: 83
		private StackTrace _st;
	}
}
