using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaFilterListItemButtonWidget : ButtonWidget
	{
		public EncyclopediaFilterListItemButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			if (base.IsHovered)
			{
				this.SetState("Hovered");
				return;
			}
			if (base.IsSelected)
			{
				this.SetState("Selected");
				return;
			}
			if (base.IsPressed)
			{
				this.SetState("Pressed");
				return;
			}
			this.SetState("Default");
		}
	}
}
