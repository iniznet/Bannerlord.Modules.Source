using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x02000007 RID: 7
	public interface IGauntletMovie
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600006B RID: 107
		Widget RootWidget { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006C RID: 108
		string MovieName { get; }

		// Token: 0x0600006D RID: 109
		void Update();

		// Token: 0x0600006E RID: 110
		void Release();

		// Token: 0x0600006F RID: 111
		void RefreshBindingWithChildren();
	}
}
