using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class CircularAutoScrollablePanelWidget : Widget
	{
		public CircularAutoScrollablePanelWidget(UIContext context)
			: base(context)
		{
			this.IdleTime = 0.8f;
			this.ScrollRatioPerSecond = 0.25f;
			this.ScrollPixelsPerSecond = 35f;
			this.ScrollType = CircularAutoScrollablePanelWidget.ScrollMovementType.ByPixels;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._autoScroll = this._autoScroll || (base.CurrentState == "Selected" && this.AutoScrollWhenSelected);
			this._maxScroll = 0f;
			Widget innerPanel = this.InnerPanel;
			if (innerPanel != null && innerPanel.Size.Y > 0f)
			{
				Widget clipRect = this.ClipRect;
				if (clipRect != null && clipRect.Size.Y > 0f && this.InnerPanel.Size.Y > this.ClipRect.Size.Y)
				{
					this._maxScroll = this.InnerPanel.Size.Y - this.ClipRect.Size.Y;
				}
			}
			if (this._autoScroll && !this._isIdle)
			{
				this.ScrollToDirection(this._direction, dt);
				if (this._currentScrollValue.ApproximatelyEqualsTo(0f, 1E-05f) || this._currentScrollValue.ApproximatelyEqualsTo(this._maxScroll, 1E-05f))
				{
					this._isIdle = true;
					this._idleTimer = 0f;
					this._direction *= -1;
					return;
				}
			}
			else if (this._autoScroll && this._isIdle)
			{
				this._idleTimer += dt;
				if (this._idleTimer > this.IdleTime)
				{
					this._isIdle = false;
					this._idleTimer = 0f;
					return;
				}
			}
			else if (this._currentScrollValue > 0f)
			{
				this.ScrollToDirection(-1, dt);
			}
		}

		private void ScrollToDirection(int direction, float dt)
		{
			float num = 0f;
			if (this.ScrollType == CircularAutoScrollablePanelWidget.ScrollMovementType.ByPixels)
			{
				num = this.ScrollPixelsPerSecond;
			}
			else if (this.ScrollType == CircularAutoScrollablePanelWidget.ScrollMovementType.ByRatio)
			{
				num = this.ScrollRatioPerSecond * this._maxScroll;
			}
			this._currentScrollValue += num * (float)direction * dt;
			this._currentScrollValue = MathF.Clamp(this._currentScrollValue, 0f, this._maxScroll);
			this.InnerPanel.ScaledPositionYOffset = -this._currentScrollValue;
		}

		protected override void OnMouseScroll()
		{
			base.OnMouseScroll();
			this._autoScroll = false;
			float num = ((this.ScrollPixelsPerSecond != 0f) ? (this.ScrollPixelsPerSecond * 0.2f) : 10f);
			float num2 = base.EventManager.DeltaMouseScroll * num;
			this._currentScrollValue += num2;
			this.InnerPanel.ScaledPositionYOffset = -this._currentScrollValue;
		}

		public void SetScrollMouse()
		{
			this.OnMouseScroll();
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			if (!this._autoScroll)
			{
				this._autoScroll = true;
				this._isIdle = false;
				this._direction = 1;
				this._idleTimer = 0f;
			}
		}

		public void SetHoverBegin()
		{
			this.OnHoverBegin();
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			if (this._autoScroll)
			{
				this._autoScroll = false;
				this._direction = -1;
				if (this._isIdle && this._currentScrollValue < 1E-45f)
				{
					this._currentScrollValue = 1f;
				}
			}
		}

		public void SetHoverEnd()
		{
			this.OnHoverEnd();
		}

		public Widget InnerPanel { get; set; }

		public Widget ClipRect { get; set; }

		public float ScrollRatioPerSecond { get; set; }

		public float ScrollPixelsPerSecond { get; set; }

		public float IdleTime { get; set; }

		public bool AutoScrollWhenSelected { get; set; }

		public CircularAutoScrollablePanelWidget.ScrollMovementType ScrollType { get; set; }

		private float _currentScrollValue;

		private bool _autoScroll;

		private bool _isIdle;

		private float _idleTimer;

		private int _direction = 1;

		private float _maxScroll;

		public enum ScrollMovementType
		{
			ByPixels,
			ByRatio
		}
	}
}
