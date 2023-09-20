using System;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	public class MarkerRect
	{
		public float Left { get; private set; }

		public float Right { get; private set; }

		public float Top { get; private set; }

		public float Bottom { get; private set; }

		public float CenterX
		{
			get
			{
				return this.Left + (this.Right - this.Left) / 2f;
			}
		}

		public float CenterY
		{
			get
			{
				return this.Top + (this.Bottom - this.Top) / 2f;
			}
		}

		public float Width
		{
			get
			{
				return this.Right - this.Left;
			}
		}

		public float Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
		}

		public MarkerRect()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.Left = 0f;
			this.Right = 0f;
			this.Top = 0f;
			this.Bottom = 0f;
		}

		public void UpdatePoints(float left, float right, float top, float bottom)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
		}

		public bool IsOverlapping(MarkerRect other)
		{
			return other.Left <= this.Right && other.Right >= this.Left && other.Top <= this.Bottom && other.Bottom >= this.Top;
		}
	}
}
