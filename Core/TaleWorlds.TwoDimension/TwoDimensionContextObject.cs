using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000037 RID: 55
	public class TwoDimensionContextObject
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0000984F File Offset: 0x00007A4F
		// (set) Token: 0x06000272 RID: 626 RVA: 0x00009857 File Offset: 0x00007A57
		public TwoDimensionContext Context { get; private set; }

		// Token: 0x06000273 RID: 627 RVA: 0x00009860 File Offset: 0x00007A60
		protected TwoDimensionContextObject(TwoDimensionContext context)
		{
			this.Context = context;
		}
	}
}
