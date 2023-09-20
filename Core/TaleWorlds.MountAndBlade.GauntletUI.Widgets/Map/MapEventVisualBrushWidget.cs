using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map
{
	public class MapEventVisualBrushWidget : BrushWidget
	{
		public MapEventVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual(int type)
		{
			if (this._initialUpdate)
			{
				this.RegisterBrushStatesOfWidget();
				this._initialUpdate = false;
			}
			switch (type)
			{
			case 1:
				this.SetState("Raid");
				return;
			case 2:
				this.SetState("Siege");
				return;
			case 3:
				this.SetState("Battle");
				return;
			case 4:
				this.SetState("Rebellion");
				return;
			case 5:
				this.SetState("SallyOut");
				return;
			default:
				this.SetState("None");
				return;
			}
		}

		[Editor(false)]
		public int MapEventType
		{
			get
			{
				return this._mapEventType;
			}
			set
			{
				if (this._mapEventType != value)
				{
					this._mapEventType = value;
					this.UpdateVisual(value);
				}
			}
		}

		private bool _initialUpdate = true;

		private int _mapEventType = -1;
	}
}
