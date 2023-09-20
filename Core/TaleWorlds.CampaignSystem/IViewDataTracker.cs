using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000093 RID: 147
	public interface IViewDataTracker
	{
		// Token: 0x060010ED RID: 4333
		void SetInventoryLocks(IEnumerable<string> locks);

		// Token: 0x060010EE RID: 4334
		IEnumerable<string> GetInventoryLocks();

		// Token: 0x060010EF RID: 4335
		bool GetMapBarExtendedState();

		// Token: 0x060010F0 RID: 4336
		void SetMapBarExtendedState(bool value);

		// Token: 0x060010F1 RID: 4337
		void SetPartyTroopLocks(IEnumerable<string> locks);

		// Token: 0x060010F2 RID: 4338
		void SetPartyPrisonerLocks(IEnumerable<string> locks);

		// Token: 0x060010F3 RID: 4339
		void SetPartySortType(int sortType);

		// Token: 0x060010F4 RID: 4340
		void SetIsPartySortAscending(bool isAscending);

		// Token: 0x060010F5 RID: 4341
		IEnumerable<string> GetPartyTroopLocks();

		// Token: 0x060010F6 RID: 4342
		IEnumerable<string> GetPartyPrisonerLocks();

		// Token: 0x060010F7 RID: 4343
		int GetPartySortType();

		// Token: 0x060010F8 RID: 4344
		bool GetIsPartySortAscending();

		// Token: 0x060010F9 RID: 4345
		void AddEncyclopediaBookmarkToItem(Concept concept);

		// Token: 0x060010FA RID: 4346
		void AddEncyclopediaBookmarkToItem(Kingdom kingdom);

		// Token: 0x060010FB RID: 4347
		void AddEncyclopediaBookmarkToItem(Settlement settlement);

		// Token: 0x060010FC RID: 4348
		void AddEncyclopediaBookmarkToItem(CharacterObject unit);

		// Token: 0x060010FD RID: 4349
		void AddEncyclopediaBookmarkToItem(Hero item);

		// Token: 0x060010FE RID: 4350
		void AddEncyclopediaBookmarkToItem(Clan clan);

		// Token: 0x060010FF RID: 4351
		void RemoveEncyclopediaBookmarkFromItem(Hero hero);

		// Token: 0x06001100 RID: 4352
		void RemoveEncyclopediaBookmarkFromItem(Clan clan);

		// Token: 0x06001101 RID: 4353
		void RemoveEncyclopediaBookmarkFromItem(Concept concept);

		// Token: 0x06001102 RID: 4354
		void RemoveEncyclopediaBookmarkFromItem(Kingdom kingdom);

		// Token: 0x06001103 RID: 4355
		void RemoveEncyclopediaBookmarkFromItem(Settlement settlement);

		// Token: 0x06001104 RID: 4356
		void RemoveEncyclopediaBookmarkFromItem(CharacterObject unit);

		// Token: 0x06001105 RID: 4357
		bool IsEncyclopediaBookmarked(Hero hero);

		// Token: 0x06001106 RID: 4358
		bool IsEncyclopediaBookmarked(Clan clan);

		// Token: 0x06001107 RID: 4359
		bool IsEncyclopediaBookmarked(Concept concept);

		// Token: 0x06001108 RID: 4360
		bool IsEncyclopediaBookmarked(Kingdom kingdom);

		// Token: 0x06001109 RID: 4361
		bool IsEncyclopediaBookmarked(Settlement settlement);

		// Token: 0x0600110A RID: 4362
		bool IsEncyclopediaBookmarked(CharacterObject unit);

		// Token: 0x0600110B RID: 4363
		void SetQuestSelection(QuestBase selection);

		// Token: 0x0600110C RID: 4364
		QuestBase GetQuestSelection();

		// Token: 0x0600110D RID: 4365
		void SetQuestSortTypeSelection(int questSortTypeSelection);

		// Token: 0x0600110E RID: 4366
		int GetQuestSortTypeSelection();

		// Token: 0x0600110F RID: 4367
		void InventorySetSortPreference(int inventoryMode, int sortOption, int sortState);

		// Token: 0x06001110 RID: 4368
		Tuple<int, int> InventoryGetSortPreference(int inventoryMode);
	}
}
