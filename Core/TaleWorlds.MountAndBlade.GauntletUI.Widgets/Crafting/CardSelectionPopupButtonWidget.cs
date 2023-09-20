using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CardSelectionPopupButtonWidget : ButtonWidget
	{
		public CircularAutoScrollablePanelWidget PropertiesContainer { get; set; }

		public CardSelectionPopupButtonWidget(UIContext context)
			: base(context)
		{
		}

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
