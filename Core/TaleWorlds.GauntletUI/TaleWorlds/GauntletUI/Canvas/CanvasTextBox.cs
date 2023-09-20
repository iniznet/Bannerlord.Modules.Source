using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x02000050 RID: 80
	public class CanvasTextBox : CanvasElement
	{
		// Token: 0x06000528 RID: 1320 RVA: 0x000169DE File Offset: 0x00014BDE
		public CanvasTextBox(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
			: base(parent, fontFactory, spriteData)
		{
			this._lines = new List<CanvasLine>();
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x000169F4 File Offset: 0x00014BF4
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

		// Token: 0x0600052A RID: 1322 RVA: 0x00016A84 File Offset: 0x00014C84
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

		// Token: 0x0600052B RID: 1323 RVA: 0x00016AFC File Offset: 0x00014CFC
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

		// Token: 0x04000287 RID: 647
		private List<CanvasLine> _lines;
	}
}
