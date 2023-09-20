using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x0200004F RID: 79
	public class CanvasObject
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x000166E5 File Offset: 0x000148E5
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x000166ED File Offset: 0x000148ED
		public CanvasObject Parent { get; private set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x000166F6 File Offset: 0x000148F6
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x000166FE File Offset: 0x000148FE
		public List<CanvasObject> Children { get; private set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00016707 File Offset: 0x00014907
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x0001670F File Offset: 0x0001490F
		public float Scale { get; private set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00016718 File Offset: 0x00014918
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00016720 File Offset: 0x00014920
		public Vector2 LocalPosition { get; private set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x00016729 File Offset: 0x00014929
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x00016731 File Offset: 0x00014931
		public float Width { get; private set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0001673A File Offset: 0x0001493A
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x00016742 File Offset: 0x00014942
		public float Height { get; private set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x0001674B File Offset: 0x0001494B
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x00016753 File Offset: 0x00014953
		public FontFactory FontFactory { get; private set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x0001675C File Offset: 0x0001495C
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x00016764 File Offset: 0x00014964
		public SpriteData SpriteData { get; private set; }

		// Token: 0x0600051D RID: 1309 RVA: 0x0001676D File Offset: 0x0001496D
		public CanvasObject(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
		{
			this.Children = new List<CanvasObject>(32);
			this.Parent = parent;
			this.FontFactory = fontFactory;
			this.SpriteData = spriteData;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x00016798 File Offset: 0x00014998
		public virtual void Update(float scale)
		{
			this.Scale = scale;
			this.OnUpdate(scale);
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.Update(scale);
			}
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x000167F8 File Offset: 0x000149F8
		protected virtual void OnUpdate(float scale)
		{
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000167FA File Offset: 0x000149FA
		public void BeginMeasure(bool fixedWidth, bool fixedHeight, float width, float height)
		{
			this.DoMeasure();
			if (fixedWidth)
			{
				this.Width = width;
			}
			if (fixedHeight)
			{
				this.Height = height;
			}
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00016818 File Offset: 0x00014A18
		public void DoMeasure()
		{
			Vector2 zero = Vector2.Zero;
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoMeasure();
				Vector2 vector = new Vector2(canvasObject.Width, canvasObject.Height);
				Vector2 marginSize = canvasObject.GetMarginSize();
				Vector2 vector2 = vector + marginSize;
				zero.X = Mathf.Max(zero.X, vector2.X);
				zero.Y = Mathf.Max(zero.Y, vector2.Y);
			}
			Vector2 vector3 = this.Measure();
			this.Width = Mathf.Max(zero.X, vector3.X);
			this.Height = Mathf.Max(zero.Y, vector3.Y);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000168FC File Offset: 0x00014AFC
		public void DoLayout()
		{
			Vector2 vector = this.Layout();
			this.LocalPosition = vector;
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoLayout();
			}
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001695C File Offset: 0x00014B5C
		protected virtual Vector2 Measure()
		{
			return Vector2.Zero;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00016963 File Offset: 0x00014B63
		public virtual Vector2 GetMarginSize()
		{
			return Vector2.Zero;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001696A File Offset: 0x00014B6A
		protected virtual Vector2 Layout()
		{
			return Vector2.Zero;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00016974 File Offset: 0x00014B74
		public void DoRender(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			this.Render(globalPosition, drawContext);
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoRender(globalPosition + this.LocalPosition, drawContext);
			}
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x000169DC File Offset: 0x00014BDC
		protected virtual void Render(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
		}
	}
}
