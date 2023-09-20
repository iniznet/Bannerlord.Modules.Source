using System;
using TaleWorlds.CampaignSystem.Roster;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000339 RID: 825
	public interface IMenuContextHandler
	{
		// Token: 0x06002E44 RID: 11844
		void OnBackgroundMeshNameSet(string name);

		// Token: 0x06002E45 RID: 11845
		void OnOpenTownManagement();

		// Token: 0x06002E46 RID: 11846
		void OnOpenRecruitVolunteers();

		// Token: 0x06002E47 RID: 11847
		void OnOpenTournamentLeaderboard();

		// Token: 0x06002E48 RID: 11848
		void OnOpenTroopSelection(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount);

		// Token: 0x06002E49 RID: 11849
		void OnMenuCreate();

		// Token: 0x06002E4A RID: 11850
		void OnMenuActivate();

		// Token: 0x06002E4B RID: 11851
		void OnHourlyTick();

		// Token: 0x06002E4C RID: 11852
		void OnPanelSoundIDSet(string panelSoundID);

		// Token: 0x06002E4D RID: 11853
		void OnAmbientSoundIDSet(string ambientSoundID);
	}
}
