using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200033F RID: 831
	public interface IMapStateHandler
	{
		// Token: 0x06002E8E RID: 11918
		void OnRefreshState();

		// Token: 0x06002E8F RID: 11919
		void OnMainPartyEncounter();

		// Token: 0x06002E90 RID: 11920
		void BeforeTick(float dt);

		// Token: 0x06002E91 RID: 11921
		void Tick(float dt);

		// Token: 0x06002E92 RID: 11922
		void AfterTick(float dt);

		// Token: 0x06002E93 RID: 11923
		void AfterWaitTick(float dt);

		// Token: 0x06002E94 RID: 11924
		void OnIdleTick(float dt);

		// Token: 0x06002E95 RID: 11925
		void OnSignalPeriodicEvents();

		// Token: 0x06002E96 RID: 11926
		void OnExit();

		// Token: 0x06002E97 RID: 11927
		void ResetCamera(bool resetDistance, bool teleportToMainParty);

		// Token: 0x06002E98 RID: 11928
		void TeleportCameraToMainParty();

		// Token: 0x06002E99 RID: 11929
		void FastMoveCameraToMainParty();

		// Token: 0x06002E9A RID: 11930
		bool IsCameraLockedToPlayerParty();

		// Token: 0x06002E9B RID: 11931
		void StartCameraAnimation(Vec2 targetPosition, float animationStopDuration);

		// Token: 0x06002E9C RID: 11932
		void OnHourlyTick();

		// Token: 0x06002E9D RID: 11933
		void OnMenuModeTick(float dt);

		// Token: 0x06002E9E RID: 11934
		void OnEnteringMenuMode(MenuContext menuContext);

		// Token: 0x06002E9F RID: 11935
		void OnExitingMenuMode();

		// Token: 0x06002EA0 RID: 11936
		void OnBattleSimulationStarted(BattleSimulation battleSimulation);

		// Token: 0x06002EA1 RID: 11937
		void OnBattleSimulationEnded();

		// Token: 0x06002EA2 RID: 11938
		void OnMapConversationStarts(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData);

		// Token: 0x06002EA3 RID: 11939
		void OnMapConversationOver();

		// Token: 0x06002EA4 RID: 11940
		void OnPlayerSiegeActivated();

		// Token: 0x06002EA5 RID: 11941
		void OnPlayerSiegeDeactivated();

		// Token: 0x06002EA6 RID: 11942
		void OnSiegeEngineClick(MatrixFrame siegeEngineFrame);
	}
}
