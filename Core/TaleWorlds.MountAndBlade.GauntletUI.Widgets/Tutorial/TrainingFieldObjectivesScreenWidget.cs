using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TrainingFieldObjectivesScreenWidget : Widget
	{
		public Widget MouseContainerWidget { get; set; }

		public Widget ControllerContainerWidget { get; set; }

		public TrainingFieldObjectivesScreenWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool isControllerActive = base.Context.EventManager.IsControllerActive;
			this.MouseContainerWidget.IsVisible = !isControllerActive;
			this.ControllerContainerWidget.IsVisible = isControllerActive;
		}
	}
}
