using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	// Token: 0x02000074 RID: 116
	public class SettlementNameplateManagerWidget : Widget
	{
		// Token: 0x0600066F RID: 1647 RVA: 0x000131A1 File Offset: 0x000113A1
		public SettlementNameplateManagerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x000131C0 File Offset: 0x000113C0
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

		// Token: 0x06000671 RID: 1649 RVA: 0x00013288 File Offset: 0x00011488
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.DisableRender = true;
			this._allChildrenNameplates.Add(child as SettlementNameplateWidget);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000132A9 File Offset: 0x000114A9
		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			this._allChildrenNameplates.Remove(child as SettlementNameplateWidget);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000132C4 File Offset: 0x000114C4
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._allChildrenNameplates.Clear();
			this._allChildrenNameplates = null;
		}

		// Token: 0x040002D2 RID: 722
		private readonly List<SettlementNameplateWidget> _visibleNameplates = new List<SettlementNameplateWidget>();

		// Token: 0x040002D3 RID: 723
		private List<SettlementNameplateWidget> _allChildrenNameplates = new List<SettlementNameplateWidget>();
	}
}
