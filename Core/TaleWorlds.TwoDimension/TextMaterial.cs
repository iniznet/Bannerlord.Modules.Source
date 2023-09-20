using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000028 RID: 40
	public class TextMaterial : Material
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600018E RID: 398 RVA: 0x000072E1 File Offset: 0x000054E1
		// (set) Token: 0x0600018F RID: 399 RVA: 0x000072E9 File Offset: 0x000054E9
		public Texture Texture { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000190 RID: 400 RVA: 0x000072F2 File Offset: 0x000054F2
		// (set) Token: 0x06000191 RID: 401 RVA: 0x000072FA File Offset: 0x000054FA
		public Color Color { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00007303 File Offset: 0x00005503
		// (set) Token: 0x06000193 RID: 403 RVA: 0x0000730B File Offset: 0x0000550B
		public float SmoothingConstant { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00007314 File Offset: 0x00005514
		// (set) Token: 0x06000195 RID: 405 RVA: 0x0000731C File Offset: 0x0000551C
		public bool Smooth { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000196 RID: 406 RVA: 0x00007325 File Offset: 0x00005525
		// (set) Token: 0x06000197 RID: 407 RVA: 0x0000732D File Offset: 0x0000552D
		public float ScaleFactor { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00007336 File Offset: 0x00005536
		// (set) Token: 0x06000199 RID: 409 RVA: 0x0000733E File Offset: 0x0000553E
		public Color GlowColor { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00007347 File Offset: 0x00005547
		// (set) Token: 0x0600019B RID: 411 RVA: 0x0000734F File Offset: 0x0000554F
		public Color OutlineColor { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600019C RID: 412 RVA: 0x00007358 File Offset: 0x00005558
		// (set) Token: 0x0600019D RID: 413 RVA: 0x00007360 File Offset: 0x00005560
		public float OutlineAmount { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00007369 File Offset: 0x00005569
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00007371 File Offset: 0x00005571
		public float GlowRadius { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000737A File Offset: 0x0000557A
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00007382 File Offset: 0x00005582
		public float Blur { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000738B File Offset: 0x0000558B
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00007393 File Offset: 0x00005593
		public float ShadowOffset { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000739C File Offset: 0x0000559C
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x000073A4 File Offset: 0x000055A4
		public float ShadowAngle { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x000073AD File Offset: 0x000055AD
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x000073B5 File Offset: 0x000055B5
		public float ColorFactor { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x000073BE File Offset: 0x000055BE
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x000073C6 File Offset: 0x000055C6
		public float AlphaFactor { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001AA RID: 426 RVA: 0x000073CF File Offset: 0x000055CF
		// (set) Token: 0x060001AB RID: 427 RVA: 0x000073D7 File Offset: 0x000055D7
		public float HueFactor { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001AC RID: 428 RVA: 0x000073E0 File Offset: 0x000055E0
		// (set) Token: 0x060001AD RID: 429 RVA: 0x000073E8 File Offset: 0x000055E8
		public float SaturationFactor { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001AE RID: 430 RVA: 0x000073F1 File Offset: 0x000055F1
		// (set) Token: 0x060001AF RID: 431 RVA: 0x000073F9 File Offset: 0x000055F9
		public float ValueFactor { get; set; }

		// Token: 0x060001B0 RID: 432 RVA: 0x00007402 File Offset: 0x00005602
		public TextMaterial()
			: this(null, 0)
		{
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000740C File Offset: 0x0000560C
		public TextMaterial(Texture texture)
			: this(texture, 0)
		{
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00007416 File Offset: 0x00005616
		public TextMaterial(Texture texture, int renderOrder)
			: this(texture, renderOrder, true)
		{
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00007424 File Offset: 0x00005624
		public TextMaterial(Texture texture, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Texture = texture;
			this.ScaleFactor = 1f;
			this.SmoothingConstant = 0.47f;
			this.Smooth = true;
			this.Color = new Color(1f, 1f, 1f, 1f);
			this.GlowColor = new Color(0f, 0f, 0f, 1f);
			this.OutlineColor = new Color(0f, 0f, 0f, 1f);
			this.OutlineAmount = 0f;
			this.GlowRadius = 0f;
			this.Blur = 0f;
			this.ShadowOffset = 0f;
			this.ShadowAngle = 0f;
			this.ColorFactor = 1f;
			this.AlphaFactor = 1f;
			this.HueFactor = 0f;
			this.SaturationFactor = 0f;
			this.ValueFactor = 0f;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00007528 File Offset: 0x00005728
		public void CopyFrom(TextMaterial sourceMaterial)
		{
			this.Texture = sourceMaterial.Texture;
			this.Color = sourceMaterial.Color;
			this.ScaleFactor = sourceMaterial.ScaleFactor;
			this.SmoothingConstant = sourceMaterial.SmoothingConstant;
			this.Smooth = sourceMaterial.Smooth;
			this.GlowColor = sourceMaterial.GlowColor;
			this.OutlineColor = sourceMaterial.OutlineColor;
			this.OutlineAmount = sourceMaterial.OutlineAmount;
			this.GlowRadius = sourceMaterial.GlowRadius;
			this.Blur = sourceMaterial.Blur;
			this.ShadowOffset = sourceMaterial.ShadowOffset;
			this.ShadowAngle = sourceMaterial.ShadowAngle;
			this.ColorFactor = sourceMaterial.ColorFactor;
			this.AlphaFactor = sourceMaterial.AlphaFactor;
			this.HueFactor = sourceMaterial.HueFactor;
			this.SaturationFactor = sourceMaterial.SaturationFactor;
			this.ValueFactor = sourceMaterial.ValueFactor;
		}
	}
}
