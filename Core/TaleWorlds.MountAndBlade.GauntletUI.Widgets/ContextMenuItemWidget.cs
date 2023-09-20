using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ContextMenuItemWidget : Widget
	{
		public Widget TypeIconWidget { get; set; }

		public ButtonWidget ActionButtonWidget { get; set; }

		public string TypeIconState { get; set; }

		public ContextMenuItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				if (this.TypeIconWidget != null && !string.IsNullOrEmpty(this.TypeIconState))
				{
					this.TypeIconWidget.RegisterBrushStatesOfWidget();
					this.TypeIconWidget.SetState(this.TypeIconState);
				}
				this._isInitialized = true;
			}
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (!this.CanBeUsed)
			{
				this.SetGlobalAlphaRecursively(0.5f);
				return;
			}
			this.SetGlobalAlphaRecursively(1f);
		}

		public bool CanBeUsed
		{
			get
			{
				return this._canBeUsed;
			}
			set
			{
				if (value != this._canBeUsed)
				{
					this._canBeUsed = value;
					base.OnPropertyChanged(value, "CanBeUsed");
					this.RefreshState();
				}
			}
		}

		private const float _disabledAlpha = 0.5f;

		private const float _enabledAlpha = 1f;

		private bool _isInitialized;

		private bool _canBeUsed = true;
	}
}
