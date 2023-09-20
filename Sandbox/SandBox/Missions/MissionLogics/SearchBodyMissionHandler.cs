using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics
{
	public class SearchBodyMissionHandler : MissionLogic
	{
		public override void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			if (Campaign.Current.GameMode == 1)
			{
				if (Game.Current.GameStateManager.ActiveState is MissionState)
				{
					if (base.Mission.Mode != 1 && base.Mission.Mode != 2 && this.IsSearchable(agent))
					{
						this.AddItemsToPlayer(agent);
						return;
					}
				}
				else
				{
					Debug.FailedAssert("Agent interaction must occur in MissionState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\SearchBodyMissionHandler.cs", "OnAgentInteraction", 26);
				}
			}
		}

		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return Mission.Current.Mode != 2 && base.Mission.Mode != 3 && base.Mission.Mode != 1 && this.IsSearchable(otherAgent);
		}

		private bool IsSearchable(Agent agent)
		{
			return !agent.IsActive() && agent.IsHuman && agent.Character.IsHero;
		}

		private void AddItemsToPlayer(Agent interactedAgent)
		{
			CharacterObject characterObject = (CharacterObject)interactedAgent.Character;
			if (MBRandom.RandomInt(2) == 0)
			{
				characterObject.HeroObject.SpecialItems.Add(MBObjectManager.Instance.GetObject<ItemObject>("leafblade_throwing_knife"));
			}
			else
			{
				characterObject.HeroObject.SpecialItems.Add(MBObjectManager.Instance.GetObject<ItemObject>("falchion_sword_t2"));
				characterObject.HeroObject.SpecialItems.Add(MBObjectManager.Instance.GetObject<ItemObject>("cleaver_sword_t3"));
			}
			foreach (ItemObject itemObject in characterObject.HeroObject.SpecialItems)
			{
				PartyBase.MainParty.ItemRoster.AddToCounts(itemObject, 1);
				MBTextManager.SetTextVariable("ITEM_NAME", itemObject.Name, false);
				InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_item_taken", null).ToString()));
			}
			characterObject.HeroObject.SpecialItems.Clear();
		}
	}
}
