using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200008E RID: 142
	public interface IParallelDriver
	{
		// Token: 0x060004E1 RID: 1249
		void For(int fromInclusive, int toExclusive, TWParallel.ParallelForAuxPredicate body, int grainSize);

		// Token: 0x060004E2 RID: 1250
		void For(int fromInclusive, int toExclusive, float deltaTime, TWParallel.ParallelForWithDtAuxPredicate body, int grainSize);

		// Token: 0x060004E3 RID: 1251
		ulong GetMainThreadId();

		// Token: 0x060004E4 RID: 1252
		ulong GetCurrentThreadId();
	}
}
