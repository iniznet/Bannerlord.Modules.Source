using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000D RID: 13
	public class CircularAutoScrollablePanelWidget : Widget
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00003110 File Offset: 0x00001310
		public CircularAutoScrollablePanelWidget(UIContext context)
			: base(context)
		{
			this.IdleTime = 0.8f;
			this.ScrollRatioPerSecond = 0.25f;
			this.ScrollPixelsPerSecond = 35f;
			this.ScrollType = CircularAutoScrollablePanelWidget.ScrollMovementType.ByPixels;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003148 File Offset: 0x00001348
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

		// Token: 0x06000078 RID: 120 RVA: 0x000032E0 File Offset: 0x000014E0
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

		// Token: 0x06000079 RID: 121 RVA: 0x0000335C File Offset: 0x0000155C
		protected override void OnMouseScroll()
		{
			base.OnMouseScroll();
			this._autoScroll = false;
			float num = ((this.ScrollPixelsPerSecond != 0f) ? (this.ScrollPixelsPerSecond * 0.2f) : 10f);
			float num2 = base.EventManager.DeltaMouseScroll * num;
			this._currentScrollValue += num2;
			this.InnerPanel.ScaledPositionYOffset = -this._currentScrollValue;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000033C5 File Offset: 0x000015C5
		public void SetScrollMouse()
		{
			this.OnMouseScroll();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000033CD File Offset: 0x000015CD
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

		// Token: 0x0600007C RID: 124 RVA: 0x000033FD File Offset: 0x000015FD
		public void SetHoverBegin()
		{
			this.OnHoverBegin();
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003405 File Offset: 0x00001605
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

		// Token: 0x0600007E RID: 126 RVA: 0x00003443 File Offset: 0x00001643
		public void SetHoverEnd()
		{
			this.OnHoverEnd();
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600007F RID: 127 RVA: 0x0000344B File Offset: 0x0000164B
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00003453 File Offset: 0x00001653
		public Widget InnerPanel { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000345C File Offset: 0x0000165C
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00003464 File Offset: 0x00001664
		public Widget ClipRect { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000083 RID: 131 RVA: 0x0000346D File Offset: 0x0000166D
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00003475 File Offset: 0x00001675
		public float ScrollRatioPerSecond { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000085 RID: 133 RVA: 0x0000347E File Offset: 0x0000167E
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00003486 File Offset: 0x00001686
		public float ScrollPixelsPerSecond { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000087 RID: 135 RVA: 0x0000348F File Offset: 0x0000168F
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00003497 File Offset: 0x00001697
		public float IdleTime { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000089 RID: 137 RVA: 0x000034A0 File Offset: 0x000016A0
		// (set) Token: 0x0600008A RID: 138 RVA: 0x000034A8 File Offset: 0x000016A8
		public bool AutoScrollWhenSelected { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600008B RID: 139 RVA: 0x000034B1 File Offset: 0x000016B1
		// (set) Token: 0x0600008C RID: 140 RVA: 0x000034B9 File Offset: 0x000016B9
		public CircularAutoScrollablePanelWidget.ScrollMovementType ScrollType { get; set; }

		// Token: 0x04000037 RID: 55
		private float _currentScrollValue;

		// Token: 0x04000038 RID: 56
		private bool _autoScroll;

		// Token: 0x04000039 RID: 57
		private bool _isIdle;

		// Token: 0x0400003A RID: 58
		private float _idleTimer;

		// Token: 0x0400003B RID: 59
		private int _direction = 1;

		// Token: 0x0400003C RID: 60
		private float _maxScroll;

		// Token: 0x02000176 RID: 374
		public enum ScrollMovementType
		{
			// Token: 0x040008AB RID: 2219
			ByPixels,
			// Token: 0x040008AC RID: 2220
			ByRatio
		}
	}
}
