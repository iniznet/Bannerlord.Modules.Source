using System;
using System.Diagnostics;

namespace TaleWorlds.Library
{
	public class PerformanceTestBlock : IDisposable
	{
		public PerformanceTestBlock(string name)
		{
			this._name = name;
			Debug.Print(this._name + " block is started.", 0, Debug.DebugColor.White, 17592186044416UL);
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
		}

		void IDisposable.Dispose()
		{
			float num = (float)this._stopwatch.ElapsedMilliseconds / 1000f;
			Debug.Print(string.Concat(new object[] { this._name, " completed in ", num, " seconds." }), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private readonly string _name;

		private readonly Stopwatch _stopwatch;
	}
}
