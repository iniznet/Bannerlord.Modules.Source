using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class TooltipWidget : Widget
	{
		public TooltipPositioningType PositioningType { get; set; }

		private float _tooltipOffset
		{
			get
			{
				return 30f;
			}
		}

		public TooltipWidget(UIContext context)
			: base(context)
		{
			base.HorizontalAlignment = HorizontalAlignment.Left;
			base.VerticalAlignment = VerticalAlignment.Top;
			this._lastCheckedVisibility = true;
			base.IsVisible = true;
			this.PositioningType = TooltipPositioningType.FixedMouseMirrored;
			this.ResetAnimationProperties();
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (this._lastCheckedVisibility != base.IsVisible)
			{
				this._lastCheckedVisibility = base.IsVisible;
				if (base.IsVisible)
				{
					this.ResetAnimationProperties();
				}
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._animationState == TooltipWidget.AnimationState.NotStarted)
			{
				if (this._animationDelayTimerInFrames >= this._animationDelayInFrames)
				{
					this._animationState = TooltipWidget.AnimationState.InProgress;
				}
				else
				{
					this._animationDelayTimerInFrames++;
					this.SetAlpha(0f);
				}
			}
			if (this._animationState != TooltipWidget.AnimationState.NotStarted)
			{
				if (this._animationState == TooltipWidget.AnimationState.InProgress)
				{
					this._animationProgress += ((this.AnimTime < 1E-05f) ? 1f : (dt / this.AnimTime));
					this.SetAlpha(this._animationProgress);
					if (this._animationProgress >= 1f)
					{
						this._animationState = TooltipWidget.AnimationState.Finished;
					}
				}
				this.UpdatePosition();
			}
		}

		private void SetAlpha(float alpha)
		{
			base.AlphaFactor = alpha;
			foreach (Widget widget in base.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(base.AlphaFactor);
			}
		}

		private void UpdatePosition()
		{
			if (this.PositioningType == TooltipPositioningType.FixedMouse || this.PositioningType == TooltipPositioningType.FixedMouseMirrored)
			{
				if (MathF.Abs(this._lastCheckedSize.X - base.Size.X) > 0.1f || MathF.Abs(this._lastCheckedSize.Y - base.Size.Y) > 0.1f)
				{
					this._lastCheckedSize = base.Size;
					if (this.PositioningType == TooltipPositioningType.FixedMouse)
					{
						this.SetPosition(base.EventManager.MousePosition);
						return;
					}
					this.SetMirroredPosition(base.EventManager.MousePosition);
					return;
				}
			}
			else
			{
				if (this.PositioningType == TooltipPositioningType.FollowMouse)
				{
					this.SetPosition(base.EventManager.MousePosition);
					return;
				}
				if (this.PositioningType == TooltipPositioningType.FollowMouseMirrored)
				{
					this.SetMirroredPosition(base.EventManager.MousePosition);
				}
			}
		}

		private void SetPosition(Vector2 position)
		{
			Vector2 vector = position + new Vector2(this._tooltipOffset, this._tooltipOffset);
			bool flag = base.Size.X > base.EventManager.PageSize.X;
			bool flag2 = base.Size.Y > base.EventManager.PageSize.Y;
			base.ScaledPositionXOffset = (flag ? vector.X : MathF.Clamp(vector.X, 0f, base.EventManager.PageSize.X - base.Size.X));
			base.ScaledPositionYOffset = (flag2 ? vector.Y : MathF.Clamp(vector.Y, 0f, base.EventManager.PageSize.Y - base.Size.Y));
		}

		private void SetMirroredPosition(Vector2 tooltipPosition)
		{
			float num = 0f;
			float num2 = 0f;
			HorizontalAlignment horizontalAlignment;
			if ((double)tooltipPosition.X < (double)base.EventManager.PageSize.X * 0.5)
			{
				horizontalAlignment = HorizontalAlignment.Left;
				num = this._tooltipOffset;
			}
			else
			{
				horizontalAlignment = HorizontalAlignment.Right;
				tooltipPosition = new Vector2(-(base.EventManager.PageSize.X - tooltipPosition.X), tooltipPosition.Y);
			}
			VerticalAlignment verticalAlignment;
			if ((double)tooltipPosition.Y < (double)base.EventManager.PageSize.Y * 0.5)
			{
				verticalAlignment = VerticalAlignment.Top;
				num2 = this._tooltipOffset;
			}
			else
			{
				verticalAlignment = VerticalAlignment.Bottom;
				tooltipPosition = new Vector2(tooltipPosition.X, -(base.EventManager.PageSize.Y - tooltipPosition.Y));
			}
			tooltipPosition += new Vector2(num, num2);
			if (base.Size.X > base.EventManager.PageSize.X)
			{
				horizontalAlignment = HorizontalAlignment.Left;
				tooltipPosition = new Vector2(0f, tooltipPosition.Y);
			}
			else
			{
				if (horizontalAlignment == HorizontalAlignment.Left && tooltipPosition.X + base.Size.X > base.EventManager.PageSize.X)
				{
					tooltipPosition += new Vector2(-(tooltipPosition.X + base.Size.X - base.EventManager.PageSize.X), 0f);
				}
				if (horizontalAlignment == HorizontalAlignment.Right && tooltipPosition.X - base.Size.X + base.EventManager.PageSize.X < 0f)
				{
					tooltipPosition += new Vector2(-(tooltipPosition.X - base.Size.X + base.EventManager.PageSize.X), 0f);
				}
			}
			if (base.Size.Y > base.EventManager.PageSize.Y)
			{
				verticalAlignment = VerticalAlignment.Top;
				tooltipPosition = new Vector2(tooltipPosition.X, 0f);
			}
			else
			{
				if (verticalAlignment == VerticalAlignment.Top && tooltipPosition.Y + base.Size.Y > base.EventManager.PageSize.Y)
				{
					tooltipPosition += new Vector2(0f, -(tooltipPosition.Y + base.Size.Y - base.EventManager.PageSize.Y));
				}
				if (verticalAlignment == VerticalAlignment.Bottom && tooltipPosition.Y - base.Size.Y + base.EventManager.PageSize.Y < 0f)
				{
					tooltipPosition += new Vector2(0f, -(tooltipPosition.Y - base.Size.Y + base.EventManager.PageSize.Y));
				}
			}
			base.HorizontalAlignment = horizontalAlignment;
			base.VerticalAlignment = verticalAlignment;
			base.ScaledPositionXOffset = tooltipPosition.X - base.EventManager.LeftUsableAreaStart;
			base.ScaledPositionYOffset = tooltipPosition.Y - base.EventManager.TopUsableAreaStart;
		}

		private void ResetAnimationProperties()
		{
			this._animationState = TooltipWidget.AnimationState.NotStarted;
			this._animationProgress = 0f;
			this._animationDelayTimerInFrames = 0;
		}

		[Editor(false)]
		public float AnimTime
		{
			get
			{
				return this._animTime;
			}
			set
			{
				if (this._animTime != value)
				{
					this._animTime = value;
					base.OnPropertyChanged(value, "AnimTime");
				}
			}
		}

		protected int _animationDelayInFrames;

		private int _animationDelayTimerInFrames;

		private TooltipWidget.AnimationState _animationState;

		private float _animationProgress;

		private bool _lastCheckedVisibility;

		private Vector2 _lastCheckedSize;

		private float _animTime = 0.2f;

		private enum AnimationState
		{
			NotStarted,
			InProgress,
			Finished
		}
	}
}
