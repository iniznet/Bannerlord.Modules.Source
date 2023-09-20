using System;

namespace TaleWorlds.TwoDimension
{
	public abstract class Sprite
	{
		public abstract Texture Texture { get; }

		public string Name { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		protected Sprite(string name, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
			this.CachedDrawObject = null;
		}

		public abstract float GetScaleToUse(float width, float height, float scale);

		protected internal abstract DrawObject2D GetArrays(SpriteDrawData spriteDrawData);

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		protected DrawObject2D CachedDrawObject;

		protected SpriteDrawData CachedDrawData;
	}
}
