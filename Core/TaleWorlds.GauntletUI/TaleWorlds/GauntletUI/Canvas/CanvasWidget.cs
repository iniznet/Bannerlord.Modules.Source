using System;
using System.Numerics;
using System.Xml;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Canvas
{
	// Token: 0x02000051 RID: 81
	public class CanvasWidget : Widget, ILayout
	{
		// Token: 0x17000185 RID: 389
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x00016B64 File Offset: 0x00014D64
		// (set) Token: 0x0600052D RID: 1325 RVA: 0x00016B74 File Offset: 0x00014D74
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

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x00016BE2 File Offset: 0x00014DE2
		// (set) Token: 0x0600052F RID: 1327 RVA: 0x00016BEA File Offset: 0x00014DEA
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

		// Token: 0x06000530 RID: 1328 RVA: 0x00016C15 File Offset: 0x00014E15
		public CanvasWidget(UIContext context)
			: base(context)
		{
			this._defaultLayout = new DefaultLayout();
			base.LayoutImp = this;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00016C30 File Offset: 0x00014E30
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.DoUpdate();
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00016C3F File Offset: 0x00014E3F
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.DoUpdate();
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00016C4E File Offset: 0x00014E4E
		private void DoUpdate()
		{
			if (this._requiresUpdate || this._canvas == null)
			{
				this.UpdateCanvas();
			}
			this._canvas.Update(base._scaleToUse);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00016C78 File Offset: 0x00014E78
		private void UpdateCanvas()
		{
			this._canvas = new Canvas(base.EventManager.Context.SpriteData, base.EventManager.Context.FontFactory);
			this._canvas.LoadFrom(this.CanvasNode);
			this._requiresUpdate = false;
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00016CC8 File Offset: 0x00014EC8
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			if (this._canvas != null)
			{
				this._canvas.DoRender(base.GlobalPosition, drawContext);
			}
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00016CEC File Offset: 0x00014EEC
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

		// Token: 0x06000537 RID: 1335 RVA: 0x00016DA9 File Offset: 0x00014FA9
		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			if (this._canvas != null)
			{
				this._canvas.DoLayout();
			}
			this._defaultLayout.OnLayout(widget, left, bottom, right, top);
		}

		// Token: 0x04000288 RID: 648
		private ILayout _defaultLayout;

		// Token: 0x04000289 RID: 649
		private bool _requiresUpdate;

		// Token: 0x0400028A RID: 650
		private XmlElement _canvasNode;

		// Token: 0x0400028B RID: 651
		private Canvas _canvas;
	}
}
