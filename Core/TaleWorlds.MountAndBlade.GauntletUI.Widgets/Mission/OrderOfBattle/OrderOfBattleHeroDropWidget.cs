using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleHeroDropWidget : ButtonWidget
	{
		public OrderOfBattleHeroDropWidget(UIContext context)
			: base(context)
		{
		}

		protected override bool OnPreviewDrop()
		{
			return true;
		}

		protected override bool OnPreviewDragHover()
		{
			return true;
		}
	}
}
