using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Loading
{
	// Token: 0x0200010F RID: 271
	public class LoadingWindowWidget : Widget
	{
		// Token: 0x06000DCF RID: 3535 RVA: 0x00026B99 File Offset: 0x00024D99
		public LoadingWindowWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x00026BA4 File Offset: 0x00024DA4
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._edgeOffsetPadding = base.EventManager.PageSize.X / 2.72f;
			if (this.AnimWidget != null && base.IsVisible && this.AnimWidget.IsVisible)
			{
				this.AnimWidget.ScaledPositionXOffset = MathF.PingPong(this._edgeOffsetPadding, base.EventManager.PageSize.X - this.AnimWidget.Size.X - this._edgeOffsetPadding, this._totalDt);
				this._totalDt += dt * 500f;
			}
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x00026C49 File Offset: 0x00024E49
		private void UpdateStates()
		{
			base.IsVisible = this.IsActive;
			base.IsEnabled = this.IsActive;
			base.ParentWidget.IsVisible = this.IsActive;
			base.ParentWidget.IsEnabled = this.IsActive;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00026C88 File Offset: 0x00024E88
		private void UpdateImage(string imageName)
		{
			Sprite sprite = base.Context.SpriteData.GetSprite(imageName);
			if (sprite == null)
			{
				base.Sprite = base.Context.SpriteData.GetSprite("background_1");
				return;
			}
			base.Sprite = sprite;
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x00026CCD File Offset: 0x00024ECD
		// (set) Token: 0x06000DD4 RID: 3540 RVA: 0x00026CD5 File Offset: 0x00024ED5
		[Editor(false)]
		public Widget AnimWidget
		{
			get
			{
				return this._animWidget;
			}
			set
			{
				if (this._animWidget != value)
				{
					this._animWidget = value;
					base.OnPropertyChanged<Widget>(value, "AnimWidget");
				}
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x00026CF3 File Offset: 0x00024EF3
		// (set) Token: 0x06000DD6 RID: 3542 RVA: 0x00026CFB File Offset: 0x00024EFB
		[Editor(false)]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.UpdateStates();
				}
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06000DD7 RID: 3543 RVA: 0x00026D1F File Offset: 0x00024F1F
		// (set) Token: 0x06000DD8 RID: 3544 RVA: 0x00026D27 File Offset: 0x00024F27
		[Editor(false)]
		public string ImageName
		{
			get
			{
				return this._imageName;
			}
			set
			{
				if (this._imageName != value)
				{
					this._imageName = value;
					base.OnPropertyChanged<string>(value, "ImageName");
					this.UpdateImage(value);
				}
			}
		}

		// Token: 0x0400065B RID: 1627
		private const string _defaultBackgroundSpriteData = "background_1";

		// Token: 0x0400065C RID: 1628
		private float _edgeOffsetPadding;

		// Token: 0x0400065D RID: 1629
		private float _totalDt;

		// Token: 0x0400065E RID: 1630
		private Widget _animWidget;

		// Token: 0x0400065F RID: 1631
		private bool _isActive;

		// Token: 0x04000660 RID: 1632
		private string _imageName;
	}
}
