using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x0200001C RID: 28
	public static class UIColors
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000F72D File Offset: 0x0000D92D
		public static Color PositiveIndicator
		{
			get
			{
				return UIColors._positiveIndicator;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000F734 File Offset: 0x0000D934
		public static Color NegativeIndicator
		{
			get
			{
				return UIColors._negativeIndicator;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000F73B File Offset: 0x0000D93B
		public static Color Gold
		{
			get
			{
				return UIColors._gold;
			}
		}

		// Token: 0x040000BB RID: 187
		private static Color _positiveIndicator = Color.FromUint(4285250886U);

		// Token: 0x040000BC RID: 188
		private static Color _negativeIndicator = Color.FromUint(4290070086U);

		// Token: 0x040000BD RID: 189
		private static Color _gold = Color.FromUint(4294957447U);
	}
}
