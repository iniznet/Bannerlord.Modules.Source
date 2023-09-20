using System;

namespace psai.net
{
	// Token: 0x0200001A RID: 26
	public class ThemeInfo
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x00008CD0 File Offset: 0x00006ED0
		public override string ToString()
		{
			return string.Concat(new object[] { this.id, ": ", this.name, " [", this.type, "]" });
		}

		// Token: 0x0400010B RID: 267
		public int id;

		// Token: 0x0400010C RID: 268
		public ThemeType type;

		// Token: 0x0400010D RID: 269
		public int[] segmentIds;

		// Token: 0x0400010E RID: 270
		public string name;
	}
}
