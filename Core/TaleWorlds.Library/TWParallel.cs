using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	public static class TWParallel
	{
		public static void InitializeAndSetImplementation(IParallelDriver parallelDriver)
		{
			TWParallel._parallelDriver = parallelDriver;
			TWParallel._mainThreadId = TWParallel.GetMainThreadId();
		}

		public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
		{
			return Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source), Common.ParallelOptions, body);
		}

		[Obsolete("Please use For() not ForEach() for better Parallel Performance.", true)]
		public static void ForEach<TSource>(IList<TSource> source, Action<TSource> body)
		{
			Parallel.ForEach<TSource>(Partitioner.Create<TSource>(source), Common.ParallelOptions, body);
		}

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

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void AssertIsMainThread()
		{
			TWParallel.GetCurrentThreadId();
		}

		public static bool IsMainThread()
		{
			return TWParallel._mainThreadId == TWParallel.GetCurrentThreadId();
		}

		private static ulong GetMainThreadId()
		{
			return TWParallel._parallelDriver.GetMainThreadId();
		}

		private static ulong GetCurrentThreadId()
		{
			return TWParallel._parallelDriver.GetCurrentThreadId();
		}

		private static IParallelDriver _parallelDriver = new DefaultParallelDriver();

		private static ulong _mainThreadId;

		public static bool IsInParallelFor = false;

		public class SingleThreadTestData
		{
			public static TWParallel.SingleThreadTestData GlobalData = new TWParallel.SingleThreadTestData();

			public int InsideThreadCount;
		}

		public struct SingleThreadTestBlock : IDisposable
		{
			public SingleThreadTestBlock(TWParallel.SingleThreadTestData data)
			{
				TWParallel.SingleThreadTestBlock.SingleThreadTestAssert(Interlocked.Increment(ref data.InsideThreadCount) == 1);
				this._data = data;
			}

			public void Dispose()
			{
				TWParallel.SingleThreadTestBlock.SingleThreadTestAssert(Interlocked.Decrement(ref this._data.InsideThreadCount) == 0);
			}

			private static void SingleThreadTestAssert(bool b)
			{
				if (!b)
				{
					Debugger.Break();
					Debug.FailedAssert("Single thread test have failed!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\TWParallel.cs", "SingleThreadTestAssert", 89);
				}
			}

			private readonly TWParallel.SingleThreadTestData _data;
		}

		public class RecursiveSingleThreadTestData
		{
			public static TWParallel.RecursiveSingleThreadTestData GlobalData = new TWParallel.RecursiveSingleThreadTestData();

			public static TWParallel.RecursiveSingleThreadTestData ScriptComponentAddRemove = new TWParallel.RecursiveSingleThreadTestData();

			public int InsideCallCount;

			public int InsideThreadId = -1;
		}

		public struct RecursiveSingleThreadTestBlock : IDisposable
		{
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

			private static void SingleThreadTestAssert(bool b)
			{
				if (!b)
				{
					Debugger.Break();
					Debug.FailedAssert("Single thread test have failed!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\TWParallel.cs", "SingleThreadTestAssert", 149);
				}
			}

			private int GetThreadId()
			{
				return Thread.CurrentThread.ManagedThreadId;
			}

			private readonly TWParallel.RecursiveSingleThreadTestData _data;
		}

		public delegate void ParallelForAuxPredicate(int localStartIndex, int localEndIndex);

		public delegate void ParallelForWithDtAuxPredicate(int localStartIndex, int localEndIndex, float dt);
	}
}
