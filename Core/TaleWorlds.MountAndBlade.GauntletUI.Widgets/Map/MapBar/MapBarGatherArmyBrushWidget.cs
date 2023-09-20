using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapBarGatherArmyBrushWidget : BrushWidget
	{
		public MapBarGatherArmyBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.UpdateVisualState();
				this._initialized = true;
			}
		}

		private void UpdateVisualState()
		{
			base.IsEnabled = this.IsGatherArmyVisible;
			if (!this.IsGatherArmyVisible)
			{
				this.SetState("Disabled");
				return;
			}
			if (this._isInfoBarExtended)
			{
				this.SetState("Extended");
				return;
			}
			this.SetState("Default");
		}

		private void OnMapInfoBarExtendStateChange(bool newState)
		{
			this._isInfoBarExtended = newState;
			this.UpdateVisualState();
		}

		public MapInfoBarWidget InfoBarWidget
		{
			get
			{
				return this._infoBarWidget;
			}
			set
			{
				if (this._infoBarWidget != value)
				{
					this._infoBarWidget = value;
					this._infoBarWidget.OnMapInfoBarExtendStateChange += this.OnMapInfoBarExtendStateChange;
				}
			}
		}

		public bool IsGatherArmyEnabled
		{
			get
			{
				return this._isGatherArmyEnabled;
			}
			set
			{
				if (this._isGatherArmyEnabled != value)
				{
					this._isGatherArmyEnabled = value;
					this.UpdateVisualState();
				}
			}
		}

		public bool IsGatherArmyVisible
		{
			get
			{
				return this._isGatherArmyVisible;
			}
			set
			{
				if (this._isGatherArmyVisible != value)
				{
					this._isGatherArmyVisible = value;
					this.UpdateVisualState();
				}
			}
		}

		private bool _isInfoBarExtended;

		private bool _initialized;

		private MapInfoBarWidget _infoBarWidget;

		private bool _isGatherArmyEnabled;

		private bool _isGatherArmyVisible;
	}
}
