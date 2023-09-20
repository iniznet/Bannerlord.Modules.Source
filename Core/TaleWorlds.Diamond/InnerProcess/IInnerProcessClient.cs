using System;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x02000048 RID: 72
	public interface IInnerProcessClient
	{
		// Token: 0x060001A3 RID: 419
		void EnqueueMessage(Message message);

		// Token: 0x060001A4 RID: 420
		void HandleConnected(InnerProcessServerSession serverSession);
	}
}
