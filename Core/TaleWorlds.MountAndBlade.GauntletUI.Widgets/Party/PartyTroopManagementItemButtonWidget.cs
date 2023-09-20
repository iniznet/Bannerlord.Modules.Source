using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x0200005D RID: 93
	public class PartyTroopManagementItemButtonWidget : ButtonWidget
	{
		// Token: 0x060004F5 RID: 1269 RVA: 0x0000F435 File Offset: 0x0000D635
		public PartyTroopManagementItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0000F440 File Offset: 0x0000D640
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

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0000F4B8 File Offset: 0x0000D6B8
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x0000F4C0 File Offset: 0x0000D6C0
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

		// Token: 0x04000227 RID: 551
		private Widget _actionButtonsContainer;
	}
}
