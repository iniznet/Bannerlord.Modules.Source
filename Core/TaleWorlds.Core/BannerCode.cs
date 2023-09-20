using System;

namespace TaleWorlds.Core
{
	public class BannerCode
	{
		public string Code { get; private set; }

		public Banner CalculateBanner()
		{
			return new Banner(this.Code);
		}

		public static BannerCode CreateFrom(Banner banner)
		{
			BannerCode bannerCode = new BannerCode();
			if (banner != null)
			{
				bannerCode.Code = banner.Serialize();
			}
			return bannerCode;
		}

		public static BannerCode CreateFrom(string bannerCodeCode)
		{
			return new BannerCode
			{
				Code = bannerCodeCode
			};
		}

		public override int GetHashCode()
		{
			return this.Code.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is BannerCode && !(((BannerCode)obj).Code != this.Code);
		}

		public static bool operator ==(BannerCode a, BannerCode b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		public static bool operator !=(BannerCode a, BannerCode b)
		{
			return !(a == b);
		}
	}
}
