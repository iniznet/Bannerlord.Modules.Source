using System;
using System.Globalization;
using System.Numerics;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasLineText : CanvasLineElement
	{
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

		public float FontSize { get; set; }

		public Color FontColor { get; set; }

		public CanvasLineText(CanvasLine line, int segmentIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(line, segmentIndex, fontFactory, spriteData)
		{
			this._text = new Text(400, 400, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			this.FontColor = Color.White;
		}

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

		public override void Update(float scale)
		{
			base.Update(scale);
			this._text.FontSize = this.FontSize * scale;
		}

		protected override Vector2 Measure()
		{
			return this._text.GetPreferredSize(false, 0f, false, 0f, null, 1f);
		}

		protected override void Render(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			TextMaterial textMaterial = new TextMaterial();
			textMaterial.Color = this.FontColor;
			Vector2 vector = globalPosition + base.LocalPosition;
			drawContext.Draw(this._text, textMaterial, vector.X, vector.Y, base.Width, base.Height);
		}

		private readonly Text _text;
	}
}
