using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200004F RID: 79
	public interface ICampaignMission
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000781 RID: 1921
		GameState State { get; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000782 RID: 1922
		IMissionTroopSupplier AgentSupplier { get; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000783 RID: 1923
		// (set) Token: 0x06000784 RID: 1924
		Location Location { get; set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000785 RID: 1925
		// (set) Token: 0x06000786 RID: 1926
		Alley LastVisitedAlley { get; set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000787 RID: 1927
		MissionMode Mode { get; }

		// Token: 0x06000788 RID: 1928
		void SetMissionMode(MissionMode newMode, bool atStart);

		// Token: 0x06000789 RID: 1929
		void OnCloseEncounterMenu();

		// Token: 0x0600078A RID: 1930
		bool AgentLookingAtAgent(IAgent agent1, IAgent agent2);

		// Token: 0x0600078B RID: 1931
		void OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation);

		// Token: 0x0600078C RID: 1932
		void OnProcessSentence();

		// Token: 0x0600078D RID: 1933
		void OnConversationContinue();

		// Token: 0x0600078E RID: 1934
		bool CheckIfAgentCanFollow(IAgent agent);

		// Token: 0x0600078F RID: 1935
		void AddAgentFollowing(IAgent agent);

		// Token: 0x06000790 RID: 1936
		bool CheckIfAgentCanUnFollow(IAgent agent);

		// Token: 0x06000791 RID: 1937
		void RemoveAgentFollowing(IAgent agent);

		// Token: 0x06000792 RID: 1938
		void OnConversationPlay(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath);

		// Token: 0x06000793 RID: 1939
		void OnConversationStart(IAgent agent, bool setActionsInstantly);

		// Token: 0x06000794 RID: 1940
		void OnConversationEnd(IAgent agent);

		// Token: 0x06000795 RID: 1941
		void EndMission();
	}
}
