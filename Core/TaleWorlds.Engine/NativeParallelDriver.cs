using System;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006A RID: 106
	public sealed class NativeParallelDriver : IParallelDriver
	{
		// Token: 0x0600085D RID: 2141 RVA: 0x000084DC File Offset: 0x000066DC
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

		// Token: 0x0600085E RID: 2142 RVA: 0x00008529 File Offset: 0x00006729
		[EngineCallback]
		internal static void ParalelForLoopBodyCaller(long loopBodyKey, int localStartIndex, int localEndIndex)
		{
			NativeParallelDriver._loopBodyCache[(int)(checked((IntPtr)loopBodyKey))].LoopBody(localStartIndex, localEndIndex);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00008544 File Offset: 0x00006744
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

		// Token: 0x06000860 RID: 2144 RVA: 0x000085A4 File Offset: 0x000067A4
		[EngineCallback]
		internal static void ParalelForLoopBodyWithDtCaller(long loopBodyKey, int localStartIndex, int localEndIndex)
		{
			checked
			{
				NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)loopBodyKey)].LoopBody(localStartIndex, localEndIndex, NativeParallelDriver._loopBodyWithDtCache[(int)((IntPtr)loopBodyKey)].DeltaTime);
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x000085CF File Offset: 0x000067CF
		public ulong GetMainThreadId()
		{
			return Utilities.GetMainThreadId();
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x000085D6 File Offset: 0x000067D6
		public ulong GetCurrentThreadId()
		{
			return Utilities.GetCurrentThreadId();
		}

		// Token: 0x0400013F RID: 319
		private const int K = 256;

		// Token: 0x04000140 RID: 320
		private static NativeParallelDriver.LoopBodyHolder[] _loopBodyCache = new NativeParallelDriver.LoopBodyHolder[256];

		// Token: 0x04000141 RID: 321
		private static NativeParallelDriver.LoopBodyWithDtHolder[] _loopBodyWithDtCache = new NativeParallelDriver.LoopBodyWithDtHolder[256];

		// Token: 0x020000BE RID: 190
		private struct LoopBodyHolder
		{
			// Token: 0x040003F5 RID: 1013
			public static long UniqueLoopBodyKeySeed;

			// Token: 0x040003F6 RID: 1014
			public TWParallel.ParallelForAuxPredicate LoopBody;
		}

		// Token: 0x020000BF RID: 191
		private struct LoopBodyWithDtHolder
		{
			// Token: 0x040003F7 RID: 1015
			public static long UniqueLoopBodyKeySeed;

			// Token: 0x040003F8 RID: 1016
			public TWParallel.ParallelForWithDtAuxPredicate LoopBody;

			// Token: 0x040003F9 RID: 1017
			public float DeltaTime;
		}
	}
}
