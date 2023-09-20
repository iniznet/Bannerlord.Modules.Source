using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000016 RID: 22
	public struct BannerColor
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00005050 File Offset: 0x00003250
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00005058 File Offset: 0x00003258
		public uint Color { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00005061 File Offset: 0x00003261
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00005069 File Offset: 0x00003269
		public bool PlayerCanChooseForSigil { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00005072 File Offset: 0x00003272
		// (set) Token: 0x060000FE RID: 254 RVA: 0x0000507A File Offset: 0x0000327A
		public bool PlayerCanChooseForBackground { get; private set; }

		// Token: 0x060000FF RID: 255 RVA: 0x00005083 File Offset: 0x00003283
		public BannerColor(uint color, bool playerCanChooseForSigil, bool playerCanChooseForBackground)
		{
			this.Color = color;
			this.PlayerCanChooseForSigil = playerCanChooseForSigil;
			this.PlayerCanChooseForBackground = playerCanChooseForBackground;
		}
	}
}
