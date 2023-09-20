using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200000F RID: 15
	public class BannerCode
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00003D6D File Offset: 0x00001F6D
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00003D75 File Offset: 0x00001F75
		public string Code { get; private set; }

		// Token: 0x0600009E RID: 158 RVA: 0x00003D7E File Offset: 0x00001F7E
		public Banner CalculateBanner()
		{
			return new Banner(this.Code);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003D8C File Offset: 0x00001F8C
		public static BannerCode CreateFrom(Banner banner)
		{
			BannerCode bannerCode = new BannerCode();
			if (banner != null)
			{
				bannerCode.Code = banner.Serialize();
			}
			return bannerCode;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003DAF File Offset: 0x00001FAF
		public static BannerCode CreateFrom(string bannerCodeCode)
		{
			return new BannerCode
			{
				Code = bannerCodeCode
			};
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003DBD File Offset: 0x00001FBD
		public override int GetHashCode()
		{
			return this.Code.GetHashCode();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003DCA File Offset: 0x00001FCA
		public override bool Equals(object obj)
		{
			return obj != null && obj is BannerCode && !(((BannerCode)obj).Code != this.Code);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003DF6 File Offset: 0x00001FF6
		public static bool operator ==(BannerCode a, BannerCode b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003E0D File Offset: 0x0000200D
		public static bool operator !=(BannerCode a, BannerCode b)
		{
			return !(a == b);
		}
	}
}
