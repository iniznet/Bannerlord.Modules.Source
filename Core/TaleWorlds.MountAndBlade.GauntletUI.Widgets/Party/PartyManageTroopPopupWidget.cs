using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005A RID: 90
	public class PartyManageTroopPopupWidget : Widget
	{
		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x0000E661 File Offset: 0x0000C861
		// (set) Token: 0x060004AC RID: 1196 RVA: 0x0000E669 File Offset: 0x0000C869
		public Widget PrimaryInputKeyVisualParent { get; set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x0000E672 File Offset: 0x0000C872
		// (set) Token: 0x060004AE RID: 1198 RVA: 0x0000E67A File Offset: 0x0000C87A
		public Widget SecondaryInputKeyVisualParent { get; set; }

		// Token: 0x060004AF RID: 1199 RVA: 0x0000E683 File Offset: 0x0000C883
		public PartyManageTroopPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0000E68C File Offset: 0x0000C88C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible)
			{
				Widget hoveredView = base.EventManager.HoveredView;
				if (hoveredView != null && this.PrimaryInputKeyVisualParent != null && this.SecondaryInputKeyVisualParent != null)
				{
					PartyTroopManagementItemButtonWidget firstParentTupleOfWidget = this.GetFirstParentTupleOfWidget(hoveredView);
					if (firstParentTupleOfWidget != null)
					{
						Widget actionButtonAtIndex = firstParentTupleOfWidget.GetActionButtonAtIndex(0);
						bool flag = false;
						if (this.IsPrimaryActionAvailable && actionButtonAtIndex != null)
						{
							this.PrimaryInputKeyVisualParent.IsVisible = true;
							this.PrimaryInputKeyVisualParent.ScaledPositionXOffset = actionButtonAtIndex.GlobalPosition.X - 10f;
							this.PrimaryInputKeyVisualParent.ScaledPositionYOffset = actionButtonAtIndex.GlobalPosition.Y - 10f;
							flag = true;
						}
						else
						{
							this.PrimaryInputKeyVisualParent.IsVisible = false;
						}
						Widget widget = (flag ? firstParentTupleOfWidget.GetActionButtonAtIndex(1) : actionButtonAtIndex);
						if (this.IsSecondaryActionAvailable && widget != null)
						{
							this.SecondaryInputKeyVisualParent.IsVisible = true;
							this.SecondaryInputKeyVisualParent.ScaledPositionXOffset = widget.GlobalPosition.X + widget.Size.X + 4f;
							this.SecondaryInputKeyVisualParent.ScaledPositionYOffset = widget.GlobalPosition.Y - 10f;
							return;
						}
						this.SecondaryInputKeyVisualParent.IsVisible = false;
						return;
					}
					else
					{
						this.PrimaryInputKeyVisualParent.IsVisible = false;
						this.SecondaryInputKeyVisualParent.IsVisible = false;
					}
				}
			}
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0000E7E0 File Offset: 0x0000C9E0
		private PartyTroopManagementItemButtonWidget GetFirstParentTupleOfWidget(Widget widget)
		{
			for (Widget widget2 = widget; widget2 != null; widget2 = widget2.ParentWidget)
			{
				PartyTroopManagementItemButtonWidget partyTroopManagementItemButtonWidget;
				if ((partyTroopManagementItemButtonWidget = widget2 as PartyTroopManagementItemButtonWidget) != null)
				{
					return partyTroopManagementItemButtonWidget;
				}
			}
			return null;
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0000E808 File Offset: 0x0000CA08
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x0000E810 File Offset: 0x0000CA10
		public bool IsPrimaryActionAvailable
		{
			get
			{
				return this._isPrimaryActionAvailable;
			}
			set
			{
				if (value != this._isPrimaryActionAvailable)
				{
					this._isPrimaryActionAvailable = value;
					base.OnPropertyChanged(value, "IsPrimaryActionAvailable");
				}
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0000E82E File Offset: 0x0000CA2E
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x0000E836 File Offset: 0x0000CA36
		public bool IsSecondaryActionAvailable
		{
			get
			{
				return this._isSecondaryActionAvailable;
			}
			set
			{
				if (value != this._isSecondaryActionAvailable)
				{
					this._isSecondaryActionAvailable = value;
					base.OnPropertyChanged(value, "IsSecondaryActionAvailable");
				}
			}
		}

		// Token: 0x0400020B RID: 523
		private bool _isPrimaryActionAvailable;

		// Token: 0x0400020C RID: 524
		private bool _isSecondaryActionAvailable;
	}
}
