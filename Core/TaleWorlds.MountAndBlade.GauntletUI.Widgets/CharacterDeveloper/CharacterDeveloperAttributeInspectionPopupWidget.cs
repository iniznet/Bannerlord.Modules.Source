using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class CharacterDeveloperAttributeInspectionPopupWidget : Widget
	{
		public CharacterDeveloperAttributeInspectionPopupWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.ParentWidget.IsVisible && this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseUpWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
			{
				this.Deactivate();
			}
		}

		private void Activate()
		{
			this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			base.ParentWidget.IsVisible = true;
		}

		private void Deactivate()
		{
			base.EventFired("Deactivate", Array.Empty<object>());
			base.ParentWidget.IsVisible = false;
		}

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
					if (this._isActive)
					{
						this.Activate();
						return;
					}
					this.Deactivate();
				}
			}
		}

		private Widget _latestMouseUpWidgetWhenActivated;

		private bool _isActive;
	}
}
