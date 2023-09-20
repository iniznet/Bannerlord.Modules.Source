using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryTupleExtensionControlsWidget : Widget
	{
		public Widget NavigationParent { get; set; }

		private GamepadNavigationScope _parentScope { get; set; }

		private GamepadNavigationScope _extensionSliderScope { get; set; }

		private GamepadNavigationScope _extensionIncreaseDecreaseScope { get; set; }

		private GamepadNavigationScope _extensionButtonsScope { get; set; }

		public InventoryTupleExtensionControlsWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool ValidateParentScope()
		{
			if (this._parentScope == null)
			{
				this._parentScope = this.GetParentScope();
			}
			return this._parentScope != null;
		}

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

		private bool _isNavigationActive;

		private bool _isExtended;

		private Widget _transferSlider;

		private Widget _increaseDecreaseButtonsParent;

		private Widget _buttonCarrier;
	}
}
