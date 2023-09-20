using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000011 RID: 17
	public interface ISessionlessClient
	{
		// Token: 0x0600004F RID: 79
		Task<bool> CheckConnection();
	}
}
