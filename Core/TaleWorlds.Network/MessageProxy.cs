using System;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
	// Token: 0x0200000D RID: 13
	public abstract class MessageProxy
	{
		// Token: 0x06000045 RID: 69
		public abstract Task Invoke(string methodName, params object[] args);
	}
}
