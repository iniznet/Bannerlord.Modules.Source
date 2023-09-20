using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TaleWorlds.DotNet
{
	public class ManagedToUnmanagedScopedCallCounter : IDisposable
	{
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

		public void Dispose()
		{
			ThreadLocal<int> depth = ManagedToUnmanagedScopedCallCounter._depth;
			int value = depth.Value;
			depth.Value = value - 1;
		}

		private static ThreadLocal<Dictionary<int, List<StackTrace>>> _table = new ThreadLocal<Dictionary<int, List<StackTrace>>>();

		private static ThreadLocal<int> _depth = new ThreadLocal<int>();

		private static int _depthThreshold = 4;

		private StackTrace _st;
	}
}
