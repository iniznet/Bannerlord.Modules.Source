using System;

namespace TaleWorlds.Core
{
	public static class BannerExtensions
	{
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
