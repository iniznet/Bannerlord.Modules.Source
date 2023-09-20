using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasObject
	{
		public CanvasObject Parent { get; private set; }

		public List<CanvasObject> Children { get; private set; }

		public float Scale { get; private set; }

		public Vector2 LocalPosition { get; private set; }

		public float Width { get; private set; }

		public float Height { get; private set; }

		public FontFactory FontFactory { get; private set; }

		public SpriteData SpriteData { get; private set; }

		public CanvasObject(CanvasObject parent, FontFactory fontFactory, SpriteData spriteData)
		{
			this.Children = new List<CanvasObject>(32);
			this.Parent = parent;
			this.FontFactory = fontFactory;
			this.SpriteData = spriteData;
		}

		public virtual void Update(float scale)
		{
			this.Scale = scale;
			this.OnUpdate(scale);
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.Update(scale);
			}
		}

		protected virtual void OnUpdate(float scale)
		{
		}

		public void BeginMeasure(bool fixedWidth, bool fixedHeight, float width, float height)
		{
			this.DoMeasure();
			if (fixedWidth)
			{
				this.Width = width;
			}
			if (fixedHeight)
			{
				this.Height = height;
			}
		}

		public void DoMeasure()
		{
			Vector2 zero = Vector2.Zero;
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoMeasure();
				Vector2 vector = new Vector2(canvasObject.Width, canvasObject.Height);
				Vector2 marginSize = canvasObject.GetMarginSize();
				Vector2 vector2 = vector + marginSize;
				zero.X = Mathf.Max(zero.X, vector2.X);
				zero.Y = Mathf.Max(zero.Y, vector2.Y);
			}
			Vector2 vector3 = this.Measure();
			this.Width = Mathf.Max(zero.X, vector3.X);
			this.Height = Mathf.Max(zero.Y, vector3.Y);
		}

		public void DoLayout()
		{
			Vector2 vector = this.Layout();
			this.LocalPosition = vector;
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoLayout();
			}
		}

		protected virtual Vector2 Measure()
		{
			return Vector2.Zero;
		}

		public virtual Vector2 GetMarginSize()
		{
			return Vector2.Zero;
		}

		protected virtual Vector2 Layout()
		{
			return Vector2.Zero;
		}

		public void DoRender(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
			this.Render(globalPosition, drawContext);
			foreach (CanvasObject canvasObject in this.Children)
			{
				canvasObject.DoRender(globalPosition + this.LocalPosition, drawContext);
			}
		}

		protected virtual void Render(Vector2 globalPosition, TwoDimensionDrawContext drawContext)
		{
		}
	}
}
