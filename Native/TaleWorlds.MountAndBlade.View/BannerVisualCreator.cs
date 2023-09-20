using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000008 RID: 8
	public class BannerVisualCreator : IBannerVisualCreator
	{
		// Token: 0x06000040 RID: 64 RVA: 0x000038E6 File Offset: 0x00001AE6
		IBannerVisual IBannerVisualCreator.CreateBannerVisual(Banner banner)
		{
			return new BannerVisual(banner);
		}
	}
}
