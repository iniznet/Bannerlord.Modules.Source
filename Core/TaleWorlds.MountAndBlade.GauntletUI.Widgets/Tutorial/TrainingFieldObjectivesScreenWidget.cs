using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x0200003E RID: 62
	public class TrainingFieldObjectivesScreenWidget : Widget
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000A7D3 File Offset: 0x000089D3
		// (set) Token: 0x06000342 RID: 834 RVA: 0x0000A7DB File Offset: 0x000089DB
		public Widget MouseContainerWidget { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000A7E4 File Offset: 0x000089E4
		// (set) Token: 0x06000344 RID: 836 RVA: 0x0000A7EC File Offset: 0x000089EC
		public Widget ControllerContainerWidget { get; set; }

		// Token: 0x06000345 RID: 837 RVA: 0x0000A7F5 File Offset: 0x000089F5
		public TrainingFieldObjectivesScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000A800 File Offset: 0x00008A00
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			bool isControllerActive = base.Context.EventManager.IsControllerActive;
			this.MouseContainerWidget.IsVisible = !isControllerActive;
			this.ControllerContainerWidget.IsVisible = isControllerActive;
		}
	}
}
