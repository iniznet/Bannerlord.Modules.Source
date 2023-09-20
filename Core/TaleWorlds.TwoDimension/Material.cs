using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000025 RID: 37
	public abstract class Material
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00007026 File Offset: 0x00005226
		// (set) Token: 0x06000153 RID: 339 RVA: 0x0000702E File Offset: 0x0000522E
		public bool Blending { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00007037 File Offset: 0x00005237
		// (set) Token: 0x06000155 RID: 341 RVA: 0x0000703F File Offset: 0x0000523F
		public int RenderOrder { get; private set; }

		// Token: 0x06000156 RID: 342 RVA: 0x00007048 File Offset: 0x00005248
		protected Material(bool blending, int renderOrder)
		{
			this.Blending = blending;
			this.RenderOrder = renderOrder;
		}
	}
}
