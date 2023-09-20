using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public readonly struct ClanCardSelectionInfo
	{
		public ClanCardSelectionInfo(TextObject title, IEnumerable<ClanCardSelectionItemInfo> items, Action<List<object>, Action> onClosedAction, bool isMultiSelection)
		{
			this.Title = title;
			this.Items = items;
			this.OnClosedAction = onClosedAction;
			this.IsMultiSelection = isMultiSelection;
		}

		public readonly TextObject Title;

		public readonly IEnumerable<ClanCardSelectionItemInfo> Items;

		public readonly Action<List<object>, Action> OnClosedAction;

		public readonly bool IsMultiSelection;
	}
}
