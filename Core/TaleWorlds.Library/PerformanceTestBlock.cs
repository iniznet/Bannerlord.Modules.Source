using System;
using System.Diagnostics;

namespace TaleWorlds.Library
{
	// Token: 0x02000071 RID: 113
	public class PerformanceTestBlock : IDisposable
	{
		// Token: 0x060003D1 RID: 977 RVA: 0x0000C0AB File Offset: 0x0000A2AB
		public PerformanceTestBlock(string name)
		{
			this._name = name;
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0000C0D0 File Offset: 0x0000A2D0
		void IDisposable.Dispose()
		{
			long elapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
			Debug.Print(string.Concat(new object[] { this._name, " took ", elapsedMilliseconds, " ms." }), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x04000126 RID: 294
		private readonly string _name;

		// Token: 0x04000127 RID: 295
		private readonly Stopwatch _stopwatch;
	}
}
