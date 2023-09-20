using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class Widget : PropertyOwnerObject
	{
		public float ColorFactor { get; set; } = 1f;

		public float AlphaFactor { get; set; } = 1f;

		public float ValueFactor { get; set; }

		public float SaturationFactor { get; set; }

		public float ExtendLeft { get; set; }

		public float ExtendRight { get; set; }

		public float ExtendTop { get; set; }

		public float ExtendBottom { get; set; }

		public bool VerticalFlip { get; set; }

		public bool HorizontalFlip { get; set; }

		public bool FrictionEnabled { get; set; }

		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (this._color != value)
				{
					this._color = value;
				}
			}
		}

		[Editor(false)]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (this._id != value)
				{
					this._id = value;
					base.OnPropertyChanged<string>(value, "Id");
				}
			}
		}

		public Vector2 LocalPosition { get; private set; }

		public Vector2 GlobalPosition
		{
			get
			{
				if (this.ParentWidget != null)
				{
					return this.LocalPosition + this.ParentWidget.GlobalPosition;
				}
				return this.LocalPosition;
			}
		}

		[Editor(false)]
		public bool DoNotUseCustomScaleAndChildren
		{
			get
			{
				return this._doNotUseCustomScaleAndChildren;
			}
			set
			{
				if (this._doNotUseCustomScaleAndChildren != value)
				{
					this._doNotUseCustomScaleAndChildren = value;
					base.OnPropertyChanged(value, "DoNotUseCustomScaleAndChildren");
					this.DoNotUseCustomScale = value;
					this._children.ForEach(delegate(Widget _)
					{
						_.DoNotUseCustomScaleAndChildren = value;
					});
				}
			}
		}

		public bool DoNotUseCustomScale { get; set; }

		protected float _scaleToUse
		{
			get
			{
				if (!this.DoNotUseCustomScale)
				{
					return this.Context.CustomScale;
				}
				return this.Context.Scale;
			}
		}

		protected float _inverseScaleToUse
		{
			get
			{
				if (!this.DoNotUseCustomScale)
				{
					return this.Context.CustomInverseScale;
				}
				return this.Context.InverseScale;
			}
		}

		public Vector2 Size { get; private set; }

		[Editor(false)]
		public float SuggestedWidth
		{
			get
			{
				return this._suggestedWidth;
			}
			set
			{
				if (this._suggestedWidth != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._suggestedWidth = value;
					base.OnPropertyChanged(value, "SuggestedWidth");
				}
			}
		}

		[Editor(false)]
		public float SuggestedHeight
		{
			get
			{
				return this._suggestedHeight;
			}
			set
			{
				if (this._suggestedHeight != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._suggestedHeight = value;
					base.OnPropertyChanged(value, "SuggestedHeight");
				}
			}
		}

		public float ScaledSuggestedWidth
		{
			get
			{
				return this._scaleToUse * this.SuggestedWidth;
			}
			set
			{
				this.SuggestedWidth = value * this._inverseScaleToUse;
			}
		}

		public float ScaledSuggestedHeight
		{
			get
			{
				return this._scaleToUse * this.SuggestedHeight;
			}
			set
			{
				this.SuggestedHeight = value * this._inverseScaleToUse;
			}
		}

		[Editor(false)]
		public bool TweenPosition
		{
			get
			{
				return this._tweenPosition;
			}
			set
			{
				if (this._tweenPosition != value)
				{
					bool tweenPosition = this._tweenPosition;
					this._tweenPosition = value;
					if (this.ConnectedToRoot && (!tweenPosition || !this._tweenPosition))
					{
						this.EventManager.OnWidgetTweenPositionChanged(this);
					}
				}
			}
		}

		[Editor(false)]
		public string HoveredCursorState
		{
			get
			{
				return this._hoveredCursorState;
			}
			set
			{
				if (this._hoveredCursorState != value)
				{
					string hoveredCursorState = this._hoveredCursorState;
					this._hoveredCursorState = value;
				}
			}
		}

		[Editor(false)]
		public bool AlternateClickEventHasSpecialEvent
		{
			get
			{
				return this._alternateClickEventHasSpecialEvent;
			}
			set
			{
				if (this._alternateClickEventHasSpecialEvent != value)
				{
					bool alternateClickEventHasSpecialEvent = this._alternateClickEventHasSpecialEvent;
					this._alternateClickEventHasSpecialEvent = value;
				}
			}
		}

		public Vector2 PosOffset
		{
			get
			{
				return this._positionOffset;
			}
			set
			{
				if (this._positionOffset != value)
				{
					this.SetLayoutDirty();
					this._positionOffset = value;
					base.OnPropertyChanged(value, "PosOffset");
				}
			}
		}

		public Vector2 ScaledPositionOffset
		{
			get
			{
				return this._positionOffset * this._scaleToUse;
			}
		}

		[Editor(false)]
		public float PositionXOffset
		{
			get
			{
				return this._positionOffset.X;
			}
			set
			{
				if (this._positionOffset.X != value)
				{
					this.SetLayoutDirty();
					this._positionOffset.X = value;
					base.OnPropertyChanged(value, "PositionXOffset");
				}
			}
		}

		[Editor(false)]
		public float PositionYOffset
		{
			get
			{
				return this._positionOffset.Y;
			}
			set
			{
				if (this._positionOffset.Y != value)
				{
					this.SetLayoutDirty();
					this._positionOffset.Y = value;
					base.OnPropertyChanged(value, "PositionYOffset");
				}
			}
		}

		public float ScaledPositionXOffset
		{
			get
			{
				return this._positionOffset.X * this._scaleToUse;
			}
			set
			{
				float num = value * this._inverseScaleToUse;
				if (num != this._positionOffset.X)
				{
					this.SetLayoutDirty();
					this._positionOffset.X = num;
				}
			}
		}

		public float ScaledPositionYOffset
		{
			get
			{
				return this._positionOffset.Y * this._scaleToUse;
			}
			set
			{
				float num = value * this._inverseScaleToUse;
				if (num != this._positionOffset.Y)
				{
					this.SetLayoutDirty();
					this._positionOffset.Y = num;
				}
			}
		}

		public Widget ParentWidget
		{
			get
			{
				return this._parent;
			}
			set
			{
				if (this.ParentWidget != value)
				{
					if (this._parent != null)
					{
						this._parent.OnChildRemoved(this);
						if (this.ConnectedToRoot)
						{
							this.EventManager.OnWidgetDisconnectedFromRoot(this);
						}
						this._parent._children.Remove(this);
						this._parent.OnAfterChildRemoved(this);
					}
					this._parent = value;
					if (this._parent != null)
					{
						this._parent._children.Add(this);
						if (this.ConnectedToRoot)
						{
							this.EventManager.OnWidgetConnectedToRoot(this);
						}
						this._parent.OnChildAdded(this);
					}
					this.SetMeasureAndLayoutDirty();
				}
			}
		}

		public EventManager EventManager
		{
			get
			{
				return this.Context.EventManager;
			}
		}

		public UIContext Context { get; private set; }

		public Vector2 MeasuredSize { get; private set; }

		[Editor(false)]
		public float MarginTop
		{
			get
			{
				return this._marginTop;
			}
			set
			{
				if (this._marginTop != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginTop = value;
					base.OnPropertyChanged(value, "MarginTop");
				}
			}
		}

		[Editor(false)]
		public float MarginLeft
		{
			get
			{
				return this._marginLeft;
			}
			set
			{
				if (this._marginLeft != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginLeft = value;
					base.OnPropertyChanged(value, "MarginLeft");
				}
			}
		}

		[Editor(false)]
		public float MarginBottom
		{
			get
			{
				return this._marginBottom;
			}
			set
			{
				if (this._marginBottom != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginBottom = value;
					base.OnPropertyChanged(value, "MarginBottom");
				}
			}
		}

		[Editor(false)]
		public float MarginRight
		{
			get
			{
				return this._marginRight;
			}
			set
			{
				if (this._marginRight != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginRight = value;
					base.OnPropertyChanged(value, "MarginRight");
				}
			}
		}

		public float ScaledMarginTop
		{
			get
			{
				return this._scaleToUse * this.MarginTop;
			}
		}

		public float ScaledMarginLeft
		{
			get
			{
				return this._scaleToUse * this.MarginLeft;
			}
		}

		public float ScaledMarginBottom
		{
			get
			{
				return this._scaleToUse * this.MarginBottom;
			}
		}

		public float ScaledMarginRight
		{
			get
			{
				return this._scaleToUse * this.MarginRight;
			}
		}

		[Editor(false)]
		public VerticalAlignment VerticalAlignment
		{
			get
			{
				return this._verticalAlignment;
			}
			set
			{
				if (this._verticalAlignment != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._verticalAlignment = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(VerticalAlignment), value), "VerticalAlignment");
				}
			}
		}

		[Editor(false)]
		public HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return this._horizontalAlignment;
			}
			set
			{
				if (this._horizontalAlignment != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._horizontalAlignment = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(HorizontalAlignment), value), "HorizontalAlignment");
				}
			}
		}

		public float Left
		{
			get
			{
				return this._topLeft.X;
			}
			private set
			{
				if (value != this._topLeft.X)
				{
					this.EventManager.SetPositionsDirty();
					this._topLeft.X = value;
				}
			}
		}

		public float Top
		{
			get
			{
				return this._topLeft.Y;
			}
			private set
			{
				if (value != this._topLeft.Y)
				{
					this.EventManager.SetPositionsDirty();
					this._topLeft.Y = value;
				}
			}
		}

		public float Right
		{
			get
			{
				return this._topLeft.X + this.Size.X;
			}
		}

		public float Bottom
		{
			get
			{
				return this._topLeft.Y + this.Size.Y;
			}
		}

		public int ChildCount
		{
			get
			{
				return this._children.Count;
			}
		}

		[Editor(false)]
		public bool ForcePixelPerfectRenderPlacement
		{
			get
			{
				return this._forcePixelPerfectRenderPlacement;
			}
			set
			{
				if (this._forcePixelPerfectRenderPlacement != value)
				{
					this._forcePixelPerfectRenderPlacement = value;
					base.OnPropertyChanged(value, "ForcePixelPerfectRenderPlacement");
				}
			}
		}

		public bool UseGlobalTimeForAnimation { get; set; }

		[Editor(false)]
		public SizePolicy WidthSizePolicy
		{
			get
			{
				return this._widthSizePolicy;
			}
			set
			{
				if (value != this._widthSizePolicy)
				{
					this.SetMeasureAndLayoutDirty();
					this._widthSizePolicy = value;
				}
			}
		}

		[Editor(false)]
		public SizePolicy HeightSizePolicy
		{
			get
			{
				return this._heightSizePolicy;
			}
			set
			{
				if (value != this._heightSizePolicy)
				{
					this.SetMeasureAndLayoutDirty();
					this._heightSizePolicy = value;
				}
			}
		}

		[Editor(false)]
		public bool AcceptDrag { get; set; }

		[Editor(false)]
		public bool AcceptDrop { get; set; }

		[Editor(false)]
		public bool HideOnDrag { get; set; } = true;

		[Editor(false)]
		public Widget DragWidget
		{
			get
			{
				return this._dragWidget;
			}
			set
			{
				if (this._dragWidget != value)
				{
					if (value != null)
					{
						this._dragWidget = value;
						this._dragWidget.IsVisible = false;
						return;
					}
					this._dragWidget = null;
				}
			}
		}

		[Editor(false)]
		public bool ClipContents
		{
			get
			{
				return this.ClipVerticalContent && this.ClipHorizontalContent;
			}
			set
			{
				this.ClipHorizontalContent = value;
				this.ClipVerticalContent = value;
			}
		}

		[Editor(false)]
		public bool ClipHorizontalContent { get; set; }

		[Editor(false)]
		public bool ClipVerticalContent { get; set; }

		[Editor(false)]
		public bool CircularClipEnabled { get; set; }

		[Editor(false)]
		public float CircularClipRadius { get; set; }

		[Editor(false)]
		public bool IsCircularClipRadiusHalfOfWidth { get; set; }

		[Editor(false)]
		public bool IsCircularClipRadiusHalfOfHeight { get; set; }

		[Editor(false)]
		public float CircularClipSmoothingRadius { get; set; }

		[Editor(false)]
		public float CircularClipXOffset { get; set; }

		[Editor(false)]
		public float CircularClipYOffset { get; set; }

		[Editor(false)]
		public bool RenderLate { get; set; }

		[Editor(false)]
		public bool DoNotRenderIfNotFullyInsideScissor { get; set; }

		public bool FixedWidth
		{
			get
			{
				return this.WidthSizePolicy == SizePolicy.Fixed;
			}
		}

		public bool FixedHeight
		{
			get
			{
				return this.HeightSizePolicy == SizePolicy.Fixed;
			}
		}

		public bool IsHovered
		{
			get
			{
				return this._isHovered;
			}
			private set
			{
				if (this._isHovered != value)
				{
					this._isHovered = value;
					this.RefreshState();
					base.OnPropertyChanged(value, "IsHovered");
				}
			}
		}

		[Editor(false)]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (this._isDisabled != value)
				{
					this._isDisabled = value;
					base.OnPropertyChanged(value, "IsDisabled");
					this.RefreshState();
				}
			}
		}

		[Editor(false)]
		public bool IsFocusable
		{
			get
			{
				return this._isFocusable;
			}
			set
			{
				if (this._isFocusable != value)
				{
					this._isFocusable = value;
					if (this.ConnectedToRoot)
					{
						base.OnPropertyChanged(value, "IsFocusable");
						this.RefreshState();
					}
				}
			}
		}

		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			private set
			{
				if (this._isFocused != value)
				{
					this._isFocused = value;
					this.RefreshState();
				}
			}
		}

		[Editor(false)]
		public bool IsEnabled
		{
			get
			{
				return !this.IsDisabled;
			}
			set
			{
				if (value == this.IsDisabled)
				{
					this.IsDisabled = !value;
					base.OnPropertyChanged(value, "IsEnabled");
				}
			}
		}

		[Editor(false)]
		public bool RestartAnimationFirstFrame
		{
			get
			{
				return this._restartAnimationFirstFrame;
			}
			set
			{
				if (this._restartAnimationFirstFrame != value)
				{
					this._restartAnimationFirstFrame = value;
				}
			}
		}

		[Editor(false)]
		public bool DoNotPassEventsToChildren
		{
			get
			{
				return this._doNotPassEventsToChildren;
			}
			set
			{
				if (this._doNotPassEventsToChildren != value)
				{
					this._doNotPassEventsToChildren = value;
					base.OnPropertyChanged(value, "DoNotPassEventsToChildren");
				}
			}
		}

		[Editor(false)]
		public bool DoNotAcceptEvents
		{
			get
			{
				return this._doNotAcceptEvents;
			}
			set
			{
				if (this._doNotAcceptEvents != value)
				{
					this._doNotAcceptEvents = value;
					base.OnPropertyChanged(value, "DoNotAcceptEvents");
				}
			}
		}

		[Editor(false)]
		public bool CanAcceptEvents
		{
			get
			{
				return !this.DoNotAcceptEvents;
			}
			set
			{
				this.DoNotAcceptEvents = !value;
			}
		}

		public bool IsPressed
		{
			get
			{
				return this._isPressed;
			}
			internal set
			{
				if (this._isPressed != value)
				{
					this._isPressed = value;
					this.RefreshState();
				}
			}
		}

		[Editor(false)]
		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (this._isHidden != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._isHidden = value;
					this.RefreshState();
					base.OnPropertyChanged(value, "IsHidden");
					base.OnPropertyChanged(!value, "IsVisible");
					if (this.OnVisibilityChanged != null)
					{
						this.OnVisibilityChanged(this);
					}
				}
			}
		}

		[Editor(false)]
		public bool IsVisible
		{
			get
			{
				return !this._isHidden;
			}
			set
			{
				if (value == this._isHidden)
				{
					this.IsHidden = !value;
				}
			}
		}

		[Editor(false)]
		public Sprite Sprite
		{
			get
			{
				return this._sprite;
			}
			set
			{
				if (value != this._sprite)
				{
					this._sprite = value;
				}
			}
		}

		[Editor(false)]
		public VisualDefinition VisualDefinition
		{
			get
			{
				return this._visualDefinition;
			}
			set
			{
				if (this._visualDefinition != value)
				{
					VisualDefinition visualDefinition = this._visualDefinition;
					this._visualDefinition = value;
					this._stateTimer = 0f;
					if (this.ConnectedToRoot && (visualDefinition == null || this._visualDefinition == null))
					{
						this.EventManager.OnWidgetVisualDefinitionChanged(this);
					}
				}
			}
		}

		public string CurrentState { get; protected set; } = "";

		[Editor(false)]
		public bool UpdateChildrenStates
		{
			get
			{
				return this._updateChildrenStates;
			}
			set
			{
				if (this._updateChildrenStates != value)
				{
					this._updateChildrenStates = value;
					base.OnPropertyChanged(value, "UpdateChildrenStates");
					if (value && this.ChildCount > 0)
					{
						this.SetState(this.CurrentState);
					}
				}
			}
		}

		public object Tag { get; set; }

		public ILayout LayoutImp { get; protected set; }

		[Editor(false)]
		public bool DropEventHandledManually { get; set; }

		internal WidgetInfo WidgetInfo { get; private set; }

		public IEnumerable<Widget> AllChildrenAndThis
		{
			get
			{
				yield return this;
				foreach (Widget widget in this._children)
				{
					foreach (Widget widget2 in widget.AllChildrenAndThis)
					{
						yield return widget2;
					}
					IEnumerator<Widget> enumerator2 = null;
				}
				List<Widget>.Enumerator enumerator = default(List<Widget>.Enumerator);
				yield break;
				yield break;
			}
		}

		public void ApplyActionOnAllChildren(Action<Widget> action)
		{
			foreach (Widget widget in this._children)
			{
				action(widget);
				widget.ApplyActionOnAllChildren(action);
			}
		}

		public IEnumerable<Widget> AllChildren
		{
			get
			{
				foreach (Widget widget in this._children)
				{
					yield return widget;
					foreach (Widget widget2 in widget.AllChildren)
					{
						yield return widget2;
					}
					IEnumerator<Widget> enumerator2 = null;
					widget = null;
				}
				List<Widget>.Enumerator enumerator = default(List<Widget>.Enumerator);
				yield break;
				yield break;
			}
		}

		public List<Widget> Children
		{
			get
			{
				return this._children;
			}
		}

		public IEnumerable<Widget> Parents
		{
			get
			{
				for (Widget parent = this.ParentWidget; parent != null; parent = parent.ParentWidget)
				{
					yield return parent;
				}
				yield break;
			}
		}

		internal bool ConnectedToRoot
		{
			get
			{
				return this.Id == "Root" || (this.ParentWidget != null && this.ParentWidget.ConnectedToRoot);
			}
		}

		internal int OnUpdateListIndex { get; set; } = -1;

		internal int OnLateUpdateListIndex { get; set; } = -1;

		internal int OnUpdateBrushesIndex { get; set; } = -1;

		internal int OnParallelUpdateListIndex { get; set; } = -1;

		internal int OnVisualDefinitionListIndex { get; set; } = -1;

		internal int OnTweenPositionListIndex { get; set; } = -1;

		[Editor(false)]
		public float MaxWidth
		{
			get
			{
				return this._maxWidth;
			}
			set
			{
				if (this._maxWidth != value)
				{
					this._maxWidth = value;
					this._gotMaxWidth = true;
					base.OnPropertyChanged(value, "MaxWidth");
				}
			}
		}

		[Editor(false)]
		public float MaxHeight
		{
			get
			{
				return this._maxHeight;
			}
			set
			{
				if (this._maxHeight != value)
				{
					this._maxHeight = value;
					this._gotMaxHeight = true;
					base.OnPropertyChanged(value, "MaxHeight");
				}
			}
		}

		[Editor(false)]
		public float MinWidth
		{
			get
			{
				return this._minWidth;
			}
			set
			{
				if (this._minWidth != value)
				{
					this._minWidth = value;
					this._gotMinWidth = true;
					base.OnPropertyChanged(value, "MinWidth");
				}
			}
		}

		[Editor(false)]
		public float MinHeight
		{
			get
			{
				return this._minHeight;
			}
			set
			{
				if (this._minHeight != value)
				{
					this._minHeight = value;
					this._gotMinHeight = true;
					base.OnPropertyChanged(value, "MinHeight");
				}
			}
		}

		public float ScaledMaxWidth
		{
			get
			{
				return this._scaleToUse * this._maxWidth;
			}
		}

		public float ScaledMaxHeight
		{
			get
			{
				return this._scaleToUse * this._maxHeight;
			}
		}

		public float ScaledMinWidth
		{
			get
			{
				return this._scaleToUse * this._minWidth;
			}
		}

		public float ScaledMinHeight
		{
			get
			{
				return this._scaleToUse * this._minHeight;
			}
		}

		public Widget(UIContext context)
		{
			this.DropEventHandledManually = true;
			this.LayoutImp = new DefaultLayout();
			this._children = new List<Widget>();
			this.Context = context;
			this._states = new List<string>();
			this.WidgetInfo = WidgetInfo.GetWidgetInfo(base.GetType());
			this.Sprite = null;
			this._stateTimer = 0f;
			this._currentVisualStateAnimationState = VisualStateAnimationState.None;
			this._isFocusable = false;
			this._seed = 0;
			this._components = new List<WidgetComponent>();
			this.AddState("Default");
			this.SetState("Default");
		}

		public T GetComponent<T>() where T : WidgetComponent
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				WidgetComponent widgetComponent = this._components[i];
				if (widgetComponent is T)
				{
					return (T)((object)widgetComponent);
				}
			}
			return default(T);
		}

		public void AddComponent(WidgetComponent component)
		{
			this._components.Add(component);
		}

		protected void SetMeasureAndLayoutDirty()
		{
			this.SetMeasureDirty();
			this.SetLayoutDirty();
		}

		protected void SetMeasureDirty()
		{
			this.EventManager.SetMeasureDirty();
		}

		protected void SetLayoutDirty()
		{
			this.EventManager.SetLayoutDirty();
		}

		public void AddState(string stateName)
		{
			if (!this._states.Contains(stateName))
			{
				this._states.Add(stateName);
			}
		}

		public bool ContainsState(string stateName)
		{
			return this._states.Contains(stateName);
		}

		public virtual void SetState(string stateName)
		{
			if (this.CurrentState != stateName)
			{
				this.CurrentState = stateName;
				this._stateTimer = 0f;
				if (this._currentVisualStateAnimationState != VisualStateAnimationState.None)
				{
					this._startVisualState = new VisualState("@StartState");
					this._startVisualState.FillFromWidget(this);
				}
				this._currentVisualStateAnimationState = VisualStateAnimationState.PlayingBasicTranisition;
			}
			if (this.UpdateChildrenStates)
			{
				for (int i = 0; i < this.ChildCount; i++)
				{
					Widget child = this.GetChild(i);
					if (!(child is ImageWidget) || !((ImageWidget)child).OverrideDefaultStateSwitchingEnabled)
					{
						child.SetState(this.CurrentState);
					}
				}
			}
		}

		public Widget FindChild(BindingPath path)
		{
			string firstNode = path.FirstNode;
			BindingPath subPath = path.SubPath;
			if (firstNode == "..")
			{
				return this.ParentWidget.FindChild(subPath);
			}
			if (firstNode == ".")
			{
				return this;
			}
			foreach (Widget widget in this._children)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == firstNode)
				{
					if (subPath == null)
					{
						return widget;
					}
					return widget.FindChild(subPath);
				}
			}
			return null;
		}

		public Widget FindChild(string singlePathNode)
		{
			if (singlePathNode == "..")
			{
				return this.ParentWidget;
			}
			if (singlePathNode == ".")
			{
				return this;
			}
			foreach (Widget widget in this._children)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == singlePathNode)
				{
					return widget;
				}
			}
			return null;
		}

		public Widget FindChild(WidgetSearchDelegate widgetSearchDelegate)
		{
			for (int i = 0; i < this._children.Count; i++)
			{
				Widget widget = this._children[i];
				if (widgetSearchDelegate(widget))
				{
					return widget;
				}
			}
			return null;
		}

		public Widget FindChild(string id, bool includeAllChildren = false)
		{
			IEnumerable<Widget> enumerable;
			if (!includeAllChildren)
			{
				IEnumerable<Widget> children = this._children;
				enumerable = children;
			}
			else
			{
				enumerable = this.AllChildren;
			}
			foreach (Widget widget in enumerable)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == id)
				{
					return widget;
				}
			}
			return null;
		}

		public void RemoveAllChildren()
		{
			while (this._children.Count > 0)
			{
				this._children[0].ParentWidget = null;
			}
		}

		private static float GetEaseOutBack(float t)
		{
			float num = 0.5f;
			float num2 = num + 1f;
			return 1f + num2 * MathF.Pow(t - 1f, 3f) + num * MathF.Pow(t - 1f, 2f);
		}

		internal void UpdateVisualDefinitions(float dt)
		{
			if (this.VisualDefinition != null && this._currentVisualStateAnimationState == VisualStateAnimationState.PlayingBasicTranisition)
			{
				if (this._startVisualState == null)
				{
					this._startVisualState = new VisualState("@StartState");
					this._startVisualState.FillFromWidget(this);
				}
				VisualState visualState = this.VisualDefinition.GetVisualState(this.CurrentState);
				if (visualState != null)
				{
					float num = (visualState.GotTransitionDuration ? visualState.TransitionDuration : this.VisualDefinition.TransitionDuration);
					float delayOnBegin = this.VisualDefinition.DelayOnBegin;
					if (this._stateTimer < num)
					{
						if (this._stateTimer >= delayOnBegin)
						{
							float num2 = (this._stateTimer - delayOnBegin) / (num - delayOnBegin);
							if (this.VisualDefinition.EaseIn)
							{
								num2 = Widget.GetEaseOutBack(num2);
							}
							this.PositionXOffset = (visualState.GotPositionXOffset ? Mathf.Lerp(this._startVisualState.PositionXOffset, visualState.PositionXOffset, num2) : this.PositionXOffset);
							this.PositionYOffset = (visualState.GotPositionYOffset ? Mathf.Lerp(this._startVisualState.PositionYOffset, visualState.PositionYOffset, num2) : this.PositionYOffset);
							this.SuggestedWidth = (visualState.GotSuggestedWidth ? Mathf.Lerp(this._startVisualState.SuggestedWidth, visualState.SuggestedWidth, num2) : this.SuggestedWidth);
							this.SuggestedHeight = (visualState.GotSuggestedHeight ? Mathf.Lerp(this._startVisualState.SuggestedHeight, visualState.SuggestedHeight, num2) : this.SuggestedHeight);
							this.MarginTop = (visualState.GotMarginTop ? Mathf.Lerp(this._startVisualState.MarginTop, visualState.MarginTop, num2) : this.MarginTop);
							this.MarginBottom = (visualState.GotMarginBottom ? Mathf.Lerp(this._startVisualState.MarginBottom, visualState.MarginBottom, num2) : this.MarginBottom);
							this.MarginLeft = (visualState.GotMarginLeft ? Mathf.Lerp(this._startVisualState.MarginLeft, visualState.MarginLeft, num2) : this.MarginLeft);
							this.MarginRight = (visualState.GotMarginRight ? Mathf.Lerp(this._startVisualState.MarginRight, visualState.MarginRight, num2) : this.MarginRight);
						}
					}
					else
					{
						this.PositionXOffset = (visualState.GotPositionXOffset ? visualState.PositionXOffset : this.PositionXOffset);
						this.PositionYOffset = (visualState.GotPositionYOffset ? visualState.PositionYOffset : this.PositionYOffset);
						this.SuggestedWidth = (visualState.GotSuggestedWidth ? visualState.SuggestedWidth : this.SuggestedWidth);
						this.SuggestedHeight = (visualState.GotSuggestedHeight ? visualState.SuggestedHeight : this.SuggestedHeight);
						this.MarginTop = (visualState.GotMarginTop ? visualState.MarginTop : this.MarginTop);
						this.MarginBottom = (visualState.GotMarginBottom ? visualState.MarginBottom : this.MarginBottom);
						this.MarginLeft = (visualState.GotMarginLeft ? visualState.MarginLeft : this.MarginLeft);
						this.MarginRight = (visualState.GotMarginRight ? visualState.MarginRight : this.MarginRight);
						this._startVisualState = visualState;
						this._currentVisualStateAnimationState = VisualStateAnimationState.None;
					}
				}
				else
				{
					this._currentVisualStateAnimationState = VisualStateAnimationState.None;
				}
			}
			this._stateTimer += dt;
		}

		internal void Update(float dt)
		{
			this.OnUpdate(dt);
		}

		internal void LateUpdate(float dt)
		{
			this.OnLateUpdate(dt);
		}

		internal void ParallelUpdate(float dt)
		{
			this.OnParallelUpdate(dt);
		}

		protected virtual void OnUpdate(float dt)
		{
		}

		protected virtual void OnParallelUpdate(float dt)
		{
		}

		protected virtual void OnLateUpdate(float dt)
		{
		}

		protected virtual void RefreshState()
		{
		}

		public virtual void UpdateAnimationPropertiesSubTask(float alphaFactor)
		{
			this.AlphaFactor = alphaFactor;
			foreach (Widget widget in this.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(alphaFactor);
			}
		}

		public void Measure(Vector2 measureSpec)
		{
			if (this.IsHidden)
			{
				this.MeasuredSize = Vector2.Zero;
				return;
			}
			this.OnMeasure(measureSpec);
		}

		private Vector2 ProcessSizeWithBoundaries(Vector2 input)
		{
			Vector2 vector = input;
			if (this._gotMinWidth && input.X < this.ScaledMinWidth)
			{
				vector.X = this.ScaledMinWidth;
			}
			if (this._gotMinHeight && input.Y < this.ScaledMinHeight)
			{
				vector.Y = this.ScaledMinHeight;
			}
			if (this._gotMaxWidth && input.X > this.ScaledMaxWidth)
			{
				vector.X = this.ScaledMaxWidth;
			}
			if (this._gotMaxHeight && input.Y > this.ScaledMaxHeight)
			{
				vector.Y = this.ScaledMaxHeight;
			}
			return vector;
		}

		private void OnMeasure(Vector2 measureSpec)
		{
			if (this.WidthSizePolicy == SizePolicy.Fixed)
			{
				measureSpec.X = this.ScaledSuggestedWidth;
			}
			else if (this.WidthSizePolicy == SizePolicy.StretchToParent)
			{
				measureSpec.X -= this.ScaledMarginLeft + this.ScaledMarginRight;
			}
			else
			{
				SizePolicy widthSizePolicy = this.WidthSizePolicy;
			}
			if (this.HeightSizePolicy == SizePolicy.Fixed)
			{
				measureSpec.Y = this.ScaledSuggestedHeight;
			}
			else if (this.HeightSizePolicy == SizePolicy.StretchToParent)
			{
				measureSpec.Y -= this.ScaledMarginTop + this.ScaledMarginBottom;
			}
			else
			{
				SizePolicy heightSizePolicy = this.HeightSizePolicy;
			}
			measureSpec = this.ProcessSizeWithBoundaries(measureSpec);
			Vector2 vector = this.MeasureChildren(measureSpec);
			Vector2 vector2 = new Vector2(0f, 0f);
			if (this.WidthSizePolicy == SizePolicy.Fixed)
			{
				vector2.X = this.ScaledSuggestedWidth;
			}
			else if (this.WidthSizePolicy == SizePolicy.CoverChildren)
			{
				vector2.X = vector.X;
			}
			else if (this.WidthSizePolicy == SizePolicy.StretchToParent)
			{
				vector2.X = measureSpec.X;
			}
			if (this.HeightSizePolicy == SizePolicy.Fixed)
			{
				vector2.Y = this.ScaledSuggestedHeight;
			}
			else if (this.HeightSizePolicy == SizePolicy.CoverChildren)
			{
				vector2.Y = vector.Y;
			}
			else if (this.HeightSizePolicy == SizePolicy.StretchToParent)
			{
				vector2.Y = measureSpec.Y;
			}
			vector2 = this.ProcessSizeWithBoundaries(vector2);
			this.MeasuredSize = vector2;
		}

		public bool CheckIsMyChildRecursive(Widget child)
		{
			for (Widget widget = ((child != null) ? child.ParentWidget : null); widget != null; widget = widget.ParentWidget)
			{
				if (widget == this)
				{
					return true;
				}
			}
			return false;
		}

		private Vector2 MeasureChildren(Vector2 measureSpec)
		{
			return this.LayoutImp.MeasureChildren(this, measureSpec, this.Context.SpriteData, this._scaleToUse);
		}

		public void AddChild(Widget widget)
		{
			widget.ParentWidget = this;
		}

		public void AddChildAtIndex(Widget widget, int index)
		{
			widget.ParentWidget = this;
			widget.SetSiblingIndex(index, false);
		}

		public void SwapChildren(Widget widget1, Widget widget2)
		{
			int num = this._children.IndexOf(widget1);
			int num2 = this._children.IndexOf(widget2);
			Widget widget3 = this._children[num];
			this._children[num] = this._children[num2];
			this._children[num2] = widget3;
		}

		protected virtual void OnChildAdded(Widget child)
		{
			this.EventFired("ItemAdd", new object[] { child });
			if (this.DoNotUseCustomScaleAndChildren)
			{
				child.DoNotUseCustomScaleAndChildren = true;
			}
			if (this.UpdateChildrenStates && (!(child is ImageWidget) || !((ImageWidget)child).OverrideDefaultStateSwitchingEnabled))
			{
				child.SetState(this.CurrentState);
			}
		}

		public void RemoveChild(Widget widget)
		{
			widget.ParentWidget = null;
		}

		public virtual void OnBeforeRemovedChild(Widget widget)
		{
			if (this.IsHovered)
			{
				this.EventFired("HoverEnd", Array.Empty<object>());
			}
			this.Children.ForEach(delegate(Widget c)
			{
				c.OnBeforeRemovedChild(widget);
			});
		}

		public bool HasChild(Widget widget)
		{
			return this._children.Contains(widget);
		}

		protected virtual void OnChildRemoved(Widget child)
		{
			this.EventFired("ItemRemove", new object[] { child });
		}

		protected virtual void OnAfterChildRemoved(Widget child)
		{
			this.EventFired("AfterItemRemove", new object[] { child });
		}

		public virtual void UpdateBrushes(float dt)
		{
		}

		public int GetChildIndex(Widget child)
		{
			return this._children.IndexOf(child);
		}

		public int GetVisibleChildIndex(Widget child)
		{
			int num = -1;
			List<Widget> list = this._children.Where((Widget c) => c.IsVisible).ToList<Widget>();
			if (list.Count > 0)
			{
				num = list.IndexOf(child);
			}
			return num;
		}

		public int GetFilterChildIndex(Widget child, Func<Widget, bool> childrenFilter)
		{
			int num = -1;
			List<Widget> list = this._children.Where((Widget c) => childrenFilter(c)).ToList<Widget>();
			if (list.Count > 0)
			{
				num = list.IndexOf(child);
			}
			return num;
		}

		public Widget GetChild(int i)
		{
			if (i < this._children.Count)
			{
				return this._children[i];
			}
			return null;
		}

		public void Layout(float left, float bottom, float right, float top)
		{
			if (this.IsVisible)
			{
				this.SetLayout(left, bottom, right, top);
				Vector2 scaledPositionOffset = this.ScaledPositionOffset;
				this.Left += scaledPositionOffset.X;
				this.Top += scaledPositionOffset.Y;
				this.OnLayout(this.Left, this.Bottom, this.Right, this.Top);
			}
		}

		private void SetLayout(float left, float bottom, float right, float top)
		{
			left += this.ScaledMarginLeft;
			right -= this.ScaledMarginRight;
			top += this.ScaledMarginTop;
			bottom -= this.ScaledMarginBottom;
			float num = right - left;
			float num2 = bottom - top;
			float num3;
			if (this.HorizontalAlignment == HorizontalAlignment.Left)
			{
				num3 = left;
			}
			else if (this.HorizontalAlignment == HorizontalAlignment.Center)
			{
				num3 = left + num / 2f - this.MeasuredSize.X / 2f;
			}
			else
			{
				num3 = right - this.MeasuredSize.X;
			}
			float num4;
			if (this.VerticalAlignment == VerticalAlignment.Top)
			{
				num4 = top;
			}
			else if (this.VerticalAlignment == VerticalAlignment.Center)
			{
				num4 = top + num2 / 2f - this.MeasuredSize.Y / 2f;
			}
			else
			{
				num4 = bottom - this.MeasuredSize.Y;
			}
			this.Left = num3;
			this.Top = num4;
			this.Size = this.MeasuredSize;
		}

		private void OnLayout(float left, float bottom, float right, float top)
		{
			this.LayoutImp.OnLayout(this, left, bottom, right, top);
		}

		internal void DoTweenPosition(float dt)
		{
			if (this.IsVisible && dt > 0f)
			{
				float num = this.Left - this.LocalPosition.X;
				float num2 = this.Top - this.LocalPosition.Y;
				if (Mathf.Abs(num) + Mathf.Abs(num2) < 0.003f)
				{
					this.LocalPosition = new Vector2(this.Left, this.Top);
					return;
				}
				num = Mathf.Clamp(num, -100f, 100f);
				num2 = Mathf.Clamp(num2, -100f, 100f);
				float num3 = Mathf.Min(dt * 18f, 1f);
				this.LocalPosition = new Vector2(this.LocalPosition.X + num3 * num, this.LocalPosition.Y + num3 * num2);
			}
		}

		internal void ParallelUpdateChildPositions(Vector2 globalPosition)
		{
			Widget.<>c__DisplayClass458_0 CS$<>8__locals1 = new Widget.<>c__DisplayClass458_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.globalPosition = globalPosition;
			TWParallel.For(0, this._children.Count, new TWParallel.ParallelForAuxPredicate(CS$<>8__locals1.<ParallelUpdateChildPositions>g__UpdateChildPositionMT|0), 16);
		}

		internal void UpdatePosition(Vector2 parentPosition)
		{
			if (this.IsVisible)
			{
				if (!this.TweenPosition)
				{
					this.LocalPosition = new Vector2(this.Left, this.Top);
				}
				Vector2 vector = this.LocalPosition + parentPosition;
				if (this._children.Count >= 64)
				{
					this.ParallelUpdateChildPositions(vector);
				}
				else
				{
					for (int i = 0; i < this._children.Count; i++)
					{
						this._children[i].UpdatePosition(vector);
					}
				}
				this._cachedGlobalPosition = vector;
			}
		}

		public virtual void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
		}

		public bool IsPointInsideMeasuredArea(Vector2 p)
		{
			Vector2 globalPosition = this.GlobalPosition;
			float num = p.X - globalPosition.X;
			float num2 = p.Y - globalPosition.Y;
			return 0f <= num && num <= this.Size.X && 0f <= num2 && num2 <= this.Size.Y;
		}

		public bool IsPointInsideGamepadCursorArea(Vector2 p)
		{
			Vector2 globalPosition = this.GlobalPosition;
			globalPosition.X -= this.ExtendCursorAreaLeft;
			globalPosition.Y -= this.ExtendCursorAreaTop;
			Vector2 size = this.Size;
			size.X += this.ExtendCursorAreaLeft + this.ExtendCursorAreaRight;
			size.Y += this.ExtendCursorAreaTop + this.ExtendCursorAreaBottom;
			return p.X >= globalPosition.X && p.Y > globalPosition.Y && p.X < globalPosition.X + size.X && p.Y < globalPosition.Y + size.Y;
		}

		public void Hide()
		{
			this.IsHidden = true;
		}

		public void Show()
		{
			this.IsHidden = false;
		}

		public Vector2 GetLocalPoint(Vector2 globalPoint)
		{
			return globalPoint - this.GlobalPosition;
		}

		public void SetSiblingIndex(int index, bool force = false)
		{
			int siblingIndex = this.GetSiblingIndex();
			if (siblingIndex != index || force)
			{
				this.ParentWidget._children.RemoveAt(siblingIndex);
				this.ParentWidget._children.Insert(index, this);
				this.SetMeasureAndLayoutDirty();
				this.EventFired("SiblingIndexChanged", Array.Empty<object>());
			}
		}

		public int GetSiblingIndex()
		{
			Widget parentWidget = this.ParentWidget;
			if (parentWidget == null)
			{
				return -1;
			}
			return parentWidget.GetChildIndex(this);
		}

		public int GetVisibleSiblingIndex()
		{
			return this.ParentWidget.GetVisibleChildIndex(this);
		}

		public bool DisableRender { get; set; }

		public void Render(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (!this.IsHidden && !this.DisableRender)
			{
				Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
				if (this.ClipHorizontalContent || this.ClipVerticalContent)
				{
					int num = (this.ClipHorizontalContent ? ((int)this.Size.X) : (-1));
					int num2 = (this.ClipVerticalContent ? ((int)this.Size.Y) : (-1));
					drawContext.PushScissor((int)cachedGlobalPosition.X, (int)cachedGlobalPosition.Y, num, num2);
				}
				if (this.CircularClipEnabled)
				{
					if (this.IsCircularClipRadiusHalfOfHeight)
					{
						this.CircularClipRadius = this.Size.Y / 2f * this._inverseScaleToUse;
					}
					else if (this.IsCircularClipRadiusHalfOfWidth)
					{
						this.CircularClipRadius = this.Size.X / 2f * this._inverseScaleToUse;
					}
					Vector2 vector = new Vector2(cachedGlobalPosition.X + this.Size.X * 0.5f + this.CircularClipXOffset * this._scaleToUse, cachedGlobalPosition.Y + this.Size.Y * 0.5f + this.CircularClipYOffset * this._scaleToUse);
					drawContext.SetCircualMask(vector, this.CircularClipRadius * this._scaleToUse, this.CircularClipSmoothingRadius * this._scaleToUse);
				}
				bool flag = false;
				if (drawContext.ScissorTestEnabled)
				{
					ScissorTestInfo currentScissor = drawContext.CurrentScissor;
					Rectangle rectangle = new Rectangle(cachedGlobalPosition.X, cachedGlobalPosition.Y, this.Size.X, this.Size.Y);
					Rectangle rectangle2 = new Rectangle((float)currentScissor.X, (float)currentScissor.Y, (float)currentScissor.Width, (float)currentScissor.Height);
					if (rectangle.IsCollide(rectangle2) || this._calculateSizeFirstFrame)
					{
						flag = !this.DoNotRenderIfNotFullyInsideScissor || rectangle.IsSubRectOf(rectangle2);
					}
				}
				else
				{
					Rectangle rectangle3 = new Rectangle(this._cachedGlobalPosition.X, this._cachedGlobalPosition.Y, this.MeasuredSize.X, this.MeasuredSize.Y);
					Rectangle rectangle4 = new Rectangle(this.EventManager.LeftUsableAreaStart, this.EventManager.TopUsableAreaStart, this.EventManager.PageSize.X, this.EventManager.PageSize.Y);
					if (rectangle3.IsCollide(rectangle4) || this._calculateSizeFirstFrame)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.OnRender(twoDimensionContext, drawContext);
					for (int i = 0; i < this._children.Count; i++)
					{
						Widget widget = this._children[i];
						if (!widget.RenderLate)
						{
							widget.Render(twoDimensionContext, drawContext);
						}
					}
					for (int j = 0; j < this._children.Count; j++)
					{
						Widget widget2 = this._children[j];
						if (widget2.RenderLate)
						{
							widget2.Render(twoDimensionContext, drawContext);
						}
					}
				}
				if (this.CircularClipEnabled)
				{
					drawContext.ClearCircualMask();
				}
				if (this.ClipHorizontalContent || this.ClipVerticalContent)
				{
					drawContext.PopScissor();
				}
			}
			this._calculateSizeFirstFrame = false;
		}

		protected virtual void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			Vector2 globalPosition = this.GlobalPosition;
			if (this.ForcePixelPerfectRenderPlacement)
			{
				globalPosition.X = (float)MathF.Round(globalPosition.X);
				globalPosition.Y = (float)MathF.Round(globalPosition.Y);
			}
			if (this._sprite != null)
			{
				Texture texture = this._sprite.Texture;
				if (texture != null)
				{
					float num = globalPosition.X;
					float num2 = globalPosition.Y;
					SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
					simpleMaterial.OverlayEnabled = false;
					simpleMaterial.CircularMaskingEnabled = false;
					simpleMaterial.Texture = texture;
					simpleMaterial.Color = this.Color;
					simpleMaterial.ColorFactor = this.ColorFactor;
					simpleMaterial.AlphaFactor = this.AlphaFactor * this.Context.ContextAlpha;
					simpleMaterial.HueFactor = 0f;
					simpleMaterial.SaturationFactor = this.SaturationFactor;
					simpleMaterial.ValueFactor = this.ValueFactor;
					float num3 = this.ExtendLeft;
					if (this.HorizontalFlip)
					{
						num3 = this.ExtendRight;
					}
					float num4 = this.Size.X;
					num4 += (this.ExtendRight + this.ExtendLeft) * this._scaleToUse;
					num -= num3 * this._scaleToUse;
					float num5 = this.Size.Y;
					float num6 = this.ExtendTop;
					if (this.HorizontalFlip)
					{
						num6 = this.ExtendBottom;
					}
					num5 += (this.ExtendTop + this.ExtendBottom) * this._scaleToUse;
					num2 -= num6 * this._scaleToUse;
					drawContext.DrawSprite(this._sprite, simpleMaterial, num, num2, this._scaleToUse, num4, num5, this.HorizontalFlip, this.VerticalFlip);
				}
			}
		}

		protected void EventFired(string eventName, params object[] args)
		{
			if (this._eventTargets != null)
			{
				for (int i = 0; i < this._eventTargets.Count; i++)
				{
					this._eventTargets[i](this, eventName, args);
				}
			}
		}

		public event Action<Widget, string, object[]> EventFire
		{
			add
			{
				if (this._eventTargets == null)
				{
					this._eventTargets = new List<Action<Widget, string, object[]>>();
				}
				this._eventTargets.Add(value);
			}
			remove
			{
				if (this._eventTargets != null)
				{
					this._eventTargets.Remove(value);
				}
			}
		}

		public event Action<Widget> OnVisibilityChanged;

		public bool IsRecursivelyVisible()
		{
			for (Widget widget = this; widget != null; widget = widget.ParentWidget)
			{
				if (!widget.IsVisible)
				{
					return false;
				}
			}
			return true;
		}

		internal void HandleOnDisconnectedFromRoot()
		{
			this.OnDisconnectedFromRoot();
			if (this.IsHovered)
			{
				this.EventFired("HoverEnd", Array.Empty<object>());
			}
		}

		internal void HandleOnConnectedToRoot()
		{
			if (!this._seedSet)
			{
				this._seed = this.GetSiblingIndex();
				this._seedSet = true;
			}
			this.OnConnectedToRoot();
		}

		protected virtual void OnDisconnectedFromRoot()
		{
		}

		protected virtual void OnConnectedToRoot()
		{
			this.EventFired("ConnectedToRoot", Array.Empty<object>());
		}

		public override string ToString()
		{
			return this.GetFullIDPath();
		}

		public float ExtendCursorAreaTop { get; set; }

		public float ExtendCursorAreaRight { get; set; }

		public float ExtendCursorAreaBottom { get; set; }

		public float ExtendCursorAreaLeft { get; set; }

		public float CursorAreaXOffset { get; set; }

		public float CursorAreaYOffset { get; set; }

		public bool DoNotAcceptNavigation
		{
			get
			{
				return this._doNotAcceptNavigation;
			}
			set
			{
				if (value != this._doNotAcceptNavigation)
				{
					this._doNotAcceptNavigation = value;
					this.EventManager.OnWidgetNavigationStatusChanged(this);
				}
			}
		}

		public bool IsUsingNavigation
		{
			get
			{
				return this._isUsingNavigation;
			}
			set
			{
				if (value != this._isUsingNavigation)
				{
					this._isUsingNavigation = value;
					base.OnPropertyChanged(value, "IsUsingNavigation");
				}
			}
		}

		public bool UseSiblingIndexForNavigation
		{
			get
			{
				return this._useSiblingIndexForNavigation;
			}
			set
			{
				if (value != this._useSiblingIndexForNavigation)
				{
					this._useSiblingIndexForNavigation = value;
					if (value)
					{
						this.GamepadNavigationIndex = this.GetSiblingIndex();
					}
				}
			}
		}

		public int GamepadNavigationIndex
		{
			get
			{
				return this._gamepadNavigationIndex;
			}
			set
			{
				if (value != this._gamepadNavigationIndex)
				{
					this._gamepadNavigationIndex = value;
					this.EventManager.OnWidgetNavigationIndexUpdated(this);
					this.OnGamepadNavigationIndexUpdated(value);
					base.OnPropertyChanged(value, "GamepadNavigationIndex");
				}
			}
		}

		public GamepadNavigationTypes UsedNavigationMovements
		{
			get
			{
				return this._usedNavigationMovements;
			}
			set
			{
				if (value != this._usedNavigationMovements)
				{
					this._usedNavigationMovements = value;
					this.EventManager.OnWidgetUsedNavigationMovementsUpdated(this);
				}
			}
		}

		protected virtual void OnGamepadNavigationIndexUpdated(int newIndex)
		{
		}

		public void OnGamepadNavigationFocusGain()
		{
			Action<Widget> onGamepadNavigationFocusGained = this.OnGamepadNavigationFocusGained;
			if (onGamepadNavigationFocusGained == null)
			{
				return;
			}
			onGamepadNavigationFocusGained(this);
		}

		internal bool PreviewEvent(GauntletEvent gauntletEvent)
		{
			bool flag = false;
			switch (gauntletEvent)
			{
			case GauntletEvent.MouseMove:
				flag = this.OnPreviewMouseMove();
				break;
			case GauntletEvent.MousePressed:
				flag = this.OnPreviewMousePressed();
				break;
			case GauntletEvent.MouseReleased:
				flag = this.OnPreviewMouseReleased();
				break;
			case GauntletEvent.MouseAlternatePressed:
				flag = this.OnPreviewMouseAlternatePressed();
				break;
			case GauntletEvent.MouseAlternateReleased:
				flag = this.OnPreviewMouseAlternateReleased();
				break;
			case GauntletEvent.DragHover:
				flag = this.OnPreviewDragHover();
				break;
			case GauntletEvent.DragBegin:
				flag = this.OnPreviewDragBegin();
				break;
			case GauntletEvent.DragEnd:
				flag = this.OnPreviewDragEnd();
				break;
			case GauntletEvent.Drop:
				flag = this.OnPreviewDrop();
				break;
			case GauntletEvent.MouseScroll:
				flag = this.OnPreviewMouseScroll();
				break;
			case GauntletEvent.RightStickMovement:
				flag = this.OnPreviewRightStickMovement();
				break;
			}
			return flag;
		}

		protected virtual bool OnPreviewMousePressed()
		{
			return true;
		}

		protected virtual bool OnPreviewMouseReleased()
		{
			return true;
		}

		protected virtual bool OnPreviewMouseAlternatePressed()
		{
			return true;
		}

		protected virtual bool OnPreviewMouseAlternateReleased()
		{
			return true;
		}

		protected virtual bool OnPreviewDragBegin()
		{
			return this.AcceptDrag;
		}

		protected virtual bool OnPreviewDragEnd()
		{
			return this.AcceptDrag;
		}

		protected virtual bool OnPreviewDrop()
		{
			return this.AcceptDrop;
		}

		protected virtual bool OnPreviewMouseScroll()
		{
			return false;
		}

		protected virtual bool OnPreviewRightStickMovement()
		{
			return false;
		}

		protected virtual bool OnPreviewMouseMove()
		{
			return true;
		}

		protected virtual bool OnPreviewDragHover()
		{
			return false;
		}

		protected internal virtual void OnMousePressed()
		{
			this.IsPressed = true;
			this.EventFired("MouseDown", Array.Empty<object>());
		}

		protected internal virtual void OnMouseReleased()
		{
			this.IsPressed = false;
			this.EventFired("MouseUp", Array.Empty<object>());
		}

		protected internal virtual void OnMouseAlternatePressed()
		{
			this.EventFired("MouseAlternateDown", Array.Empty<object>());
		}

		protected internal virtual void OnMouseAlternateReleased()
		{
			this.EventFired("MouseAlternateUp", Array.Empty<object>());
		}

		protected internal virtual void OnMouseMove()
		{
			this.EventFired("MouseMove", Array.Empty<object>());
		}

		protected internal virtual void OnHoverBegin()
		{
			this.IsHovered = true;
			this.EventFired("HoverBegin", Array.Empty<object>());
		}

		protected internal virtual void OnHoverEnd()
		{
			this.EventFired("HoverEnd", Array.Empty<object>());
			this.IsHovered = false;
		}

		protected internal virtual void OnDragBegin()
		{
			this.EventManager.BeginDragging(this);
			this.EventFired("DragBegin", Array.Empty<object>());
		}

		protected internal virtual void OnDragEnd()
		{
			this.EventFired("DragEnd", Array.Empty<object>());
		}

		protected internal virtual bool OnDrop()
		{
			if (this.AcceptDrop)
			{
				bool flag = true;
				if (this.AcceptDropHandler != null)
				{
					flag = this.AcceptDropHandler(this, this.EventManager.DraggedWidget);
				}
				if (flag)
				{
					Widget widget = this.EventManager.ReleaseDraggedWidget();
					int num = -1;
					if (!this.DropEventHandledManually)
					{
						widget.ParentWidget = this;
					}
					this.EventFired("Drop", new object[] { widget, num });
					return true;
				}
			}
			return false;
		}

		protected internal virtual void OnMouseScroll()
		{
			this.EventFired("MouseScroll", Array.Empty<object>());
		}

		protected internal virtual void OnRightStickMovement()
		{
		}

		protected internal virtual void OnDragHoverBegin()
		{
			this.EventFired("DragHoverBegin", Array.Empty<object>());
		}

		protected internal virtual void OnDragHoverEnd()
		{
			this.EventFired("DragHoverEnd", Array.Empty<object>());
		}

		protected internal virtual void OnGainFocus()
		{
			this.IsFocused = true;
			this.EventFired("FocusGained", Array.Empty<object>());
		}

		protected internal virtual void OnLoseFocus()
		{
			this.IsFocused = false;
			this.EventFired("FocusLost", Array.Empty<object>());
		}

		protected internal virtual void OnMouseOverBegin()
		{
			this.EventFired("MouseOverBegin", Array.Empty<object>());
		}

		protected internal virtual void OnMouseOverEnd()
		{
			this.EventFired("MouseOverEnd", Array.Empty<object>());
		}

		private Color _color = Color.White;

		private string _id;

		protected Vector2 _cachedGlobalPosition;

		private Widget _parent;

		private List<Widget> _children;

		private bool _doNotUseCustomScaleAndChildren;

		protected bool _calculateSizeFirstFrame = true;

		private float _suggestedWidth;

		private float _suggestedHeight;

		private bool _tweenPosition;

		private string _hoveredCursorState;

		private bool _alternateClickEventHasSpecialEvent;

		private Vector2 _positionOffset;

		private float _marginTop;

		private float _marginLeft;

		private float _marginBottom;

		private float _marginRight;

		private VerticalAlignment _verticalAlignment;

		private HorizontalAlignment _horizontalAlignment;

		private Vector2 _topLeft;

		private bool _forcePixelPerfectRenderPlacement;

		private SizePolicy _widthSizePolicy;

		private SizePolicy _heightSizePolicy;

		private Widget _dragWidget;

		private bool _isHovered;

		private bool _isDisabled;

		private bool _isFocusable;

		private bool _isFocused;

		private bool _restartAnimationFirstFrame;

		private bool _doNotPassEventsToChildren;

		private bool _doNotAcceptEvents;

		public Func<Widget, Widget, bool> AcceptDropHandler;

		private bool _isPressed;

		private bool _isHidden;

		private Sprite _sprite;

		private VisualDefinition _visualDefinition;

		private List<string> _states;

		protected float _stateTimer;

		protected VisualState _startVisualState;

		protected VisualStateAnimationState _currentVisualStateAnimationState;

		private bool _updateChildrenStates;

		protected int _seed;

		private bool _seedSet;

		private float _maxWidth;

		private float _maxHeight;

		private float _minWidth;

		private float _minHeight;

		private bool _gotMaxWidth;

		private bool _gotMaxHeight;

		private bool _gotMinWidth;

		private bool _gotMinHeight;

		private List<WidgetComponent> _components;

		private List<Action<Widget, string, object[]>> _eventTargets;

		private bool _doNotAcceptNavigation;

		private bool _isUsingNavigation;

		private bool _useSiblingIndexForNavigation;

		protected internal int _gamepadNavigationIndex = -1;

		private GamepadNavigationTypes _usedNavigationMovements;

		public Action<Widget> OnGamepadNavigationFocusGained;
	}
}
