using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingPieceTypeSelectorButtonWidget : ButtonWidget
	{
		public CraftingPieceTypeSelectorButtonWidget(UIContext context)
			: base(context)
		{
		}

		public override void SetState(string stateName)
		{
			base.SetState(stateName);
			Widget visualsWidget = this.VisualsWidget;
			if (visualsWidget == null)
			{
				return;
			}
			visualsWidget.SetState(stateName);
		}

		public Widget VisualsWidget
		{
			get
			{
				return this._visualsWidget;
			}
			set
			{
				if (value != this._visualsWidget)
				{
					this._visualsWidget = value;
				}
			}
		}

		private Widget _visualsWidget;
	}
}
