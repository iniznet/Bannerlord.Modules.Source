using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A8 RID: 680
	public class MultiplayerPreloadHelper : MissionNetwork
	{
		// Token: 0x06002591 RID: 9617 RVA: 0x0008E5FC File Offset: 0x0008C7FC
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
