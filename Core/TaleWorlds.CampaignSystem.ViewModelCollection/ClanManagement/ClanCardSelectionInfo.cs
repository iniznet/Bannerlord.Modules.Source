using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000FC RID: 252
	public readonly struct ClanCardSelectionInfo
	{
		// Token: 0x0600177D RID: 6013 RVA: 0x00056ACD File Offset: 0x00054CCD
		public ClanCardSelectionInfo(TextObject title, IEnumerable<ClanCardSelectionItemInfo> items, Action<List<object>, Action> onClosedAction, bool isMultiSelection)
		{
			this.Title = title;
			this.Items = items;
			this.OnClosedAction = onClosedAction;
			this.IsMultiSelection = isMultiSelection;
		}

		// Token: 0x04000B18 RID: 2840
		public readonly TextObject Title;

		// Token: 0x04000B19 RID: 2841
		public readonly IEnumerable<ClanCardSelectionItemInfo> Items;

		// Token: 0x04000B1A RID: 2842
		public readonly Action<List<object>, Action> OnClosedAction;

		// Token: 0x04000B1B RID: 2843
		public readonly bool IsMultiSelection;
	}
}
