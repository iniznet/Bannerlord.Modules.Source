using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TutorialHighlightItemBrushWidget : BrushWidget
	{
		public Widget CustomSizeSyncTarget { get; set; }

		public bool DoNotOverrideWidth { get; set; }

		public bool DoNotOverrideHeight { get; set; }

		private Widget _syncTarget
		{
			get
			{
				return this.CustomSizeSyncTarget ?? base.ParentWidget;
			}
		}

		public TutorialHighlightItemBrushWidget(UIContext context)
			: base(context)
		{
			base.UseGlobalTimeForAnimation = true;
			base.DoNotAcceptEvents = true;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animState == TutorialHighlightItemBrushWidget.AnimState.Start)
			{
				this._animState = TutorialHighlightItemBrushWidget.AnimState.FirstFrame;
			}
			else if (this._animState == TutorialHighlightItemBrushWidget.AnimState.FirstFrame)
			{
				if (base.BrushRenderer.Brush == null)
				{
					this._animState = TutorialHighlightItemBrushWidget.AnimState.Start;
				}
				else
				{
					this._animState = TutorialHighlightItemBrushWidget.AnimState.Playing;
					base.BrushRenderer.RestartAnimation();
				}
			}
			if (this.IsHighlightEnabled && this._isDisabled)
			{
				this._isDisabled = false;
				this.SetState("Default");
			}
			else if (!this.IsHighlightEnabled && !this._isDisabled)
			{
				this.SetState("Disabled");
				this._isDisabled = true;
			}
			if (this._shouldSyncSize && this._syncTarget.Size.X > 1f && this._syncTarget.Size.Y > 1f)
			{
				if (!this.DoNotOverrideWidth)
				{
					base.ScaledSuggestedWidth = this._syncTarget.Size.X - 1f;
				}
				if (!this.DoNotOverrideHeight)
				{
					base.ScaledSuggestedHeight = this._syncTarget.Size.Y - 1f;
				}
			}
			if (this._syncTarget.HeightSizePolicy == SizePolicy.CoverChildren || this._syncTarget.WidthSizePolicy == SizePolicy.CoverChildren)
			{
				if (!this.DoNotOverrideWidth)
				{
					base.WidthSizePolicy = SizePolicy.Fixed;
				}
				if (!this.DoNotOverrideHeight)
				{
					base.HeightSizePolicy = SizePolicy.Fixed;
				}
				this._shouldSyncSize = true;
				return;
			}
			base.WidthSizePolicy = SizePolicy.StretchToParent;
			base.HeightSizePolicy = SizePolicy.StretchToParent;
			this._shouldSyncSize = false;
		}

		[Editor(false)]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (this._isHighlightEnabled != value)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChanged(value, "IsHighlightEnabled");
					if (this.IsHighlightEnabled)
					{
						this._animState = TutorialHighlightItemBrushWidget.AnimState.Start;
					}
					base.IsVisible = value;
					TaleWorlds.GauntletUI.EventManager.UIEventManager.TriggerEvent<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(new TutorialHighlightItemBrushWidget.HighlightElementToggledEvent(value, value ? this : null));
				}
			}
		}

		private TutorialHighlightItemBrushWidget.AnimState _animState;

		private bool _isDisabled;

		private bool _shouldSyncSize;

		private bool _isHighlightEnabled;

		public enum AnimState
		{
			Idle,
			Start,
			FirstFrame,
			Playing
		}

		public class HighlightElementToggledEvent : EventBase
		{
			public bool IsEnabled { get; private set; }

			public TutorialHighlightItemBrushWidget HighlightFrameWidget { get; private set; }

			public HighlightElementToggledEvent(bool isEnabled, TutorialHighlightItemBrushWidget highlightFrameWidget)
			{
				this.IsEnabled = isEnabled;
				this.HighlightFrameWidget = highlightFrameWidget;
			}
		}
	}
}
