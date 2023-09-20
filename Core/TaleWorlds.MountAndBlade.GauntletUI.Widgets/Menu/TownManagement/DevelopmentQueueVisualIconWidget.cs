using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class DevelopmentQueueVisualIconWidget : Widget
	{
		public DevelopmentQueueVisualIconWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animState == DevelopmentQueueVisualIconWidget.AnimState.Start)
			{
				this._tickCount += 1f;
				if (this._tickCount > 20f)
				{
					this._animState = DevelopmentQueueVisualIconWidget.AnimState.Starting;
					return;
				}
			}
			else if (this._animState == DevelopmentQueueVisualIconWidget.AnimState.Starting)
			{
				BrushWidget inProgressIconWidget = this.InProgressIconWidget;
				if (inProgressIconWidget != null)
				{
					inProgressIconWidget.BrushRenderer.RestartAnimation();
				}
				this._animState = DevelopmentQueueVisualIconWidget.AnimState.Playing;
			}
		}

		private void UpdateVisual(int index)
		{
			if (this.InProgressIconWidget != null && this.QueueIconWidget != null)
			{
				base.IsVisible = index >= 0;
				this.InProgressIconWidget.IsVisible = index == 0;
				this._animState = (this.InProgressIconWidget.IsVisible ? DevelopmentQueueVisualIconWidget.AnimState.Start : DevelopmentQueueVisualIconWidget.AnimState.Idle);
				this._tickCount = 0f;
				this.QueueIconWidget.IsVisible = index > 0;
			}
		}

		[Editor(false)]
		public int QueueIndex
		{
			get
			{
				return this._queueIndex;
			}
			set
			{
				if (this._queueIndex != value)
				{
					this._queueIndex = value;
					base.OnPropertyChanged(value, "QueueIndex");
					this.UpdateVisual(value);
				}
			}
		}

		[Editor(false)]
		public Widget QueueIconWidget
		{
			get
			{
				return this._queueIconWidget;
			}
			set
			{
				if (this._queueIconWidget != value)
				{
					this._queueIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "QueueIconWidget");
					this.UpdateVisual(this.QueueIndex);
				}
			}
		}

		[Editor(false)]
		public BrushWidget InProgressIconWidget
		{
			get
			{
				return this._inProgressIconWidget;
			}
			set
			{
				if (this._inProgressIconWidget != value)
				{
					this._inProgressIconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "InProgressIconWidget");
					this.UpdateVisual(this.QueueIndex);
				}
			}
		}

		private DevelopmentQueueVisualIconWidget.AnimState _animState;

		private float _tickCount;

		private int _queueIndex = -1;

		private Widget _queueIconWidget;

		private BrushWidget _inProgressIconWidget;

		public enum AnimState
		{
			Idle,
			Start,
			Starting,
			Playing
		}
	}
}
