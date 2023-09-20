using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasLine : CanvasObject
	{
		public CanvasLineAlignment Alignment { get; set; }

		public CanvasLine(CanvasTextBox textBox, int lineIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(textBox, fontFactory, spriteData)
		{
			this._elements = new List<CanvasLineElement>();
			this._lineIndex = lineIndex;
			this._textBox = textBox;
		}

		protected override Vector2 Measure()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (CanvasLineElement canvasLineElement in this._elements)
			{
				num += canvasLineElement.Width;
				num2 = Mathf.Max(num2, canvasLineElement.Height);
			}
			return new Vector2(num, num2);
		}

		protected override Vector2 Layout()
		{
			Vector2 zero = Vector2.Zero;
			if (this.Alignment == CanvasLineAlignment.Left)
			{
				zero.X = 0f;
			}
			else if (this.Alignment == CanvasLineAlignment.Center)
			{
				zero.X = (base.Parent.Width - base.Width) * 0.5f;
			}
			else if (this.Alignment == CanvasLineAlignment.Right)
			{
				zero.X = base.Parent.Width - base.Width;
			}
			zero.Y = this._textBox.GetVerticalPositionOf(this._lineIndex);
			return zero;
		}

		public void LoadFrom(XmlNode lineNode)
		{
			foreach (object obj in lineNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "Alignment")
				{
					this.Alignment = (CanvasLineAlignment)Enum.Parse(typeof(CanvasLineAlignment), value);
				}
			}
			int num = 0;
			foreach (object obj2 in lineNode)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				CanvasLineElement canvasLineElement = null;
				if (xmlNode.Name == "LineImage")
				{
					CanvasLineImage canvasLineImage = new CanvasLineImage(this, num, base.FontFactory, base.SpriteData);
					canvasLineImage.LoadFrom(xmlNode);
					canvasLineElement = canvasLineImage;
				}
				else if (xmlNode.Name == "Text")
				{
					CanvasLineText canvasLineText = new CanvasLineText(this, num, base.FontFactory, base.SpriteData);
					canvasLineText.LoadFrom(xmlNode);
					canvasLineElement = canvasLineText;
				}
				if (canvasLineElement != null)
				{
					this._elements.Add(canvasLineElement);
					base.Children.Add(canvasLineElement);
					num++;
				}
			}
		}

		public float GetHorizontalPositionOf(int index)
		{
			float num = 0f;
			int num2 = 0;
			foreach (CanvasLineElement canvasLineElement in this._elements)
			{
				if (num2 == index)
				{
					break;
				}
				num += canvasLineElement.Width;
				num2++;
			}
			return num;
		}

		private List<CanvasLineElement> _elements;

		private CanvasTextBox _textBox;

		private int _lineIndex;
	}
}
