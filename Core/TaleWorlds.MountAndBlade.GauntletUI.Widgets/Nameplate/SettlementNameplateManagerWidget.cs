using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	public class SettlementNameplateManagerWidget : Widget
	{
		public SettlementNameplateManagerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			this._visibleNameplates.Clear();
			foreach (SettlementNameplateWidget settlementNameplateWidget in this._allChildrenNameplates)
			{
				if (settlementNameplateWidget != null && settlementNameplateWidget.IsVisibleOnMap)
				{
					this._visibleNameplates.Add(settlementNameplateWidget);
				}
			}
			this._visibleNameplates.Sort();
			foreach (SettlementNameplateWidget settlementNameplateWidget2 in this._visibleNameplates)
			{
				settlementNameplateWidget2.DisableRender = false;
				settlementNameplateWidget2.Render(twoDimensionContext, drawContext);
				settlementNameplateWidget2.DisableRender = true;
			}
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.DisableRender = true;
			this._allChildrenNameplates.Add(child as SettlementNameplateWidget);
		}

		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			this._allChildrenNameplates.Remove(child as SettlementNameplateWidget);
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._allChildrenNameplates.Clear();
			this._allChildrenNameplates = null;
		}

		private readonly List<SettlementNameplateWidget> _visibleNameplates = new List<SettlementNameplateWidget>();

		private List<SettlementNameplateWidget> _allChildrenNameplates = new List<SettlementNameplateWidget>();
	}
}
