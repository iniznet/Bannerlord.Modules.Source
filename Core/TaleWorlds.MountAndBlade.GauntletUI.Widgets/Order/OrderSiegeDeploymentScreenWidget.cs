using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderSiegeDeploymentScreenWidget : Widget
	{
		public OrderSiegeDeploymentScreenWidget(UIContext context)
			: base(context)
		{
		}

		public void SetSelectedDeploymentItem(OrderSiegeDeploymentItemButtonWidget deploymentItem)
		{
			this.DeploymentListPanel.ParentWidget.IsVisible = deploymentItem != null;
			if (deploymentItem == null)
			{
				return;
			}
			this.DeploymentListPanel.MarginLeft = (deploymentItem.GlobalPosition.X + deploymentItem.Size.Y + 20f - base.EventManager.LeftUsableAreaStart) / base._scaleToUse;
			this.DeploymentListPanel.MarginTop = (deploymentItem.GlobalPosition.Y + (deploymentItem.Size.Y / 2f - this.DeploymentListPanel.Size.Y / 2f) - base.EventManager.TopUsableAreaStart) / base._scaleToUse;
		}

		private void UpdateEnabledState(bool isEnabled)
		{
			this.SetGlobalAlphaRecursively(isEnabled ? 1f : 0.5f);
			base.DoNotPassEventsToChildren = !isEnabled;
		}

		public bool IsSiegeDeploymentDisabled
		{
			get
			{
				return this._isSiegeDeploymentDisabled;
			}
			set
			{
				if (value != this._isSiegeDeploymentDisabled)
				{
					this._isSiegeDeploymentDisabled = value;
					base.OnPropertyChanged(value, "IsSiegeDeploymentDisabled");
					this.UpdateEnabledState(!value);
				}
			}
		}

		public Widget DeploymentTargetsParent
		{
			get
			{
				return this._deploymentTargetsParent;
			}
			set
			{
				if (this._deploymentTargetsParent != value)
				{
					this._deploymentTargetsParent = value;
					base.OnPropertyChanged<Widget>(value, "DeploymentTargetsParent");
				}
			}
		}

		public ListPanel DeploymentListPanel
		{
			get
			{
				return this._deploymentListPanel;
			}
			set
			{
				if (this._deploymentListPanel != value)
				{
					this._deploymentListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "DeploymentListPanel");
				}
			}
		}

		private bool _isSiegeDeploymentDisabled;

		private Widget _deploymentTargetsParent;

		private ListPanel _deploymentListPanel;
	}
}
