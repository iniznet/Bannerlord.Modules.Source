using System;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public sealed class NativeParallelDriver : IParallelDriver
	{
		public void For(int fromInclusive, int toExclusive, TWParallel.ParallelForAuxPredicate loopBody, int grainSize)
		{
			long num = Interlocked.Increment(ref NativeParallelDriver.LoopBodyHolder.UniqueLoopBodyKeySeed) % 256L;
			checked
			{
				NativeParallelDriver._loopBodyCache[(int)((IntPtr)num)].LoopBody = loopBody;
				Utilities.ParallelFor(fromInclusive, toExclusive, num, grainSize);
				NativeParallelDriver._loopBodyCache[(int)((IntPtr)num)].LoopBody = null;
			}
		}

		[EngineCallback]
		internal static void ParalelForLoopBodyCaller(long loopBodyKey, int localStartIndex, int localEndIndex)
		{
			NativeParallelDriver._loopBodyCache[(int)(checked((IntPtr)loopBodyKey))].LoopBody(localStartIndex, localEndIndex);
		}

		public void For(int fromInclusive, int toExclusive, float deltaTime, TWParallel.ParallelForWithDtAuxPredicate loopBody, int grainSize)
		{
			long num = Interlocked.Increment(ref NativeParallelDriver.LoopBodyWithDtHolder.UniqueLoopBodyKeySeed) % 256L;
			checked
			{
				NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)num)].LoopBody = loopBody;
				NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)num)].DeltaTime = deltaTime;
				Utilities.ParallelForWithDt(fromInclusive, toExclusive, num, grainSize);
				NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)num)].LoopBody = null;
			}
		}

		[EngineCallback]
		internal static void ParalelForLoopBodyWithDtCaller(long loopBodyKey, int localStartIndex, int localEndIndex)
		{
			checked
			{
				NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)loopBodyKey)].LoopBody(localStartIndex, localEndIndex, NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)loopBodyKey)].DeltaTime);
			}
		}

		public ulong GetMainThreadId()
		{
			return Utilities.GetMainThreadId();
		}

		public ulong GetCurrentThreadId()
		{
			return Utilities.GetCurrentThreadId();
		}

		private const int K = 256;

		private static NativeParallelDriver.LoopBodyHolder[] _loopBodyCache = new NativeParallelDriver.LoopBodyHolder[256];

		private static NativeParallelDriver.LoopBodyWithDtHolder[] _loopBodyWithDtCache = new NativeParallelDriver.LoopBodyWithDtHolder[256];

		private struct LoopBodyHolder
		{
			public static long UniqueLoopBodyKeySeed;

			public TWParallel.ParallelForAuxPredicate LoopBody;
		}

		private struct LoopBodyWithDtHolder
		{
			public static long UniqueLoopBodyKeySeed;

			public TWParallel.ParallelForWithDtAuxPredicate LoopBody;

			public float DeltaTime;
		}
	}
}
