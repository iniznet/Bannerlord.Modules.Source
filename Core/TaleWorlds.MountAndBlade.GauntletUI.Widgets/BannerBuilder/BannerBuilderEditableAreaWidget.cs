using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.BannerBuilder
{
	public class BannerBuilderEditableAreaWidget : Widget
	{
		public ButtonWidget DragWidgetTopRight { get; set; }

		public ButtonWidget DragWidgetRight { get; set; }

		public ButtonWidget DragWidgetTop { get; set; }

		public ButtonWidget RotateWidget { get; set; }

		public BannerTableauWidget BannerTableauWidget { get; set; }

		public Widget EditableAreaVisualWidget { get; set; }

		public int LayerIndex { get; set; }

		public bool IsMirrorActive { get; set; }

		public BannerBuilderEditableAreaWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.BannerTableauWidget.MeshIndexToUpdate = this.LayerIndex;
			if (!this._initialized)
			{
				this.Initialize();
			}
			this.UpdateRequiredValues();
			this.UpdateEditableAreaVisual();
			this.HandleCursor();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				BannerBuilderEditableAreaWidget.BuilderMode currentMode = this._currentMode;
				if (currentMode != BannerBuilderEditableAreaWidget.BuilderMode.None && currentMode - BannerBuilderEditableAreaWidget.BuilderMode.Rotating <= 4)
				{
					base.EventFired("RefreshBanner", Array.Empty<object>());
				}
			}
			this.HandleRotation();
			this.HandlePositioning();
			this.HandleForEdge(BannerBuilderEditableAreaWidget.EdgeResizeType.Right);
			this.HandleForEdge(BannerBuilderEditableAreaWidget.EdgeResizeType.Top);
			this.HandleForCorner();
			this._latestMousePosition = base.EventManager.MousePosition;
		}

		private void Initialize()
		{
			this._centerOfSigil = new Vec2(0f, 0f);
			this._sizeOfSigil = new Vec2(0f, 0f);
			this._initialized = true;
			this.OnIsLayerPatternChanged(false);
		}

		private void UpdateRequiredValues()
		{
			float num = this._positionValue.X / (float)this.TotalAreaSize * base.Size.X;
			float num2 = this._positionValue.Y / (float)this.TotalAreaSize * base.Size.Y;
			this._centerOfSigil.x = num;
			this._centerOfSigil.y = num2;
			float num3 = this._sizeValue.X / (float)this.TotalAreaSize * base.Size.X;
			float num4 = this._sizeValue.Y / (float)this.TotalAreaSize * base.Size.Y;
			this._sizeOfSigil.x = num3;
			this._sizeOfSigil.y = num4;
			this._positionLimitMin = 0f;
			this._positionLimitMax = this._positionLimitMin + (float)this.TotalAreaSize;
			this._sizeLimitMax = this.TotalAreaSize;
			this._areaScale = (float)this.TotalAreaSize / base.Size.X;
		}

		private void HandlePositioning()
		{
			if (Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.Positioning)
				{
					Vector2 vector = base.EventManager.MousePosition - this._latestMousePosition;
					vector *= (float)this.TotalAreaSize / base.Size.X;
					Vector2 vector2 = new Vector2(this.PositionValue.X, this.PositionValue.Y);
					vector2 += vector;
					vector2 = new Vector2(MathF.Clamp(vector2.X, this._positionLimitMin, this._positionLimitMax), MathF.Clamp(vector2.Y, this._positionLimitMin, this._positionLimitMax));
					this.PositionValue = vector2;
					this.BannerTableauWidget.UpdatePositionValueManual = this.PositionValue;
				}
				if (this._currentMode != BannerBuilderEditableAreaWidget.BuilderMode.Positioning && base.EventManager.HoveredView == this)
				{
					this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.Positioning;
					return;
				}
			}
			else
			{
				this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.None;
			}
		}

		private void HandleRotation()
		{
			if (Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.Rotating)
				{
					Vec2 vec = base.GlobalPosition + this._centerOfSigil;
					Vector2 vector = base.EventManager.MousePosition - new Vector2(vec.X, vec.y);
					vector.Y *= -1f;
					float num = BannerBuilderEditableAreaWidget.AngleFromDir(vector);
					this.RotationValue = (float)Math.Round((double)num, 3);
					this.BannerTableauWidget.UpdateRotationValueManualWithMirror = new ValueTuple<float, bool>(this.RotationValue, this.IsMirrorActive);
				}
				if (this._currentMode != BannerBuilderEditableAreaWidget.BuilderMode.Rotating && base.EventManager.HoveredView == this.RotateWidget)
				{
					this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.Rotating;
				}
			}
			else
			{
				this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.None;
			}
			this.UpdatePositionOfWidget(this.RotateWidget, BannerBuilderEditableAreaWidget.WidgetPlacementType.Vertical, this.RotationValue, 55f, 30f);
		}

		private void HandleForEdge(BannerBuilderEditableAreaWidget.EdgeResizeType resizeType)
		{
			ButtonWidget widgetFor = this.GetWidgetFor(resizeType);
			if (Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.HorizontalResizing || this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.VerticalResizing)
				{
					Vector2 vector = base.EventManager.MousePosition - this._resizeStartMousePosition;
					vector.Y *= -1f;
					Vec2 vec = BannerBuilderEditableAreaWidget.DirFromAngle(this.RotationValue);
					vec.y *= -1f;
					vector = BannerBuilderEditableAreaWidget.TransformToParent(vector, vec);
					vector.X *= -1f;
					vector.Y *= -1f;
					BannerBuilderEditableAreaWidget.BuilderMode currentMode = this._currentMode;
					if (currentMode != BannerBuilderEditableAreaWidget.BuilderMode.HorizontalResizing)
					{
						if (currentMode == BannerBuilderEditableAreaWidget.BuilderMode.VerticalResizing)
						{
							vector.X = 0f;
						}
					}
					else
					{
						vector.Y = 0f;
					}
					vector *= (float)this.TotalAreaSize / base.Size.X * 2f;
					Vec2 vec2 = new Vec2(this._resizeStartSize.X, this._resizeStartSize.Y);
					vec2 += vector;
					vec2 = new Vector2((float)((int)MathF.Clamp((float)((int)vec2.X), 2f, (float)this._sizeLimitMax)), (float)((int)MathF.Clamp((float)((int)vec2.Y), 2f, (float)this._sizeLimitMax)));
					Vec2 vec3 = this._resizeStartSize - vec2;
					if (vec3.x != 0f || vec3.y != 0f)
					{
						this.BannerTableauWidget.UpdateSizeValueManual = vec2;
						this.SizeValue = vec2;
					}
				}
				if ((this._currentMode != BannerBuilderEditableAreaWidget.BuilderMode.HorizontalResizing || this._currentMode != BannerBuilderEditableAreaWidget.BuilderMode.VerticalResizing) && base.EventManager.HoveredView == widgetFor)
				{
					this._resizeStartMousePosition = base.EventManager.MousePosition;
					this._resizeStartWidget = base.EventManager.HoveredView;
					this._resizeStartSize = this.SizeValue;
					this._currentMode = ((base.EventManager.HoveredView == this.GetWidgetFor(BannerBuilderEditableAreaWidget.EdgeResizeType.Right)) ? BannerBuilderEditableAreaWidget.BuilderMode.HorizontalResizing : BannerBuilderEditableAreaWidget.BuilderMode.VerticalResizing);
				}
			}
			else
			{
				this._resizeStartWidget = null;
				this._resizeStartSize = Vec2.Zero;
				this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.None;
			}
			this.UpdatePositionOfWidget(widgetFor, (resizeType == BannerBuilderEditableAreaWidget.EdgeResizeType.Right) ? BannerBuilderEditableAreaWidget.WidgetPlacementType.Horizontal : BannerBuilderEditableAreaWidget.WidgetPlacementType.Vertical, this.RotationValue + BannerBuilderEditableAreaWidget.AngleOffsetForEdge(resizeType), 15f, 0f);
		}

		private void HandleForCorner()
		{
			ButtonWidget dragWidgetTopRight = this.DragWidgetTopRight;
			if (Input.IsKeyDown(InputKey.LeftMouseButton))
			{
				bool flag = Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift);
				if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.RightCornerResizing)
				{
					Vector2 vector = base.EventManager.MousePosition - this._resizeStartMousePosition;
					vector.Y *= -1f;
					vector *= (float)this.TotalAreaSize / base.Size.X * 2f;
					Vec2 vec = BannerBuilderEditableAreaWidget.DirFromAngle(this.RotationValue);
					vec.y *= -1f;
					vector = BannerBuilderEditableAreaWidget.TransformToParent(vector, vec);
					vector.X *= -1f;
					vector.Y *= -1f;
					Vec2 vec2 = new Vec2(this._resizeStartSize.X, this._resizeStartSize.Y);
					if (flag)
					{
						Vector2 vector2 = new Vector2(this._centerOfSigil.X, this._centerOfSigil.Y) + base.GlobalPosition;
						float num = (vector2 - this._resizeStartMousePosition).Length();
						bool flag2 = (vector2 - base.EventManager.MousePosition).Length() < num;
						float num2 = vector.Length() * this._areaScale * (float)(flag2 ? (-1) : 1);
						float length = this._resizeStartSize.Length;
						float num3 = num2 / length;
						vec2 += num3 * vec2 * this._areaScale / 4f;
					}
					else
					{
						vec2 += vector;
					}
					vec2 = new Vector2((float)((int)MathF.Clamp((float)((int)vec2.X), 2f, (float)this._sizeLimitMax)), (float)((int)MathF.Clamp((float)((int)vec2.Y), 2f, (float)this._sizeLimitMax)));
					Vec2 vec3 = this._resizeStartSize - vec2;
					if (vec3.x != 0f || vec3.y != 0f)
					{
						this.BannerTableauWidget.UpdateSizeValueManual = vec2;
						this.SizeValue = vec2;
					}
				}
				if (this._currentMode != BannerBuilderEditableAreaWidget.BuilderMode.RightCornerResizing && base.EventManager.HoveredView == dragWidgetTopRight)
				{
					if (!flag || this._resizeStartWidget == null)
					{
						this._resizeStartMousePosition = base.EventManager.MousePosition;
						this._resizeStartWidget = base.EventManager.HoveredView;
						this._resizeStartSize = this.SizeValue;
					}
					this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.RightCornerResizing;
				}
			}
			else
			{
				this._resizeStartWidget = null;
				this._resizeStartSize = Vec2.Zero;
				this._currentMode = BannerBuilderEditableAreaWidget.BuilderMode.None;
			}
			this.UpdatePositionOfWidget(dragWidgetTopRight, BannerBuilderEditableAreaWidget.WidgetPlacementType.Max, this.RotationValue + BannerBuilderEditableAreaWidget.AngleOffsetForCorner(), 20f, 0f);
		}

		private void HandleCursor()
		{
			if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.Rotating || base.EventManager.HoveredView == this.RotateWidget)
			{
				base.Context.ActiveCursorOfContext = UIContext.MouseCursors.Rotate;
				return;
			}
			if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.Positioning || base.EventManager.HoveredView == this)
			{
				base.Context.ActiveCursorOfContext = UIContext.MouseCursors.Move;
				return;
			}
			if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.HorizontalResizing || base.EventManager.HoveredView == this.GetWidgetFor(BannerBuilderEditableAreaWidget.EdgeResizeType.Right))
			{
				base.Context.ActiveCursorOfContext = UIContext.MouseCursors.HorizontalResize;
				return;
			}
			if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.VerticalResizing || base.EventManager.HoveredView == this.GetWidgetFor(BannerBuilderEditableAreaWidget.EdgeResizeType.Top))
			{
				base.Context.ActiveCursorOfContext = UIContext.MouseCursors.VerticalResize;
				return;
			}
			if (this._currentMode == BannerBuilderEditableAreaWidget.BuilderMode.RightCornerResizing || base.EventManager.HoveredView == this.DragWidgetTopRight)
			{
				base.Context.ActiveCursorOfContext = UIContext.MouseCursors.DiagonalRightResize;
				return;
			}
			base.Context.ActiveCursorOfContext = UIContext.MouseCursors.Default;
		}

		private void UpdateEditableAreaVisual()
		{
			this.EditableAreaVisualWidget.HorizontalAlignment = HorizontalAlignment.Center;
			this.EditableAreaVisualWidget.VerticalAlignment = VerticalAlignment.Center;
			this.EditableAreaVisualWidget.WidthSizePolicy = SizePolicy.Fixed;
			this.EditableAreaVisualWidget.HeightSizePolicy = SizePolicy.Fixed;
			float num = (float)this.EditableAreaSize / (float)this.TotalAreaSize;
			this.EditableAreaVisualWidget.ScaledSuggestedWidth = base.Size.X * num;
			this.EditableAreaVisualWidget.ScaledSuggestedHeight = base.Size.Y * num;
		}

		private ButtonWidget GetWidgetFor(BannerBuilderEditableAreaWidget.EdgeResizeType edgeResizeType)
		{
			if (edgeResizeType != BannerBuilderEditableAreaWidget.EdgeResizeType.Top)
			{
				return this.DragWidgetRight;
			}
			return this.DragWidgetTop;
		}

		private void UpdatePositionOfWidget(Widget widget, BannerBuilderEditableAreaWidget.WidgetPlacementType placementType, float directionFromCenter, float distanceFromCenterModifier, float distanceFromEdgesModifier = 0f)
		{
			Vec2 vec = BannerBuilderEditableAreaWidget.DirFromAngle(directionFromCenter);
			vec.y *= -1f;
			float num = 0f;
			switch (placementType)
			{
			case BannerBuilderEditableAreaWidget.WidgetPlacementType.Horizontal:
				num = this._sizeOfSigil.X;
				break;
			case BannerBuilderEditableAreaWidget.WidgetPlacementType.Vertical:
				num = this._sizeOfSigil.Y;
				break;
			case BannerBuilderEditableAreaWidget.WidgetPlacementType.Max:
				num = this._sizeOfSigil.Length;
				break;
			}
			float num2 = (num * base._inverseScaleToUse + distanceFromCenterModifier) * 0.5f * base._scaleToUse;
			Vec2 vec2 = this._centerOfSigil + vec * num2;
			vec2.x -= widget.Size.X / 2f;
			vec2.y -= widget.Size.Y / 2f;
			this.ApplyPositionOffsetToWidget(widget, vec2, distanceFromEdgesModifier * base._scaleToUse);
		}

		private void ApplyPositionOffsetToWidget(Widget widget, Vec2 pos, float additionalModifier = 0f)
		{
			widget.ScaledPositionXOffset = MathF.Clamp(pos.x, 12f + additionalModifier, base.Size.X - (12f + additionalModifier));
			widget.ScaledPositionYOffset = MathF.Clamp(pos.y, 12f + additionalModifier, base.Size.Y - (12f + additionalModifier));
		}

		private void OnIsLayerPatternChanged(bool isLayerPattern)
		{
		}

		private void OnPositionChanged(Vec2 newPosition)
		{
		}

		private void OnSizeChanged(Vec2 newSize)
		{
		}

		private void OnRotationChanged(float newRotation)
		{
		}

		[Editor(false)]
		public bool IsLayerPattern
		{
			get
			{
				return this._isLayerPattern;
			}
			set
			{
				if (this._isLayerPattern != value)
				{
					this._isLayerPattern = value;
					base.OnPropertyChanged(value, "IsLayerPattern");
					this.OnIsLayerPatternChanged(value);
				}
			}
		}

		[Editor(false)]
		public Vec2 PositionValue
		{
			get
			{
				return this._positionValue;
			}
			set
			{
				if (this._positionValue != value)
				{
					this._positionValue = value;
					base.OnPropertyChanged(value, "PositionValue");
					this.OnPositionChanged(value);
				}
			}
		}

		[Editor(false)]
		public Vec2 SizeValue
		{
			get
			{
				return this._sizeValue;
			}
			set
			{
				if (this._sizeValue != value)
				{
					this._sizeValue = value;
					base.OnPropertyChanged(value, "SizeValue");
					this.OnSizeChanged(value);
				}
			}
		}

		[Editor(false)]
		public float RotationValue
		{
			get
			{
				return this._rotationValue;
			}
			set
			{
				if (this._rotationValue != value)
				{
					this._rotationValue = value;
					base.OnPropertyChanged(value, "RotationValue");
					this.OnRotationChanged(value);
				}
			}
		}

		[Editor(false)]
		public int EditableAreaSize
		{
			get
			{
				return this._editableAreaSize;
			}
			set
			{
				if (this._editableAreaSize != value)
				{
					this._editableAreaSize = value;
					base.OnPropertyChanged(value, "EditableAreaSize");
				}
			}
		}

		[Editor(false)]
		public int TotalAreaSize
		{
			get
			{
				return this._totalAreaSize;
			}
			set
			{
				if (this._totalAreaSize != value)
				{
					this._totalAreaSize = value;
					base.OnPropertyChanged(value, "TotalAreaSize");
				}
			}
		}

		private static Vec2 DirFromAngle(float angle)
		{
			float num = angle * 6.2831855f;
			return new Vec2(-MathF.Sin(num), MathF.Cos(num));
		}

		private static float AngleFromDir(Vec2 directionVector)
		{
			float num;
			if (directionVector.X < 0f)
			{
				num = (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f * -1f;
			}
			else
			{
				num = 360f - (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f;
			}
			return num / 360f;
		}

		private static float AngleOffsetForEdge(BannerBuilderEditableAreaWidget.EdgeResizeType edge)
		{
			return 1f - (float)edge * 0.25f;
		}

		private static float AngleOffsetForCorner()
		{
			return 0.875f;
		}

		private static Vec2 TransformToParent(Vec2 a, Vec2 b)
		{
			return new Vec2(b.Y * a.X + b.X * a.Y, -b.X * a.X + b.Y * a.Y);
		}

		private static Vector2 TransformToParent(Vector2 a, Vec2 b)
		{
			return new Vector2(b.Y * a.X + b.X * a.Y, -b.X * a.X + b.Y * a.Y);
		}

		private static Vector2 TransformToParent(Vec2 a, Vector2 b)
		{
			return new Vector2(b.Y * a.X + b.X * a.Y, -b.X * a.X + b.Y * a.Y);
		}

		private static Vector2 TransformToParent(Vector2 a, Vector2 b)
		{
			return new Vector2(b.Y * a.X + b.X * a.Y, -b.X * a.Y + b.Y * a.Y);
		}

		private bool _initialized;

		private Vec2 _centerOfSigil;

		private Vec2 _sizeOfSigil;

		private float _positionLimitMin;

		private float _positionLimitMax;

		private float _areaScale;

		private const int _sizeLimitMin = 2;

		private int _sizeLimitMax;

		private BannerBuilderEditableAreaWidget.BuilderMode _currentMode;

		private Vector2 _latestMousePosition;

		private Vector2 _resizeStartMousePosition;

		private Widget _resizeStartWidget;

		private Vec2 _resizeStartSize;

		private bool _isLayerPattern;

		private Vec2 _positionValue;

		private Vec2 _sizeValue;

		private float _rotationValue;

		private int _editableAreaSize;

		private int _totalAreaSize;

		private enum BuilderMode
		{
			None,
			Rotating,
			Positioning,
			HorizontalResizing,
			VerticalResizing,
			RightCornerResizing
		}

		private enum WidgetPlacementType
		{
			Horizontal,
			Vertical,
			Max
		}

		private enum EdgeResizeType
		{
			Top,
			Right
		}
	}
}
