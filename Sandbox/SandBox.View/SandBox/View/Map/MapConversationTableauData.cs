using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace SandBox.View.Map
{
	// Token: 0x02000041 RID: 65
	public class MapConversationTableauData
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00015056 File Offset: 0x00013256
		// (set) Token: 0x06000234 RID: 564 RVA: 0x0001505E File Offset: 0x0001325E
		public ConversationCharacterData PlayerCharacterData { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000235 RID: 565 RVA: 0x00015067 File Offset: 0x00013267
		// (set) Token: 0x06000236 RID: 566 RVA: 0x0001506F File Offset: 0x0001326F
		public ConversationCharacterData ConversationPartnerData { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00015078 File Offset: 0x00013278
		// (set) Token: 0x06000238 RID: 568 RVA: 0x00015080 File Offset: 0x00013280
		public TerrainType ConversationTerrainType { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000239 RID: 569 RVA: 0x00015089 File Offset: 0x00013289
		// (set) Token: 0x0600023A RID: 570 RVA: 0x00015091 File Offset: 0x00013291
		public bool IsCurrentTerrainUnderSnow { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600023B RID: 571 RVA: 0x0001509A File Offset: 0x0001329A
		// (set) Token: 0x0600023C RID: 572 RVA: 0x000150A2 File Offset: 0x000132A2
		public bool IsInside { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600023D RID: 573 RVA: 0x000150AB File Offset: 0x000132AB
		// (set) Token: 0x0600023E RID: 574 RVA: 0x000150B3 File Offset: 0x000132B3
		public float TimeOfDay { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600023F RID: 575 RVA: 0x000150BC File Offset: 0x000132BC
		// (set) Token: 0x06000240 RID: 576 RVA: 0x000150C4 File Offset: 0x000132C4
		public Settlement Settlement { get; private set; }

		// Token: 0x06000241 RID: 577 RVA: 0x000150CD File Offset: 0x000132CD
		private MapConversationTableauData()
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000150D5 File Offset: 0x000132D5
		public static MapConversationTableauData CreateFrom(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, TerrainType terrainType, float timeOfDay, bool isCurrentTerrainUnderSnow, Settlement settlement, bool isInside)
		{
			return new MapConversationTableauData
			{
				PlayerCharacterData = playerCharacterData,
				ConversationPartnerData = conversationPartnerData,
				ConversationTerrainType = terrainType,
				TimeOfDay = timeOfDay,
				IsCurrentTerrainUnderSnow = isCurrentTerrainUnderSnow,
				Settlement = settlement,
				IsInside = isInside
			};
		}
	}
}
