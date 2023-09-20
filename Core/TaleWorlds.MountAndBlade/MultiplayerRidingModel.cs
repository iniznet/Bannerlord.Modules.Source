﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerRidingModel : RidingModel
	{
		public override float CalculateAcceleration(in EquipmentElement mountElement, in EquipmentElement harnessElement, int ridingSkill)
		{
			EquipmentElement equipmentElement = mountElement;
			float num = (float)equipmentElement.GetModifiedMountManeuver(harnessElement) * 0.008f;
			if (ridingSkill >= 0)
			{
				float num2 = num;
				float num3 = 0.7f;
				float num4 = 0.003f;
				float num5 = (float)ridingSkill;
				float num6 = 1.5f;
				equipmentElement = mountElement;
				num = num2 * (num3 + num4 * (num5 - num6 * (float)equipmentElement.Item.Difficulty));
			}
			return MathF.Clamp(num, 0.15f, 0.7f);
		}
	}
}
