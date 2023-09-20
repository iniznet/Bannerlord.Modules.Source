using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x02000090 RID: 144
	public static class TWParallel
	{
		// Token: 0x060004EA RID: 1258 RVA: 0x0000FD0A File Offset: 0x0000DF0A
		public static void InitializeAndSetImplementation(IParallelDriver parallelDriver)
		{
			TWParallel._parallelDriver = parallelDriver;
			TWParallel._mainThreadId = TWParallel.GetMainThreadId();
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0000FD1C File Offset: 0x0000DF1C
		public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
		{
			return Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source), Common.ParallelOptions, body);
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0000FD2F File Offset: 0x0000DF2F
		[Obsolete("Please use For() not ForEach() for better Parallel Performance.", true)]
		public static void ForEach<TSource>(IList<TSource> source, Action<TSource> body)
		{
			Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source), Common.ParallelOptions, body);
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0000FD43 File Offset: 0x0000DF43
		public static void For(int fromInclusive, int toExclusive, TWParallel.ParallelForAuxPredicate body, int grainSize = 16)
		{
			TWParallel.IsInParallelFor = true;
			if (toExclusive - fromInclusive < grainSize)
			{
				body(fromInclusive, toExclusive);
			}
			else
			{
				TWParallel._parallelDriver.For(fromInclusive, toExclusive, body, grainSize);
			}
			TWParallel.IsInParallelFor = false;
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0000FD6F File Offset: 0x0000DF6F
		public static void For(int fromInclusive, int toExclusive, float deltaTime, TWParallel.ParallelForWithDtAuxPredicate body, int grainSize = 16)
		{
			TWParallel.IsInParallelFor = true;
			if (toExclusive - fromInclusive < grainSize)
			{
				body(fromInclusive, toExclusive, deltaTime);
			}
			else
			{
				TWParallel._parallelDriver.For(fromInclusive, toExclusive, deltaTime, body, grainSize);
			}
			TWParallel.IsInParallelFor = false;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0000FD9F File Offset: 0x0000DF9F
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void AssertIsMainThread()
		{
			TWParallel.GetCurrentThreadId();
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0000FDA7 File Offset: 0x0000DFA7
		public static bool IsMainThread()
		{
			return TWParallel._mainThreadId == TWParallel.GetCurrentThreadId();
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0000FDB5 File Offset: 0x0000DFB5
		private static ulong GetMainThreadId()
		{
			return TWParallel._parallelDriver.GetMainThreadId();
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0000FDC1 File Offset: 0x0000DFC1
		private static ulong GetCurrentThreadId()
		{
			return TWParallel._parallelDriver.GetCurrentThreadId();
		}

		// Token: 0x04000177 RID: 375
		private static IParallelDriver _parallelDriver = new DefaultParallelDriver();

		// Token: 0x04000178 RID: 376
		private static ulong _mainThreadId;

		// Token: 0x04000179 RID: 377
		public static bool IsInParallelFor = false;

		// Token: 0x020000D7 RID: 215
		public class SingleThreadTestData
		{
			// Token: 0x040002A7 RID: 679
			public static TWParallel.SingleThreadTestData GlobalData = new TWParallel.SingleThreadTestData();

			// Token: 0x040002A8 RID: 680
			public int InsideThreadCount;
		}

		// Token: 0x020000D8 RID: 216
		public struct SingleThreadTestBlock : IDisposable
		{
			// Token: 0x06000702 RID: 1794 RVA: 0x0001581A File Offset: 0x00013A1A
			public SingleThreadTestBlock(TWParallel.SingleThreadTestData data)
			{
				TWParallel.SingleThreadTestBlock.SingleThreadTestAssert(Interlocked.Increment(ref data.InsideThreadCount) == 1);
				this._data = data;
			}

			// Token: 0x06000703 RID: 1795 RVA: 0x00015836 File Offset: 0x00013A36
			public void Dispose()
			{
				TWParallel.SingleThreadTestBlock.SingleThreadTestAssert(Interlocked.Decrement(ref this._data.InsideThreadCount) == 0);
			}

			// Token: 0x06000704 RID: 1796 RVA: 0x00015850 File Offset: 0x00013A50
			private static void SingleThreadTestAssert(bool b)
			{
				if (!b)
				{
					Debugger.Break();
					Debug.FailedAssert("Single thread test have failed!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\TWParallel.cs", "SingleThreadTestAssert", 89);
				}
			}

			// Token: 0x040002A9 RID: 681
			private readonly TWParallel.SingleThreadTestData _data;
		}

		// Token: 0x020000D9 RID: 217
		public class RecursiveSingleThreadTestData
		{
			// Token: 0x040002AA RID: 682
			public static TWParallel.RecursiveSingleThreadTestData GlobalData = new TWParallel.RecursiveSingleThreadTestData();

			// Token: 0x040002AB RID: 683
			public static TWParallel.RecursiveSingleThreadTestData ScriptComponentAddRemove = new TWParallel.RecursiveSingleThreadTestData();

			// Token: 0x040002AC RID: 684
			public int InsideCallCount;

			// Token: 0x040002AD RID: 685
			public int InsideThreadId = -1;
		}

		// Token: 0x020000DA RID: 218
		public struct RecursiveSingleThreadTestBlock : IDisposable
		{
			// Token: 0x06000707 RID: 1799 RVA: 0x00015898 File Offset: 0x00013A98
			public RecursiveSingleThreadTestBlock(TWParallel.RecursiveSingleThreadTestData data)
			{
				this._data = data;
				int threadId = this.GetThreadId();
				TWParallel.RecursiveSingleThreadTestData data2 = this._data;
				lock (data2)
				{
					if (Interlocked.Increment(ref data.InsideCallCount) == 1)
					{
						data.InsideThreadId = threadId;
					}
				}
				TWParallel.RecursiveSingleThreadTestBlock.SingleThreadTestAssert(data.InsideThreadId == threadId);
			}

			// Token: 0x06000708 RID: 1800 RVA: 0x00015904 File Offset: 0x00013B04
			public void Dispose()
			{
				int threadId = this.GetThreadId();
				TWParallel.RecursiveSingleThreadTestBlock.SingleThreadTestAssert(this._data.InsideThreadId == threadId);
				TWParallel.RecursiveSingleThreadTestData data = this._data;
				lock (data)
				{
					if (Interlocked.Decrement(ref this._data.InsideCallCount) == 0)
					{
						this._data.InsideThreadId = -1;
					}
				}
			}

			// Token: 0x06000709 RID: 1801 RVA: 0x00015978 File Offset: 0x00013B78
			private static void SingleThreadTestAssert(bool b)
			{
				if (!b)
				{
					Debugger.Break();
					Debug.FailedAssert("Single thread test have failed!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\TWParallel.cs", "SingleThreadTestAssert", 149);
				}
			}

			// Token: 0x0600070A RID: 1802 RVA: 0x0001599B File Offset: 0x00013B9B
			private int GetThreadId()
			{
				return Thread.CurrentThread.ManagedThreadId;
			}

			// Token: 0x040002AE RID: 686
			private readonly TWParallel.RecursiveSingleThreadTestData _data;
		}

		// Token: 0x020000DB RID: 219
		// (Invoke) Token: 0x0600070C RID: 1804
		public delegate void ParallelForAuxPredicate(int localStartIndex, int localEndIndex);

		// Token: 0x020000DC RID: 220
		// (Invoke) Token: 0x06000710 RID: 1808
		public delegate void ParallelForWithDtAuxPredicate(int localStartIndex, int localEndIndex, float dt);
	}
}
