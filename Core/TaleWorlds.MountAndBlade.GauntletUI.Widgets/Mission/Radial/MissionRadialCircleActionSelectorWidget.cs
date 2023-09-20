using System;
using TaleWorlds.GauntletUI;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Radial
{
	public class MissionRadialCircleActionSelectorWidget : CircleActionSelectorWidget
	{
		public MissionRadialCircleActionSelectorWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnSelectedIndexChanged(int selectedIndex)
		{
			base.OnSelectedIndexChanged(selectedIndex);
			for (int i = 0; i < base.Children.Count; i++)
			{
				MissionRadialButtonWidget missionRadialButtonWidget;
				if ((missionRadialButtonWidget = base.Children[i] as MissionRadialButtonWidget) != null)
				{
					if (i == selectedIndex)
					{
						missionRadialButtonWidget.ExecuteFocused();
					}
					else
					{
						missionRadialButtonWidget.ExecuteUnfocused();
					}
				}
			}
		}
	}
}
