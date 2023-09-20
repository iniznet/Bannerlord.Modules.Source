using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x0200004D RID: 77
	public class CanvasLineImage : CanvasLineElement
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x000163A9 File Offset: 0x000145A9
		// (set) Token: 0x060004FD RID: 1277 RVA: 0x000163B1 File Offset: 0x000145B1
		public Sprite Sprite { get; set; }

		// Token: 0x060004FE RID: 1278 RVA: 0x000163BA File Offset: 0x000145BA
		public CanvasLineImage(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, segmentIndex, fontFactory, spriteData)
		{
			this._fontFactory = fontFactory;
			this._spriteData = spriteData;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x000163D8 File Offset: 0x000145D8
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

		// Token: 0x06000500 RID: 1280 RVA: 0x00016458 File Offset: 0x00014658
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

		// Token: 0x06000501 RID: 1281 RVA: 0x000164A8 File Offset: 0x000146A8
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

		// Token: 0x04000279 RID: 633
		private FontFactory _fontFactory;

		// Token: 0x0400027A RID: 634
		private SpriteData _spriteData;
	}
}
