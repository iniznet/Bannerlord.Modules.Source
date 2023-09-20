using System;

namespace TaleWorlds.TwoDimension
{
	internal class TextTokenOutput
	{
		public float X { get; private set; }

		public float Y { get; private set; }

		public float Width { get; private set; }

		public float Height { get; private set; }

		public float Scale { get; private set; }

		public Rectangle Rectangle { get; private set; }

		public TextToken Token { get; private set; }

		public string Style { get; private set; }

		public TextTokenOutput(TextToken token, float width, float height, string style, float scaleValue)
		{
			this.Token = token;
			this.Width = width;
			this.Height = height;
			this.Rectangle = new Rectangle(0f, 0f, this.Width, this.Height);
			this.Style = style;
			this.Scale = scaleValue;
		}

		public void SetPosition(float x, float y)
		{
			this.X = x;
			this.Y = y;
			this.Rectangle = new Rectangle(x, y, this.Width, this.Height);
		}
	}
}
