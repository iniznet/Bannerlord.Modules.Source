using System;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000012 RID: 18
	public interface ISessionlessClientDriver
	{
		// Token: 0x06000050 RID: 80
		void SendMessage(Message message);

		// Token: 0x06000051 RID: 81
		Task<T> CallFunction<T>(Message message) where T : FunctionResult;

		// Token: 0x06000052 RID: 82
		Task<bool> CheckConnection();
	}
}
