using System;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x0200000D RID: 13
	public abstract class AwaitableAsyncRunner
	{
		// Token: 0x0600002E RID: 46
		public abstract Task RunAsync();

		// Token: 0x0600002F RID: 47
		public abstract void OnTick(float dt);
	}
}
