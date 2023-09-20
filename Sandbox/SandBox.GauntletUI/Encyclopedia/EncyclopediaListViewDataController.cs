using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.Library;

namespace SandBox.GauntletUI.Encyclopedia
{
	// Token: 0x02000035 RID: 53
	public class EncyclopediaListViewDataController
	{
		// Token: 0x060001DF RID: 479 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
		public EncyclopediaListViewDataController()
		{
			this._listData = new Dictionary<EncyclopediaPage, EncyclopediaListViewDataController.EncyclopediaListViewData>();
			foreach (EncyclopediaPage encyclopediaPage in Campaign.Current.EncyclopediaManager.GetEncyclopediaPages())
			{
				if (!this._listData.ContainsKey(encyclopediaPage))
				{
					this._listData.Add(encyclopediaPage, new EncyclopediaListViewDataController.EncyclopediaListViewData(new MBBindingList<EncyclopediaFilterGroupVM>(), 0, "", false));
				}
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000DA3C File Offset: 0x0000BC3C
		public void SaveListData(EncyclopediaListVM list, string id)
		{
			if (list != null && this._listData.ContainsKey(list.Page))
			{
				EncyclopediaListSortControllerVM sortController = list.SortController;
				int? num;
				if (sortController == null)
				{
					num = null;
				}
				else
				{
					EncyclopediaListSelectorVM sortSelection = sortController.SortSelection;
					num = ((sortSelection != null) ? new int?(sortSelection.SelectedIndex) : null);
				}
				int num2 = num ?? 0;
				Dictionary<EncyclopediaPage, EncyclopediaListViewDataController.EncyclopediaListViewData> listData = this._listData;
				EncyclopediaPage page = list.Page;
				MBBindingList<EncyclopediaFilterGroupVM> filterGroups = list.FilterGroups;
				int num3 = num2;
				EncyclopediaListSortControllerVM sortController2 = list.SortController;
				listData[page] = new EncyclopediaListViewDataController.EncyclopediaListViewData(filterGroups, num3, id, sortController2 != null && sortController2.GetSortOrder());
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000DADC File Offset: 0x0000BCDC
		public void LoadListData(EncyclopediaListVM list)
		{
			if (list != null && this._listData.ContainsKey(list.Page))
			{
				EncyclopediaListViewDataController.EncyclopediaListViewData encyclopediaListViewData = this._listData[list.Page];
				EncyclopediaListSortControllerVM sortController = list.SortController;
				if (sortController != null)
				{
					sortController.SetSortSelection(encyclopediaListViewData.SelectedSortIndex);
				}
				EncyclopediaListSortControllerVM sortController2 = list.SortController;
				if (sortController2 != null)
				{
					sortController2.SetSortOrder(encyclopediaListViewData.IsAscending);
				}
				list.CopyFiltersFrom(encyclopediaListViewData.Filters);
				list.LastSelectedItemId = encyclopediaListViewData.LastSelectedItemId;
			}
		}

		// Token: 0x040000FE RID: 254
		private Dictionary<EncyclopediaPage, EncyclopediaListViewDataController.EncyclopediaListViewData> _listData;

		// Token: 0x0200004A RID: 74
		private readonly struct EncyclopediaListViewData
		{
			// Token: 0x060002A5 RID: 677 RVA: 0x00012A08 File Offset: 0x00010C08
			public EncyclopediaListViewData(MBBindingList<EncyclopediaFilterGroupVM> filters, int selectedSortIndex, string lastSelectedItemId, bool isAscending)
			{
				Dictionary<EncyclopediaFilterItem, bool> dictionary = new Dictionary<EncyclopediaFilterItem, bool>();
				foreach (EncyclopediaFilterGroupVM encyclopediaFilterGroupVM in filters)
				{
					foreach (EncyclopediaListFilterVM encyclopediaListFilterVM in encyclopediaFilterGroupVM.Filters)
					{
						if (!dictionary.ContainsKey(encyclopediaListFilterVM.Filter))
						{
							dictionary.Add(encyclopediaListFilterVM.Filter, encyclopediaListFilterVM.IsSelected);
						}
					}
				}
				this.Filters = dictionary;
				this.SelectedSortIndex = selectedSortIndex;
				this.LastSelectedItemId = lastSelectedItemId;
				this.IsAscending = isAscending;
			}

			// Token: 0x0400019C RID: 412
			public readonly Dictionary<EncyclopediaFilterItem, bool> Filters;

			// Token: 0x0400019D RID: 413
			public readonly int SelectedSortIndex;

			// Token: 0x0400019E RID: 414
			public readonly string LastSelectedItemId;

			// Token: 0x0400019F RID: 415
			public readonly bool IsAscending;
		}
	}
}
