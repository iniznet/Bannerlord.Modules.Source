using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000035 RID: 53
	public interface ITexture
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000247 RID: 583
		int Width { get; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000248 RID: 584
		int Height { get; }

		// Token: 0x06000249 RID: 585
		void Release();

		// Token: 0x0600024A RID: 586
		bool IsLoaded();
	}
}
