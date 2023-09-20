using System;
using System.Globalization;
using System.Numerics;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public abstract class CanvasElement : CanvasObject
	{
		public float PositionX { get; set; }

		public float PositionY { get; set; }

		public float RelativePositionX
		{
			get
			{
				return this._relativePositionX;
			}
			set
			{
				this._relativePositionX = value;
				this._usingRelativeX = true;
			}
		}

		public float RelativePositionY
		{
			get
			{
				return this._relativePositionY;
			}
			set
			{
				this._relativePositionY = value;
				this._usingRelativeY = true;
			}
		}

		public float PivotX { get; set; }

		public float PivotY { get; set; }

		protected CanvasElement(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
			: base(parent, fontFactory, spriteData)
		{
		}

		public virtual void LoadFrom(XmlNode canvasImageNode)
		{
			foreach (object obj in canvasImageNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				string value = xmlAttribute.Value;
				if (name == "PositionX")
				{
					this.PositionX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PositionY")
				{
					this.PositionY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "RelativePositionX")
				{
					this.RelativePositionX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "RelativePositionY")
				{
					this.RelativePositionY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PivotX")
				{
					this.PivotX = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
				else if (name == "PivotY")
				{
					this.PivotY = Convert.ToSingle(value, CultureInfo.InvariantCulture);
				}
			}
		}

		protected sealed override Vector2 Layout()
		{
			Vector2 vector = new Vector2(this.PositionX, this.PositionY);
			if (this._usingRelativeX)
			{
				vector.X = base.Parent.Width * this.RelativePositionX;
			}
			else
			{
				vector.X *= base.Scale;
			}
			if (this._usingRelativeY)
			{
				vector.Y = base.Parent.Height * this.RelativePositionY;
			}
			else
			{
				vector.Y *= base.Scale;
			}
			vector.X -= this.PivotX * base.Width;
			vector.Y -= this.PivotY * base.Height;
			return vector;
		}

		public override Vector2 GetMarginSize()
		{
			return new Vector2(this.PositionX * base.Scale, this.PositionY * base.Scale);
		}

		public bool _usingRelativeX;

		public bool _usingRelativeY;

		private float _relativePositionX;

		private float _relativePositionY;
	}
}
