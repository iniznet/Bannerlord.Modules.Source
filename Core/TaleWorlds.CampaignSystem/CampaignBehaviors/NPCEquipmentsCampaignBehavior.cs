using System;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003B1 RID: 945
	public class NPCEquipmentsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600387B RID: 14459 RVA: 0x001011DD File Offset: 0x000FF3DD
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
		}

		// Token: 0x0600387C RID: 14460 RVA: 0x001011F6 File Offset: 0x000FF3F6
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600387D RID: 14461 RVA: 0x001011F8 File Offset: 0x000FF3F8
		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			foreach (CharacterObject characterObject in CharacterObject.All)
			{
				bool isTemplate = characterObject.IsTemplate;
			}
		}
	}
}
