using System;

namespace psai.net
{
	// Token: 0x02000024 RID: 36
	public class Weighting
	{
		// Token: 0x06000234 RID: 564 RVA: 0x000097D8 File Offset: 0x000079D8
		internal Weighting()
		{
			this.intensityVsVariety = 0.5f;
			this.lowPlaycountVsRandom = 0.9f;
			this.switchGroups = 0.5f;
		}

		// Token: 0x04000144 RID: 324
		public float switchGroups;

		// Token: 0x04000145 RID: 325
		public float intensityVsVariety;

		// Token: 0x04000146 RID: 326
		public float lowPlaycountVsRandom;
	}
}
