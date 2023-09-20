using System;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x0200004C RID: 76
	public interface IInnerProcessServer
	{
		// Token: 0x060001B9 RID: 441
		InnerProcessServerSession AddNewConnection(IInnerProcessClient client);

		// Token: 0x060001BA RID: 442
		void Update();
	}
}
