using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class EventManager
	{
		public float Time { get; private set; }

		public Vec2 UsableArea { get; set; } = new Vec2(1f, 1f);

		public float LeftUsableAreaStart { get; private set; }

		public float TopUsableAreaStart { get; private set; }

		public static EventManager UIEventManager { get; private set; }

		public Vector2 MousePositionInReferenceResolution
		{
			get
			{
				return this.MousePosition * this.Context.CustomInverseScale;
			}
		}

		public bool IsControllerActive { get; private set; }

		public Vector2 PageSize { get; private set; }

		public UIContext Context { get; private set; }

		public event Action OnDragStarted;

		public event Action OnDragEnded;

		public IInputService InputService { get; internal set; }

		public IInputContext InputContext { get; internal set; }

		public Widget Root { get; private set; }

		public Widget FocusedWidget
		{
			get
			{
				return this._focusedWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._focusedWidget = value;
					return;
				}
				this._focusedWidget = null;
			}
		}

		public Widget HoveredView
		{
			get
			{
				return this._hoveredView;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._hoveredView = value;
					return;
				}
				this._hoveredView = null;
			}
		}

		public List<Widget> MouseOveredViews
		{
			get
			{
				return this._mouseOveredViews;
			}
			private set
			{
				if (value != null)
				{
					this._mouseOveredViews = value;
					return;
				}
				this._mouseOveredViews = null;
			}
		}

		public Widget DragHoveredView
		{
			get
			{
				return this._dragHoveredView;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._dragHoveredView = value;
					return;
				}
				this._dragHoveredView = null;
			}
		}

		public Widget DraggedWidget
		{
			get
			{
				return this._draggedWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._draggedWidget = value;
					Action onDragStarted = this.OnDragStarted;
					if (onDragStarted == null)
					{
						return;
					}
					onDragStarted();
					return;
				}
				else
				{
					this._draggedWidget = null;
					Action onDragEnded = this.OnDragEnded;
					if (onDragEnded == null)
					{
						return;
					}
					onDragEnded();
					return;
				}
			}
		}

		public Vector2 DraggedWidgetPosition
		{
			get
			{
				if (this.DraggedWidget != null)
				{
					return this._dragCarrier.GlobalPosition * this.Context.CustomScale - new Vector2(this.LeftUsableAreaStart, this.TopUsableAreaStart);
				}
				return this.MousePositionInReferenceResolution;
			}
		}

		public Widget LatestMouseDownWidget
		{
			get
			{
				return this._latestMouseDownWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._latestMouseDownWidget = value;
					return;
				}
				this._latestMouseDownWidget = null;
			}
		}

		public Widget LatestMouseUpWidget
		{
			get
			{
				return this._latestMouseUpWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._latestMouseUpWidget = value;
					return;
				}
				this._latestMouseUpWidget = null;
			}
		}

		public Widget LatestMouseAlternateDownWidget
		{
			get
			{
				return this._latestMouseAlternateDownWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._latestMouseAlternateDownWidget = value;
					return;
				}
				this._latestMouseAlternateDownWidget = null;
			}
		}

		public Widget LatestMouseAlternateUpWidget
		{
			get
			{
				return this._latestMouseAlternateUpWidget;
			}
			private set
			{
				if (value != null && value.ConnectedToRoot)
				{
					this._latestMouseAlternateUpWidget = value;
					return;
				}
				this._latestMouseAlternateUpWidget = null;
			}
		}

		public Vector2 MousePosition { get; private set; }

		private bool IsDragging
		{
			get
			{
				return this.DraggedWidget != null;
			}
		}

		public float DeltaMouseScroll
		{
			get
			{
				return this.InputContext.GetDeltaMouseScroll() * 0.4f;
			}
		}

		public float RightStickVerticalScrollAmount
		{
			get
			{
				float y = Input.GetKeyState(InputKey.ControllerRStick).Y;
				return 3000f * y * 0.4f * this.CachedDt;
			}
		}

		public float RightStickHorizontalScrollAmount
		{
			get
			{
				float x = Input.GetKeyState(InputKey.ControllerRStick).X;
				return 3000f * x * 0.4f * this.CachedDt;
			}
		}

		internal float CachedDt { get; private set; }

		internal EventManager(UIContext context)
		{
			this.Context = context;
			this.Root = new Widget(context)
			{
				Id = "Root"
			};
			if (EventManager.UIEventManager == null)
			{
				EventManager.UIEventManager = new EventManager();
			}
			this._widgetsWithUpdateContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.Update);
			this._widgetsWithParallelUpdateContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.ParallelUpdate);
			this._widgetsWithLateUpdateContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.LateUpdate);
			this._widgetsWithTweenPositionsContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.TweenPosition);
			this._widgetsWithVisualDefinitionsContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.VisualDefinition);
			this._widgetsWithUpdateBrushesContainer = new WidgetContainer(context, 64, WidgetContainer.ContainerType.UpdateBrushes);
			this._lateUpdateActionLocker = new object();
			this._lateUpdateActions = new Dictionary<int, List<UpdateAction>>();
			this._lateUpdateActionsRunning = new Dictionary<int, List<UpdateAction>>();
			for (int i = 1; i <= 5; i++)
			{
				this._lateUpdateActions.Add(i, new List<UpdateAction>(32));
				this._lateUpdateActionsRunning.Add(i, new List<UpdateAction>(32));
			}
			this._drawContext = new TwoDimensionDrawContext();
			this.MouseOveredViews = new List<Widget>();
			this.ParallelUpdateWidgetPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.ParallelUpdateWidget);
			this.WidgetDoTweenPositionAuxPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.WidgetDoTweenPositionAux);
			this.UpdateBrushesWidgetPredicate = new TWParallel.ParallelForWithDtAuxPredicate(this.UpdateBrushesWidget);
			this.IsControllerActive = Input.IsControllerConnected && !Input.IsMouseActive;
		}

		internal void OnFinalize()
		{
			if (!this._lastSetFrictionValue.ApproximatelyEqualsTo(1f, 1E-05f))
			{
				this._lastSetFrictionValue = 1f;
				Input.SetCursorFriction(this._lastSetFrictionValue);
			}
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnEventManagerFinalized(this);
		}

		internal void OnWidgetConnectedToRoot(Widget widget)
		{
			widget.HandleOnConnectedToRoot();
			foreach (Widget widget2 in widget.AllChildrenAndThis)
			{
				widget2.HandleOnConnectedToRoot();
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.Update, widget2);
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.LateUpdate, widget2);
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, widget2);
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.ParallelUpdate, widget2);
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget2);
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget2);
			}
		}

		internal void OnWidgetDisconnectedFromRoot(Widget widget)
		{
			widget.HandleOnDisconnectedFromRoot();
			if (widget == this.DraggedWidget && this.DraggedWidget.DragWidget != null)
			{
				this.ReleaseDraggedWidget();
				this.ClearDragObject();
			}
			foreach (Widget widget2 in widget.AllChildrenAndThis)
			{
				widget2.HandleOnDisconnectedFromRoot();
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.Update, widget2);
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.LateUpdate, widget2);
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.UpdateBrushes, widget2);
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.ParallelUpdate, widget2);
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget2);
				this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget2);
				GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
				if (instance != null)
				{
					instance.OnWidgetDisconnectedFromRoot(this, widget2);
				}
				widget2.GamepadNavigationIndex = -1;
				widget2.UsedNavigationMovements = GamepadNavigationTypes.None;
				widget2.IsUsingNavigation = false;
			}
		}

		internal void RegisterWidgetForEvent(WidgetContainer.ContainerType type, Widget widget)
		{
			WidgetContainer widgetContainer;
			switch (type)
			{
			case WidgetContainer.ContainerType.None:
				return;
			case WidgetContainer.ContainerType.Update:
				widgetContainer = this._widgetsWithUpdateContainer;
				lock (widgetContainer)
				{
					if (widget.WidgetInfo.GotCustomUpdate && widget.OnUpdateListIndex < 0)
					{
						widget.OnUpdateListIndex = this._widgetsWithUpdateContainer.Add(widget);
					}
					return;
				}
				break;
			case WidgetContainer.ContainerType.ParallelUpdate:
				break;
			case WidgetContainer.ContainerType.LateUpdate:
				goto IL_B3;
			case WidgetContainer.ContainerType.VisualDefinition:
				goto IL_FB;
			case WidgetContainer.ContainerType.TweenPosition:
				goto IL_13E;
			case WidgetContainer.ContainerType.UpdateBrushes:
				goto IL_17E;
			default:
				return;
			}
			widgetContainer = this._widgetsWithParallelUpdateContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotCustomParallelUpdate && widget.OnParallelUpdateListIndex < 0)
				{
					widget.OnParallelUpdateListIndex = this._widgetsWithParallelUpdateContainer.Add(widget);
				}
				return;
			}
			IL_B3:
			widgetContainer = this._widgetsWithLateUpdateContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotCustomLateUpdate && widget.OnLateUpdateListIndex < 0)
				{
					widget.OnLateUpdateListIndex = this._widgetsWithLateUpdateContainer.Add(widget);
				}
				return;
			}
			IL_FB:
			widgetContainer = this._widgetsWithVisualDefinitionsContainer;
			lock (widgetContainer)
			{
				if (widget.VisualDefinition != null && widget.OnVisualDefinitionListIndex < 0)
				{
					widget.OnVisualDefinitionListIndex = this._widgetsWithVisualDefinitionsContainer.Add(widget);
				}
				return;
			}
			IL_13E:
			widgetContainer = this._widgetsWithTweenPositionsContainer;
			lock (widgetContainer)
			{
				if (widget.TweenPosition && widget.OnTweenPositionListIndex < 0)
				{
					widget.OnTweenPositionListIndex = this._widgetsWithTweenPositionsContainer.Add(widget);
				}
				return;
			}
			IL_17E:
			widgetContainer = this._widgetsWithUpdateBrushesContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotUpdateBrushes && widget.OnUpdateBrushesIndex < 0)
				{
					widget.OnUpdateBrushesIndex = this._widgetsWithUpdateBrushesContainer.Add(widget);
				}
			}
		}

		internal void UnRegisterWidgetForEvent(WidgetContainer.ContainerType type, Widget widget)
		{
			WidgetContainer widgetContainer;
			switch (type)
			{
			case WidgetContainer.ContainerType.None:
				return;
			case WidgetContainer.ContainerType.Update:
				widgetContainer = this._widgetsWithUpdateContainer;
				lock (widgetContainer)
				{
					if (widget.WidgetInfo.GotCustomUpdate && widget.OnUpdateListIndex != -1)
					{
						this._widgetsWithUpdateContainer.RemoveFromIndex(widget.OnUpdateListIndex);
						widget.OnUpdateListIndex = -1;
					}
					return;
				}
				break;
			case WidgetContainer.ContainerType.ParallelUpdate:
				break;
			case WidgetContainer.ContainerType.LateUpdate:
				goto IL_BF;
			case WidgetContainer.ContainerType.VisualDefinition:
				goto IL_10D;
			case WidgetContainer.ContainerType.TweenPosition:
				goto IL_156;
			case WidgetContainer.ContainerType.UpdateBrushes:
				goto IL_19C;
			default:
				return;
			}
			widgetContainer = this._widgetsWithParallelUpdateContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotCustomParallelUpdate && widget.OnParallelUpdateListIndex != -1)
				{
					this._widgetsWithParallelUpdateContainer.RemoveFromIndex(widget.OnParallelUpdateListIndex);
					widget.OnParallelUpdateListIndex = -1;
				}
				return;
			}
			IL_BF:
			widgetContainer = this._widgetsWithLateUpdateContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotCustomLateUpdate && widget.OnLateUpdateListIndex != -1)
				{
					this._widgetsWithLateUpdateContainer.RemoveFromIndex(widget.OnLateUpdateListIndex);
					widget.OnLateUpdateListIndex = -1;
				}
				return;
			}
			IL_10D:
			widgetContainer = this._widgetsWithVisualDefinitionsContainer;
			lock (widgetContainer)
			{
				if (widget.VisualDefinition != null && widget.OnVisualDefinitionListIndex != -1)
				{
					this._widgetsWithVisualDefinitionsContainer.RemoveFromIndex(widget.OnVisualDefinitionListIndex);
					widget.OnVisualDefinitionListIndex = -1;
				}
				return;
			}
			IL_156:
			widgetContainer = this._widgetsWithTweenPositionsContainer;
			lock (widgetContainer)
			{
				if (widget.TweenPosition && widget.OnTweenPositionListIndex != -1)
				{
					this._widgetsWithTweenPositionsContainer.RemoveFromIndex(widget.OnTweenPositionListIndex);
					widget.OnTweenPositionListIndex = -1;
				}
				return;
			}
			IL_19C:
			widgetContainer = this._widgetsWithUpdateBrushesContainer;
			lock (widgetContainer)
			{
				if (widget.WidgetInfo.GotUpdateBrushes && widget.OnUpdateBrushesIndex != -1)
				{
					this._widgetsWithUpdateBrushesContainer.RemoveFromIndex(widget.OnUpdateBrushesIndex);
					widget.OnUpdateBrushesIndex = -1;
				}
			}
		}

		internal void OnWidgetVisualDefinitionChanged(Widget widget)
		{
			if (widget.VisualDefinition != null)
			{
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget);
				return;
			}
			this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget);
		}

		internal void OnWidgetTweenPositionChanged(Widget widget)
		{
			if (widget.TweenPosition)
			{
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget);
				return;
			}
			this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget);
		}

		private void MeasureAll()
		{
			this.Root.Measure(this.PageSize);
		}

		private void LayoutAll(float left, float bottom, float right, float top)
		{
			this.Root.Layout(left, bottom, right, top);
		}

		private void UpdatePositions()
		{
			this.Root.UpdatePosition(Vector2.Zero);
		}

		private void WidgetDoTweenPositionAux(int startInclusive, int endExclusive, float deltaTime)
		{
			List<Widget> currentList = this._widgetsWithParallelUpdateContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].DoTweenPosition(deltaTime);
			}
		}

		private void ParallelDoTweenPositions(float dt)
		{
			TWParallel.For(0, this._widgetsWithTweenPositionsContainer.Count, dt, this.WidgetDoTweenPositionAuxPredicate, 16);
		}

		private void TweenPositions(float dt)
		{
			if (this._widgetsWithTweenPositionsContainer.CheckFragmentation())
			{
				WidgetContainer widgetsWithTweenPositionsContainer = this._widgetsWithTweenPositionsContainer;
				lock (widgetsWithTweenPositionsContainer)
				{
					this._widgetsWithTweenPositionsContainer.DoDefragmentation();
				}
			}
			if (this._widgetsWithTweenPositionsContainer.Count > 64)
			{
				this.ParallelDoTweenPositions(dt);
				return;
			}
			List<Widget> currentList = this._widgetsWithTweenPositionsContainer.GetCurrentList();
			for (int i = 0; i < currentList.Count; i++)
			{
				currentList[i].DoTweenPosition(dt);
			}
		}

		internal void CalculateCanvas(Vector2 pageSize, float dt)
		{
			if (this._measureDirty > 0 || this._layoutDirty > 0)
			{
				this.PageSize = pageSize;
				Vec2 vec = new Vec2(pageSize.X / this.UsableArea.X, pageSize.Y / this.UsableArea.Y);
				this.LeftUsableAreaStart = (vec.X - vec.X * this.UsableArea.X) / 2f;
				this.TopUsableAreaStart = (vec.Y - vec.Y * this.UsableArea.Y) / 2f;
				if (this._measureDirty > 0)
				{
					this.MeasureAll();
				}
				this.LayoutAll(this.LeftUsableAreaStart, this.PageSize.Y, this.PageSize.X, this.TopUsableAreaStart);
				this.TweenPositions(dt);
				this.UpdatePositions();
				if (this._measureDirty > 0)
				{
					this._measureDirty--;
				}
				if (this._layoutDirty > 0)
				{
					this._layoutDirty--;
				}
				this._positionsDirty = false;
			}
		}

		internal void RecalculateCanvas()
		{
			if (this._measureDirty == 2 || this._layoutDirty == 2)
			{
				if (this._measureDirty == 2)
				{
					this.MeasureAll();
				}
				this.LayoutAll(this.LeftUsableAreaStart, this.PageSize.Y, this.PageSize.X, this.TopUsableAreaStart);
				if (this._positionsDirty)
				{
					this.UpdatePositions();
					this._positionsDirty = false;
				}
			}
		}

		internal void MouseDown()
		{
			this._mouseIsDown = true;
			this._lastClickPosition = new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY());
			Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.MousePressed, this._lastClickPosition);
			if (widgetAtPositionForEvent != null)
			{
				this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.MousePressed);
			}
		}

		internal void MouseUp()
		{
			this._mouseIsDown = false;
			this.MousePosition = new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY());
			if (this.IsDragging)
			{
				if (this.DraggedWidget.PreviewEvent(GauntletEvent.DragEnd))
				{
					this.DispatchEvent(this.DraggedWidget, GauntletEvent.DragEnd);
				}
				Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.Drop, this.MousePosition);
				if (widgetAtPositionForEvent != null)
				{
					this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.Drop);
				}
				else
				{
					this.CancelAndReturnDrag();
				}
				if (this.DraggedWidget != null)
				{
					this.ClearDragObject();
					return;
				}
			}
			else
			{
				Widget widgetAtPositionForEvent2 = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseReleased, new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY()));
				this.DispatchEvent(widgetAtPositionForEvent2, GauntletEvent.MouseReleased);
				this.LatestMouseUpWidget = widgetAtPositionForEvent2;
			}
		}

		internal void MouseAlternateDown()
		{
			this._mouseAlternateIsDown = true;
			this._lastAlternateClickPosition = new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY());
			Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseAlternatePressed, this._lastAlternateClickPosition);
			if (widgetAtPositionForEvent != null)
			{
				this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.MouseAlternatePressed);
			}
		}

		internal void MouseAlternateUp()
		{
			this._mouseAlternateIsDown = false;
			this.MousePosition = new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY());
			Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseAlternateReleased, this._lastAlternateClickPosition);
			this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.MouseAlternateReleased);
			this.LatestMouseAlternateUpWidget = widgetAtPositionForEvent;
		}

		internal void MouseScroll()
		{
			if (MathF.Abs(this.DeltaMouseScroll) > 0.001f)
			{
				Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseScroll, this.MousePosition);
				if (widgetAtPositionForEvent != null)
				{
					this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.MouseScroll);
				}
			}
		}

		internal void RightStickMovement()
		{
			if (Input.GetKeyState(InputKey.ControllerRStick).X != 0f || Input.GetKeyState(InputKey.ControllerRStick).Y != 0f)
			{
				Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.RightStickMovement, this.MousePosition);
				if (widgetAtPositionForEvent != null)
				{
					this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.RightStickMovement);
				}
			}
		}

		public void ClearFocus()
		{
			this.SetWidgetFocused(null, false);
			this.SetHoveredView(null);
		}

		private void CancelAndReturnDrag()
		{
			if (this._draggedWidgetPreviousParent != null)
			{
				this.DraggedWidget.ParentWidget = this._draggedWidgetPreviousParent;
				this.DraggedWidget.SetSiblingIndex(this._draggedWidgetIndex, false);
				this.DraggedWidget.PosOffset = new Vector2(0f, 0f);
				if (this.DraggedWidget.DragWidget != null)
				{
					this.DraggedWidget.DragWidget.ParentWidget = this.DraggedWidget;
					this.DraggedWidget.DragWidget.IsVisible = false;
				}
			}
			else
			{
				this.ReleaseDraggedWidget();
			}
			this._draggedWidgetPreviousParent = null;
			this._draggedWidgetIndex = -1;
		}

		private void ClearDragObject()
		{
			this.DraggedWidget = null;
			this._dragOffset = new Vector2(0f, 0f);
			this._dragCarrier.ParentWidget = null;
			this._dragCarrier = null;
		}

		internal void UpdateMousePosition(Vector2 mousePos)
		{
			this.MousePosition = mousePos;
		}

		internal void MouseMove()
		{
			if (this._mouseIsDown)
			{
				if (this.IsDragging)
				{
					Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.DragHover, this.MousePosition);
					if (widgetAtPositionForEvent != null)
					{
						this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.DragHover);
					}
					else
					{
						this.SetDragHoveredView(null);
					}
				}
				else if (this.LatestMouseDownWidget != null)
				{
					if (this.LatestMouseDownWidget.PreviewEvent(GauntletEvent.MouseMove))
					{
						this.DispatchEvent(this.LatestMouseDownWidget, GauntletEvent.MouseMove);
					}
					if (!this.IsDragging && this.LatestMouseDownWidget.PreviewEvent(GauntletEvent.DragBegin))
					{
						Vector2 vector = this._lastClickPosition - this.MousePosition;
						Vector2 vector2 = new Vector2(vector.X, vector.Y);
						if (vector2.LengthSquared() > 100f * this.Context.Scale)
						{
							this.DispatchEvent(this.LatestMouseDownWidget, GauntletEvent.DragBegin);
						}
					}
				}
			}
			else if (!this._mouseAlternateIsDown)
			{
				Widget widgetAtPositionForEvent2 = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseMove, this.MousePosition);
				if (widgetAtPositionForEvent2 != null)
				{
					this.DispatchEvent(widgetAtPositionForEvent2, GauntletEvent.MouseMove);
				}
			}
			List<Widget> list = new List<Widget>();
			foreach (Widget widget in EventManager.AllVisibleWidgetsAt(this.Root, this.MousePosition))
			{
				if (!this.MouseOveredViews.Contains(widget))
				{
					widget.OnMouseOverBegin();
					GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
					if (instance != null)
					{
						instance.OnWidgetHoverBegin(this, widget);
					}
				}
				list.Add(widget);
			}
			foreach (Widget widget2 in this.MouseOveredViews.Except(list))
			{
				widget2.OnMouseOverEnd();
				if (widget2.GamepadNavigationIndex != -1)
				{
					GauntletGamepadNavigationManager instance2 = GauntletGamepadNavigationManager.Instance;
					if (instance2 != null)
					{
						instance2.OnWidgetHoverEnd(widget2);
					}
				}
			}
			this.MouseOveredViews = list;
		}

		private static bool IsPointInsideMeasuredArea(Widget w, Vector2 p)
		{
			return w.Left <= p.X && p.X <= w.Right && w.Top <= p.Y && p.Y <= w.Bottom;
		}

		private Widget GetWidgetAtPositionForEvent(GauntletEvent gauntletEvent, Vector2 pointerPosition)
		{
			Widget widget = null;
			foreach (Widget widget2 in EventManager.AllEnabledWidgetsAt(this.Root, pointerPosition))
			{
				if (widget2.PreviewEvent(gauntletEvent))
				{
					widget = widget2;
					break;
				}
			}
			return widget;
		}

		public event Action LoseFocus;

		public event Action GainFocus;

		private void DispatchEvent(Widget selectedWidget, GauntletEvent gauntletEvent)
		{
			if (gauntletEvent != GauntletEvent.MouseReleased)
			{
			}
			switch (gauntletEvent)
			{
			case GauntletEvent.MouseMove:
				selectedWidget.OnMouseMove();
				this.SetHoveredView(selectedWidget);
				return;
			case GauntletEvent.MousePressed:
				this.LatestMouseDownWidget = selectedWidget;
				selectedWidget.OnMousePressed();
				if (this.FocusedWidget != selectedWidget)
				{
					if (this.FocusedWidget != null)
					{
						this.FocusedWidget.OnLoseFocus();
						Action loseFocus = this.LoseFocus;
						if (loseFocus != null)
						{
							loseFocus();
						}
					}
					if (selectedWidget.IsFocusable)
					{
						selectedWidget.OnGainFocus();
						this.FocusedWidget = selectedWidget;
						Action gainFocus = this.GainFocus;
						if (gainFocus != null)
						{
							gainFocus();
						}
					}
					else
					{
						this.FocusedWidget = null;
					}
					EditableTextWidget editableTextWidget;
					if ((editableTextWidget = selectedWidget as EditableTextWidget) != null && this.IsControllerActive)
					{
						string text = editableTextWidget.Text ?? string.Empty;
						string text2 = editableTextWidget.KeyboardInfoText ?? string.Empty;
						int maxLength = editableTextWidget.MaxLength;
						int num = (editableTextWidget.IsObfuscationEnabled ? 2 : 0);
						if (this.FocusedWidget is IntegerInputTextWidget || this.FocusedWidget is FloatInputTextWidget)
						{
							num = 1;
						}
						this.Context.TwoDimensionContext.Platform.OpenOnScreenKeyboard(text, text2, maxLength, num);
						return;
					}
				}
				break;
			case GauntletEvent.MouseReleased:
				if (this.LatestMouseDownWidget != null && this.LatestMouseDownWidget != selectedWidget)
				{
					this.LatestMouseDownWidget.OnMouseReleased();
				}
				if (selectedWidget != null)
				{
					selectedWidget.OnMouseReleased();
					return;
				}
				break;
			case GauntletEvent.MouseAlternatePressed:
				this.LatestMouseAlternateDownWidget = selectedWidget;
				selectedWidget.OnMouseAlternatePressed();
				if (this.FocusedWidget != selectedWidget)
				{
					if (this.FocusedWidget != null)
					{
						this.FocusedWidget.OnLoseFocus();
						Action loseFocus2 = this.LoseFocus;
						if (loseFocus2 != null)
						{
							loseFocus2();
						}
					}
					if (selectedWidget.IsFocusable)
					{
						selectedWidget.OnGainFocus();
						this.FocusedWidget = selectedWidget;
						Action gainFocus2 = this.GainFocus;
						if (gainFocus2 != null)
						{
							gainFocus2();
						}
					}
					else
					{
						this.FocusedWidget = null;
					}
					EditableTextWidget editableTextWidget2;
					if ((editableTextWidget2 = selectedWidget as EditableTextWidget) != null && this.IsControllerActive)
					{
						string text3 = editableTextWidget2.Text ?? string.Empty;
						string text4 = editableTextWidget2.KeyboardInfoText ?? string.Empty;
						int maxLength2 = editableTextWidget2.MaxLength;
						int num2 = (editableTextWidget2.IsObfuscationEnabled ? 2 : 0);
						if (this.FocusedWidget is IntegerInputTextWidget || this.FocusedWidget is FloatInputTextWidget)
						{
							num2 = 1;
						}
						this.Context.TwoDimensionContext.Platform.OpenOnScreenKeyboard(text3, text4, maxLength2, num2);
						return;
					}
				}
				break;
			case GauntletEvent.MouseAlternateReleased:
				if (this.LatestMouseAlternateDownWidget != null && this.LatestMouseAlternateDownWidget != selectedWidget)
				{
					this.LatestMouseAlternateDownWidget.OnMouseAlternateReleased();
				}
				if (selectedWidget != null)
				{
					selectedWidget.OnMouseAlternateReleased();
					return;
				}
				break;
			case GauntletEvent.DragHover:
				this.SetDragHoveredView(selectedWidget);
				return;
			case GauntletEvent.DragBegin:
				selectedWidget.OnDragBegin();
				return;
			case GauntletEvent.DragEnd:
				selectedWidget.OnDragEnd();
				return;
			case GauntletEvent.Drop:
				selectedWidget.OnDrop();
				return;
			case GauntletEvent.MouseScroll:
				selectedWidget.OnMouseScroll();
				return;
			case GauntletEvent.RightStickMovement:
				selectedWidget.OnRightStickMovement();
				break;
			default:
				return;
			}
		}

		public static bool HitTest(Widget widget, Vector2 position)
		{
			Vector2 vector = new Vector2(position.X - widget.EventManager.Root.Left, position.Y - widget.EventManager.Root.Top);
			return EventManager.AnyWidgetsAt(widget, vector);
		}

		public bool FocusTest(Widget root)
		{
			for (Widget widget = this.FocusedWidget; widget != null; widget = widget.ParentWidget)
			{
				if (root == widget)
				{
					return true;
				}
			}
			return false;
		}

		private static bool AnyWidgetsAt(Widget widget, Vector2 position)
		{
			if (widget.IsEnabled && widget.IsVisible)
			{
				if (!widget.DoNotAcceptEvents && EventManager.IsPointInsideMeasuredArea(widget, position))
				{
					return true;
				}
				if (!widget.DoNotPassEventsToChildren)
				{
					for (int i = widget.ChildCount - 1; i >= 0; i--)
					{
						Widget child = widget.GetChild(i);
						Vector2 vector = new Vector2(position.X - widget.Left, position.Y - widget.Top);
						if (!child.IsHidden && !child.IsDisabled && EventManager.AnyWidgetsAt(child, vector))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static IEnumerable<Widget> AllEnabledWidgetsAt(Widget widget, Vector2 position)
		{
			if (widget.IsEnabled && widget.IsVisible)
			{
				if (!widget.DoNotPassEventsToChildren)
				{
					int num;
					for (int i = widget.ChildCount - 1; i >= 0; i = num - 1)
					{
						Widget child = widget.GetChild(i);
						Vector2 vector = new Vector2(position.X - widget.Left, position.Y - widget.Top);
						if (!child.IsHidden && !child.IsDisabled && EventManager.IsPointInsideMeasuredArea(child, vector))
						{
							foreach (Widget widget2 in EventManager.AllEnabledWidgetsAt(child, vector))
							{
								yield return widget2;
							}
							IEnumerator<Widget> enumerator = null;
						}
						num = i;
					}
				}
				if (!widget.DoNotAcceptEvents && EventManager.IsPointInsideMeasuredArea(widget, position))
				{
					yield return widget;
				}
			}
			yield break;
			yield break;
		}

		private static IEnumerable<Widget> AllVisibleWidgetsAt(Widget widget, Vector2 position)
		{
			if (widget.IsVisible)
			{
				int num;
				for (int i = widget.ChildCount - 1; i >= 0; i = num - 1)
				{
					Widget child = widget.GetChild(i);
					Vector2 vector = new Vector2(position.X - widget.Left, position.Y - widget.Top);
					if (child.IsVisible && EventManager.IsPointInsideMeasuredArea(child, vector))
					{
						foreach (Widget widget2 in EventManager.AllVisibleWidgetsAt(child, vector))
						{
							yield return widget2;
						}
						IEnumerator<Widget> enumerator = null;
					}
					num = i;
				}
				if (EventManager.IsPointInsideMeasuredArea(widget, position))
				{
					yield return widget;
				}
			}
			yield break;
			yield break;
		}

		internal void ManualAddRange(List<Widget> list, LinkedList<Widget> linked_list)
		{
			if (list.Capacity < linked_list.Count)
			{
				list.Capacity = linked_list.Count;
			}
			for (LinkedListNode<Widget> linkedListNode = linked_list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				list.Add(linkedListNode.Value);
			}
		}

		private void ParallelUpdateWidget(int startInclusive, int endExclusive, float dt)
		{
			List<Widget> currentList = this._widgetsWithParallelUpdateContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].ParallelUpdate(dt);
			}
		}

		internal void ParallelUpdateWidgets(float dt)
		{
			TWParallel.For(0, this._widgetsWithParallelUpdateContainer.Count, dt, this.ParallelUpdateWidgetPredicate, 16);
		}

		internal void Update(float dt)
		{
			this.Time += dt;
			this.CachedDt = dt;
			this.IsControllerActive = Input.IsControllerConnected && !Input.IsMouseActive;
			int realCount = this._widgetsWithUpdateContainer.RealCount;
			int realCount2 = this._widgetsWithParallelUpdateContainer.RealCount;
			int realCount3 = this._widgetsWithLateUpdateContainer.RealCount;
			int num = MathF.Max(this._widgetsWithUpdateBrushesContainer.RealCount, MathF.Max(realCount, MathF.Max(realCount2, realCount3)));
			if (this._widgetsWithUpdateContainerDoDefragmentationDelegate == null)
			{
				this._widgetsWithUpdateContainerDoDefragmentationDelegate = delegate
				{
					WidgetContainer widgetsWithUpdateContainer = this._widgetsWithUpdateContainer;
					lock (widgetsWithUpdateContainer)
					{
						this._widgetsWithUpdateContainer.DoDefragmentation();
					}
				};
				this._widgetsWithParallelUpdateContainerDoDefragmentationDelegate = delegate
				{
					WidgetContainer widgetsWithParallelUpdateContainer = this._widgetsWithParallelUpdateContainer;
					lock (widgetsWithParallelUpdateContainer)
					{
						this._widgetsWithParallelUpdateContainer.DoDefragmentation();
					}
				};
				this._widgetsWithLateUpdateContainerDoDefragmentationDelegate = delegate
				{
					WidgetContainer widgetsWithLateUpdateContainer = this._widgetsWithLateUpdateContainer;
					lock (widgetsWithLateUpdateContainer)
					{
						this._widgetsWithLateUpdateContainer.DoDefragmentation();
					}
				};
				this._widgetsWithUpdateBrushesContainerDoDefragmentationDelegate = delegate
				{
					WidgetContainer widgetsWithUpdateBrushesContainer = this._widgetsWithUpdateBrushesContainer;
					lock (widgetsWithUpdateBrushesContainer)
					{
						this._widgetsWithUpdateBrushesContainer.DoDefragmentation();
					}
				};
			}
			bool flag = this._widgetsWithUpdateContainer.CheckFragmentation() || this._widgetsWithParallelUpdateContainer.CheckFragmentation() || this._widgetsWithLateUpdateContainer.CheckFragmentation() || this._widgetsWithUpdateBrushesContainer.CheckFragmentation();
			Task task = null;
			Task task2 = null;
			Task task3 = null;
			Task task4 = null;
			if (flag && num > 64)
			{
				task = Task.Run(this._widgetsWithUpdateContainerDoDefragmentationDelegate);
				task2 = Task.Run(this._widgetsWithParallelUpdateContainerDoDefragmentationDelegate);
				task3 = Task.Run(this._widgetsWithLateUpdateContainerDoDefragmentationDelegate);
				task4 = Task.Run(this._widgetsWithUpdateBrushesContainerDoDefragmentationDelegate);
			}
			this.UpdateDragCarrier();
			if (this._widgetsWithVisualDefinitionsContainer.CheckFragmentation())
			{
				WidgetContainer widgetsWithVisualDefinitionsContainer = this._widgetsWithVisualDefinitionsContainer;
				lock (widgetsWithVisualDefinitionsContainer)
				{
					this._widgetsWithVisualDefinitionsContainer.DoDefragmentation();
				}
			}
			List<Widget> currentList = this._widgetsWithVisualDefinitionsContainer.GetCurrentList();
			for (int i = 0; i < currentList.Count; i++)
			{
				currentList[i].UpdateVisualDefinitions(dt);
			}
			if (flag)
			{
				if (num > 64)
				{
					Task.WaitAll(new Task[] { task, task2, task3, task4 });
				}
				else
				{
					this._widgetsWithUpdateContainerDoDefragmentationDelegate();
					this._widgetsWithParallelUpdateContainerDoDefragmentationDelegate();
					this._widgetsWithLateUpdateContainerDoDefragmentationDelegate();
					this._widgetsWithUpdateBrushesContainerDoDefragmentationDelegate();
				}
			}
			Widget hoveredView = this.HoveredView;
			UIContext.MouseCursors mouseCursors = ((((hoveredView != null) ? hoveredView.HoveredCursorState : null) != null) ? ((UIContext.MouseCursors)Enum.Parse(typeof(UIContext.MouseCursors), this.HoveredView.HoveredCursorState)) : UIContext.MouseCursors.Default);
			this.Context.ActiveCursorOfContext = mouseCursors;
			List<Widget> currentList2 = this._widgetsWithUpdateContainer.GetCurrentList();
			for (int j = 0; j < currentList2.Count; j++)
			{
				currentList2[j].Update(dt);
			}
			this._doingParallelTask = true;
			if (this._widgetsWithParallelUpdateContainer.Count > 64)
			{
				this.ParallelUpdateWidgets(dt);
			}
			else
			{
				List<Widget> currentList3 = this._widgetsWithParallelUpdateContainer.GetCurrentList();
				for (int k = 0; k < currentList3.Count; k++)
				{
					currentList3[k].ParallelUpdate(dt);
				}
			}
			this._doingParallelTask = false;
		}

		internal void ParallelUpdateBrushes(float dt)
		{
			TWParallel.For(0, this._widgetsWithUpdateBrushesContainer.Count, dt, this.UpdateBrushesWidgetPredicate, 16);
		}

		internal void UpdateBrushes(float dt)
		{
			if (this._widgetsWithUpdateBrushesContainer.Count > 64)
			{
				this.ParallelUpdateBrushes(dt);
				return;
			}
			List<Widget> currentList = this._widgetsWithUpdateBrushesContainer.GetCurrentList();
			for (int i = 0; i < currentList.Count; i++)
			{
				currentList[i].UpdateBrushes(dt);
			}
		}

		private void UpdateBrushesWidget(int startInclusive, int endExclusive, float dt)
		{
			List<Widget> currentList = this._widgetsWithUpdateBrushesContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].UpdateBrushes(dt);
			}
		}

		public void AddLateUpdateAction(Widget owner, Action<float> action, int order)
		{
			UpdateAction updateAction = default(UpdateAction);
			updateAction.Target = owner;
			updateAction.Action = action;
			updateAction.Order = order;
			if (this._doingParallelTask)
			{
				object lateUpdateActionLocker = this._lateUpdateActionLocker;
				lock (lateUpdateActionLocker)
				{
					this._lateUpdateActions[order].Add(updateAction);
					return;
				}
			}
			this._lateUpdateActions[order].Add(updateAction);
		}

		internal void LateUpdate(float dt)
		{
			List<Widget> currentList = this._widgetsWithLateUpdateContainer.GetCurrentList();
			for (int i = 0; i < currentList.Count; i++)
			{
				currentList[i].LateUpdate(dt);
			}
			Dictionary<int, List<UpdateAction>> lateUpdateActions = this._lateUpdateActions;
			this._lateUpdateActions = this._lateUpdateActionsRunning;
			this._lateUpdateActionsRunning = lateUpdateActions;
			for (int j = 1; j <= 5; j++)
			{
				List<UpdateAction> list = this._lateUpdateActionsRunning[j];
				foreach (UpdateAction updateAction in list)
				{
					updateAction.Action(dt);
				}
				list.Clear();
			}
			if (this.IsControllerActive)
			{
				if (this.HoveredView != null && this.HoveredView.IsRecursivelyVisible())
				{
					if (this.HoveredView.FrictionEnabled && this.DraggedWidget == null)
					{
						this._lastSetFrictionValue = 0.45f;
					}
					else
					{
						this._lastSetFrictionValue = 1f;
					}
					Input.SetCursorFriction(this._lastSetFrictionValue);
				}
				if (!this._lastSetFrictionValue.ApproximatelyEqualsTo(1f, 1E-05f) && this.HoveredView == null)
				{
					this._lastSetFrictionValue = 1f;
					Input.SetCursorFriction(this._lastSetFrictionValue);
				}
			}
		}

		public void SetWidgetFocused(Widget widget, bool fromClick = false)
		{
			if (this.FocusedWidget != widget)
			{
				Widget focusedWidget = this.FocusedWidget;
				if (focusedWidget != null)
				{
					focusedWidget.OnLoseFocus();
				}
				if (widget != null)
				{
					widget.OnGainFocus();
				}
				this.FocusedWidget = widget;
				EditableTextWidget editableTextWidget;
				if ((editableTextWidget = this.FocusedWidget as EditableTextWidget) != null && this.IsControllerActive)
				{
					string text = editableTextWidget.Text ?? string.Empty;
					string text2 = editableTextWidget.KeyboardInfoText ?? string.Empty;
					int maxLength = editableTextWidget.MaxLength;
					int num = (editableTextWidget.IsObfuscationEnabled ? 2 : 0);
					if (this.FocusedWidget is IntegerInputTextWidget || this.FocusedWidget is FloatInputTextWidget)
					{
						num = 1;
					}
					this.Context.TwoDimensionContext.Platform.OpenOnScreenKeyboard(text, text2, maxLength, num);
				}
			}
		}

		private void UpdateDragCarrier()
		{
			if (this._dragCarrier != null)
			{
				this._dragCarrier.PosOffset = this.MousePositionInReferenceResolution + this._dragOffset - new Vector2(this.LeftUsableAreaStart, this.TopUsableAreaStart) * this.Context.InverseScale;
			}
		}

		public void SetHoveredView(Widget view)
		{
			if (this.HoveredView != view)
			{
				if (this.HoveredView != null)
				{
					this.HoveredView.OnHoverEnd();
				}
				this.HoveredView = view;
				if (this.HoveredView != null)
				{
					this.HoveredView.OnHoverBegin();
				}
			}
		}

		internal bool SetDragHoveredView(Widget view)
		{
			if (this.DragHoveredView != view && this.DragHoveredView != null)
			{
				this.DragHoveredView.OnDragHoverEnd();
			}
			this.DragHoveredView = view;
			if (this.DragHoveredView != null && this.DragHoveredView.AcceptDrop)
			{
				this.DragHoveredView.OnDragHoverBegin();
				return true;
			}
			this.DragHoveredView = null;
			return false;
		}

		internal void BeginDragging(Widget draggedObject)
		{
			draggedObject.IsPressed = false;
			this._draggedWidgetPreviousParent = null;
			this._draggedWidgetIndex = -1;
			Widget parentWidget = draggedObject.ParentWidget;
			this.DraggedWidget = draggedObject;
			Vector2 globalPosition = this.DraggedWidget.GlobalPosition;
			this._dragCarrier = new DragCarrierWidget(this.Context);
			this._dragCarrier.ParentWidget = this.Root;
			if (draggedObject.DragWidget != null)
			{
				Widget dragWidget = draggedObject.DragWidget;
				this._dragCarrier.WidthSizePolicy = SizePolicy.CoverChildren;
				this._dragCarrier.HeightSizePolicy = SizePolicy.CoverChildren;
				this._dragOffset = Vector2.Zero;
				dragWidget.IsVisible = true;
				dragWidget.ParentWidget = this._dragCarrier;
				if (this.DraggedWidget.HideOnDrag)
				{
					this.DraggedWidget.IsVisible = false;
				}
				this._draggedWidgetPreviousParent = null;
			}
			else
			{
				this._dragOffset = (globalPosition - this.MousePosition) * this.Context.InverseScale;
				this._dragCarrier.WidthSizePolicy = SizePolicy.Fixed;
				this._dragCarrier.HeightSizePolicy = SizePolicy.Fixed;
				if (this.DraggedWidget.WidthSizePolicy == SizePolicy.StretchToParent)
				{
					this._dragCarrier.ScaledSuggestedWidth = this.DraggedWidget.Size.X + (this.DraggedWidget.MarginRight + this.DraggedWidget.MarginLeft) * this.Context.Scale;
					this._dragOffset += new Vector2(-this.DraggedWidget.MarginLeft, 0f);
				}
				else
				{
					this._dragCarrier.ScaledSuggestedWidth = this.DraggedWidget.Size.X;
				}
				if (this.DraggedWidget.HeightSizePolicy == SizePolicy.StretchToParent)
				{
					this._dragCarrier.ScaledSuggestedHeight = this.DraggedWidget.Size.Y + (this.DraggedWidget.MarginTop + this.DraggedWidget.MarginBottom) * this.Context.Scale;
					this._dragOffset += new Vector2(0f, -this.DraggedWidget.MarginTop);
				}
				else
				{
					this._dragCarrier.ScaledSuggestedHeight = this.DraggedWidget.Size.Y;
				}
				if (parentWidget != null)
				{
					this._draggedWidgetPreviousParent = parentWidget;
					this._draggedWidgetIndex = draggedObject.GetSiblingIndex();
				}
				this.DraggedWidget.ParentWidget = this._dragCarrier;
			}
			this._dragCarrier.PosOffset = this.MousePositionInReferenceResolution + this._dragOffset - new Vector2(this.LeftUsableAreaStart, this.TopUsableAreaStart) * this.Context.InverseScale;
		}

		internal Widget ReleaseDraggedWidget()
		{
			Widget draggedWidget = this.DraggedWidget;
			if (this._draggedWidgetPreviousParent != null)
			{
				this.DraggedWidget.ParentWidget = this._draggedWidgetPreviousParent;
				this._draggedWidgetIndex = MathF.Max(0, MathF.Min(MathF.Max(0, this.DraggedWidget.ParentWidget.ChildCount - 1), this._draggedWidgetIndex));
				this.DraggedWidget.SetSiblingIndex(this._draggedWidgetIndex, false);
			}
			else
			{
				this.DraggedWidget.IsVisible = true;
			}
			this.SetDragHoveredView(null);
			return draggedWidget;
		}

		internal void Render(TwoDimensionContext twoDimensionContext)
		{
			this._drawContext.Reset();
			this.Root.Render(twoDimensionContext, this._drawContext);
			this._drawContext.DrawTo(twoDimensionContext);
		}

		public void UpdateLayout()
		{
			this.SetMeasureDirty();
			this.SetLayoutDirty();
		}

		internal void SetMeasureDirty()
		{
			this._measureDirty = 2;
		}

		internal void SetLayoutDirty()
		{
			this._layoutDirty = 2;
		}

		internal void SetPositionsDirty()
		{
			this._positionsDirty = true;
		}

		public void OnMovieLoaded(string movieName)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnMovieLoaded(this, movieName);
		}

		public void OnMovieReleased(string movieName)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnMovieReleased(this, movieName);
		}

		public bool GetIsHitThisFrame()
		{
			return this.OnGetIsHitThisFrame();
		}

		public bool GetIsBlockedAtPosition(Vector2 position)
		{
			return this.OnGetIsBlockedAtPosition(position);
		}

		public int GetLastScreenOrder()
		{
			return this.OnGetLastScreenOrder();
		}

		public bool IsAvailableForNavigation()
		{
			return this.OnGetIsAvailableForGamepadNavigation();
		}

		public void OnWidgetUsedNavigationMovementsUpdated(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetUsedNavigationMovementsUpdated(widget);
		}

		public void OnGainNavigation()
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnEventManagerGainedNavigation(this);
		}

		public void GainNavigationAfterFrames(int frameCount, Func<bool> predicate = null)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetEventManagerNavigationGainAfterFrames(this, frameCount, predicate);
		}

		public void GainNavigationAfterTime(float seconds, Func<bool> predicate = null)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetEventManagerNavigationGainAfterTime(this, seconds, predicate);
		}

		public void OnWidgetNavigationStatusChanged(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetNavigationStatusChanged(this, widget);
		}

		public void OnWidgetNavigationIndexUpdated(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetNavigationIndexUpdated(this, widget);
		}

		public void AddNavigationScope(GamepadNavigationScope scope, bool initialize = false)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.AddNavigationScope(this, scope, initialize);
		}

		public void RemoveNavigationScope(GamepadNavigationScope scope)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.RemoveNavigationScope(this, scope);
		}

		public void AddForcedScopeCollection(GamepadNavigationForcedScopeCollection collection)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.AddForcedScopeCollection(collection);
		}

		public void RemoveForcedScopeCollection(GamepadNavigationForcedScopeCollection collection)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.RemoveForcedScopeCollection(collection);
		}

		public bool HasNavigationScope(GamepadNavigationScope scope)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			return instance != null && instance.HasNavigationScope(this, scope);
		}

		public bool HasNavigationScope(Func<GamepadNavigationScope, bool> predicate)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			return instance != null && instance.HasNavigationScope(this, predicate);
		}

		public const int MinParallelUpdateCount = 64;

		private const int DirtyCount = 2;

		private const float DragStartThreshold = 100f;

		private const float ScrollScale = 0.4f;

		private Widget _focusedWidget;

		private Widget _hoveredView;

		private List<Widget> _mouseOveredViews;

		private Widget _dragHoveredView;

		private Widget _draggedWidget;

		private Widget _latestMouseDownWidget;

		private Widget _latestMouseUpWidget;

		private Widget _latestMouseAlternateDownWidget;

		private Widget _latestMouseAlternateUpWidget;

		private int _measureDirty;

		private int _layoutDirty;

		private bool _positionsDirty;

		private const int _stickMovementScaleAmount = 3000;

		private Vector2 _lastClickPosition;

		private bool _mouseIsDown;

		private Vector2 _lastAlternateClickPosition;

		private bool _mouseAlternateIsDown;

		private Vector2 _dragOffset = new Vector2(0f, 0f);

		private Widget _draggedWidgetPreviousParent;

		private int _draggedWidgetIndex;

		private DragCarrierWidget _dragCarrier;

		private object _lateUpdateActionLocker;

		private Dictionary<int, List<UpdateAction>> _lateUpdateActions;

		private Dictionary<int, List<UpdateAction>> _lateUpdateActionsRunning;

		private WidgetContainer _widgetsWithUpdateContainer;

		private WidgetContainer _widgetsWithLateUpdateContainer;

		private WidgetContainer _widgetsWithParallelUpdateContainer;

		private WidgetContainer _widgetsWithVisualDefinitionsContainer;

		private WidgetContainer _widgetsWithTweenPositionsContainer;

		private WidgetContainer _widgetsWithUpdateBrushesContainer;

		private const int UpdateActionOrderCount = 5;

		private volatile bool _doingParallelTask;

		private TwoDimensionDrawContext _drawContext;

		private Action _widgetsWithUpdateContainerDoDefragmentationDelegate;

		private Action _widgetsWithParallelUpdateContainerDoDefragmentationDelegate;

		private Action _widgetsWithLateUpdateContainerDoDefragmentationDelegate;

		private Action _widgetsWithUpdateBrushesContainerDoDefragmentationDelegate;

		private readonly TWParallel.ParallelForWithDtAuxPredicate ParallelUpdateWidgetPredicate;

		private readonly TWParallel.ParallelForWithDtAuxPredicate UpdateBrushesWidgetPredicate;

		private readonly TWParallel.ParallelForWithDtAuxPredicate WidgetDoTweenPositionAuxPredicate;

		private float _lastSetFrictionValue = 1f;

		public Func<bool> OnGetIsHitThisFrame;

		public Func<Vector2, bool> OnGetIsBlockedAtPosition;

		public Func<int> OnGetLastScreenOrder;

		public Func<bool> OnGetIsAvailableForGamepadNavigation;
	}
}
