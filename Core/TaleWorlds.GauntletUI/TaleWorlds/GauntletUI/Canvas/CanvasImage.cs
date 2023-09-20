using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x02000049 RID: 73
	public class CanvasImage : CanvasElement
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00015ECA File Offset: 0x000140CA
		// (set) Token: 0x060004EA RID: 1258 RVA: 0x00015ED2 File Offset: 0x000140D2
		public Sprite Sprite { get; set; }

		// Token: 0x060004EB RID: 1259 RVA: 0x00015EDB File Offset: 0x000140DB
		public CanvasImage(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
			: base(parent, fontFactory, spriteData)
		{
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00015EE8 File Offset: 0x000140E8
		public override void LoadFrom(XmlNode canvasImageNode)
		{
			base.LoadFrom(canvasImageNode);
			foreach (object obj in canvasImageNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "Sprite")
				{
					this.Sprite = base.SpriteData.GetSprite(value);
				}
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x00015F6C File Offset: 0x0001416C
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

		// Token: 0x060004EE RID: 1262 RVA: 0x00015FBC File Offset: 0x000141BC
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
	}
}
