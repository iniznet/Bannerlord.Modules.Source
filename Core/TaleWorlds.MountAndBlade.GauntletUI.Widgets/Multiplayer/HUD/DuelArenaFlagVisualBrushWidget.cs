using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	// Token: 0x020000B1 RID: 177
	public class DuelArenaFlagVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x0001A296 File Offset: 0x00018496
		public DuelArenaFlagVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0001A2A8 File Offset: 0x000184A8
		private void UpdateVisual()
		{
			switch (this.ArenaType)
			{
			case 0:
				this.SetState("Infantry");
				return;
			case 1:
				this.SetState("Archery");
				return;
			case 2:
				this.SetState("Cavalry");
				return;
			default:
				this.SetState("Infantry");
				return;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x0001A2FF File Offset: 0x000184FF
		// (set) Token: 0x06000923 RID: 2339 RVA: 0x0001A307 File Offset: 0x00018507
		[Editor(false)]
		public int ArenaType
		{
			get
			{
				return this._arenaType;
			}
			set
			{
				if (this._arenaType != value)
				{
					this._arenaType = value;
					base.OnPropertyChanged(value, "ArenaType");
					this.UpdateVisual();
				}
			}
		}

		// Token: 0x04000426 RID: 1062
		private int _arenaType = -1;
	}
}
