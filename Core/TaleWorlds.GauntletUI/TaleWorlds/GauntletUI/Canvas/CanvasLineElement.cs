using System;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x0200004C RID: 76
	public abstract class CanvasLineElement : CanvasObject
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x00016328 File Offset: 0x00014528
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x00016330 File Offset: 0x00014530
		public CanvasLine Line { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00016339 File Offset: 0x00014539
		// (set) Token: 0x060004F9 RID: 1273 RVA: 0x00016341 File Offset: 0x00014541
		public int SegmentIndex { get; set; }

		// Token: 0x060004FA RID: 1274 RVA: 0x0001634A File Offset: 0x0001454A
		protected CanvasLineElement(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, fontFactory, spriteData)
		{
			this.Line = line;
			this.SegmentIndex = segmentIndex;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00016364 File Offset: 0x00014564
		protected sealed override Vector2 Layout()
		{
			Vector2 zero = Vector2.Zero;
			zero.X = this.Line.GetHorizontalPositionOf(this.SegmentIndex);
			zero.Y = this.Line.Height - base.Height;
			return zero;
		}
	}
}
