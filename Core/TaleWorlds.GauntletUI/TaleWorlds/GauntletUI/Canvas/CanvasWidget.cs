using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	public class CanvasWidget : Widget, ILayout
	{
		[Editor(false)]
		public string CanvasAsString
		{
			get
			{
				return this._canvasNode.ToString();
			}
			set
			{
				if ((this._canvasNode == null && value != null) || this._canvasNode.ToString() != value)
				{
					if (!string.IsNullOrEmpty(value))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(value);
						this._canvasNode = xmlDocument.DocumentElement;
					}
					else
					{
						this._canvasNode = null;
					}
					this._requiresUpdate = true;
					base.OnPropertyChanged<string>(value, "CanvasAsString");
					base.SetMeasureAndLayoutDirty();
				}
			}
		}

		public XmlElement CanvasNode
		{
			get
			{
				return this._canvasNode;
			}
			set
			{
				if (this._canvasNode != value)
				{
					this._canvasNode = value;
					this._requiresUpdate = true;
					base.OnPropertyChanged<XmlElement>(value, "CanvasNode");
					base.SetMeasureAndLayoutDirty();
				}
			}
		}

		public CanvasWidget(UIContext context)
			: base(context)
		{
			this._defaultLayout = new DefaultLayout();
			base.LayoutImp = this;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.DoUpdate();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.DoUpdate();
		}

		private void DoUpdate()
		{
			if (this._requiresUpdate || this._canvas == null)
			{
				this.UpdateCanvas();
			}
			this._canvas.Update(base._scaleToUse);
		}

		private void UpdateCanvas()
		{
			this._canvas = new Canvas(base.EventManager.Context.SpriteData, base.EventManager.Context.FontFactory);
			this._canvas.LoadFrom(this.CanvasNode);
			this._requiresUpdate = false;
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			if (this._canvas != null)
			{
				this._canvas.DoRender(base.GlobalPosition, drawContext);
			}
		}

		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Vector2 vector = this._defaultLayout.MeasureChildren(widget, measureSpec, spriteData, renderScale);
			if (this._canvas != null)
			{
				this._canvas.DoMeasure(base.WidthSizePolicy != SizePolicy.CoverChildren || base.MaxWidth != 0f, base.HeightSizePolicy != SizePolicy.CoverChildren || base.MaxHeight != 0f, measureSpec.X, measureSpec.Y);
				vector.X = Mathf.Max(this._canvas.Root.Width, vector.X);
				vector.Y = Mathf.Max(this._canvas.Root.Height, vector.Y);
			}
			return vector;
		}

		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			if (this._canvas != null)
			{
				this._canvas.DoLayout();
			}
			this._defaultLayout.OnLayout(widget, left, bottom, right, top);
		}

		private ILayout _defaultLayout;

		private bool _requiresUpdate;

		private XmlElement _canvasNode;

		private Canvas _canvas;
	}
}
