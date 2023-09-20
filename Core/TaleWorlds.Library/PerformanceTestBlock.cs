using System;
using System.Diagnostics;

namespace TaleWorlds.Library
{
	public class PerformanceTestBlock : IDisposable
	{
		public PerformanceTestBlock(string name)
		{
			this._name = name;
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
		}

		void IDisposable.Dispose()
		{
			long elapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
			Debug.Print(string.Concat(new object[] { this._name, " took ", elapsedMilliseconds, " ms." }), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private readonly string _name;

		private readonly Stopwatch _stopwatch;
	}
}
