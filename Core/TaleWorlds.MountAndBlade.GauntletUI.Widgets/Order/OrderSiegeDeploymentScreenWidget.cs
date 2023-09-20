using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000066 RID: 102
	public class OrderSiegeDeploymentScreenWidget : Widget
	{
		// Token: 0x0600056A RID: 1386 RVA: 0x00010421 File Offset: 0x0000E621
		public OrderSiegeDeploymentScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001042C File Offset: 0x0000E62C
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

		// Token: 0x0600056C RID: 1388 RVA: 0x000104DE File Offset: 0x0000E6DE
		private void UpdateEnabledState(bool isEnabled)
		{
			this.SetGlobalAlphaRecursively(isEnabled ? 1f : 0.5f);
			base.DoNotPassEventsToChildren = !isEnabled;
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x000104FF File Offset: 0x0000E6FF
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x00010507 File Offset: 0x0000E707
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

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x0001052F File Offset: 0x0000E72F
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x00010537 File Offset: 0x0000E737
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

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x00010555 File Offset: 0x0000E755
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x0001055D File Offset: 0x0000E75D
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

		// Token: 0x04000258 RID: 600
		private bool _isSiegeDeploymentDisabled;

		// Token: 0x04000259 RID: 601
		private Widget _deploymentTargetsParent;

		// Token: 0x0400025A RID: 602
		private ListPanel _deploymentListPanel;
	}
}
