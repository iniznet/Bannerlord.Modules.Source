using System;
using System.Globalization;
using System.Numerics;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x0200004E RID: 78
	public class CanvasLineText : CanvasLineElement
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x00016513 File Offset: 0x00014713
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x00016520 File Offset: 0x00014720
		public string Value
		{
			get
			{
				return this._text.Value;
			}
			set
			{
				this._text.CurrentLanguage = base.FontFactory.GetCurrentLanguage();
				this._text.Value = value;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x00016544 File Offset: 0x00014744
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x0001654C File Offset: 0x0001474C
		public float FontSize { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x00016555 File Offset: 0x00014755
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x0001655D File Offset: 0x0001475D
		public Color FontColor { get; set; }

		// Token: 0x06000508 RID: 1288 RVA: 0x00016566 File Offset: 0x00014766
		public CanvasLineText(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, segmentIndex, fontFactory, spriteData)
		{
			this._text = new Text(400, 400, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			this.FontColor = Color.White;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x000165A8 File Offset: 0x000147A8
		public void LoadFrom(XmlNode textNode)
		{
			foreach (object obj in textNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "Value")
				{
					this.Value = value;
				}
				else if (name == "FontSize")
				{
					this.FontSize = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "FontColor")
				{
					this.FontColor = Color.ConvertStringToColor(value);
				}
			}
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00016658 File Offset: 0x00014858
		public override void Update(float scale)
		{
			base.Update(scale);
			this._text.FontSize = this.FontSize * scale;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x00016674 File Offset: 0x00014874
		protected override Vector2 Measure()
		{
			return this._text.GetPreferredSize(false, 0f, false, 0f, null, 1f);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00016694 File Offset: 0x00014894
		protected override void Render(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			TextMaterial textMaterial = new TextMaterial();
			textMaterial.Color = this.FontColor;
			Vector2 vector = globalPosition + base.LocalPosition;
			drawContext.Draw(this._text, textMaterial, vector.X, vector.Y, base.Width, base.Height);
		}

		// Token: 0x0400027C RID: 636
		private readonly Text _text;
	}
}
