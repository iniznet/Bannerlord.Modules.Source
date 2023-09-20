using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class UIContext
	{
		public UIContext.MouseCursors ActiveCursorOfContext { get; internal set; }

		public bool IsDynamicScaleEnabled { get; set; } = true;

		public float ScaleModifier { get; set; } = 1f;

		public float ContextAlpha { get; set; } = 1f;

		public float Scale { get; private set; }

		public float CustomScale { get; private set; }

		public float CustomInverseScale { get; private set; }

		public string CurrentLanugageCode { get; private set; }

		public Random UIRandom { get; private set; }

		public float InverseScale { get; private set; }

		public EventManager EventManager { get; private set; }

		public Widget Root
		{
			get
			{
				return this.EventManager.Root;
			}
		}

		public ResourceDepot ResourceDepot
		{
			get
			{
				return this.TwoDimensionContext.ResourceDepot;
			}
		}

		public TwoDimensionContext TwoDimensionContext { get; private set; }

		public IEnumerable<Brush> Brushes
		{
			get
			{
				return this.BrushFactory.Brushes;
			}
		}

		public Brush DefaultBrush
		{
			get
			{
				return this.BrushFactory.DefaultBrush;
			}
		}

		public SpriteData SpriteData { get; private set; }

		public BrushFactory BrushFactory { get; private set; }

		public FontFactory FontFactory { get; private set; }

		public UIContext(TwoDimensionContext twoDimensionContext, IInputContext inputContext, IInputService inputService, SpriteData spriteData, FontFactory fontFactory, BrushFactory brushFactory)
		{
			this.TwoDimensionContext = twoDimensionContext;
			this._inputContext = inputContext;
			this._inputService = inputService;
			this.SpriteData = spriteData;
			this.FontFactory = fontFactory;
			this.BrushFactory = brushFactory;
			this._initializedWithExistingResources = true;
			this.ReferenceHeight = twoDimensionContext.Platform.ReferenceHeight;
			this.InverseReferenceHeight = 1f / this.ReferenceHeight;
		}

		public UIContext(TwoDimensionContext twoDimensionContext, IInputContext inputContext, IInputService inputService)
		{
			this.TwoDimensionContext = twoDimensionContext;
			this._inputContext = inputContext;
			this._inputService = inputService;
			this._initializedWithExistingResources = false;
			this.ReferenceHeight = twoDimensionContext.Platform.ReferenceHeight;
			this.InverseReferenceHeight = 1f / this.ReferenceHeight;
		}

		public Brush GetBrush(string name)
		{
			return this.BrushFactory.GetBrush(name);
		}

		public void Initialize()
		{
			if (!this._initializedWithExistingResources)
			{
				this.SpriteData = new SpriteData("SpriteData");
				this.SpriteData.Load(this.ResourceDepot);
				this.FontFactory = new FontFactory(this.ResourceDepot);
				this.FontFactory.LoadAllFonts(this.SpriteData);
				this.BrushFactory = new BrushFactory(this.ResourceDepot, "Brushes", this.SpriteData, this.FontFactory);
				this.BrushFactory.Initialize();
			}
			this.EventManager = new EventManager(this);
			this.EventManager.InputService = this._inputService;
			this.EventManager.InputContext = this._inputContext;
			this.EventManager.UpdateMousePosition(this._inputContext.GetPointerPosition());
			Widget root = this.Root;
			root.WidthSizePolicy = SizePolicy.Fixed;
			root.HeightSizePolicy = SizePolicy.Fixed;
			root.SuggestedWidth = this.TwoDimensionContext.Width;
			root.SuggestedHeight = this.TwoDimensionContext.Height;
			this.UIRandom = new Random();
			this.UpdateScale();
		}

		private void UpdateScale()
		{
			float num;
			if (this.TwoDimensionContext != null)
			{
				num = this.TwoDimensionContext.Height * this.InverseReferenceHeight;
				float num2 = this.TwoDimensionContext.Width / this.TwoDimensionContext.Height;
				if (num2 < 1.7422223f)
				{
					float num3 = num2 / 1.7422223f;
					num *= num3;
				}
			}
			else
			{
				num = 1f;
			}
			if (this.Scale != num || this.CustomScale != this.Scale * this.ScaleModifier)
			{
				this.Scale = num;
				this.CustomScale = this.Scale * this.ScaleModifier;
				this.InverseScale = 1f / num;
				this.CustomInverseScale = 1f / this.CustomScale;
				this.EventManager.UpdateLayout();
			}
		}

		public void OnFinalize()
		{
			this.EventManager.OnFinalize();
		}

		public void Update(float dt)
		{
			this.ActiveCursorOfContext = UIContext.MouseCursors.Default;
			if (!this._initializedWithExistingResources)
			{
				this.BrushFactory.CheckForUpdates();
			}
			if (this.IsDynamicScaleEnabled)
			{
				this.UpdateScale();
			}
			Widget root = this.Root;
			root.SuggestedWidth = this.TwoDimensionContext.Width;
			root.SuggestedHeight = this.TwoDimensionContext.Height;
			this.EventManager.Update(dt);
		}

		public void LateUpdate(float dt)
		{
			Vector2 vector;
			vector..ctor(this.TwoDimensionContext.Width, this.TwoDimensionContext.Height);
			this.EventManager.CalculateCanvas(vector, dt);
			this.EventManager.LateUpdate(dt);
			this.EventManager.RecalculateCanvas();
			this.EventManager.UpdateBrushes(dt);
			this.EventManager.Render(this.TwoDimensionContext);
		}

		public void OnOnScreenkeyboardTextInputDone(string inputText)
		{
			EditableTextWidget editableTextWidget;
			if ((editableTextWidget = this.EventManager.FocusedWidget as EditableTextWidget) != null)
			{
				editableTextWidget.SetAllText(inputText);
			}
			this.EventManager.ClearFocus();
		}

		public void OnOnScreenKeyboardCanceled()
		{
			this.EventManager.ClearFocus();
		}

		public bool HitTest(Widget root, Vector2 position)
		{
			return EventManager.HitTest(root, position);
		}

		public bool HitTest(Widget root)
		{
			return EventManager.HitTest(root, this._inputContext.GetPointerPosition());
		}

		public bool FocusTest(Widget root)
		{
			return this.EventManager.FocusTest(root);
		}

		public void UpdateInput(InputType handleInputs)
		{
			if (this._inputService.MouseEnabled)
			{
				this.EventManager.UpdateMousePosition(this._inputContext.GetPointerPosition());
				if (handleInputs.HasAnyFlag(InputType.MouseButton))
				{
					this.EventManager.MouseMove();
					InputKey controllerClickKey = this._inputContext.GetControllerClickKey();
					if (this._inputContext.IsKeyPressed(InputKey.LeftMouseButton) || this._inputContext.IsKeyPressed(controllerClickKey))
					{
						this.EventManager.MouseDown();
					}
					if (this._inputContext.IsKeyReleased(InputKey.LeftMouseButton) || this._inputContext.IsKeyReleased(controllerClickKey))
					{
						this.EventManager.MouseUp();
					}
					if (this._inputContext.IsKeyPressed(InputKey.RightMouseButton))
					{
						this.EventManager.MouseAlternateDown();
					}
					if (this._inputContext.IsKeyReleased(InputKey.RightMouseButton))
					{
						this.EventManager.MouseAlternateUp();
					}
				}
				if (handleInputs.HasAnyFlag(InputType.MouseWheel))
				{
					this.EventManager.MouseScroll();
				}
				this.EventManager.RightStickMovement();
				this._previousFrameMouseEnabled = true;
				return;
			}
			if (this._previousFrameMouseEnabled)
			{
				this.EventManager.UpdateMousePosition(new Vector2(-5000f, -5000f));
				this.EventManager.MouseMove();
				this.EventManager.SetHoveredView(null);
				this._previousFrameMouseEnabled = false;
			}
		}

		public void DrawWidgetDebugInfo()
		{
			if (Input.IsKeyDown(InputKey.LeftShift) && Input.IsKeyPressed(InputKey.F))
			{
				this.IsDebugWidgetInformationFroze = !this.IsDebugWidgetInformationFroze;
				this._currentRootNode = new UIContext.DebugWidgetTreeNode(this.TwoDimensionContext, this.Root, 0);
			}
			if (this.IsDebugWidgetInformationFroze)
			{
				UIContext.DebugWidgetTreeNode currentRootNode = this._currentRootNode;
				if (currentRootNode == null)
				{
					return;
				}
				currentRootNode.DebugDraw();
			}
		}

		private readonly float ReferenceHeight;

		private readonly float InverseReferenceHeight;

		private const float ReferenceAspectRatio = 1.7777778f;

		private const float ReferenceAspectRatioCoeff = 0.98f;

		private IInputContext _inputContext;

		private IInputService _inputService;

		private bool _initializedWithExistingResources;

		private bool _previousFrameMouseEnabled;

		private bool IsDebugWidgetInformationFroze;

		private UIContext.DebugWidgetTreeNode _currentRootNode;

		public enum MouseCursors
		{
			System,
			Default,
			Attack,
			Move,
			HorizontalResize,
			VerticalResize,
			DiagonalRightResize,
			DiagonalLeftResize,
			Rotate,
			Custom,
			Disabled,
			RightClickLink
		}

		private class DebugWidgetTreeNode
		{
			private string ID
			{
				get
				{
					return string.Format("{0}.{1}.{2}", this._depth, this._current.GetSiblingIndex(), this._fullIDPath);
				}
			}

			public DebugWidgetTreeNode(TwoDimensionContext context, Widget current, int depth)
			{
				this._context = context;
				this._current = current;
				this._depth = depth;
				Widget current2 = this._current;
				this._fullIDPath = ((current2 != null) ? current2.GetFullIDPath() : null) ?? string.Empty;
				int num = this._fullIDPath.LastIndexOf('\\');
				if (num != -1)
				{
					this._displayedName = this._fullIDPath.Substring(num + 1);
				}
				if (string.IsNullOrEmpty(this._displayedName))
				{
					this._displayedName = this._current.Id;
				}
				this._children = new List<UIContext.DebugWidgetTreeNode>();
				this.AddChildren();
			}

			private void AddChildren()
			{
				foreach (Widget widget in this._current.Children)
				{
					if (widget.ParentWidget == this._current)
					{
						UIContext.DebugWidgetTreeNode debugWidgetTreeNode = new UIContext.DebugWidgetTreeNode(this._context, widget, this._depth + 1);
						this._children.Add(debugWidgetTreeNode);
					}
				}
			}

			public void DebugDraw()
			{
				if (this._context.DrawDebugTreeNode(this._displayedName + "###Root." + this.ID))
				{
					if (this._context.IsDebugItemHovered())
					{
						this.DrawArea();
					}
					this._context.DrawCheckbox("Show Area###Area." + this.ID, ref this._isShowingArea);
					if (this._isShowingArea)
					{
						this.DrawArea();
					}
					this.DrawProperties();
					this.DrawChildren();
					this._context.PopDebugTreeNode();
					return;
				}
				if (this._context.IsDebugItemHovered())
				{
					this.DrawArea();
				}
			}

			private void DrawProperties()
			{
				if (this._context.DrawDebugTreeNode("Properties###Properties." + this.ID))
				{
					this._context.DrawDebugText("General");
					string text = (string.IsNullOrEmpty(this._current.Id) ? "_No ID_" : this._current.Id);
					this._context.DrawDebugText("\tID: " + text);
					this._context.DrawDebugText("\tPath: " + this._current.GetFullIDPath());
					this._context.DrawDebugText(string.Format("\tVisible: {0}", this._current.IsVisible));
					this._context.DrawDebugText(string.Format("\tEnabled: {0}", this._current.IsEnabled));
					this._context.DrawDebugText("\nSize");
					this._context.DrawDebugText(string.Format("\tWidth Size Policy: {0}", this._current.WidthSizePolicy));
					this._context.DrawDebugText(string.Format("\tHeight Size Policy: {0}", this._current.HeightSizePolicy));
					this._context.DrawDebugText(string.Format("\tSize: {0}", this._current.Size));
					this._context.DrawDebugText("\nPosition");
					this._context.DrawDebugText(string.Format("\tGlobal Position: {0}", this._current.GlobalPosition));
					this._context.DrawDebugText(string.Format("\tLocal Position: {0}", this._current.LocalPosition));
					this._context.DrawDebugText(string.Format("\tPosition Offset: <{0}, {1}>", this._current.PositionXOffset, this._current.PositionYOffset));
					this._context.DrawDebugText("\nEvents");
					this._context.DrawDebugText("\tCurrent State: " + this._current.CurrentState);
					this._context.DrawDebugText(string.Format("\tCan Accept Events: {0}", this._current.CanAcceptEvents));
					this._context.DrawDebugText(string.Format("\tPasses Events To Children: {0}", !this._current.DoNotPassEventsToChildren));
					this._context.DrawDebugText("\nVisuals");
					BrushWidget brushWidget = this._current as BrushWidget;
					if (brushWidget != null)
					{
						this._context.DrawDebugText("\tBrush: " + brushWidget.Brush.Name);
					}
					TextWidget textWidget;
					RichTextWidget richTextWidget;
					if ((textWidget = this._current as TextWidget) != null)
					{
						this._context.DrawDebugText("\tText: " + textWidget.Text);
					}
					else if ((richTextWidget = this._current as RichTextWidget) != null)
					{
						this._context.DrawDebugText("\tText: " + richTextWidget.Text);
					}
					else if (brushWidget != null)
					{
						TwoDimensionContext context = this._context;
						string text2 = "\tSprite: ";
						BrushRenderer brushRenderer = brushWidget.BrushRenderer;
						string text3;
						if (brushRenderer == null)
						{
							text3 = null;
						}
						else
						{
							Style currentStyle = brushRenderer.CurrentStyle;
							if (currentStyle == null)
							{
								text3 = null;
							}
							else
							{
								StyleLayer layer = currentStyle.GetLayer(brushWidget.BrushRenderer.CurrentState);
								if (layer == null)
								{
									text3 = null;
								}
								else
								{
									Sprite sprite = layer.Sprite;
									text3 = ((sprite != null) ? sprite.Name : null);
								}
							}
						}
						context.DrawDebugText(text2 + (text3 ?? "None"));
						TwoDimensionContext context2 = this._context;
						string text4 = "\tColor: ";
						Brush brush = brushWidget.Brush;
						string text5;
						if (brush == null)
						{
							text5 = null;
						}
						else
						{
							BrushLayer layer2 = brush.GetLayer(brushWidget.CurrentState);
							text5 = ((layer2 != null) ? layer2.ToString() : null);
						}
						context2.DrawDebugText(text4 + text5);
					}
					else
					{
						TwoDimensionContext context3 = this._context;
						string text6 = "\tSprite: ";
						Sprite sprite2 = this._current.Sprite;
						context3.DrawDebugText(text6 + (((sprite2 != null) ? sprite2.Name : null) ?? "None"));
						this._context.DrawDebugText("\tColor: " + this._current.Color.ToString());
					}
					this._context.PopDebugTreeNode();
				}
			}

			private void DrawChildren()
			{
				if (this._children.Count > 0 && this._context.DrawDebugTreeNode("Children###Children." + this.ID))
				{
					foreach (UIContext.DebugWidgetTreeNode debugWidgetTreeNode in this._children)
					{
						debugWidgetTreeNode.DebugDraw();
					}
					this._context.PopDebugTreeNode();
				}
			}

			private void DrawArea()
			{
				float x = this._current.GlobalPosition.X;
				float y = this._current.GlobalPosition.Y;
				float num = this._current.GlobalPosition.X + this._current.Size.X;
				float num2 = this._current.GlobalPosition.Y + this._current.Size.Y;
				if (x == num || y == num2 || this._current.Size.X == 0f || this._current.Size.Y == 0f)
				{
					return;
				}
				float num3 = 2f;
				float num4 = num3 / 2f;
				float num5 = num3 / 2f;
				float num6 = num3 / 2f;
				float num7 = num3 / 2f;
				float num8 = num3 / 2f;
				float num9 = num3 / 2f;
				float num10 = num3 / 2f;
				float num11 = num3 / 2f;
			}

			private readonly TwoDimensionContext _context;

			private readonly Widget _current;

			private readonly List<UIContext.DebugWidgetTreeNode> _children;

			private readonly string _fullIDPath;

			private readonly string _displayedName;

			private readonly int _depth;

			private bool _isShowingArea;
		}
	}
}
