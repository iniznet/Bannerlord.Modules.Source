using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x02000047 RID: 71
	public class Canvas
	{
		// Token: 0x060004D2 RID: 1234 RVA: 0x00015AFA File Offset: 0x00013CFA
		public Canvas(SpriteData spriteData, FontFactory fontFactory)
		{
			this._spriteData = spriteData;
			this._fontFactory = fontFactory;
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x00015B10 File Offset: 0x00013D10
		public CanvasObject Root
		{
			get
			{
				return this._root;
			}
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00015B18 File Offset: 0x00013D18
		public void LoadFrom(XmlNode canvasNode)
		{
			this._root = null;
			if (canvasNode != null)
			{
				this._root = new CanvasObject(null, this._fontFactory, this._spriteData);
				foreach (object obj in canvasNode)
				{
					XmlNode xmlNode = (XmlNode)obj;
					CanvasElement canvasElement = null;
					if (xmlNode.Name == "Image")
					{
						CanvasImage canvasImage = new CanvasImage(this._root, this._fontFactory, this._spriteData);
						canvasImage.LoadFrom(xmlNode);
						canvasElement = canvasImage;
					}
					else if (xmlNode.Name == "TextBox")
					{
						CanvasTextBox canvasTextBox = new CanvasTextBox(this._root, this._fontFactory, this._spriteData);
						canvasTextBox.LoadFrom(xmlNode);
						canvasElement = canvasTextBox;
					}
					if (canvasElement != null)
					{
						this._root.Children.Add(canvasElement);
					}
				}
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x00015C0C File Offset: 0x00013E0C
		public void Update(float scale)
		{
			this._root.Update(scale);
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00015C1A File Offset: 0x00013E1A
		public void DoMeasure(bool fixedWidth, bool fixedHeight, float width, float height)
		{
			this._root.BeginMeasure(fixedWidth, fixedHeight, width, height);
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00015C2C File Offset: 0x00013E2C
		public void DoLayout()
		{
			this._root.DoLayout();
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00015C39 File Offset: 0x00013E39
		public void DoRender(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			this._root.DoRender(globalPosition, drawContext);
		}

		// Token: 0x04000263 RID: 611
		private SpriteData _spriteData;

		// Token: 0x04000264 RID: 612
		private FontFactory _fontFactory;

		// Token: 0x04000265 RID: 613
		private CanvasObject _root;
	}
}
