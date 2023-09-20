using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000055 RID: 85
	public interface IEntityComponent
	{
		// Token: 0x0600062B RID: 1579
		void OnInitialize();

		// Token: 0x0600062C RID: 1580
		void OnFinalize();
	}
}
