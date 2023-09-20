using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class GamepadCursorMarkerWidget : BrushWidget
	{
		public GamepadCursorMarkerWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _flipVisual;
	}
}
