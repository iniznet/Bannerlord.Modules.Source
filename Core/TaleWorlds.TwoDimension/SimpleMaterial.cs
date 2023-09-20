using System;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class SimpleMaterial : Material
	{
		public Texture Texture { get; set; }

		public Color Color { get; set; }

		public float ColorFactor { get; set; }

		public float AlphaFactor { get; set; }

		public float HueFactor { get; set; }

		public float SaturationFactor { get; set; }

		public float ValueFactor { get; set; }

		public bool CircularMaskingEnabled { get; set; }

		public Vector2 CircularMaskingCenter { get; set; }

		public float CircularMaskingRadius { get; set; }

		public float CircularMaskingSmoothingRadius { get; set; }

		public bool OverlayEnabled { get; set; }

		public Vector2 StartCoordinate { get; set; }

		public Vector2 Size { get; set; }

		public Texture OverlayTexture { get; set; }

		public bool UseOverlayAlphaAsMask { get; set; }

		public float Scale { get; set; }

		public float OverlayTextureWidth { get; set; }

		public float OverlayTextureHeight { get; set; }

		public float OverlayXOffset { get; set; }

		public float OverlayYOffset { get; set; }

		public SimpleMaterial()
			: this(null, 0)
		{
		}

		public SimpleMaterial(Texture texture)
			: this(texture, 0)
		{
		}

		public SimpleMaterial(Texture texture, int renderOrder)
			: this(texture, renderOrder, true)
		{
		}

		public SimpleMaterial(Texture texture, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Reset(texture);
		}

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

		public Vec2 GetCircularMaskingCenter()
		{
			return this.CircularMaskingCenter;
		}

		public Vec2 GetOverlayStartCoordinate()
		{
			return this.StartCoordinate;
		}

		public Vec2 GetOverlaySize()
		{
			return this.Size;
		}
	}
}
