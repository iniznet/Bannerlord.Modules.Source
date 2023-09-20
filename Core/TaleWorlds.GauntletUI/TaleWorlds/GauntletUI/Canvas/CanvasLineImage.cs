using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasLineImage : CanvasLineElement
	{
		public Sprite Sprite { get; set; }

		public CanvasLineImage(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, segmentIndex, fontFactory, spriteData)
		{
			this._fontFactory = fontFactory;
			this._spriteData = spriteData;
		}

		public void LoadFrom(XmlNode imageNode)
		{
			foreach (object obj in imageNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "Sprite")
				{
					this.Sprite = this._spriteData.GetSprite(value);
				}
			}
		}

		protected override Vector2 Measure()
		{
			Vector2 zero = Vector2.Zero;
			if (this.Sprite != null)
			{
				zero.X = (float)this.Sprite.Width * base.Scale;
				zero.Y = (float)this.Sprite.Height * base.Scale;
			}
			return zero;
		}

		protected override void Render(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			if (this.Sprite != null)
			{
				Texture texture = this.Sprite.Texture;
				if (texture != null)
				{
					SimpleMaterial simpleMaterial = new SimpleMaterial();
					simpleMaterial.Texture = texture;
					Vector2 vector = globalPosition + base.LocalPosition;
					drawContext.DrawSprite(this.Sprite, simpleMaterial, vector.X, vector.Y, base.Scale, base.Width, base.Height, false, false);
				}
			}
		}

		private FontFactory _fontFactory;

		private SpriteData _spriteData;
	}
}
