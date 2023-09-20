using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200002D RID: 45
	public abstract class Sprite
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001D0 RID: 464
		public abstract Texture Texture { get; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x000078A2 File Offset: 0x00005AA2
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x000078AA File Offset: 0x00005AAA
		public string Name { get; private set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000078B3 File Offset: 0x00005AB3
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x000078BB File Offset: 0x00005ABB
		public int Width { get; private set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x000078C4 File Offset: 0x00005AC4
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x000078CC File Offset: 0x00005ACC
		public int Height { get; private set; }

		// Token: 0x060001D7 RID: 471 RVA: 0x000078D5 File Offset: 0x00005AD5
		protected Sprite(string name, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
			this.CachedDrawObject = null;
		}

		// Token: 0x060001D8 RID: 472
		public abstract float GetScaleToUse(float width, float height, float scale);

		// Token: 0x060001D9 RID: 473
		protected internal abstract DrawObject2D GetArrays(SpriteDrawData spriteDrawData);

		// Token: 0x060001DA RID: 474 RVA: 0x000078F9 File Offset: 0x00005AF9
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		// Token: 0x040000F7 RID: 247
		protected DrawObject2D CachedDrawObject;

		// Token: 0x040000F8 RID: 248
		protected SpriteDrawData CachedDrawData;
	}
}
