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
	// Token: 0x0200001B RID: 27
	public class EventManager
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000ADA0 File Offset: 0x00008FA0
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000ADA8 File Offset: 0x00008FA8
		public float Time { get; private set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000ADB1 File Offset: 0x00008FB1
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000ADB9 File Offset: 0x00008FB9
		public Vec2 UsableArea { get; set; } = new Vec2(1f, 1f);

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000ADC2 File Offset: 0x00008FC2
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000ADCA File Offset: 0x00008FCA
		public float LeftUsableAreaStart { get; private set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000ADD3 File Offset: 0x00008FD3
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000ADDB File Offset: 0x00008FDB
		public float TopUsableAreaStart { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000ADE4 File Offset: 0x00008FE4
		// (set) Token: 0x060001EB RID: 491 RVA: 0x0000ADEB File Offset: 0x00008FEB
		public static EventManager UIEventManager { get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000ADF3 File Offset: 0x00008FF3
		public Vector2 MousePositionInReferenceResolution
		{
			get
			{
				return this.MousePosition * this.Context.CustomInverseScale;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000AE0B File Offset: 0x0000900B
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000AE13 File Offset: 0x00009013
		public bool IsControllerActive { get; private set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000AE1C File Offset: 0x0000901C
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000AE24 File Offset: 0x00009024
		public Vector2 PageSize { get; private set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000AE2D File Offset: 0x0000902D
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000AE35 File Offset: 0x00009035
		public UIContext Context { get; private set; }

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060001F3 RID: 499 RVA: 0x0000AE40 File Offset: 0x00009040
		// (remove) Token: 0x060001F4 RID: 500 RVA: 0x0000AE78 File Offset: 0x00009078
		public event Action OnDragStarted;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060001F5 RID: 501 RVA: 0x0000AEB0 File Offset: 0x000090B0
		// (remove) Token: 0x060001F6 RID: 502 RVA: 0x0000AEE8 File Offset: 0x000090E8
		public event Action OnDragEnded;

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000AF1D File Offset: 0x0000911D
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000AF25 File Offset: 0x00009125
		public IInputService InputService { get; internal set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000AF2E File Offset: 0x0000912E
		// (set) Token: 0x060001FA RID: 506 RVA: 0x0000AF36 File Offset: 0x00009136
		public IInputContext InputContext { get; internal set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000AF3F File Offset: 0x0000913F
		// (set) Token: 0x060001FC RID: 508 RVA: 0x0000AF47 File Offset: 0x00009147
		public Widget Root { get; private set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060001FD RID: 509 RVA: 0x0000AF50 File Offset: 0x00009150
		// (set) Token: 0x060001FE RID: 510 RVA: 0x0000AF58 File Offset: 0x00009158
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

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060001FF RID: 511 RVA: 0x0000AF74 File Offset: 0x00009174
		// (set) Token: 0x06000200 RID: 512 RVA: 0x0000AF7C File Offset: 0x0000917C
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

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000AF98 File Offset: 0x00009198
		// (set) Token: 0x06000202 RID: 514 RVA: 0x0000AFA0 File Offset: 0x000091A0
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

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000AFB4 File Offset: 0x000091B4
		// (set) Token: 0x06000204 RID: 516 RVA: 0x0000AFBC File Offset: 0x000091BC
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

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000205 RID: 517 RVA: 0x0000AFD8 File Offset: 0x000091D8
		// (set) Token: 0x06000206 RID: 518 RVA: 0x0000AFE0 File Offset: 0x000091E0
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

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0000B01C File Offset: 0x0000921C
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

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000B069 File Offset: 0x00009269
		// (set) Token: 0x06000209 RID: 521 RVA: 0x0000B071 File Offset: 0x00009271
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

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000B08D File Offset: 0x0000928D
		// (set) Token: 0x0600020B RID: 523 RVA: 0x0000B095 File Offset: 0x00009295
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

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000B0B1 File Offset: 0x000092B1
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000B0B9 File Offset: 0x000092B9
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

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000B0D5 File Offset: 0x000092D5
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000B0DD File Offset: 0x000092DD
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000B0F9 File Offset: 0x000092F9
		// (set) Token: 0x06000211 RID: 529 RVA: 0x0000B101 File Offset: 0x00009301
		public Vector2 MousePosition { get; private set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000B10A File Offset: 0x0000930A
		private bool IsDragging
		{
			get
			{
				return this.DraggedWidget != null;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000B115 File Offset: 0x00009315
		public float DeltaMouseScroll
		{
			get
			{
				return this.InputContext.GetDeltaMouseScroll() * 0.4f;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000B128 File Offset: 0x00009328
		public float RightStickVerticalScrollAmount
		{
			get
			{
				float y = Input.GetKeyState(InputKey.ControllerRStick).Y;
				return 3000f * y * 0.4f * this.CachedDt;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000B15C File Offset: 0x0000935C
		public float RightStickHorizontalScrollAmount
		{
			get
			{
				float x = Input.GetKeyState(InputKey.ControllerRStick).X;
				return 3000f * x * 0.4f * this.CachedDt;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000B190 File Offset: 0x00009390
		// (set) Token: 0x06000217 RID: 535 RVA: 0x0000B198 File Offset: 0x00009398
		internal float CachedDt { get; private set; }

		// Token: 0x06000218 RID: 536 RVA: 0x0000B1A4 File Offset: 0x000093A4
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

		// Token: 0x06000219 RID: 537 RVA: 0x0000B32E File Offset: 0x0000952E
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

		// Token: 0x0600021A RID: 538 RVA: 0x0000B370 File Offset: 0x00009570
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

		// Token: 0x0600021B RID: 539 RVA: 0x0000B3F4 File Offset: 0x000095F4
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

		// Token: 0x0600021C RID: 540 RVA: 0x0000B4C0 File Offset: 0x000096C0
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

		// Token: 0x0600021D RID: 541 RVA: 0x0000B6DC File Offset: 0x000098DC
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

		// Token: 0x0600021E RID: 542 RVA: 0x0000B91C File Offset: 0x00009B1C
		internal void OnWidgetVisualDefinitionChanged(Widget widget)
		{
			if (widget.VisualDefinition != null)
			{
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget);
				return;
			}
			this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.VisualDefinition, widget);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000B937 File Offset: 0x00009B37
		internal void OnWidgetTweenPositionChanged(Widget widget)
		{
			if (widget.TweenPosition)
			{
				this.RegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget);
				return;
			}
			this.UnRegisterWidgetForEvent(WidgetContainer.ContainerType.TweenPosition, widget);
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000B952 File Offset: 0x00009B52
		private void MeasureAll()
		{
			this.Root.Measure(this.PageSize);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000B965 File Offset: 0x00009B65
		private void LayoutAll(float left, float bottom, float right, float top)
		{
			this.Root.Layout(left, bottom, right, top);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000B977 File Offset: 0x00009B77
		private void UpdatePositions()
		{
			this.Root.UpdatePosition(Vector2.Zero);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000B98C File Offset: 0x00009B8C
		private void WidgetDoTweenPositionAux(int startInclusive, int endExclusive, float deltaTime)
		{
			List<Widget> currentList = this._widgetsWithParallelUpdateContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].DoTweenPosition(deltaTime);
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000B9BE File Offset: 0x00009BBE
		private void ParallelDoTweenPositions(float dt)
		{
			TWParallel.For(0, this._widgetsWithTweenPositionsContainer.Count, dt, this.WidgetDoTweenPositionAuxPredicate, 16);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000B9DC File Offset: 0x00009BDC
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

		// Token: 0x06000226 RID: 550 RVA: 0x0000BA70 File Offset: 0x00009C70
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

		// Token: 0x06000227 RID: 551 RVA: 0x0000BB98 File Offset: 0x00009D98
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

		// Token: 0x06000228 RID: 552 RVA: 0x0000BC04 File Offset: 0x00009E04
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

		// Token: 0x06000229 RID: 553 RVA: 0x0000BC54 File Offset: 0x00009E54
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

		// Token: 0x0600022A RID: 554 RVA: 0x0000BD14 File Offset: 0x00009F14
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

		// Token: 0x0600022B RID: 555 RVA: 0x0000BD64 File Offset: 0x00009F64
		internal void MouseAlternateUp()
		{
			this._mouseAlternateIsDown = false;
			this.MousePosition = new Vector2((float)this.InputContext.GetPointerX(), (float)this.InputContext.GetPointerY());
			Widget widgetAtPositionForEvent = this.GetWidgetAtPositionForEvent(GauntletEvent.MouseAlternateReleased, this._lastAlternateClickPosition);
			this.DispatchEvent(widgetAtPositionForEvent, GauntletEvent.MouseAlternateReleased);
			this.LatestMouseAlternateUpWidget = widgetAtPositionForEvent;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000BDB8 File Offset: 0x00009FB8
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

		// Token: 0x0600022D RID: 557 RVA: 0x0000BDF4 File Offset: 0x00009FF4
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

		// Token: 0x0600022E RID: 558 RVA: 0x0000BE4E File Offset: 0x0000A04E
		public void ClearFocus()
		{
			this.SetWidgetFocused(null, false);
			this.SetHoveredView(null);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000BE60 File Offset: 0x0000A060
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

		// Token: 0x06000230 RID: 560 RVA: 0x0000BEFD File Offset: 0x0000A0FD
		private void ClearDragObject()
		{
			this.DraggedWidget = null;
			this._dragOffset = new Vector2(0f, 0f);
			this._dragCarrier.ParentWidget = null;
			this._dragCarrier = null;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000BF2E File Offset: 0x0000A12E
		internal void UpdateMousePosition(Vector2 mousePos)
		{
			this.MousePosition = mousePos;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000BF38 File Offset: 0x0000A138
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

		// Token: 0x06000233 RID: 563 RVA: 0x0000C120 File Offset: 0x0000A320
		private static bool IsPointInsideMeasuredArea(Widget w, Vector2 p)
		{
			return w.Left <= p.X && p.X <= w.Right && w.Top <= p.Y && p.Y <= w.Bottom;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000C160 File Offset: 0x0000A360
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

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000235 RID: 565 RVA: 0x0000C1BC File Offset: 0x0000A3BC
		// (remove) Token: 0x06000236 RID: 566 RVA: 0x0000C1F4 File Offset: 0x0000A3F4
		public event Action LoseFocus;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000237 RID: 567 RVA: 0x0000C22C File Offset: 0x0000A42C
		// (remove) Token: 0x06000238 RID: 568 RVA: 0x0000C264 File Offset: 0x0000A464
		public event Action GainFocus;

		// Token: 0x06000239 RID: 569 RVA: 0x0000C29C File Offset: 0x0000A49C
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

		// Token: 0x0600023A RID: 570 RVA: 0x0000C55C File Offset: 0x0000A75C
		public static bool HitTest(Widget widget, Vector2 position)
		{
			Vector2 vector = new Vector2(position.X - widget.EventManager.Root.Left, position.Y - widget.EventManager.Root.Top);
			return EventManager.AnyWidgetsAt(widget, vector);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
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

		// Token: 0x0600023C RID: 572 RVA: 0x0000C5D0 File Offset: 0x0000A7D0
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

		// Token: 0x0600023D RID: 573 RVA: 0x0000C660 File Offset: 0x0000A860
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

		// Token: 0x0600023E RID: 574 RVA: 0x0000C677 File Offset: 0x0000A877
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

		// Token: 0x0600023F RID: 575 RVA: 0x0000C690 File Offset: 0x0000A890
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

		// Token: 0x06000240 RID: 576 RVA: 0x0000C6D8 File Offset: 0x0000A8D8
		private void ParallelUpdateWidget(int startInclusive, int endExclusive, float dt)
		{
			List<Widget> currentList = this._widgetsWithParallelUpdateContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].ParallelUpdate(dt);
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000C70A File Offset: 0x0000A90A
		internal void ParallelUpdateWidgets(float dt)
		{
			TWParallel.For(0, this._widgetsWithParallelUpdateContainer.Count, dt, this.ParallelUpdateWidgetPredicate, 16);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C728 File Offset: 0x0000A928
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

		// Token: 0x06000243 RID: 579 RVA: 0x0000CA20 File Offset: 0x0000AC20
		internal void ParallelUpdateBrushes(float dt)
		{
			TWParallel.For(0, this._widgetsWithUpdateBrushesContainer.Count, dt, this.UpdateBrushesWidgetPredicate, 16);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000CA3C File Offset: 0x0000AC3C
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

		// Token: 0x06000245 RID: 581 RVA: 0x0000CA8C File Offset: 0x0000AC8C
		private void UpdateBrushesWidget(int startInclusive, int endExclusive, float dt)
		{
			List<Widget> currentList = this._widgetsWithUpdateBrushesContainer.GetCurrentList();
			for (int i = startInclusive; i < endExclusive; i++)
			{
				currentList[i].UpdateBrushes(dt);
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
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

		// Token: 0x06000247 RID: 583 RVA: 0x0000CB48 File Offset: 0x0000AD48
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

		// Token: 0x06000248 RID: 584 RVA: 0x0000CC90 File Offset: 0x0000AE90
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

		// Token: 0x06000249 RID: 585 RVA: 0x0000CD50 File Offset: 0x0000AF50
		private void UpdateDragCarrier()
		{
			if (this._dragCarrier != null)
			{
				this._dragCarrier.PosOffset = this.MousePositionInReferenceResolution + this._dragOffset - new Vector2(this.LeftUsableAreaStart, this.TopUsableAreaStart) * this.Context.InverseScale;
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000CDA7 File Offset: 0x0000AFA7
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

		// Token: 0x0600024B RID: 587 RVA: 0x0000CDE0 File Offset: 0x0000AFE0
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

		// Token: 0x0600024C RID: 588 RVA: 0x0000CE3C File Offset: 0x0000B03C
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

		// Token: 0x0600024D RID: 589 RVA: 0x0000D0C8 File Offset: 0x0000B2C8
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

		// Token: 0x0600024E RID: 590 RVA: 0x0000D14B File Offset: 0x0000B34B
		internal void Render(TwoDimensionContext twoDimensionContext)
		{
			this._drawContext.Reset();
			this.Root.Render(twoDimensionContext, this._drawContext);
			this._drawContext.DrawTo(twoDimensionContext);
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000D176 File Offset: 0x0000B376
		public void UpdateLayout()
		{
			this.SetMeasureDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000D184 File Offset: 0x0000B384
		internal void SetMeasureDirty()
		{
			this._measureDirty = 2;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000D18D File Offset: 0x0000B38D
		internal void SetLayoutDirty()
		{
			this._layoutDirty = 2;
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000D196 File Offset: 0x0000B396
		internal void SetPositionsDirty()
		{
			this._positionsDirty = true;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000D19F File Offset: 0x0000B39F
		public void OnMovieLoaded(string movieName)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnMovieLoaded(this, movieName);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000D1B2 File Offset: 0x0000B3B2
		public void OnMovieReleased(string movieName)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnMovieReleased(this, movieName);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000D1C5 File Offset: 0x0000B3C5
		public bool GetIsHitThisFrame()
		{
			return this.OnGetIsHitThisFrame();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000D1D2 File Offset: 0x0000B3D2
		public bool GetIsBlockedAtPosition(Vector2 position)
		{
			return this.OnGetIsBlockedAtPosition(position);
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000D1E0 File Offset: 0x0000B3E0
		public int GetLastScreenOrder()
		{
			return this.OnGetLastScreenOrder();
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000D1ED File Offset: 0x0000B3ED
		public bool IsAvailableForNavigation()
		{
			return this.OnGetIsAvailableForGamepadNavigation();
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000D1FA File Offset: 0x0000B3FA
		public void OnWidgetUsedNavigationMovementsUpdated(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetUsedNavigationMovementsUpdated(widget);
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000D20C File Offset: 0x0000B40C
		public void OnGainNavigation()
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnEventManagerGainedNavigation(this);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000D21E File Offset: 0x0000B41E
		public void GainNavigationAfterFrames(int frameCount, Func<bool> predicate = null)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetEventManagerNavigationGainAfterFrames(this, frameCount, predicate);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000D232 File Offset: 0x0000B432
		public void GainNavigationAfterTime(float seconds, Func<bool> predicate = null)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetEventManagerNavigationGainAfterTime(this, seconds, predicate);
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000D246 File Offset: 0x0000B446
		public void OnWidgetNavigationStatusChanged(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetNavigationStatusChanged(this, widget);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000D259 File Offset: 0x0000B459
		public void OnWidgetNavigationIndexUpdated(Widget widget)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.OnWidgetNavigationIndexUpdated(this, widget);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000D26C File Offset: 0x0000B46C
		public void AddNavigationScope(GamepadNavigationScope scope, bool initialize = false)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.AddNavigationScope(this, scope, initialize);
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000D280 File Offset: 0x0000B480
		public void RemoveNavigationScope(GamepadNavigationScope scope)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.RemoveNavigationScope(this, scope);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000D293 File Offset: 0x0000B493
		public void AddForcedScopeCollection(GamepadNavigationForcedScopeCollection collection)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.AddForcedScopeCollection(collection);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000D2A5 File Offset: 0x0000B4A5
		public void RemoveForcedScopeCollection(GamepadNavigationForcedScopeCollection collection)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.RemoveForcedScopeCollection(collection);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000D2B7 File Offset: 0x0000B4B7
		public bool HasNavigationScope(GamepadNavigationScope scope)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			return instance != null && instance.HasNavigationScope(this, scope);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000D2CB File Offset: 0x0000B4CB
		public bool HasNavigationScope(Func<GamepadNavigationScope, bool> predicate)
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			return instance != null && instance.HasNavigationScope(this, predicate);
		}

		// Token: 0x04000107 RID: 263
		public const int MinParallelUpdateCount = 64;

		// Token: 0x04000108 RID: 264
		private const int DirtyCount = 2;

		// Token: 0x04000109 RID: 265
		private const float DragStartThreshold = 100f;

		// Token: 0x0400010A RID: 266
		private const float ScrollScale = 0.4f;

		// Token: 0x04000116 RID: 278
		private Widget _focusedWidget;

		// Token: 0x04000117 RID: 279
		private Widget _hoveredView;

		// Token: 0x04000118 RID: 280
		private List<Widget> _mouseOveredViews;

		// Token: 0x04000119 RID: 281
		private Widget _dragHoveredView;

		// Token: 0x0400011A RID: 282
		private Widget _draggedWidget;

		// Token: 0x0400011B RID: 283
		private Widget _latestMouseDownWidget;

		// Token: 0x0400011C RID: 284
		private Widget _latestMouseUpWidget;

		// Token: 0x0400011D RID: 285
		private Widget _latestMouseAlternateDownWidget;

		// Token: 0x0400011E RID: 286
		private Widget _latestMouseAlternateUpWidget;

		// Token: 0x04000120 RID: 288
		private int _measureDirty;

		// Token: 0x04000121 RID: 289
		private int _layoutDirty;

		// Token: 0x04000122 RID: 290
		private bool _positionsDirty;

		// Token: 0x04000123 RID: 291
		private const int _stickMovementScaleAmount = 3000;

		// Token: 0x04000125 RID: 293
		private Vector2 _lastClickPosition;

		// Token: 0x04000126 RID: 294
		private bool _mouseIsDown;

		// Token: 0x04000127 RID: 295
		private Vector2 _lastAlternateClickPosition;

		// Token: 0x04000128 RID: 296
		private bool _mouseAlternateIsDown;

		// Token: 0x04000129 RID: 297
		private Vector2 _dragOffset = new Vector2(0f, 0f);

		// Token: 0x0400012A RID: 298
		private Widget _draggedWidgetPreviousParent;

		// Token: 0x0400012B RID: 299
		private int _draggedWidgetIndex;

		// Token: 0x0400012C RID: 300
		private DragCarrierWidget _dragCarrier;

		// Token: 0x0400012D RID: 301
		private object _lateUpdateActionLocker;

		// Token: 0x0400012E RID: 302
		private Dictionary<int, List<UpdateAction>> _lateUpdateActions;

		// Token: 0x0400012F RID: 303
		private Dictionary<int, List<UpdateAction>> _lateUpdateActionsRunning;

		// Token: 0x04000130 RID: 304
		private WidgetContainer _widgetsWithUpdateContainer;

		// Token: 0x04000131 RID: 305
		private WidgetContainer _widgetsWithLateUpdateContainer;

		// Token: 0x04000132 RID: 306
		private WidgetContainer _widgetsWithParallelUpdateContainer;

		// Token: 0x04000133 RID: 307
		private WidgetContainer _widgetsWithVisualDefinitionsContainer;

		// Token: 0x04000134 RID: 308
		private WidgetContainer _widgetsWithTweenPositionsContainer;

		// Token: 0x04000135 RID: 309
		private WidgetContainer _widgetsWithUpdateBrushesContainer;

		// Token: 0x04000136 RID: 310
		private const int UpdateActionOrderCount = 5;

		// Token: 0x04000137 RID: 311
		private volatile bool _doingParallelTask;

		// Token: 0x04000138 RID: 312
		private TwoDimensionDrawContext _drawContext;

		// Token: 0x04000139 RID: 313
		private Action _widgetsWithUpdateContainerDoDefragmentationDelegate;

		// Token: 0x0400013A RID: 314
		private Action _widgetsWithParallelUpdateContainerDoDefragmentationDelegate;

		// Token: 0x0400013B RID: 315
		private Action _widgetsWithLateUpdateContainerDoDefragmentationDelegate;

		// Token: 0x0400013C RID: 316
		private Action _widgetsWithUpdateBrushesContainerDoDefragmentationDelegate;

		// Token: 0x0400013D RID: 317
		private readonly TWParallel.ParallelForWithDtAuxPredicate ParallelUpdateWidgetPredicate;

		// Token: 0x0400013E RID: 318
		private readonly TWParallel.ParallelForWithDtAuxPredicate UpdateBrushesWidgetPredicate;

		// Token: 0x0400013F RID: 319
		private readonly TWParallel.ParallelForWithDtAuxPredicate WidgetDoTweenPositionAuxPredicate;

		// Token: 0x04000140 RID: 320
		private float _lastSetFrictionValue = 1f;

		// Token: 0x04000143 RID: 323
		public Func<bool> OnGetIsHitThisFrame;

		// Token: 0x04000144 RID: 324
		public Func<Vector2, bool> OnGetIsBlockedAtPosition;

		// Token: 0x04000145 RID: 325
		public Func<int> OnGetLastScreenOrder;

		// Token: 0x04000146 RID: 326
		public Func<bool> OnGetIsAvailableForGamepadNavigation;
	}
}
