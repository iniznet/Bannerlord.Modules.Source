using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View
{
	public class CraftedDataViewManager
	{
		public static void Initialize()
		{
			CraftedDataViewManager._craftedDataViews = new Dictionary<WeaponDesign, CraftedDataView>();
		}

		public static void Clear()
		{
			foreach (CraftedDataView craftedDataView in CraftedDataViewManager._craftedDataViews.Values)
			{
				craftedDataView.Clear();
			}
			CraftedDataViewManager._craftedDataViews.Clear();
		}

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

		private static Dictionary<WeaponDesign, CraftedDataView> _craftedDataViews;
	}
}
