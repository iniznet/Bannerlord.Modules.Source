using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x0200010B RID: 267
	public class MapBarUnreadBrushWidget : BrushWidget
	{
		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x00026668 File Offset: 0x00024868
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x00026670 File Offset: 0x00024870
		public bool IsBannerNotification { get; set; }

		// Token: 0x06000DB2 RID: 3506 RVA: 0x00026679 File Offset: 0x00024879
		public MapBarUnreadBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x00026684 File Offset: 0x00024884
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible && this._animState == MapBarUnreadBrushWidget.AnimState.Idle)
			{
				this._animState = MapBarUnreadBrushWidget.AnimState.Start;
			}
			if (this._animState == MapBarUnreadBrushWidget.AnimState.Start)
			{
				this._animState = MapBarUnreadBrushWidget.AnimState.FirstFrame;
			}
			else if (this._animState == MapBarUnreadBrushWidget.AnimState.FirstFrame)
			{
				if (base.BrushRenderer.Brush == null)
				{
					this._animState = MapBarUnreadBrushWidget.AnimState.Start;
				}
				else
				{
					this._animState = MapBarUnreadBrushWidget.AnimState.Playing;
					base.BrushRenderer.RestartAnimation();
				}
			}
			if (this.IsBannerNotification && base.IsVisible && this._animState == MapBarUnreadBrushWidget.AnimState.Idle)
			{
				this._animState = MapBarUnreadBrushWidget.AnimState.Start;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06000DB4 RID: 3508 RVA: 0x00026711 File Offset: 0x00024911
		// (set) Token: 0x06000DB5 RID: 3509 RVA: 0x00026719 File Offset: 0x00024919
		[Editor(false)]
		public TextWidget UnreadTextWidget
		{
			get
			{
				return this._unreadTextWidget;
			}
			set
			{
				if (this._unreadTextWidget != value)
				{
					this._unreadTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "UnreadTextWidget");
					if (value != null)
					{
						value.boolPropertyChanged += this.UnreadTextWidgetOnPropertyChanged;
					}
				}
			}
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x0002674C File Offset: 0x0002494C
		private void UnreadTextWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsVisible")
			{
				base.IsVisible = propertyValue;
				this._animState = (base.IsVisible ? MapBarUnreadBrushWidget.AnimState.Start : MapBarUnreadBrushWidget.AnimState.Idle);
			}
		}

		// Token: 0x04000651 RID: 1617
		private MapBarUnreadBrushWidget.AnimState _animState;

		// Token: 0x04000652 RID: 1618
		private TextWidget _unreadTextWidget;

		// Token: 0x02000197 RID: 407
		public enum AnimState
		{
			// Token: 0x04000916 RID: 2326
			Idle,
			// Token: 0x04000917 RID: 2327
			Start,
			// Token: 0x04000918 RID: 2328
			FirstFrame,
			// Token: 0x04000919 RID: 2329
			Playing
		}
	}
}
