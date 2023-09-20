using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000140 RID: 320
	public class CardSelectionPopupButtonWidget : ButtonWidget
	{
		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x060010C8 RID: 4296 RVA: 0x0002EF6A File Offset: 0x0002D16A
		// (set) Token: 0x060010C9 RID: 4297 RVA: 0x0002EF72 File Offset: 0x0002D172
		public CircularAutoScrollablePanelWidget PropertiesContainer { get; set; }

		// Token: 0x060010CA RID: 4298 RVA: 0x0002EF7B File Offset: 0x0002D17B
		public CardSelectionPopupButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0002EF84 File Offset: 0x0002D184
		public override void SetState(string stateName)
		{
			base.SetState(stateName);
			CircularAutoScrollablePanelWidget propertiesContainer = this.PropertiesContainer;
			if (propertiesContainer == null)
			{
				return;
			}
			propertiesContainer.SetState(stateName);
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0002EF9E File Offset: 0x0002D19E
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			CircularAutoScrollablePanelWidget propertiesContainer = this.PropertiesContainer;
			if (propertiesContainer == null)
			{
				return;
			}
			propertiesContainer.SetHoverBegin();
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0002EFB6 File Offset: 0x0002D1B6
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			CircularAutoScrollablePanelWidget propertiesContainer = this.PropertiesContainer;
			if (propertiesContainer == null)
			{
				return;
			}
			propertiesContainer.SetHoverEnd();
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0002EFCE File Offset: 0x0002D1CE
		protected override void OnMouseScroll()
		{
			base.OnMouseScroll();
			CircularAutoScrollablePanelWidget propertiesContainer = this.PropertiesContainer;
			if (propertiesContainer == null)
			{
				return;
			}
			propertiesContainer.SetScrollMouse();
		}
	}
}
