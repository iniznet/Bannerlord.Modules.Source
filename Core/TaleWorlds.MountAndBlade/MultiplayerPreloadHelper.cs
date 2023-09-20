using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerPreloadHelper : MissionNetwork
	{
		public override List<EquipmentElement> GetExtraEquipmentElementsForCharacter(BasicCharacterObject character, bool getAllEquipments = false)
		{
			List<EquipmentElement> list = new List<EquipmentElement>();
			foreach (List<IReadOnlyPerkObject> list2 in MultiplayerClassDivisions.GetAllPerksForHeroClass(MultiplayerClassDivisions.GetMPHeroClassForCharacter(character), null))
			{
				List<ValueTuple<EquipmentIndex, EquipmentElement>> list3 = null;
				foreach (IReadOnlyPerkObject readOnlyPerkObject in list2)
				{
					int num = ((list3 != null) ? list3.Count : 0);
					list3 = readOnlyPerkObject.GetAlternativeEquipments(false, true, list3, getAllEquipments);
					int num2 = ((list3 != null) ? list3.Count : 0);
					for (int i = num; i < num2; i++)
					{
						list.Add(list3[i].Item2);
					}
				}
			}
			return list;
		}
	}
}
