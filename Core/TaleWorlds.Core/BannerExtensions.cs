using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200000D RID: 13
	public static class BannerExtensions
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00003CFC File Offset: 0x00001EFC
		public static bool IsContentsSameWith(this Banner banner, Banner otherBanner)
		{
			if (banner == null && otherBanner == null)
			{
				return true;
			}
			if (banner == null || otherBanner == null)
			{
				return false;
			}
			if (banner.BannerDataList.Count != otherBanner.BannerDataList.Count)
			{
				return false;
			}
			for (int i = 0; i < banner.BannerDataList.Count; i++)
			{
				object obj = banner.BannerDataList[i];
				BannerData bannerData = otherBanner.BannerDataList[i];
				if (!obj.Equals(bannerData))
				{
					return false;
				}
			}
			return true;
		}
	}
}
