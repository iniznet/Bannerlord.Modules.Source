using System;

namespace TaleWorlds.TwoDimension
{
	public readonly struct SpriteDrawData : IEquatable<SpriteDrawData>
	{
		internal SpriteDrawData(float mapX, float mapY, float scale, float width, float height, bool horizontalFlip, bool verticalFlip)
		{
			this.MapX = mapX;
			this.MapY = mapY;
			this.Scale = scale;
			this.Width = width;
			this.Height = height;
			this.HorizontalFlip = horizontalFlip;
			this.VerticalFlip = verticalFlip;
			int num = this.MapX.GetHashCode();
			num = (num * 397) ^ this.MapY.GetHashCode();
			num = (num * 397) ^ this.Scale.GetHashCode();
			num = (num * 397) ^ this.Width.GetHashCode();
			num = (num * 397) ^ this.Height.GetHashCode();
			num = (num * 397) ^ this.HorizontalFlip.GetHashCode();
			num = (num * 397) ^ this.VerticalFlip.GetHashCode();
			this._hashCode = num;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			SpriteDrawData spriteDrawData = (SpriteDrawData)obj;
			return this.MapX == spriteDrawData.MapX && this.MapY == spriteDrawData.MapY && this.Scale == spriteDrawData.Scale && this.Width == spriteDrawData.Width && this.Height == spriteDrawData.Height && this.HorizontalFlip == spriteDrawData.HorizontalFlip && this.VerticalFlip == spriteDrawData.VerticalFlip;
		}

		public bool Equals(SpriteDrawData other)
		{
			return this.MapX == other.MapX && this.MapY == other.MapY && this.Scale == other.Scale && this.Width == other.Width && this.Height == other.Height && this.HorizontalFlip == other.HorizontalFlip && this.VerticalFlip == other.VerticalFlip;
		}

		public static bool operator ==(SpriteDrawData a, SpriteDrawData b)
		{
			return a.MapX == b.MapX && a.MapY == b.MapY && a.Scale == b.Scale && a.Width == b.Width && a.Height == b.Height && a.HorizontalFlip == b.HorizontalFlip && a.VerticalFlip == b.VerticalFlip;
		}

		public static bool operator !=(SpriteDrawData a, SpriteDrawData b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return this._hashCode;
		}

		internal readonly float MapX;

		internal readonly float MapY;

		internal readonly float Scale;

		internal readonly float Width;

		internal readonly float Height;

		internal readonly bool HorizontalFlip;

		internal readonly bool VerticalFlip;

		private readonly int _hashCode;
	}
}
