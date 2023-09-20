using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000082 RID: 130
	public interface IGameStateListener
	{
		// Token: 0x060007A8 RID: 1960
		void OnActivate();

		// Token: 0x060007A9 RID: 1961
		void OnDeactivate();

		// Token: 0x060007AA RID: 1962
		void OnInitialize();

		// Token: 0x060007AB RID: 1963
		void OnFinalize();
	}
}
