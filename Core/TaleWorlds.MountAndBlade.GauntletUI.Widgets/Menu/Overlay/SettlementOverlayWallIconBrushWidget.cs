using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.Overlay
{
	public class SettlementOverlayWallIconBrushWidget : BrushWidget
	{
		public SettlementOverlayWallIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.SetState(this.WallsLevel.ToString());
		}

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

		private int _wallsLevel;
	}
}
