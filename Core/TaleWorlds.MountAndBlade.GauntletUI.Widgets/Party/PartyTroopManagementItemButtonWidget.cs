using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyTroopManagementItemButtonWidget : ButtonWidget
	{
		public PartyTroopManagementItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		public Widget GetActionButtonAtIndex(int index)
		{
			if (this.ActionButtonsContainer != null)
			{
				int num = 0;
				foreach (Widget widget in this.ActionButtonsContainer.AllChildren)
				{
					if (widget.Id == "ActionButton")
					{
						if (num == index)
						{
							return widget;
						}
						num++;
					}
				}
			}
			return null;
		}

		public Widget ActionButtonsContainer
		{
			get
			{
				return this._actionButtonsContainer;
			}
			set
			{
				if (value != this._actionButtonsContainer)
				{
					this._actionButtonsContainer = value;
					base.OnPropertyChanged<Widget>(value, "ActionButtonsContainer");
				}
			}
		}

		private Widget _actionButtonsContainer;
	}
}
