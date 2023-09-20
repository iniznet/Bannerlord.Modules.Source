using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameOver
{
	// Token: 0x0200012F RID: 303
	public class GameOverCategoryIconBrushWidget : BrushWidget
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x0002D3B5 File Offset: 0x0002B5B5
		// (set) Token: 0x06000FFA RID: 4090 RVA: 0x0002D3BD File Offset: 0x0002B5BD
		public string CategoryID { get; set; }

		// Token: 0x06000FFB RID: 4091 RVA: 0x0002D3C6 File Offset: 0x0002B5C6
		public GameOverCategoryIconBrushWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnManualLateUpdate), 4);
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0002D3E8 File Offset: 0x0002B5E8
		private void OnManualLateUpdate(float obj)
		{
			base.Brush = base.Context.GetBrush("GameOver.Category.Visual." + this.CategoryID);
		}
	}
}
