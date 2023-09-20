using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002C RID: 44
	public class NavigationTargetSwitcher : Widget
	{
		// Token: 0x0600027D RID: 637 RVA: 0x00008593 File Offset: 0x00006793
		public NavigationTargetSwitcher(UIContext context)
			: base(context)
		{
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000085C7 File Offset: 0x000067C7
		private void OnFromTargetNavigationIndexUpdated(PropertyOwnerObject propertyOwner, string propertyName, int value)
		{
			if (propertyName == "GamepadNavigationIndex" && this.ToTarget != null)
			{
				this.TransferGamepadNavigation();
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000085E4 File Offset: 0x000067E4
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

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000866A File Offset: 0x0000686A
		// (set) Token: 0x06000281 RID: 641 RVA: 0x00008672 File Offset: 0x00006872
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

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000282 RID: 642 RVA: 0x000086A8 File Offset: 0x000068A8
		// (set) Token: 0x06000283 RID: 643 RVA: 0x000086B0 File Offset: 0x000068B0
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

		// Token: 0x04000104 RID: 260
		private bool _isTransferingNavigationIndices;

		// Token: 0x04000105 RID: 261
		private Widget _toTarget;

		// Token: 0x04000106 RID: 262
		private Widget _fromTarget;
	}
}
