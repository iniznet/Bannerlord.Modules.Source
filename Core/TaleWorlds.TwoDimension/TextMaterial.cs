using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class TextMaterial : Material
	{
		public Texture Texture { get; set; }

		public Color Color { get; set; }

		public float SmoothingConstant { get; set; }

		public bool Smooth { get; set; }

		public float ScaleFactor { get; set; }

		public Color GlowColor { get; set; }

		public Color OutlineColor { get; set; }

		public float OutlineAmount { get; set; }

		public float GlowRadius { get; set; }

		public float Blur { get; set; }

		public float ShadowOffset { get; set; }

		public float ShadowAngle { get; set; }

		public float ColorFactor { get; set; }

		public float AlphaFactor { get; set; }

		public float HueFactor { get; set; }

		public float SaturationFactor { get; set; }

		public float ValueFactor { get; set; }

		public TextMaterial()
			: this(null, 0)
		{
		}

		public TextMaterial(Texture texture)
			: this(texture, 0)
		{
		}

		public TextMaterial(Texture texture, int renderOrder)
			: this(texture, renderOrder, true)
		{
		}

		public TextMaterial(Texture texture, int renderOrder, bool blending)
			: base(blending, renderOrder)
		{
			this.Texture = texture;
			this.ScaleFactor = 1f;
			this.SmoothingConstant = 0.47f;
			this.Smooth = true;
			this.Color = new Color(1f, 1f, 1f, 1f);
			this.GlowColor = new Color(0f, 0f, 0f, 1f);
			this.OutlineColor = new Color(0f, 0f, 0f, 1f);
			this.OutlineAmount = 0f;
			this.GlowRadius = 0f;
			this.Blur = 0f;
			this.ShadowOffset = 0f;
			this.ShadowAngle = 0f;
			this.ColorFactor = 1f;
			this.AlphaFactor = 1f;
			this.HueFactor = 0f;
			this.SaturationFactor = 0f;
			this.ValueFactor = 0f;
		}

		public void CopyFrom(TextMaterial sourceMaterial)
		{
			this.Texture = sourceMaterial.Texture;
			this.Color = sourceMaterial.Color;
			this.ScaleFactor = sourceMaterial.ScaleFactor;
			this.SmoothingConstant = sourceMaterial.SmoothingConstant;
			this.Smooth = sourceMaterial.Smooth;
			this.GlowColor = sourceMaterial.GlowColor;
			this.OutlineColor = sourceMaterial.OutlineColor;
			this.OutlineAmount = sourceMaterial.OutlineAmount;
			this.GlowRadius = sourceMaterial.GlowRadius;
			this.Blur = sourceMaterial.Blur;
			this.ShadowOffset = sourceMaterial.ShadowOffset;
			this.ShadowAngle = sourceMaterial.ShadowAngle;
			this.ColorFactor = sourceMaterial.ColorFactor;
			this.AlphaFactor = sourceMaterial.AlphaFactor;
			this.HueFactor = sourceMaterial.HueFactor;
			this.SaturationFactor = sourceMaterial.SaturationFactor;
			this.ValueFactor = sourceMaterial.ValueFactor;
		}
	}
}
