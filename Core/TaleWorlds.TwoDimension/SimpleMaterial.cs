using System;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000027 RID: 39
	public class SimpleMaterial : Material
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00007095 File Offset: 0x00005295
		// (set) Token: 0x0600015D RID: 349 RVA: 0x0000709D File Offset: 0x0000529D
		public Texture Texture { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600015E RID: 350 RVA: 0x000070A6 File Offset: 0x000052A6
		// (set) Token: 0x0600015F RID: 351 RVA: 0x000070AE File Offset: 0x000052AE
		public Color Color { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000160 RID: 352 RVA: 0x000070B7 File Offset: 0x000052B7
		// (set) Token: 0x06000161 RID: 353 RVA: 0x000070BF File Offset: 0x000052BF
		public float ColorFactor { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000162 RID: 354 RVA: 0x000070C8 File Offset: 0x000052C8
		// (set) Token: 0x06000163 RID: 355 RVA: 0x000070D0 File Offset: 0x000052D0
		public float AlphaFactor { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000164 RID: 356 RVA: 0x000070D9 File Offset: 0x000052D9
		// (set) Token: 0x06000165 RID: 357 RVA: 0x000070E1 File Offset: 0x000052E1
		public float HueFactor { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000070EA File Offset: 0x000052EA
		// (set) Token: 0x06000167 RID: 359 RVA: 0x000070F2 File Offset: 0x000052F2
		public float SaturationFactor { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000168 RID: 360 RVA: 0x000070FB File Offset: 0x000052FB
		// (set) Token: 0x06000169 RID: 361 RVA: 0x00007103 File Offset: 0x00005303
		public float ValueFactor { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600016A RID: 362 RVA: 0x0000710C File Offset: 0x0000530C
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00007114 File Offset: 0x00005314
		public bool CircularMaskingEnabled { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600016C RID: 364 RVA: 0x0000711D File Offset: 0x0000531D
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00007125 File Offset: 0x00005325
		public Vector2 CircularMaskingCenter { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600016E RID: 366 RVA: 0x0000712E File Offset: 0x0000532E
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00007136 File Offset: 0x00005336
		public float CircularMaskingRadius { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000713F File Offset: 0x0000533F
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00007147 File Offset: 0x00005347
		public float CircularMaskingSmoothingRadius { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000172 RID: 370 RVA: 0x00007150 File Offset: 0x00005350
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00007158 File Offset: 0x00005358
		public bool OverlayEnabled { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000174 RID: 372 RVA: 0x00007161 File Offset: 0x00005361
		// (set) Token: 0x06000175 RID: 373 RVA: 0x00007169 File Offset: 0x00005369
		public Vector2 StartCoordinate { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00007172 File Offset: 0x00005372
		// (set) Token: 0x06000177 RID: 375 RVA: 0x0000717A File Offset: 0x0000537A
		public Vector2 Size { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00007183 File Offset: 0x00005383
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000718B File Offset: 0x0000538B
		public Texture OverlayTexture { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00007194 File Offset: 0x00005394
		// (set) Token: 0x0600017B RID: 379 RVA: 0x0000719C File Offset: 0x0000539C
		public bool UseOverlayAlphaAsMask { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600017C RID: 380 RVA: 0x000071A5 File Offset: 0x000053A5
		// (set) Token: 0x0600017D RID: 381 RVA: 0x000071AD File Offset: 0x000053AD
		public float Scale { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600017E RID: 382 RVA: 0x000071B6 File Offset: 0x000053B6
		// (set) Token: 0x0600017F RID: 383 RVA: 0x000071BE File Offset: 0x000053BE
		public float OverlayTextureWidth { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000180 RID: 384 RVA: 0x000071C7 File Offset: 0x000053C7
		// (set) Token: 0x06000181 RID: 385 RVA: 0x000071CF File Offset: 0x000053CF
		public float OverlayTextureHeight { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000182 RID: 386 RVA: 0x000071D8 File Offset: 0x000053D8
		// (set) Token: 0x06000183 RID: 387 RVA: 0x000071E0 File Offset: 0x000053E0
		public float OverlayXOffset { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000184 RID: 388 RVA: 0x000071E9 File Offset: 0x000053E9
		// (set) Token: 0x06000185 RID: 389 RVA: 0x000071F1 File Offset: 0x000053F1
		public float OverlayYOffset { get; set; }

		// Token: 0x06000186 RID: 390 RVA: 0x000071FA File Offset: 0x000053FA
		public SimpleMaterial()
			: this(null, 0)
		{
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00007204 File Offset: 0x00005404
		public SimpleMaterial(Texture texture)
			: this(texture, 0)
		{
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000720E File Offset: 0x0000540E
		public SimpleMaterial(Texture texture, int renderOrder)
			: this(texture, renderOrder, true)
		{
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00007219 File Offset: 0x00005419
		public SimpleMaterial(Texture texture, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Reset(texture);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000722C File Offset: 0x0000542C
		public void Reset(Texture texture = null)
		{
			this.Texture = texture;
			this.ColorFactor = 1f;
			this.AlphaFactor = 1f;
			this.HueFactor = 0f;
			this.SaturationFactor = 0f;
			this.ValueFactor = 0f;
			this.Color = new Color(1f, 1f, 1f, 1f);
			this.CircularMaskingEnabled = false;
			this.OverlayEnabled = false;
			this.OverlayTextureWidth = 512f;
			this.OverlayTextureHeight = 512f;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000072BA File Offset: 0x000054BA
		public Vec2 GetCircularMaskingCenter()
		{
			return this.CircularMaskingCenter;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000072C7 File Offset: 0x000054C7
		public Vec2 GetOverlayStartCoordinate()
		{
			return this.StartCoordinate;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000072D4 File Offset: 0x000054D4
		public Vec2 GetOverlaySize()
		{
			return this.Size;
		}
	}
}
