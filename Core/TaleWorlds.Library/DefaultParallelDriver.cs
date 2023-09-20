using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x0200008F RID: 143
	public sealed class DefaultParallelDriver : IParallelDriver
	{
		// Token: 0x060004E5 RID: 1253 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public void For(int fromInclusive, int toExclusive, TWParallel.ParallelForAuxPredicate body, int grainSize)
		{
			Parallel.ForEach<Tuple<int, int>>(Partitioner.Create(fromInclusive, toExclusive, grainSize), Common.ParallelOptions, delegate(Tuple<int, int> range, ParallelLoopState loopState)
			{
				body(range.Item1, range.Item2);
			});
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0000FCB8 File Offset: 0x0000DEB8
		public void For(int fromInclusive, int toExclusive, float deltaTime, TWParallel.ParallelForWithDtAuxPredicate body, int grainSize)
		{
			Parallel.ForEach<Tuple<int, int>>(Partitioner.Create(fromInclusive, toExclusive, grainSize), Common.ParallelOptions, delegate(Tuple<int, int> range, ParallelLoopState loopState)
			{
				body(range.Item1, range.Item2, deltaTime);
			});
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0000FCFA File Offset: 0x0000DEFA
		public ulong GetMainThreadId()
		{
			return 0UL;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0000FCFE File Offset: 0x0000DEFE
		public ulong GetCurrentThreadId()
		{
			return 0UL;
		}
	}
}
