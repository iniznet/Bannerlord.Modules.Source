﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Localization;

namespace SandBox
{
	public class CompleteBuildingProjectCheat : GameplayCheatItem
	{
		public override void ExecuteCheat()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsFortification)
			{
				foreach (Building building in Settlement.CurrentSettlement.Town.Buildings)
				{
					if (building.CurrentLevel < 3)
					{
						Building building2 = building;
						int currentLevel = building2.CurrentLevel;
						building2.CurrentLevel = currentLevel + 1;
					}
				}
			}
		}

		public override TextObject GetName()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null)
			{
				TextObject textObject = new TextObject("{=5uXs8pS9}Complete All Building Projects in {SETTLEMENT_NAME}", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", currentSettlement.Name.ToString());
				return textObject;
			}
			return TextObject.Empty;
		}
	}
}
