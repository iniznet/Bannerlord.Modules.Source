using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerArmoryPageWidget : Widget
	{
		public MultiplayerArmoryPageWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.Update), 1);
		}

		private void Update(float dt)
		{
			if (this.IsTauntAssignmentActive && !Input.IsGamepadActive)
			{
				Widget latestMouseUpWidget = base.EventManager.LatestMouseUpWidget;
				Widget latestMouseDownWidget = base.EventManager.LatestMouseDownWidget;
				if (latestMouseUpWidget != null && latestMouseUpWidget == latestMouseDownWidget && !this.IsWidgetUsedForTauntSelection(latestMouseUpWidget))
				{
					base.EventFired("ReleaseTauntSelections", Array.Empty<object>());
				}
			}
			if (this.TauntSlotsContainer != null && this.TauntCircleActionSelector != null)
			{
				this.TauntCircleActionSelector.IsCircularInputEnabled = this.IsTauntControlsOpen && this.TauntSlotsContainer.IsPointInsideMeasuredArea(base.EventManager.MousePosition);
			}
			if (this._cosmeticPanelScrollTarget != null && this._cosmeticsScrollablePanel != null)
			{
				this._cosmeticsScrollablePanel.ScrollToChild(this._cosmeticPanelScrollTarget, -1f, 0.5f, 0, 0, 0.3f, 0f);
				this._cosmeticPanelScrollTarget = null;
			}
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.Update), 1);
		}

		private bool IsWidgetUsedForTauntSelection(Widget widget)
		{
			CircleActionSelectorWidget tauntCircleActionSelector = this.TauntCircleActionSelector;
			MultiplayerLobbyArmoryCosmeticItemButtonWidget multiplayerLobbyArmoryCosmeticItemButtonWidget;
			return (tauntCircleActionSelector != null && tauntCircleActionSelector.CheckIsMyChildRecursive(widget)) || ((multiplayerLobbyArmoryCosmeticItemButtonWidget = widget as MultiplayerLobbyArmoryCosmeticItemButtonWidget) != null && multiplayerLobbyArmoryCosmeticItemButtonWidget.IsSelected);
		}

		private void RegisterForStateUpdate()
		{
			if (this._isTauntStateDirty)
			{
				return;
			}
			this._isTauntStateDirty = true;
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.UpdateTauntControlStates), 1);
		}

		private void UpdateTauntControlStates(float dt)
		{
			string text = (this.IsTauntControlsOpen ? "TauntEnabled" : "Default");
			if (this.TauntCircleActionSelector != null)
			{
				this.TauntCircleActionSelector.AnimateDistanceFromCenterTo((float)(this.IsTauntControlsOpen ? this.TauntEnabledRadialDistance : this.TauntDisabledRadialDistance), this.TauntStateAnimationDuration);
				this.TauntCircleActionSelector.IsEnabled = this.IsTauntControlsOpen;
				this.TauntCircleActionSelector.SetGlobalAlphaRecursively(this.IsTauntControlsOpen ? 1f : 0.6f);
			}
			Widget tauntSlotsContainer = this.TauntSlotsContainer;
			if (tauntSlotsContainer != null)
			{
				tauntSlotsContainer.SetState(text);
			}
			Widget manageTauntsButton = this.ManageTauntsButton;
			if (manageTauntsButton != null)
			{
				manageTauntsButton.SetState(text);
			}
			Widget leftSideParent = this.LeftSideParent;
			if (leftSideParent != null)
			{
				leftSideParent.SetState(text);
			}
			Widget gameModesDropdownParent = this.GameModesDropdownParent;
			if (gameModesDropdownParent != null)
			{
				gameModesDropdownParent.SetState(text);
			}
			Widget heroPreviewParent = this.HeroPreviewParent;
			if (heroPreviewParent != null)
			{
				heroPreviewParent.SetState(text);
			}
			if (this.RightPanelTabControl != null && this.IsTauntControlsOpen)
			{
				this.RightPanelTabControl.SelectedIndex = 1;
			}
			this._isTauntStateDirty = false;
		}

		private void OnTauntAssignmentStateChanged(bool isTauntAssignmentActive)
		{
			if (isTauntAssignmentActive && this.TauntCircleActionSelector != null)
			{
				if (this.TauntCircleActionSelector.AllChildren.FirstOrDefault(delegate(Widget c)
				{
					ButtonWidget buttonWidget = c as ButtonWidget;
					return buttonWidget != null && buttonWidget.IsSelected;
				}) != null)
				{
					if (this._cosmeticsScrollablePanel == null)
					{
						this._cosmeticsScrollablePanel = this.RightPanelTabControl.AllChildren.FirstOrDefault((Widget c) => c is ScrollablePanel) as ScrollablePanel;
					}
					if (this._cosmeticsScrollablePanel != null)
					{
						Widget widget = this._cosmeticsScrollablePanel.AllChildren.FirstOrDefault(delegate(Widget c)
						{
							MultiplayerLobbyArmoryCosmeticItemButtonWidget multiplayerLobbyArmoryCosmeticItemButtonWidget;
							return (multiplayerLobbyArmoryCosmeticItemButtonWidget = c as MultiplayerLobbyArmoryCosmeticItemButtonWidget) != null && multiplayerLobbyArmoryCosmeticItemButtonWidget.IsSelectable;
						});
						if (widget != null)
						{
							this._cosmeticPanelScrollTarget = widget;
						}
					}
				}
			}
			if (Input.IsGamepadActive && isTauntAssignmentActive)
			{
				GauntletGamepadNavigationManager.Instance.TryNavigateTo(this.ManageTauntsButton);
			}
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateTauntAssignmentStates), 1);
		}

		private void AnimateTauntAssignmentStates(float dt)
		{
			this._tauntAssignmentStateTimer += dt;
			float num;
			if (this._tauntAssignmentStateTimer < this.TauntStateAnimationDuration)
			{
				num = this._tauntAssignmentStateTimer / this.TauntStateAnimationDuration;
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateTauntAssignmentStates), 1);
			}
			else
			{
				num = 1f;
			}
			float num2 = (this.IsTauntAssignmentActive ? 0f : this.TauntAssignmentOverlayAlpha);
			float num3 = (this.IsTauntAssignmentActive ? this.TauntAssignmentOverlayAlpha : 0f);
			float num4 = MathF.Lerp(num2, num3, num, 1E-05f);
			if (this.TauntAssignmentOverlay != null)
			{
				this.TauntAssignmentOverlay.IsVisible = num4 != 0f;
				this.TauntAssignmentOverlay.SetGlobalAlphaRecursively(num4);
			}
		}

		public bool IsTauntAssignmentActive
		{
			get
			{
				return this._isTauntAssignmentActive;
			}
			set
			{
				if (value != this._isTauntAssignmentActive)
				{
					this._isTauntAssignmentActive = value;
					base.OnPropertyChanged(value, "IsTauntAssignmentActive");
					this._tauntAssignmentStateTimer = 0f;
					this.OnTauntAssignmentStateChanged(value);
				}
			}
		}

		public bool IsTauntControlsOpen
		{
			get
			{
				return this._isTauntControlsOpen;
			}
			set
			{
				if (value != this._isTauntControlsOpen)
				{
					this._isTauntControlsOpen = value;
					base.OnPropertyChanged(value, "IsTauntControlsOpen");
					this.RegisterForStateUpdate();
				}
			}
		}

		public int TauntEnabledRadialDistance
		{
			get
			{
				return this._tauntEnabledRadialDistance;
			}
			set
			{
				if (value != this._tauntEnabledRadialDistance)
				{
					this._tauntEnabledRadialDistance = value;
					base.OnPropertyChanged(value, "TauntEnabledRadialDistance");
					this.RegisterForStateUpdate();
				}
			}
		}

		public int TauntDisabledRadialDistance
		{
			get
			{
				return this._tauntDisabledRadialDistance;
			}
			set
			{
				if (value != this._tauntDisabledRadialDistance)
				{
					this._tauntDisabledRadialDistance = value;
					base.OnPropertyChanged(value, "TauntDisabledRadialDistance");
					this.RegisterForStateUpdate();
				}
			}
		}

		public float TauntStateAnimationDuration
		{
			get
			{
				return this._tauntStateAnimationDuration;
			}
			set
			{
				if (value != this._tauntStateAnimationDuration)
				{
					this._tauntStateAnimationDuration = value;
					base.OnPropertyChanged(value, "TauntStateAnimationDuration");
					this.RegisterForStateUpdate();
				}
			}
		}

		public float TauntAssignmentOverlayAlpha
		{
			get
			{
				return this._tauntAssignmentOverlayAlpha;
			}
			set
			{
				if (value != this._tauntAssignmentOverlayAlpha)
				{
					this._tauntAssignmentOverlayAlpha = value;
					base.OnPropertyChanged(value, "TauntAssignmentOverlayAlpha");
					this.RegisterForStateUpdate();
				}
			}
		}

		public Widget LeftSideParent
		{
			get
			{
				return this._leftSideParent;
			}
			set
			{
				if (value != this._leftSideParent)
				{
					this._leftSideParent = value;
					base.OnPropertyChanged<Widget>(value, "LeftSideParent");
					this.RegisterForStateUpdate();
				}
			}
		}

		public Widget GameModesDropdownParent
		{
			get
			{
				return this._gameModesDropdownParent;
			}
			set
			{
				if (value != this._gameModesDropdownParent)
				{
					this._gameModesDropdownParent = value;
					base.OnPropertyChanged<Widget>(value, "GameModesDropdownParent");
					this.RegisterForStateUpdate();
				}
			}
		}

		public Widget HeroPreviewParent
		{
			get
			{
				return this._heroPreviewParent;
			}
			set
			{
				if (value != this._heroPreviewParent)
				{
					this._heroPreviewParent = value;
					base.OnPropertyChanged<Widget>(value, "HeroPreviewParent");
					this.RegisterForStateUpdate();
				}
			}
		}

		public Widget TauntAssignmentOverlay
		{
			get
			{
				return this._tauntAssignmentOverlay;
			}
			set
			{
				if (value != this._tauntAssignmentOverlay)
				{
					this._tauntAssignmentOverlay = value;
					base.OnPropertyChanged<Widget>(value, "TauntAssignmentOverlay");
				}
			}
		}

		public Widget ManageTauntsButton
		{
			get
			{
				return this._manageTauntsButton;
			}
			set
			{
				if (value != this._manageTauntsButton)
				{
					this._manageTauntsButton = value;
					base.OnPropertyChanged<Widget>(value, "ManageTauntsButton");
				}
			}
		}

		public Widget TauntSlotsContainer
		{
			get
			{
				return this._tauntSlotsContainer;
			}
			set
			{
				if (value != this._tauntSlotsContainer)
				{
					this._tauntSlotsContainer = value;
					base.OnPropertyChanged<Widget>(value, "TauntSlotsContainer");
				}
			}
		}

		public TabControl RightPanelTabControl
		{
			get
			{
				return this._rightPanelTabControl;
			}
			set
			{
				if (value != this._rightPanelTabControl)
				{
					this._rightPanelTabControl = value;
					base.OnPropertyChanged<TabControl>(value, "RightPanelTabControl");
					this.RegisterForStateUpdate();
				}
			}
		}

		public CircleActionSelectorWidget TauntCircleActionSelector
		{
			get
			{
				return this._tauntCircleActionSelector;
			}
			set
			{
				if (value != this._tauntCircleActionSelector)
				{
					this._tauntCircleActionSelector = value;
					base.OnPropertyChanged<CircleActionSelectorWidget>(value, "TauntCircleActionSelector");
					if (this._tauntCircleActionSelector != null)
					{
						this._tauntCircleActionSelector.DistanceFromCenterModifier = (float)(this.IsTauntControlsOpen ? this.TauntEnabledRadialDistance : this.TauntDisabledRadialDistance);
					}
					this.RegisterForStateUpdate();
				}
			}
		}

		private bool _isTauntStateDirty;

		private float _tauntAssignmentStateTimer;

		private ScrollablePanel _cosmeticsScrollablePanel;

		private Widget _cosmeticPanelScrollTarget;

		private bool _isTauntAssignmentActive;

		private bool _isTauntControlsOpen;

		private int _tauntEnabledRadialDistance;

		private int _tauntDisabledRadialDistance;

		private float _tauntStateAnimationDuration;

		private float _tauntAssignmentOverlayAlpha;

		private Widget _leftSideParent;

		private Widget _gameModesDropdownParent;

		private Widget _heroPreviewParent;

		private Widget _tauntAssignmentOverlay;

		private Widget _manageTauntsButton;

		private Widget _tauntSlotsContainer;

		private TabControl _rightPanelTabControl;

		private CircleActionSelectorWidget _tauntCircleActionSelector;
	}
}
