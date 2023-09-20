using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class Canvas
	{
		public Canvas(SpriteData spriteData, FontFactory fontFactory)
		{
			this._spriteData = spriteData;
			this._fontFactory = fontFactory;
		}

		public CanvasObject Root
		{
			get
			{
				return this._root;
			}
		}

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

		public void Update(float scale)
		{
			this._root.Update(scale);
		}

		public void DoMeasure(bool fixedWidth, bool fixedHeight, float width, float height)
		{
			this._root.BeginMeasure(fixedWidth, fixedHeight, width, height);
		}

		public void DoLayout()
		{
			this._root.DoLayout();
		}

		public void DoRender(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			this._root.DoRender(globalPosition, drawContext);
		}

		private SpriteData _spriteData;

		private FontFactory _fontFactory;

		private CanvasObject _root;
	}
}
