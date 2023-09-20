using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets.Graph
{
	// Token: 0x02000015 RID: 21
	public class GraphWidget : Widget
	{
		// Token: 0x0600011B RID: 283 RVA: 0x000066CF File Offset: 0x000048CF
		public GraphWidget(UIContext context)
			: base(context)
		{
			this.RefreshOnNextLateUpdate();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000066E0 File Offset: 0x000048E0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool flag = Mathf.Abs(this._totalSizeCached.X - base.Size.X) > 1E-05f || Mathf.Abs(this._totalSizeCached.Y - base.Size.Y) > 1E-05f;
			this._totalSizeCached = base.Size;
			if (flag)
			{
				this.RefreshOnNextLateUpdate();
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006758 File Offset: 0x00004958
		private void Refresh()
		{
			if (this._dynamicWidgetsContainer != null)
			{
				base.RemoveChild(this._dynamicWidgetsContainer);
			}
			this._dynamicWidgetsContainer = new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.StretchToParent,
				HeightSizePolicy = SizePolicy.StretchToParent
			};
			base.AddChildAtIndex(this._dynamicWidgetsContainer, 0);
			this._planeExtendedSize = base.Size * base._inverseScaleToUse - new Vec2(this.LeftSpace + this.RightSpace, this.TopSpace + this.BottomSpace);
			this._planeSize = this._planeExtendedSize - new Vec2(this.PlaneMarginRight, this.PlaneMarginTop);
			Widget widget = new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.StretchToParent,
				HeightSizePolicy = SizePolicy.StretchToParent,
				MarginLeft = this.LeftSpace,
				MarginRight = this.RightSpace,
				MarginBottom = this.BottomSpace,
				MarginTop = this.TopSpace,
				DoNotAcceptEvents = true,
				DoNotPassEventsToChildren = true
			};
			this._dynamicWidgetsContainer.AddChild(widget);
			this.RefreshPlaneLines(widget);
			this.RefreshLabels(this._dynamicWidgetsContainer, true);
			this.RefreshLabels(this._dynamicWidgetsContainer, false);
			this.RefreshGraphLines();
			this._willRefreshThisFrame = false;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000689C File Offset: 0x00004A9C
		private void RefreshPlaneLines(Widget planeWidget)
		{
			int num = 1;
			ListPanel listPanel = this.CreatePlaneLinesListPanel(LayoutMethod.VerticalBottomToTop);
			float num2 = this._planeSize.Y / (float)this.RowCount - (float)num;
			for (int i = 0; i < this.RowCount; i++)
			{
				Widget widget = new Widget(base.Context)
				{
					WidthSizePolicy = SizePolicy.StretchToParent,
					HeightSizePolicy = SizePolicy.Fixed,
					SuggestedHeight = (float)num,
					MarginBottom = num2,
					Sprite = this.PlaneLineSprite,
					Color = this.PlaneLineColor
				};
				listPanel.AddChild(widget);
			}
			ListPanel listPanel2 = this.CreatePlaneLinesListPanel(LayoutMethod.HorizontalLeftToRight);
			float num3 = this._planeSize.X / (float)this.ColumnCount - (float)num;
			for (int j = 0; j < this.ColumnCount; j++)
			{
				Widget widget2 = new Widget(base.Context)
				{
					WidthSizePolicy = SizePolicy.Fixed,
					HeightSizePolicy = SizePolicy.StretchToParent,
					SuggestedWidth = (float)num,
					MarginLeft = num3,
					Sprite = this.PlaneLineSprite,
					Color = this.PlaneLineColor
				};
				listPanel2.AddChild(widget2);
			}
			planeWidget.AddChild(listPanel);
			planeWidget.AddChild(listPanel2);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000069B8 File Offset: 0x00004BB8
		private void RefreshLabels(Widget container, bool isHorizontal)
		{
			int num = (isHorizontal ? this.HorizontalLabelCount : this.VerticalLabelCount);
			float num2 = (isHorizontal ? this.HorizontalMaxValue : this.VerticalMaxValue);
			float num3 = (isHorizontal ? this.HorizontalMinValue : this.VerticalMinValue);
			if (num > 1)
			{
				int num4 = (isHorizontal ? 2 : 4);
				ListPanel listPanel = new ListPanel(base.Context)
				{
					WidthSizePolicy = (isHorizontal ? SizePolicy.StretchToParent : SizePolicy.Fixed),
					HeightSizePolicy = (isHorizontal ? SizePolicy.Fixed : SizePolicy.StretchToParent),
					SuggestedWidth = (isHorizontal ? 0f : (this.LeftSpace - (float)num4)),
					SuggestedHeight = (isHorizontal ? (this.BottomSpace - (float)num4) : 0f),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Bottom,
					MarginLeft = (isHorizontal ? this.LeftSpace : 0f),
					MarginBottom = (isHorizontal ? 0f : this.BottomSpace),
					DoNotAcceptEvents = true,
					DoNotPassEventsToChildren = true
				};
				listPanel.StackLayout.LayoutMethod = (isHorizontal ? LayoutMethod.HorizontalLeftToRight : LayoutMethod.VerticalTopToBottom);
				float num5 = (num2 - num3) / (float)(num - 1);
				for (int i = 0; i < num - 1; i++)
				{
					float num6 = num3 + num5 * (float)i;
					TextWidget textWidget = this.CreateLabelText(num6, isHorizontal);
					listPanel.AddChild(textWidget);
				}
				Widget widget = new Widget(base.Context)
				{
					WidthSizePolicy = (isHorizontal ? SizePolicy.Fixed : SizePolicy.StretchToParent),
					HeightSizePolicy = (isHorizontal ? SizePolicy.StretchToParent : SizePolicy.Fixed),
					SuggestedWidth = (isHorizontal ? (this.RightSpace + this.PlaneMarginRight) : 0f),
					SuggestedHeight = (isHorizontal ? 0f : (this.TopSpace + this.PlaneMarginTop))
				};
				TextWidget textWidget2 = this.CreateLabelText(num2, isHorizontal);
				widget.AddChild(textWidget2);
				listPanel.AddChild(widget);
				container.AddChild(listPanel);
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00006B80 File Offset: 0x00004D80
		private void RefreshGraphLines()
		{
			if (this.LineContainerWidget != null)
			{
				using (List<Widget>.Enumerator enumerator = this.LineContainerWidget.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GraphLineWidget graphLineWidget;
						if ((graphLineWidget = enumerator.Current as GraphLineWidget) != null)
						{
							this.RefreshLine(graphLineWidget);
						}
					}
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00006BE8 File Offset: 0x00004DE8
		private void RefreshLine(GraphLineWidget graphLineWidget)
		{
			graphLineWidget.MarginLeft = this.LeftSpace;
			graphLineWidget.MarginRight = this.RightSpace + this.PlaneMarginRight;
			graphLineWidget.MarginBottom = this.BottomSpace;
			graphLineWidget.MarginTop = this.TopSpace + this.PlaneMarginTop;
			Widget pointContainerWidget = graphLineWidget.PointContainerWidget;
			using (List<Widget>.Enumerator enumerator = (((pointContainerWidget != null) ? pointContainerWidget.Children : null) ?? new List<Widget>()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GraphLinePointWidget graphLinePointWidget;
					if ((graphLinePointWidget = enumerator.Current as GraphLinePointWidget) != null)
					{
						this.RefreshPoint(graphLinePointWidget, graphLineWidget);
					}
				}
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00006C98 File Offset: 0x00004E98
		private void RefreshPoint(GraphLinePointWidget graphLinePointWidget, GraphLineWidget graphLineWidget)
		{
			bool flag = this.HorizontalMaxValue - this.HorizontalMinValue > 1E-05f;
			bool flag2 = this.VerticalMaxValue - this.VerticalMinValue > 1E-05f;
			if (flag && flag2)
			{
				float num = (graphLinePointWidget.HorizontalValue - this.HorizontalMinValue) / (this.HorizontalMaxValue - this.HorizontalMinValue);
				num = MathF.Clamp(num, 0f, 1f);
				float num2 = this._planeSize.X * num - graphLinePointWidget.SuggestedWidth * 0.5f;
				float num3 = (graphLinePointWidget.VerticalValue - this.VerticalMinValue) / (this.VerticalMaxValue - this.VerticalMinValue);
				num3 = MathF.Clamp(num3, 0f, 1f);
				float num4 = this._planeSize.Y * num3 - graphLinePointWidget.SuggestedHeight * 0.5f;
				string text = (string.IsNullOrEmpty(graphLineWidget.LineBrushStateName) ? "Default" : graphLineWidget.LineBrushStateName);
				graphLinePointWidget.MarginLeft = num2;
				graphLinePointWidget.MarginBottom = num4;
				graphLinePointWidget.SetState(text);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00006D9B File Offset: 0x00004F9B
		private ListPanel CreatePlaneLinesListPanel(LayoutMethod layoutMethod)
		{
			return new ListPanel(base.Context)
			{
				WidthSizePolicy = SizePolicy.StretchToParent,
				HeightSizePolicy = SizePolicy.StretchToParent,
				MarginTop = this.PlaneMarginTop,
				MarginRight = this.PlaneMarginRight,
				StackLayout = 
				{
					LayoutMethod = layoutMethod
				}
			};
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00006DDC File Offset: 0x00004FDC
		private TextWidget CreateLabelText(float labelValue, bool isHorizontal)
		{
			TextWidget textWidget = new TextWidget(base.Context)
			{
				WidthSizePolicy = SizePolicy.StretchToParent,
				HeightSizePolicy = SizePolicy.StretchToParent,
				Text = labelValue.ToString("G" + this.NumberOfValueLabelDecimalPlaces.ToString())
			};
			Brush brush = (isHorizontal ? this.HorizontalValueLabelsBrush : this.VerticalValueLabelsBrush);
			if (brush != null)
			{
				textWidget.Brush = brush.Clone();
			}
			textWidget.Brush.TextHorizontalAlignment = (isHorizontal ? TextHorizontalAlignment.Left : TextHorizontalAlignment.Right);
			textWidget.Brush.TextVerticalAlignment = (isHorizontal ? TextVerticalAlignment.Top : TextVerticalAlignment.Bottom);
			return textWidget;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00006E70 File Offset: 0x00005070
		private void OnLineContainerEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			GraphLineWidget graphLineWidget;
			if (eventArgs.Length != 0 && (graphLineWidget = eventArgs[0] as GraphLineWidget) != null)
			{
				if (eventName == "ItemAdd")
				{
					GraphLineWidget graphLineWidget3 = graphLineWidget;
					graphLineWidget3.OnPointAdded = (Action<GraphLineWidget, GraphLinePointWidget>)Delegate.Combine(graphLineWidget3.OnPointAdded, new Action<GraphLineWidget, GraphLinePointWidget>(this.OnPointAdded));
					this.AddLateUpdateAction(delegate
					{
						this.RefreshLine(graphLineWidget);
					});
					return;
				}
				if (eventName == "ItemRemove")
				{
					GraphLineWidget graphLineWidget2 = graphLineWidget;
					graphLineWidget2.OnPointAdded = (Action<GraphLineWidget, GraphLinePointWidget>)Delegate.Remove(graphLineWidget2.OnPointAdded, new Action<GraphLineWidget, GraphLinePointWidget>(this.OnPointAdded));
				}
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00006F20 File Offset: 0x00005120
		private void OnPointAdded(GraphLineWidget graphLineWidget, GraphLinePointWidget graphLinePointWidget)
		{
			this.AddLateUpdateAction(delegate
			{
				this.RefreshPoint(graphLinePointWidget, graphLineWidget);
			});
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00006F5C File Offset: 0x0000515C
		private void AddLateUpdateAction(Action action)
		{
			base.EventManager.AddLateUpdateAction(this, delegate(float _)
			{
				Action action2 = action;
				if (action2 == null)
				{
					return;
				}
				action2();
			}, 1);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00006F8F File Offset: 0x0000518F
		private void RefreshOnNextLateUpdate()
		{
			if (!this._willRefreshThisFrame)
			{
				this._willRefreshThisFrame = true;
				this.AddLateUpdateAction(new Action(this.Refresh));
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00006FB2 File Offset: 0x000051B2
		// (set) Token: 0x0600012A RID: 298 RVA: 0x00006FBA File Offset: 0x000051BA
		public int RowCount
		{
			get
			{
				return this._rowCount;
			}
			set
			{
				if (value != this._rowCount)
				{
					this._rowCount = value;
					base.OnPropertyChanged(value, "RowCount");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00006FDE File Offset: 0x000051DE
		// (set) Token: 0x0600012C RID: 300 RVA: 0x00006FE6 File Offset: 0x000051E6
		public int ColumnCount
		{
			get
			{
				return this._columnCount;
			}
			set
			{
				if (value != this._columnCount)
				{
					this._columnCount = value;
					base.OnPropertyChanged(value, "ColumnCount");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000700A File Offset: 0x0000520A
		// (set) Token: 0x0600012E RID: 302 RVA: 0x00007012 File Offset: 0x00005212
		public int HorizontalLabelCount
		{
			get
			{
				return this._horizontalLabelCount;
			}
			set
			{
				if (value != this._horizontalLabelCount)
				{
					this._horizontalLabelCount = value;
					base.OnPropertyChanged(value, "HorizontalLabelCount");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00007036 File Offset: 0x00005236
		// (set) Token: 0x06000130 RID: 304 RVA: 0x0000703E File Offset: 0x0000523E
		public float HorizontalMinValue
		{
			get
			{
				return this._horizontalMinValue;
			}
			set
			{
				if (value != this._horizontalMinValue)
				{
					this._horizontalMinValue = value;
					base.OnPropertyChanged(value, "HorizontalMinValue");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00007062 File Offset: 0x00005262
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000706A File Offset: 0x0000526A
		public float HorizontalMaxValue
		{
			get
			{
				return this._horizontalMaxValue;
			}
			set
			{
				if (value != this._horizontalMaxValue)
				{
					this._horizontalMaxValue = value;
					base.OnPropertyChanged(value, "HorizontalMaxValue");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000133 RID: 307 RVA: 0x0000708E File Offset: 0x0000528E
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00007096 File Offset: 0x00005296
		public int VerticalLabelCount
		{
			get
			{
				return this._verticalLabelCount;
			}
			set
			{
				if (value != this._verticalLabelCount)
				{
					this._verticalLabelCount = value;
					base.OnPropertyChanged(value, "VerticalLabelCount");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000135 RID: 309 RVA: 0x000070BA File Offset: 0x000052BA
		// (set) Token: 0x06000136 RID: 310 RVA: 0x000070C2 File Offset: 0x000052C2
		public float VerticalMinValue
		{
			get
			{
				return this._verticalMinValue;
			}
			set
			{
				if (value != this._verticalMinValue)
				{
					this._verticalMinValue = value;
					base.OnPropertyChanged(value, "VerticalMinValue");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000137 RID: 311 RVA: 0x000070E6 File Offset: 0x000052E6
		// (set) Token: 0x06000138 RID: 312 RVA: 0x000070EE File Offset: 0x000052EE
		public float VerticalMaxValue
		{
			get
			{
				return this._verticalMaxValue;
			}
			set
			{
				if (value != this._verticalMaxValue)
				{
					this._verticalMaxValue = value;
					base.OnPropertyChanged(value, "VerticalMaxValue");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00007112 File Offset: 0x00005312
		// (set) Token: 0x0600013A RID: 314 RVA: 0x0000711A File Offset: 0x0000531A
		public Sprite PlaneLineSprite
		{
			get
			{
				return this._planeLineSprite;
			}
			set
			{
				if (value != this._planeLineSprite)
				{
					this._planeLineSprite = value;
					base.OnPropertyChanged<Sprite>(value, "PlaneLineSprite");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600013B RID: 315 RVA: 0x0000713E File Offset: 0x0000533E
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00007146 File Offset: 0x00005346
		public Color PlaneLineColor
		{
			get
			{
				return this._planeLineColor;
			}
			set
			{
				if (value != this._planeLineColor)
				{
					this._planeLineColor = value;
					base.OnPropertyChanged(value, "PlaneLineColor");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600013D RID: 317 RVA: 0x0000716F File Offset: 0x0000536F
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00007177 File Offset: 0x00005377
		public float LeftSpace
		{
			get
			{
				return this._leftSpace;
			}
			set
			{
				if (value != this._leftSpace)
				{
					this._leftSpace = value;
					base.OnPropertyChanged(value, "LeftSpace");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000719B File Offset: 0x0000539B
		// (set) Token: 0x06000140 RID: 320 RVA: 0x000071A3 File Offset: 0x000053A3
		public float TopSpace
		{
			get
			{
				return this._topSpace;
			}
			set
			{
				if (value != this._topSpace)
				{
					this._topSpace = value;
					base.OnPropertyChanged(value, "TopSpace");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000071C7 File Offset: 0x000053C7
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000071CF File Offset: 0x000053CF
		public float RightSpace
		{
			get
			{
				return this._rightSpace;
			}
			set
			{
				if (value != this._rightSpace)
				{
					this._rightSpace = value;
					base.OnPropertyChanged(value, "RightSpace");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000071F3 File Offset: 0x000053F3
		// (set) Token: 0x06000144 RID: 324 RVA: 0x000071FB File Offset: 0x000053FB
		public float BottomSpace
		{
			get
			{
				return this._bottomSpace;
			}
			set
			{
				if (value != this._bottomSpace)
				{
					this._bottomSpace = value;
					base.OnPropertyChanged(value, "BottomSpace");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000721F File Offset: 0x0000541F
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00007227 File Offset: 0x00005427
		public float PlaneMarginTop
		{
			get
			{
				return this._planeMarginTop;
			}
			set
			{
				if (value != this._planeMarginTop)
				{
					this._planeMarginTop = value;
					base.OnPropertyChanged(value, "PlaneMarginTop");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000724B File Offset: 0x0000544B
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00007253 File Offset: 0x00005453
		public float PlaneMarginRight
		{
			get
			{
				return this._planeMarginRight;
			}
			set
			{
				if (value != this._planeMarginRight)
				{
					this._planeMarginRight = value;
					base.OnPropertyChanged(value, "PlaneMarginRight");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00007277 File Offset: 0x00005477
		// (set) Token: 0x0600014A RID: 330 RVA: 0x0000727F File Offset: 0x0000547F
		public int NumberOfValueLabelDecimalPlaces
		{
			get
			{
				return this._numberOfValueLabelDecimalPlaces;
			}
			set
			{
				if (value != this._numberOfValueLabelDecimalPlaces)
				{
					this._numberOfValueLabelDecimalPlaces = value;
					base.OnPropertyChanged(value, "NumberOfValueLabelDecimalPlaces");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600014B RID: 331 RVA: 0x000072A3 File Offset: 0x000054A3
		// (set) Token: 0x0600014C RID: 332 RVA: 0x000072AB File Offset: 0x000054AB
		public Brush HorizontalValueLabelsBrush
		{
			get
			{
				return this._horizontalValueLabelsBrush;
			}
			set
			{
				if (value != this._horizontalValueLabelsBrush)
				{
					this._horizontalValueLabelsBrush = value;
					base.OnPropertyChanged<Brush>(value, "HorizontalValueLabelsBrush");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600014D RID: 333 RVA: 0x000072CF File Offset: 0x000054CF
		// (set) Token: 0x0600014E RID: 334 RVA: 0x000072D7 File Offset: 0x000054D7
		public Brush VerticalValueLabelsBrush
		{
			get
			{
				return this._verticalValueLabelsBrush;
			}
			set
			{
				if (value != this._verticalValueLabelsBrush)
				{
					this._verticalValueLabelsBrush = value;
					base.OnPropertyChanged<Brush>(value, "VerticalValueLabelsBrush");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600014F RID: 335 RVA: 0x000072FB File Offset: 0x000054FB
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00007303 File Offset: 0x00005503
		public Brush LineBrush
		{
			get
			{
				return this._lineBrush;
			}
			set
			{
				if (value != this._lineBrush)
				{
					this._lineBrush = value;
					base.OnPropertyChanged<Brush>(value, "LineBrush");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00007327 File Offset: 0x00005527
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00007330 File Offset: 0x00005530
		public Widget LineContainerWidget
		{
			get
			{
				return this._lineContainerWidget;
			}
			set
			{
				if (value != this._lineContainerWidget)
				{
					if (this._lineContainerWidget != null)
					{
						this._lineContainerWidget.EventFire -= this.OnLineContainerEventFire;
						using (List<Widget>.Enumerator enumerator = this.LineContainerWidget.Children.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								GraphLineWidget graphLineWidget;
								if ((graphLineWidget = enumerator.Current as GraphLineWidget) != null)
								{
									GraphLineWidget graphLineWidget2 = graphLineWidget;
									graphLineWidget2.OnPointAdded = (Action<GraphLineWidget, GraphLinePointWidget>)Delegate.Remove(graphLineWidget2.OnPointAdded, new Action<GraphLineWidget, GraphLinePointWidget>(this.OnPointAdded));
								}
							}
						}
					}
					this._lineContainerWidget = value;
					if (this._lineContainerWidget != null)
					{
						this._lineContainerWidget.EventFire += this.OnLineContainerEventFire;
						using (List<Widget>.Enumerator enumerator = this.LineContainerWidget.Children.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								GraphLineWidget graphLineWidget3;
								if ((graphLineWidget3 = enumerator.Current as GraphLineWidget) != null)
								{
									GraphLineWidget graphLineWidget4 = graphLineWidget3;
									graphLineWidget4.OnPointAdded = (Action<GraphLineWidget, GraphLinePointWidget>)Delegate.Combine(graphLineWidget4.OnPointAdded, new Action<GraphLineWidget, GraphLinePointWidget>(this.OnPointAdded));
								}
							}
						}
					}
					base.OnPropertyChanged<Widget>(value, "LineContainerWidget");
					this.RefreshOnNextLateUpdate();
				}
			}
		}

		// Token: 0x04000083 RID: 131
		private Widget _dynamicWidgetsContainer;

		// Token: 0x04000084 RID: 132
		private bool _willRefreshThisFrame;

		// Token: 0x04000085 RID: 133
		private Vec2 _planeExtendedSize;

		// Token: 0x04000086 RID: 134
		private Vec2 _planeSize;

		// Token: 0x04000087 RID: 135
		private Vec2 _totalSizeCached;

		// Token: 0x04000088 RID: 136
		private Widget _lineContainerWidget;

		// Token: 0x04000089 RID: 137
		private int _rowCount;

		// Token: 0x0400008A RID: 138
		private int _columnCount;

		// Token: 0x0400008B RID: 139
		private int _horizontalLabelCount;

		// Token: 0x0400008C RID: 140
		private float _horizontalMinValue;

		// Token: 0x0400008D RID: 141
		private float _horizontalMaxValue;

		// Token: 0x0400008E RID: 142
		private int _verticalLabelCount;

		// Token: 0x0400008F RID: 143
		private float _verticalMinValue;

		// Token: 0x04000090 RID: 144
		private float _verticalMaxValue;

		// Token: 0x04000091 RID: 145
		private Sprite _planeLineSprite;

		// Token: 0x04000092 RID: 146
		private Color _planeLineColor;

		// Token: 0x04000093 RID: 147
		private float _leftSpace;

		// Token: 0x04000094 RID: 148
		private float _topSpace;

		// Token: 0x04000095 RID: 149
		private float _rightSpace;

		// Token: 0x04000096 RID: 150
		private float _bottomSpace;

		// Token: 0x04000097 RID: 151
		private float _planeMarginTop;

		// Token: 0x04000098 RID: 152
		private float _planeMarginRight;

		// Token: 0x04000099 RID: 153
		private int _numberOfValueLabelDecimalPlaces;

		// Token: 0x0400009A RID: 154
		private Brush _horizontalValueLabelsBrush;

		// Token: 0x0400009B RID: 155
		private Brush _verticalValueLabelsBrush;

		// Token: 0x0400009C RID: 156
		private Brush _lineBrush;
	}
}
