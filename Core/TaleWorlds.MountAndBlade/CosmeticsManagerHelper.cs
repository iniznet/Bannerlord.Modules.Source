using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public static class CosmeticsManagerHelper
	{
		public static Dictionary<int, List<int>> GetUsedIndicesFromIds(Dictionary<string, List<string>> usedCosmetics)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in usedCosmetics)
			{
				int num = -1;
				for (int i = 0; i < objectTypeList.Count; i++)
				{
					if (objectTypeList[i].StringId == keyValuePair.Key)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					List<int> list = new List<int>();
					foreach (string text in keyValuePair.Value)
					{
						int num2 = -1;
						for (int j = 0; j < CosmeticsManager.GetCosmeticElementList.Count; j++)
						{
							if (CosmeticsManager.GetCosmeticElementList[j].Id == text)
							{
								num2 = j;
								break;
							}
						}
						if (num2 >= 0)
						{
							list.Add(num2);
						}
					}
					if (list.Count > 0)
					{
						dictionary.Add(num, list);
					}
				}
			}
			return dictionary;
		}
	}
}
