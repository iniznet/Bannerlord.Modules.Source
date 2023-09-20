using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	// Token: 0x02000048 RID: 72
	public class StoryModeTroopSupplierProbabilityModel : DefaultTroopSupplierProbabilityModel
	{
		// Token: 0x060003C9 RID: 969 RVA: 0x00017618 File Offset: 0x00015818
		public override void EnqueueTroopSpawnProbabilitiesAccordingToUnitSpawnPrioritization(MapEventParty battleParty, FlattenedTroopRoster priorityTroops, bool includePlayers, int sizeOfSide, bool forcePriorityTroops, List<ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>> priorityList)
		{
			int count = priorityList.Count;
			base.EnqueueTroopSpawnProbabilitiesAccordingToUnitSpawnPrioritization(battleParty, priorityTroops, includePlayers, sizeOfSide, forcePriorityTroops, priorityList);
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null && currentSettlement.IsHideout && priorityTroops != null)
			{
				if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
				{
					for (int i = count; i < priorityList.Count; i++)
					{
						CharacterObject character2 = priorityList[i].Item1.Troop;
						if (character2 == StoryModeHeroes.Radagos.CharacterObject && priorityTroops.All((FlattenedTroopRosterElement t) => t.Troop != character2))
						{
							priorityList[i] = new ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>(priorityList[i].Item1, priorityList[i].Item2, 0.01f);
							return;
						}
					}
					return;
				}
				for (int j = 0; j < priorityList.Count; j++)
				{
					CharacterObject character = priorityList[j].Item1.Troop;
					if (character == StoryModeHeroes.RadagosHencman.CharacterObject && priorityTroops.All((FlattenedTroopRosterElement t) => t.Troop != character))
					{
						priorityList[j] = new ValueTuple<FlattenedTroopRosterElement, MapEventParty, float>(priorityList[j].Item1, priorityList[j].Item2, 0.01f);
						return;
					}
				}
			}
		}
	}
}
