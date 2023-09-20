using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Radial
{
	public class MissionRadialButtonWidget : ButtonWidget
	{
		public MissionRadialButtonWidget(UIContext context)
			: base(context)
		{
		}

		public void ExecuteFocused()
		{
			if (base.IsDisabled)
			{
				this.SetState("DisabledSelected");
			}
			base.EventFired("OnFocused", Array.Empty<object>());
		}

		public void ExecuteUnfocused()
		{
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			this.SetState("Default");
		}
	}
}
