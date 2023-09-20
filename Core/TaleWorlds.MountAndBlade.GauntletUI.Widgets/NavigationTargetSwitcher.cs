using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class NavigationTargetSwitcher : Widget
	{
		public NavigationTargetSwitcher(UIContext context)
			: base(context)
		{
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		private void OnFromTargetNavigationIndexUpdated(PropertyOwnerObject propertyOwner, string propertyName, int value)
		{
			if (propertyName == "GamepadNavigationIndex" && this.ToTarget != null)
			{
				this.TransferGamepadNavigation();
			}
		}

		private void TransferGamepadNavigation()
		{
			if (!this._isTransferingNavigationIndices)
			{
				this._isTransferingNavigationIndices = true;
				int gamepadNavigationIndex = this.FromTarget.GamepadNavigationIndex;
				this.ToTarget.GamepadNavigationIndex = gamepadNavigationIndex;
				this.FromTarget.GamepadNavigationIndex = -1;
				if (this.FromTarget.OnGamepadNavigationFocusGained != null)
				{
					Widget toTarget = this.ToTarget;
					toTarget.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Combine(toTarget.OnGamepadNavigationFocusGained, this.FromTarget.OnGamepadNavigationFocusGained);
					this.FromTarget.OnGamepadNavigationFocusGained = null;
				}
				this._isTransferingNavigationIndices = false;
			}
		}

		public Widget ToTarget
		{
			get
			{
				return this._toTarget;
			}
			set
			{
				if (value != this._toTarget)
				{
					this._toTarget = value;
					if (this._toTarget != null && this.FromTarget != null && this.FromTarget.GamepadNavigationIndex != -1)
					{
						this.TransferGamepadNavigation();
					}
				}
			}
		}

		public Widget FromTarget
		{
			get
			{
				return this._fromTarget;
			}
			set
			{
				if (value != this._fromTarget)
				{
					if (this._fromTarget != null)
					{
						this._fromTarget.intPropertyChanged -= this.OnFromTargetNavigationIndexUpdated;
					}
					this._fromTarget = value;
					this._fromTarget.intPropertyChanged += this.OnFromTargetNavigationIndexUpdated;
				}
			}
		}

		private bool _isTransferingNavigationIndices;

		private Widget _toTarget;

		private Widget _fromTarget;
	}
}
