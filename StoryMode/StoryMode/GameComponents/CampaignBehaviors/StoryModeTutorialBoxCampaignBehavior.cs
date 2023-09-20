using System;
using System.Collections.Generic;
using StoryMode.Quests.TutorialPhase;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x0200004F RID: 79
	public class StoryModeTutorialBoxCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x00019B8D File Offset: 0x00017D8D
		public MBReadOnlyList<CampaignTutorial> AvailableTutorials
		{
			get
			{
				return this._availableTutorials;
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x00019B95 File Offset: 0x00017D95
		public StoryModeTutorialBoxCampaignBehavior()
		{
			this._shownTutorials = new List<string>();
			this._availableTutorials = new MBList<CampaignTutorial>();
			this._tutorialBackup = new Dictionary<string, int>();
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00019BC0 File Offset: 0x00017DC0
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnTutorialCompletedEvent.AddNonSerializedListener(this, new Action<string>(this.OnTutorialCompleted));
			CampaignEvents.CollectAvailableTutorialsEvent.AddNonSerializedListener(this, new Action<List<CampaignTutorial>>(this.OnTutorialListRequested));
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			StoryModeEvents.OnTravelToVillageTutorialQuestStartedEvent.AddNonSerializedListener(this, new Action(this.OnTravelToVillageTutorialQuestStarted));
			Game.Current.EventManager.RegisterEvent<ResetAllTutorialsEvent>(new Action<ResetAllTutorialsEvent>(this.OnResetAllTutorials));
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00019C72 File Offset: 0x00017E72
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<string>>("_shownTutorials", ref this._shownTutorials);
			dataStore.SyncData<Dictionary<string, int>>("_tutorialBackup", ref this._tutorialBackup);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00019C98 File Offset: 0x00017E98
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.BackupTutorial("MovementInMissionTutorial", 5);
			int num = 100;
			this.BackupTutorial("EncyclopediaHomeTutorial", num++);
			this.BackupTutorial("EncyclopediaSettlementsTutorial", num++);
			this.BackupTutorial("EncyclopediaTroopsTutorial", num++);
			this.BackupTutorial("EncyclopediaKingdomsTutorial", num++);
			this.BackupTutorial("EncyclopediaClansTutorial", num++);
			this.BackupTutorial("EncyclopediaConceptsTutorial", num++);
			this.BackupTutorial("EncyclopediaTrackTutorial", num++);
			this.BackupTutorial("EncyclopediaSearchTutorial", num++);
			this.BackupTutorial("EncyclopediaFiltersTutorial", num++);
			this.BackupTutorial("EncyclopediaSortTutorial", num++);
			this.BackupTutorial("EncyclopediaFogOfWarTutorial", num++);
			this.BackupTutorial("RaidVillageStep1", num++);
			this.BackupTutorial("UpgradingTroopsStep1", num++);
			this.BackupTutorial("UpgradingTroopsStep2", num++);
			this.BackupTutorial("UpgradingTroopsStep3", num++);
			this.BackupTutorial("ChoosingPerkUpgradesStep1", num++);
			this.BackupTutorial("ChoosingPerkUpgradesStep2", num++);
			this.BackupTutorial("ChoosingPerkUpgradesStep3", num++);
			this.BackupTutorial("ChoosingSkillFocusStep1", num++);
			this.BackupTutorial("ChoosingSkillFocusStep2", num++);
			this.BackupTutorial("GettingCompanionsStep1", num++);
			this.BackupTutorial("GettingCompanionsStep2", num++);
			this.BackupTutorial("GettingCompanionsStep3", num++);
			this.BackupTutorial("RansomingPrisonersStep1", num++);
			this.BackupTutorial("RansomingPrisonersStep2", num++);
			this.BackupTutorial("CivilianEquipment", num++);
			this.BackupTutorial("PartySpeed", num++);
			this.BackupTutorial("ArmyCohesionStep1", num++);
			this.BackupTutorial("ArmyCohesionStep2", num++);
			this.BackupTutorial("CreateArmyStep2", num++);
			this.BackupTutorial("CreateArmyStep3", num++);
			this.BackupTutorial("OrderOfBattleTutorialStep1", num++);
			this.BackupTutorial("OrderOfBattleTutorialStep2", num++);
			this.BackupTutorial("OrderOfBattleTutorialStep3", num++);
			this.BackupTutorial("CraftingStep1Tutorial", num++);
			this.BackupTutorial("CraftingOrdersTutorial", num++);
			this.BackupTutorial("InventoryBannerItemTutorial", num++);
			this.BackupTutorial("CrimeTutorial", num++);
			this.BackupTutorial("AssignRolesTutorial", num++);
			this.BackupTutorial("BombardmentStep1", num++);
			this.BackupTutorial("KingdomDecisionVotingTutorial", num++);
			foreach (KeyValuePair<string, int> keyValuePair in this._tutorialBackup)
			{
				this.AddTutorial(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00019F98 File Offset: 0x00018198
		private void OnTravelToVillageTutorialQuestStarted()
		{
			this.AddTutorial("SeeMarkersInMissionTutorial", 1);
			this.AddTutorial("NavigateOnMapTutorialStep1", 2);
			this.AddTutorial("NavigateOnMapTutorialStep2", 3);
			this.AddTutorial("EnterVillageTutorial", 4);
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00019FCC File Offset: 0x000181CC
		private void OnQuestStarted(QuestBase quest)
		{
			if (quest is PurchaseGrainTutorialQuest)
			{
				this.AddTutorial("PressLeaveToReturnFromMissionType1", 10);
				this.AddTutorial("GetSuppliesTutorialStep1", 20);
				this.AddTutorial("GetSuppliesTutorialStep3", 22);
			}
			else if (quest is RecruitTroopsTutorialQuest)
			{
				this.AddTutorial("RecruitmentTutorialStep1", 11);
				this.AddTutorial("RecruitmentTutorialStep2", 12);
			}
			else if (quest is LocateAndRescueTravellerTutorialQuest)
			{
				this.AddTutorial("PressLeaveToReturnFromMissionType2", 30);
				this.AddTutorial("OrderTutorial1TutorialStep2", 33);
				this.AddTutorial("TakeAndRescuePrisonerTutorial", 34);
				this.AddTutorial("OrderTutorial2Tutorial", 35);
			}
			this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
			{
				int priority = x.Priority;
				return priority.CompareTo(y.Priority);
			});
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x0001A094 File Offset: 0x00018294
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.RecruitAndPurchaseStarted && (quest is RecruitTroopsTutorialQuest || quest is PurchaseGrainTutorialQuest))
			{
				this.AddTutorial("TalkToNotableTutorialStep1", 40);
				this.AddTutorial("TalkToNotableTutorialStep2", 41);
			}
			this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
			{
				int priority = x.Priority;
				return priority.CompareTo(y.Priority);
			});
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0001A104 File Offset: 0x00018304
		private void OnTutorialCompleted(string completedTutorialType)
		{
			CampaignTutorial campaignTutorial = this._availableTutorials.Find((CampaignTutorial t) => t.TutorialTypeId == completedTutorialType);
			if (campaignTutorial != null)
			{
				this._availableTutorials.Remove(campaignTutorial);
				this._shownTutorials.Add(completedTutorialType);
				this._tutorialBackup.Remove(completedTutorialType);
			}
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001A16C File Offset: 0x0001836C
		private void OnTutorialListRequested(List<CampaignTutorial> campaignTutorials)
		{
			if (!BannerlordConfig.EnableTutorialHints)
			{
				return;
			}
			MBTextManager.SetTextVariable("TUTORIAL_SETTLEMENT_NAME", MBObjectManager.Instance.GetObject<Settlement>("village_ES3_2").Name, false);
			foreach (CampaignTutorial campaignTutorial in this.AvailableTutorials)
			{
				campaignTutorials.Add(campaignTutorial);
			}
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0001A1E8 File Offset: 0x000183E8
		private void BackupTutorial(string tutorialTypeId, int priority)
		{
			if (!this._shownTutorials.Contains(tutorialTypeId) && !this._tutorialBackup.ContainsKey(tutorialTypeId))
			{
				this._tutorialBackup.Add(tutorialTypeId, priority);
			}
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0001A214 File Offset: 0x00018414
		private void AddTutorial(string tutorialTypeId, int priority)
		{
			if (!this._shownTutorials.Contains(tutorialTypeId))
			{
				CampaignTutorial campaignTutorial = new CampaignTutorial(tutorialTypeId, priority);
				this._availableTutorials.Add(campaignTutorial);
				if (!this._tutorialBackup.ContainsKey(tutorialTypeId))
				{
					this._tutorialBackup.Add(tutorialTypeId, priority);
				}
			}
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0001A25E File Offset: 0x0001845E
		public void OnResetAllTutorials(ResetAllTutorialsEvent obj)
		{
			this._shownTutorials.Clear();
		}

		// Token: 0x040001C0 RID: 448
		private List<string> _shownTutorials;

		// Token: 0x040001C1 RID: 449
		private readonly MBList<CampaignTutorial> _availableTutorials;

		// Token: 0x040001C2 RID: 450
		private Dictionary<string, int> _tutorialBackup;
	}
}
