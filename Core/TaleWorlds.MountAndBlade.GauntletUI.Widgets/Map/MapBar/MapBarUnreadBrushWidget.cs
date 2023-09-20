using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapBarUnreadBrushWidget : BrushWidget
	{
		public bool IsBannerNotification { get; set; }

		public MapBarUnreadBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private void UnreadTextWidgetOnPropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsVisible")
			{
				base.IsVisible = propertyValue;
				this._animState = (base.IsVisible ? MapBarUnreadBrushWidget.AnimState.Start : MapBarUnreadBrushWidget.AnimState.Idle);
			}
		}

		private MapBarUnreadBrushWidget.AnimState _animState;

		private TextWidget _unreadTextWidget;

		public enum AnimState
		{
			Idle,
			Start,
			FirstFrame,
			Playing
		}
	}
}
