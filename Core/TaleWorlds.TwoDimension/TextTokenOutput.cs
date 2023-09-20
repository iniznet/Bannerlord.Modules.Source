using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000019 RID: 25
	internal class TextTokenOutput
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00006357 File Offset: 0x00004557
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000635F File Offset: 0x0000455F
		public float X { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00006368 File Offset: 0x00004568
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00006370 File Offset: 0x00004570
		public float Y { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00006379 File Offset: 0x00004579
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00006381 File Offset: 0x00004581
		public float Width { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000638A File Offset: 0x0000458A
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00006392 File Offset: 0x00004592
		public float Height { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x0000639B File Offset: 0x0000459B
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x000063A3 File Offset: 0x000045A3
		public float Scale { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000063AC File Offset: 0x000045AC
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000063B4 File Offset: 0x000045B4
		public Rectangle Rectangle { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000063BD File Offset: 0x000045BD
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x000063C5 File Offset: 0x000045C5
		public TextToken Token { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x000063CE File Offset: 0x000045CE
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x000063D6 File Offset: 0x000045D6
		public string Style { get; private set; }

		// Token: 0x060000F8 RID: 248 RVA: 0x000063E0 File Offset: 0x000045E0
		public TextTokenOutput(TextToken token, float width, float height, string style, float scaleValue)
		{
			this.Token = token;
			this.Width = width;
			this.Height = height;
			this.Rectangle = new Rectangle(0f, 0f, this.Width, this.Height);
			this.Style = style;
			this.Scale = scaleValue;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00006439 File Offset: 0x00004639
		public void SetPosition(float x, float y)
		{
			this.X = x;
			this.Y = y;
			this.Rectangle = new Rectangle(x, y, this.Width, this.Height);
		}
	}
}
