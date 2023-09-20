using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class DuelArenaFlagVisualBrushWidget : BrushWidget
	{
		public DuelArenaFlagVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private int _arenaType = -1;
	}
}
