using System;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class NPCEquipmentsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			foreach (CharacterObject characterObject in CharacterObject.All)
			{
				bool isTemplate = characterObject.IsTemplate;
			}
		}
	}
}
