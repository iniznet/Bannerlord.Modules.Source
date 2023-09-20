using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	// Token: 0x020000F6 RID: 246
	public class SettlementOverlayWallIconBrushWidget : BrushWidget
	{
		// Token: 0x06000CDD RID: 3293 RVA: 0x0002402B File Offset: 0x0002222B
		public SettlementOverlayWallIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x00024034 File Offset: 0x00022234
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.SetState(this.WallsLevel.ToString());
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06000CDF RID: 3295 RVA: 0x0002405C File Offset: 0x0002225C
		// (set) Token: 0x06000CE0 RID: 3296 RVA: 0x00024064 File Offset: 0x00022264
		[Editor(false)]
		public int WallsLevel
		{
			get
			{
				return this._wallsLevel;
			}
			set
			{
				if (this._wallsLevel != value)
				{
					this._wallsLevel = value;
					base.OnPropertyChanged(value, "WallsLevel");
				}
			}
		}

		// Token: 0x040005F5 RID: 1525
		private int _wallsLevel;
	}
}
