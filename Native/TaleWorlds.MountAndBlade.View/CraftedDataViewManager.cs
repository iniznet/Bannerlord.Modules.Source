using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200000C RID: 12
	public class CraftedDataViewManager
	{
		// Token: 0x06000056 RID: 86 RVA: 0x000042C9 File Offset: 0x000024C9
		public static void Initialize()
		{
			CraftedDataViewManager._craftedDataViews = new Dictionary<WeaponDesign, CraftedDataView>();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000042D8 File Offset: 0x000024D8
		public static void Clear()
		{
			foreach (CraftedDataView craftedDataView in CraftedDataViewManager._craftedDataViews.Values)
			{
				craftedDataView.Clear();
			}
			CraftedDataViewManager._craftedDataViews.Clear();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004338 File Offset: 0x00002538
		public static CraftedDataView GetCraftedDataView(WeaponDesign craftedData)
		{
			if (craftedData != null)
			{
				CraftedDataView craftedDataView;
				if (!CraftedDataViewManager._craftedDataViews.TryGetValue(craftedData, out craftedDataView))
				{
					craftedDataView = new CraftedDataView(craftedData);
					CraftedDataViewManager._craftedDataViews.Add(craftedData, craftedDataView);
				}
				return craftedDataView;
			}
			return null;
		}

		// Token: 0x04000015 RID: 21
		private static Dictionary<WeaponDesign, CraftedDataView> _craftedDataViews;
	}
}
