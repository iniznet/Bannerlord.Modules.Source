using System;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public abstract class CanvasLineElement : CanvasObject
	{
		public CanvasLine Line { get; set; }

		public int SegmentIndex { get; set; }

		protected CanvasLineElement(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, fontFactory, spriteData)
		{
			this.Line = line;
			this.SegmentIndex = segmentIndex;
		}

		protected sealed override Vector2 Layout()
		{
			Vector2 zero = Vector2.Zero;
			zero.X = this.Line.GetHorizontalPositionOf(this.SegmentIndex);
			zero.Y = this.Line.Height - base.Height;
			return zero;
		}
	}
}
