using System;
using System.Diagnostics;

namespace TaleWorlds.Library
{
	public class ScopedTimer : IDisposable
	{
		public ScopedTimer(string scopeName)
		{
			this.scopeName_ = scopeName;
			this.watch_ = new Stopwatch();
			this.watch_.Start();
		}

		public void Dispose()
		{
			this.watch_.Stop();
			Console.WriteLine(string.Concat(new object[]
			{
				"ScopedTimer: ",
				this.scopeName_,
				" elapsed ms: ",
				this.watch_.Elapsed.TotalMilliseconds
			}));
		}

		private readonly Stopwatch watch_;

		private readonly string scopeName_;
	}
}
