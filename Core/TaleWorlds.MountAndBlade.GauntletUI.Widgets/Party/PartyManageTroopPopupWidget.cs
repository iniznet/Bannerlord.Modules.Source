using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyManageTroopPopupWidget : Widget
	{
		public Widget PrimaryInputKeyVisualParent { get; set; }

		public Widget SecondaryInputKeyVisualParent { get; set; }

		public PartyManageTroopPopupWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _isPrimaryActionAvailable;

		private bool _isSecondaryActionAvailable;
	}
}
