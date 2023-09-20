using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map
{
	// Token: 0x020000F9 RID: 249
	public class MapEventVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000CE6 RID: 3302 RVA: 0x00024162 File Offset: 0x00022362
		public MapEventVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0002417C File Offset: 0x0002237C
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

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x00024203 File Offset: 0x00022403
		// (set) Token: 0x06000CE9 RID: 3305 RVA: 0x0002420B File Offset: 0x0002240B
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

		// Token: 0x040005F9 RID: 1529
		private bool _initialUpdate = true;

		// Token: 0x040005FA RID: 1530
		private int _mapEventType = -1;
	}
}
