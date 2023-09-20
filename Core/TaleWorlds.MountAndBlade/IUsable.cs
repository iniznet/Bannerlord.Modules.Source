using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037D RID: 893
	public interface IUsable
	{
		// Token: 0x06003066 RID: 12390
		void OnUse(Agent userAgent);

		// Token: 0x06003067 RID: 12391
		void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex);
	}
}
