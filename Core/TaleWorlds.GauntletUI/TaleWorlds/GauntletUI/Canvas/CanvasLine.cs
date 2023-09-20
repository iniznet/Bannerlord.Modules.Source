using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x0200004A RID: 74
	public class CanvasLine : CanvasObject
	{
		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060004EF RID: 1263 RVA: 0x00016027 File Offset: 0x00014227
		// (set) Token: 0x060004F0 RID: 1264 RVA: 0x0001602F File Offset: 0x0001422F
		public CanvasLineAlignment Alignment { get; set; }

		// Token: 0x060004F1 RID: 1265 RVA: 0x00016038 File Offset: 0x00014238
		public CanvasLine(CanvasTextBox textBox, int lineIndex, FontFactory fontFactory, SpriteData spriteData)
			: base(textBox, fontFactory, spriteData)
		{
			this._elements = new List<CanvasLineElement>();
			this._lineIndex = lineIndex;
			this._textBox = textBox;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00016060 File Offset: 0x00014260
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

		// Token: 0x060004F3 RID: 1267 RVA: 0x000160D8 File Offset: 0x000142D8
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

		// Token: 0x060004F4 RID: 1268 RVA: 0x00016168 File Offset: 0x00014368
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

		// Token: 0x060004F5 RID: 1269 RVA: 0x000162C0 File Offset: 0x000144C0
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

		// Token: 0x0400026F RID: 623
		private List<CanvasLineElement> _elements;

		// Token: 0x04000270 RID: 624
		private CanvasTextBox _textBox;

		// Token: 0x04000271 RID: 625
		private int _lineIndex;
	}
}
