using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000120 RID: 288
	public class InventoryTupleExtensionControlsWidget : Widget
	{
		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06000EA5 RID: 3749 RVA: 0x00028A0A File Offset: 0x00026C0A
		// (set) Token: 0x06000EA6 RID: 3750 RVA: 0x00028A12 File Offset: 0x00026C12
		public Widget NavigationParent { get; set; }

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x00028A1B File Offset: 0x00026C1B
		// (set) Token: 0x06000EA8 RID: 3752 RVA: 0x00028A23 File Offset: 0x00026C23
		private GamepadNavigationScope _parentScope { get; set; }

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x00028A2C File Offset: 0x00026C2C
		// (set) Token: 0x06000EAA RID: 3754 RVA: 0x00028A34 File Offset: 0x00026C34
		private GamepadNavigationScope _extensionSliderScope { get; set; }

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x00028A3D File Offset: 0x00026C3D
		// (set) Token: 0x06000EAC RID: 3756 RVA: 0x00028A45 File Offset: 0x00026C45
		private GamepadNavigationScope _extensionIncreaseDecreaseScope { get; set; }

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06000EAD RID: 3757 RVA: 0x00028A4E File Offset: 0x00026C4E
		// (set) Token: 0x06000EAE RID: 3758 RVA: 0x00028A56 File Offset: 0x00026C56
		private GamepadNavigationScope _extensionButtonsScope { get; set; }

		// Token: 0x06000EAF RID: 3759 RVA: 0x00028A5F File Offset: 0x00026C5F
		public InventoryTupleExtensionControlsWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00028A68 File Offset: 0x00026C68
		public void BuildNavigationData()
		{
			if (this._isNavigationActive)
			{
				return;
			}
			if (this.TransferSlider != null)
			{
				this._extensionSliderScope = new GamepadNavigationScope
				{
					ScopeID = "ExtensionSliderScope",
					ParentWidget = this.TransferSlider,
					IsEnabled = false,
					NavigateFromScopeEdges = true
				};
			}
			if (this.IncreaseDecreaseButtonsParent != null)
			{
				this._extensionIncreaseDecreaseScope = new GamepadNavigationScope
				{
					ScopeID = "ExtensionIncreaseDecreaseScope",
					ParentWidget = this.IncreaseDecreaseButtonsParent,
					IsEnabled = false,
					ScopeMovements = GamepadNavigationTypes.Horizontal,
					ExtendDiscoveryAreaTop = -40f,
					ExtendDiscoveryAreaBottom = -10f,
					ExtendDiscoveryAreaRight = -350f
				};
			}
			if (this.ButtonCarrier != null)
			{
				this._extensionButtonsScope = new GamepadNavigationScope
				{
					ScopeID = "ExtensionButtonsScope",
					ParentWidget = this.ButtonCarrier,
					IsEnabled = false,
					ScopeMovements = GamepadNavigationTypes.Horizontal
				};
			}
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x00028B4C File Offset: 0x00026D4C
		private void TransitionTick(float dt)
		{
			if (this._currentVisualStateAnimationState == VisualStateAnimationState.None)
			{
				if (!this._isNavigationActive)
				{
					this.AddGamepadNavigationControls();
					base.EventManager.AddLateUpdateAction(this, delegate(float _dt)
					{
						this.NavigateToBestChildScope();
					}, 1);
					return;
				}
			}
			else
			{
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.TransitionTick), 1);
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x00028BA4 File Offset: 0x00026DA4
		private void AddGamepadNavigationControls()
		{
			if (this.ValidateParentScope() && !this._isNavigationActive)
			{
				if (this._extensionIncreaseDecreaseScope != null)
				{
					base.EventManager.AddNavigationScope(this._extensionIncreaseDecreaseScope, false);
				}
				if (this._extensionSliderScope != null)
				{
					base.EventManager.AddNavigationScope(this._extensionSliderScope, false);
				}
				if (this._extensionButtonsScope != null)
				{
					base.EventManager.AddNavigationScope(this._extensionButtonsScope, false);
				}
				this.SetEnabledAllScopes(true);
				if (this._extensionSliderScope != null)
				{
					this._extensionSliderScope.SetParentScope(this._parentScope);
				}
				if (this._extensionIncreaseDecreaseScope != null)
				{
					this._extensionIncreaseDecreaseScope.SetParentScope(this._parentScope);
				}
				if (this._extensionButtonsScope != null)
				{
					this._extensionButtonsScope.SetParentScope(this._parentScope);
				}
				base.DoNotAcceptNavigation = false;
				this._isNavigationActive = true;
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x00028C78 File Offset: 0x00026E78
		private void RemoveGamepadNavigationControls()
		{
			if (this.ValidateParentScope() && this._isNavigationActive)
			{
				this.SetEnabledAllScopes(false);
				if (this._extensionSliderScope != null)
				{
					this._extensionSliderScope.SetParentScope(null);
					base.EventManager.RemoveNavigationScope(this._extensionSliderScope);
					this._extensionSliderScope = null;
				}
				if (this._extensionIncreaseDecreaseScope != null)
				{
					this._extensionIncreaseDecreaseScope.SetParentScope(null);
					base.EventManager.RemoveNavigationScope(this._extensionIncreaseDecreaseScope);
					this._extensionIncreaseDecreaseScope = null;
				}
				if (this._extensionButtonsScope != null)
				{
					this._extensionButtonsScope.SetParentScope(null);
					base.EventManager.RemoveNavigationScope(this._extensionButtonsScope);
					this._extensionButtonsScope = null;
				}
				base.DoNotAcceptNavigation = true;
				this._isNavigationActive = false;
			}
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00028D34 File Offset: 0x00026F34
		private void SetEnabledAllScopes(bool isEnabled)
		{
			if (this._extensionSliderScope != null)
			{
				this._extensionSliderScope.IsEnabled = isEnabled;
			}
			if (this._extensionIncreaseDecreaseScope != null)
			{
				this._extensionIncreaseDecreaseScope.IsEnabled = isEnabled;
			}
			if (this._extensionButtonsScope != null)
			{
				this._extensionButtonsScope.IsEnabled = isEnabled;
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00028D74 File Offset: 0x00026F74
		private void NavigateToBestChildScope()
		{
			if (this._parentScope.IsActiveScope)
			{
				GamepadNavigationScope[] array = new GamepadNavigationScope[] { this._extensionSliderScope, this._extensionButtonsScope, this._extensionIncreaseDecreaseScope };
				for (int i = 0; i < array.Length; i++)
				{
					if (GauntletGamepadNavigationManager.Instance.TryNavigateTo(array[i]))
					{
						return;
					}
				}
			}
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x00028DCE File Offset: 0x00026FCE
		private bool ValidateParentScope()
		{
			if (this._parentScope == null)
			{
				this._parentScope = this.GetParentScope();
			}
			return this._parentScope != null;
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x00028DF0 File Offset: 0x00026FF0
		private GamepadNavigationScope GetParentScope()
		{
			Widget navigationParent = this.NavigationParent;
			for (Widget widget = ((navigationParent != null) ? navigationParent.ParentWidget : null); widget != null; widget = widget.ParentWidget)
			{
				NavigationScopeTargeter navigationScopeTargeter;
				if ((navigationScopeTargeter = widget as NavigationScopeTargeter) != null)
				{
					return navigationScopeTargeter.NavigationScope;
				}
				NavigationScopeTargeter navigationScopeTargeter2 = widget.Children.FirstOrDefault((Widget x) => x is NavigationScopeTargeter) as NavigationScopeTargeter;
				if (navigationScopeTargeter2 != null)
				{
					return navigationScopeTargeter2.NavigationScope;
				}
			}
			return null;
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06000EB8 RID: 3768 RVA: 0x00028E68 File Offset: 0x00027068
		// (set) Token: 0x06000EB9 RID: 3769 RVA: 0x00028E70 File Offset: 0x00027070
		public bool IsExtended
		{
			get
			{
				return this._isExtended;
			}
			set
			{
				if (value != this._isExtended)
				{
					this._isExtended = value;
					base.IsEnabled = this._isExtended;
					this.SetEnabledAllScopes(false);
					if (this._isExtended)
					{
						this.BuildNavigationData();
						base.EventManager.AddLateUpdateAction(this, new Action<float>(this.TransitionTick), 1);
						return;
					}
					this.RemoveGamepadNavigationControls();
				}
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06000EBA RID: 3770 RVA: 0x00028ECE File Offset: 0x000270CE
		// (set) Token: 0x06000EBB RID: 3771 RVA: 0x00028ED6 File Offset: 0x000270D6
		[Editor(false)]
		public Widget TransferSlider
		{
			get
			{
				return this._transferSlider;
			}
			set
			{
				if (this._transferSlider != value)
				{
					this._transferSlider = value;
					base.OnPropertyChanged<Widget>(value, "TransferSlider");
				}
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06000EBC RID: 3772 RVA: 0x00028EF4 File Offset: 0x000270F4
		// (set) Token: 0x06000EBD RID: 3773 RVA: 0x00028EFC File Offset: 0x000270FC
		[Editor(false)]
		public Widget IncreaseDecreaseButtonsParent
		{
			get
			{
				return this._increaseDecreaseButtonsParent;
			}
			set
			{
				if (this._increaseDecreaseButtonsParent != value)
				{
					this._increaseDecreaseButtonsParent = value;
					base.OnPropertyChanged<Widget>(value, "IncreaseDecreaseButtonsParent");
				}
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06000EBE RID: 3774 RVA: 0x00028F1A File Offset: 0x0002711A
		// (set) Token: 0x06000EBF RID: 3775 RVA: 0x00028F22 File Offset: 0x00027122
		[Editor(false)]
		public Widget ButtonCarrier
		{
			get
			{
				return this._buttonCarrier;
			}
			set
			{
				if (this._buttonCarrier != value)
				{
					this._buttonCarrier = value;
					base.OnPropertyChanged<Widget>(value, "ButtonCarrier");
				}
			}
		}

		// Token: 0x040006B5 RID: 1717
		private bool _isNavigationActive;

		// Token: 0x040006B6 RID: 1718
		private bool _isExtended;

		// Token: 0x040006B7 RID: 1719
		private Widget _transferSlider;

		// Token: 0x040006B8 RID: 1720
		private Widget _increaseDecreaseButtonsParent;

		// Token: 0x040006B9 RID: 1721
		private Widget _buttonCarrier;
	}
}
