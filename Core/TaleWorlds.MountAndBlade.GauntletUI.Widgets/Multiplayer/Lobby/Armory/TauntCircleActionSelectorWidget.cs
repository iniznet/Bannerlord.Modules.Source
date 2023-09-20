using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class TauntCircleActionSelectorWidget : CircleActionSelectorWidget
	{
		public TauntCircleActionSelectorWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._currentSelectedIndex != -1)
			{
				Widget child = base.GetChild(this._currentSelectedIndex);
				object obj;
				if (child == null)
				{
					obj = null;
				}
				else
				{
					obj = child.Children.FirstOrDefault((Widget c) => c is ButtonWidget);
				}
				ButtonWidget buttonWidget = obj as ButtonWidget;
				Widget widget = ((buttonWidget != null) ? buttonWidget.FindChild("InputKeyContainer", true) : null);
				if (widget != null && !widget.IsVisible)
				{
					base.EventManager.SetHoveredView(null);
					base.EventManager.SetHoveredView(buttonWidget);
				}
			}
		}

		protected override void OnSelectedIndexChanged(int selectedIndex)
		{
			if (this._currentSelectedIndex == selectedIndex)
			{
				return;
			}
			this._currentSelectedIndex = selectedIndex;
			bool flag = false;
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				ButtonWidget buttonWidget = child.Children.FirstOrDefault((Widget c) => c is ButtonWidget) as ButtonWidget;
				if (child.GamepadNavigationIndex != -1 && buttonWidget != null)
				{
					bool flag2 = buttonWidget.IsEnabled && this._currentSelectedIndex == i;
					child.DoNotAcceptNavigation = !flag2;
					if (flag2)
					{
						this.SetCurrentNavigationTarget(child);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				this.SetCurrentNavigationTarget(this.FallbackNavigationWidget);
			}
		}

		private void SetCurrentNavigationTarget(Widget target)
		{
			if (this._tauntSlotNavigationTrialCount == -1)
			{
				this._currentNavigationTarget = target;
				this._tauntSlotNavigationTrialCount = 0;
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.NavigationUpdate), 1);
			}
		}

		private void NavigationUpdate(float dt)
		{
			if (this._currentNavigationTarget != null)
			{
				if (GauntletGamepadNavigationManager.Instance.TryNavigateTo(this._currentNavigationTarget))
				{
					this._currentNavigationTarget = null;
					this._tauntSlotNavigationTrialCount = -1;
					return;
				}
				if (this._tauntSlotNavigationTrialCount < 5)
				{
					this._tauntSlotNavigationTrialCount++;
					base.EventManager.AddLateUpdateAction(this, new Action<float>(this.NavigationUpdate), 1);
					return;
				}
				this._tauntSlotNavigationTrialCount = -1;
			}
		}

		public Widget FallbackNavigationWidget
		{
			get
			{
				return this._fallbackNavigationWidget;
			}
			set
			{
				if (value != this._fallbackNavigationWidget)
				{
					this._fallbackNavigationWidget = value;
					base.OnPropertyChanged<Widget>(value, "FallbackNavigationWidget");
				}
			}
		}

		private Widget _currentNavigationTarget;

		private int _currentSelectedIndex = -1;

		private int _tauntSlotNavigationTrialCount = -1;

		private Widget _fallbackNavigationWidget;
	}
}
