using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200002C RID: 44
	public class UIContext
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000E081 File Offset: 0x0000C281
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x0000E089 File Offset: 0x0000C289
		public UIContext.MouseCursors ActiveCursorOfContext { get; internal set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000E092 File Offset: 0x0000C292
		// (set) Token: 0x060002FA RID: 762 RVA: 0x0000E09A File Offset: 0x0000C29A
		public bool IsDynamicScaleEnabled { get; set; } = true;

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0000E0A3 File Offset: 0x0000C2A3
		// (set) Token: 0x060002FC RID: 764 RVA: 0x0000E0AB File Offset: 0x0000C2AB
		public float ScaleModifier { get; set; } = 1f;

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0000E0B4 File Offset: 0x0000C2B4
		// (set) Token: 0x060002FE RID: 766 RVA: 0x0000E0BC File Offset: 0x0000C2BC
		public float ContextAlpha { get; set; } = 1f;

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000E0C5 File Offset: 0x0000C2C5
		// (set) Token: 0x06000300 RID: 768 RVA: 0x0000E0CD File Offset: 0x0000C2CD
		public float Scale { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0000E0D6 File Offset: 0x0000C2D6
		// (set) Token: 0x06000302 RID: 770 RVA: 0x0000E0DE File Offset: 0x0000C2DE
		public float CustomScale { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0000E0E7 File Offset: 0x0000C2E7
		// (set) Token: 0x06000304 RID: 772 RVA: 0x0000E0EF File Offset: 0x0000C2EF
		public float CustomInverseScale { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000E0F8 File Offset: 0x0000C2F8
		// (set) Token: 0x06000306 RID: 774 RVA: 0x0000E100 File Offset: 0x0000C300
		public string CurrentLanugageCode { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000E109 File Offset: 0x0000C309
		// (set) Token: 0x06000308 RID: 776 RVA: 0x0000E111 File Offset: 0x0000C311
		public Random UIRandom { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000E11A File Offset: 0x0000C31A
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000E122 File Offset: 0x0000C322
		public float InverseScale { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000E12B File Offset: 0x0000C32B
		// (set) Token: 0x0600030C RID: 780 RVA: 0x0000E133 File Offset: 0x0000C333
		public EventManager EventManager { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000E13C File Offset: 0x0000C33C
		public Widget Root
		{
			get
			{
				return this.EventManager.Root;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0000E149 File Offset: 0x0000C349
		public ResourceDepot ResourceDepot
		{
			get
			{
				return this.TwoDimensionContext.ResourceDepot;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000E156 File Offset: 0x0000C356
		// (set) Token: 0x06000310 RID: 784 RVA: 0x0000E15E File Offset: 0x0000C35E
		public TwoDimensionContext TwoDimensionContext { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000E167 File Offset: 0x0000C367
		public IEnumerable<Brush> Brushes
		{
			get
			{
				return this.BrushFactory.Brushes;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0000E174 File Offset: 0x0000C374
		public Brush DefaultBrush
		{
			get
			{
				return this.BrushFactory.DefaultBrush;
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000E181 File Offset: 0x0000C381
		// (set) Token: 0x06000314 RID: 788 RVA: 0x0000E189 File Offset: 0x0000C389
		public SpriteData SpriteData { get; private set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000E192 File Offset: 0x0000C392
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0000E19A File Offset: 0x0000C39A
		public BrushFactory BrushFactory { get; private set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000E1A3 File Offset: 0x0000C3A3
		// (set) Token: 0x06000318 RID: 792 RVA: 0x0000E1AB File Offset: 0x0000C3AB
		public FontFactory FontFactory { get; private set; }

		// Token: 0x06000319 RID: 793 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
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

		// Token: 0x0600031A RID: 794 RVA: 0x0000E23C File Offset: 0x0000C43C
		public UIContext(TwoDimensionContext twoDimensionContext, IInputContext inputContext, IInputService inputService)
		{
			this.TwoDimensionContext = twoDimensionContext;
			this._inputContext = inputContext;
			this._inputService = inputService;
			this._initializedWithExistingResources = false;
			this.ReferenceHeight = twoDimensionContext.Platform.ReferenceHeight;
			this.InverseReferenceHeight = 1f / this.ReferenceHeight;
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000E2AB File Offset: 0x0000C4AB
		public Brush GetBrush(string name)
		{
			return this.BrushFactory.GetBrush(name);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000E2BC File Offset: 0x0000C4BC
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

		// Token: 0x0600031D RID: 797 RVA: 0x0000E3CC File Offset: 0x0000C5CC
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

		// Token: 0x0600031E RID: 798 RVA: 0x0000E490 File Offset: 0x0000C690
		public void OnFinalize()
		{
			this.EventManager.OnFinalize();
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0000E4A0 File Offset: 0x0000C6A0
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

		// Token: 0x06000320 RID: 800 RVA: 0x0000E508 File Offset: 0x0000C708
		public void LateUpdate(float dt)
		{
			Vector2 vector = new Vector2(this.TwoDimensionContext.Width, this.TwoDimensionContext.Height);
			this.EventManager.CalculateCanvas(vector, dt);
			this.EventManager.LateUpdate(dt);
			this.EventManager.RecalculateCanvas();
			this.EventManager.UpdateBrushes(dt);
			this.EventManager.Render(this.TwoDimensionContext);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0000E574 File Offset: 0x0000C774
		public void OnOnScreenkeyboardTextInputDone(string inputText)
		{
			EditableTextWidget editableTextWidget;
			if ((editableTextWidget = this.EventManager.FocusedWidget as EditableTextWidget) != null)
			{
				editableTextWidget.SetAllText(inputText);
			}
			this.EventManager.ClearFocus();
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000E5A7 File Offset: 0x0000C7A7
		public void OnOnScreenKeyboardCanceled()
		{
			this.EventManager.ClearFocus();
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000E5B4 File Offset: 0x0000C7B4
		public bool HitTest(Widget root, Vector2 position)
		{
			return EventManager.HitTest(root, position);
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000E5BD File Offset: 0x0000C7BD
		public bool HitTest(Widget root)
		{
			return EventManager.HitTest(root, this._inputContext.GetPointerPosition());
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000E5D0 File Offset: 0x0000C7D0
		public bool FocusTest(Widget root)
		{
			return this.EventManager.FocusTest(root);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000E5E0 File Offset: 0x0000C7E0
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

		// Token: 0x06000327 RID: 807 RVA: 0x0000E730 File Offset: 0x0000C930
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

		// Token: 0x04000180 RID: 384
		private readonly float ReferenceHeight;

		// Token: 0x04000181 RID: 385
		private readonly float InverseReferenceHeight;

		// Token: 0x04000182 RID: 386
		private const float ReferenceAspectRatio = 1.7777778f;

		// Token: 0x04000183 RID: 387
		private const float ReferenceAspectRatioCoeff = 0.98f;

		// Token: 0x0400018F RID: 399
		private IInputContext _inputContext;

		// Token: 0x04000190 RID: 400
		private IInputService _inputService;

		// Token: 0x04000194 RID: 404
		private bool _initializedWithExistingResources;

		// Token: 0x04000195 RID: 405
		private bool _previousFrameMouseEnabled;

		// Token: 0x04000196 RID: 406
		private bool IsDebugWidgetInformationFroze;

		// Token: 0x04000197 RID: 407
		private UIContext.DebugWidgetTreeNode _currentRootNode;

		// Token: 0x0200007A RID: 122
		public enum MouseCursors
		{
			// Token: 0x0400041A RID: 1050
			System,
			// Token: 0x0400041B RID: 1051
			Default,
			// Token: 0x0400041C RID: 1052
			Attack,
			// Token: 0x0400041D RID: 1053
			Move,
			// Token: 0x0400041E RID: 1054
			HorizontalResize,
			// Token: 0x0400041F RID: 1055
			VerticalResize,
			// Token: 0x04000420 RID: 1056
			DiagonalRightResize,
			// Token: 0x04000421 RID: 1057
			DiagonalLeftResize,
			// Token: 0x04000422 RID: 1058
			Rotate,
			// Token: 0x04000423 RID: 1059
			Custom,
			// Token: 0x04000424 RID: 1060
			Disabled,
			// Token: 0x04000425 RID: 1061
			RightClickLink
		}

		// Token: 0x0200007B RID: 123
		private class DebugWidgetTreeNode
		{
			// Token: 0x1700028E RID: 654
			// (get) Token: 0x06000889 RID: 2185 RVA: 0x000222CB File Offset: 0x000204CB
			private string ID
			{
				get
				{
					return string.Format("{0}.{1}.{2}", this._depth, this._current.GetSiblingIndex(), this._fullIDPath);
				}
			}

			// Token: 0x0600088A RID: 2186 RVA: 0x000222F8 File Offset: 0x000204F8
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

			// Token: 0x0600088B RID: 2187 RVA: 0x00022398 File Offset: 0x00020598
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

			// Token: 0x0600088C RID: 2188 RVA: 0x00022418 File Offset: 0x00020618
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

			// Token: 0x0600088D RID: 2189 RVA: 0x000224B8 File Offset: 0x000206B8
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

			// Token: 0x0600088E RID: 2190 RVA: 0x000228D4 File Offset: 0x00020AD4
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

			// Token: 0x0600088F RID: 2191 RVA: 0x0002295C File Offset: 0x00020B5C
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

			// Token: 0x04000426 RID: 1062
			private readonly TwoDimensionContext _context;

			// Token: 0x04000427 RID: 1063
			private readonly Widget _current;

			// Token: 0x04000428 RID: 1064
			private readonly List<UIContext.DebugWidgetTreeNode> _children;

			// Token: 0x04000429 RID: 1065
			private readonly string _fullIDPath;

			// Token: 0x0400042A RID: 1066
			private readonly string _displayedName;

			// Token: 0x0400042B RID: 1067
			private readonly int _depth;

			// Token: 0x0400042C RID: 1068
			private bool _isShowingArea;
		}
	}
}
