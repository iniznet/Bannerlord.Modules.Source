using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000026 RID: 38
	public class PrimitivePolygonMaterial : Material
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000157 RID: 343 RVA: 0x0000705E File Offset: 0x0000525E
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00007066 File Offset: 0x00005266
		public Color Color { get; private set; }

		// Token: 0x06000159 RID: 345 RVA: 0x0000706F File Offset: 0x0000526F
		public PrimitivePolygonMaterial(Color color)
			: this(color, 0)
		{
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007079 File Offset: 0x00005279
		public PrimitivePolygonMaterial(Color color, int renderOrder)
			: this(color, renderOrder, true)
		{
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007084 File Offset: 0x00005284
		public PrimitivePolygonMaterial(Color color, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Color = color;
		}
	}
}
