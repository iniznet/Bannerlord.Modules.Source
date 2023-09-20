using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasTextBox : CanvasElement
	{
		public CanvasTextBox(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
			: base(parent, fontFactory, spriteData)
		{
			this._lines = new List<CanvasLine>();
		}

		public override void LoadFrom(XmlNode canvasTextNode)
		{
			base.LoadFrom(canvasTextNode);
			int num = 0;
			foreach (object obj in canvasTextNode)
			{
				XmlNode xmlNode = (XmlNode)obj;
				CanvasLine canvasLine = new CanvasLine(this, num, base.FontFactory, base.SpriteData);
				canvasLine.LoadFrom(xmlNode);
				this._lines.Add(canvasLine);
				base.Children.Add(canvasLine);
				num++;
			}
		}

		protected override Vector2 Measure()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (CanvasLine canvasLine in this._lines)
			{
				num = Mathf.Max(num, canvasLine.Width);
				num2 += canvasLine.Height;
			}
			return new Vector2(num, num2);
		}

		public float GetVerticalPositionOf(int index)
		{
			float num = 0f;
			int num2 = 0;
			foreach (CanvasLine canvasLine in this._lines)
			{
				if (num2 == index)
				{
					break;
				}
				num += canvasLine.Height;
				num2++;
			}
			return num;
		}

		private List<CanvasLine> _lines;
	}
}
