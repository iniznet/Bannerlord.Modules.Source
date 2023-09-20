using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets.Graph
{
	public class GraphWidget : Widget
	{
		public GraphWidget(UIContext context)
			: base(context)
		{
			this.RefreshOnNextLateUpdate();
		}

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

		private void OnPointAdded(GraphLineWidget graphLineWidget, GraphLinePointWidget graphLinePointWidget)
		{
			this.AddLateUpdateAction(delegate
			{
				this.RefreshPoint(graphLinePointWidget, graphLineWidget);
			});
		}

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

		private void RefreshOnNextLateUpdate()
		{
			if (!this._willRefreshThisFrame)
			{
				this._willRefreshThisFrame = true;
				this.AddLateUpdateAction(new Action(this.Refresh));
			}
		}

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

		private Widget _dynamicWidgetsContainer;

		private bool _willRefreshThisFrame;

		private Vec2 _planeExtendedSize;

		private Vec2 _planeSize;

		private Vec2 _totalSizeCached;

		private Widget _lineContainerWidget;

		private int _rowCount;

		private int _columnCount;

		private int _horizontalLabelCount;

		private float _horizontalMinValue;

		private float _horizontalMaxValue;

		private int _verticalLabelCount;

		private float _verticalMinValue;

		private float _verticalMaxValue;

		private Sprite _planeLineSprite;

		private Color _planeLineColor;

		private float _leftSpace;

		private float _topSpace;

		private float _rightSpace;

		private float _bottomSpace;

		private float _planeMarginTop;

		private float _planeMarginRight;

		private int _numberOfValueLabelDecimalPlaces;

		private Brush _horizontalValueLabelsBrush;

		private Brush _verticalValueLabelsBrush;

		private Brush _lineBrush;
	}
}
