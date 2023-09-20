using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001C RID: 28
	public class GamepadCursorMarkerWidget : BrushWidget
	{
		// Token: 0x06000154 RID: 340 RVA: 0x00005BD7 File Offset: 0x00003DD7
		public GamepadCursorMarkerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00005BE0 File Offset: 0x00003DE0
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00005BE8 File Offset: 0x00003DE8
		public bool FlipVisual
		{
			get
			{
				return this._flipVisual;
			}
			set
			{
				if (value != this._flipVisual)
				{
					this._flipVisual = value;
					base.Brush.DefaultLayer.HorizontalFlip = value;
				}
			}
		}

		// Token: 0x040000A3 RID: 163
		private bool _flipVisual;
	}
}
