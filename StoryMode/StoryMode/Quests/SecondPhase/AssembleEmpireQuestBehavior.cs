using System;
using System.Collections.Generic;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace StoryMode.Quests.SecondPhase
{
	// Token: 0x02000024 RID: 36
	public class AssembleEmpireQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600019D RID: 413 RVA: 0x00009E1F File Offset: 0x0000801F
		public override void RegisterEvents()
		{
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.OnMainStoryLineSideChosen));
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00009E38 File Offset: 0x00008038
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00009E3A File Offset: 0x0000803A
		private void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			if (side == MainStoryLineSide.CreateImperialKingdom || side == MainStoryLineSide.SupportImperialKingdom)
			{
				new AssembleEmpireQuestBehavior.AssembleEmpireQuest(StoryModeHeroes.ImperialMentor).StartQuest();
			}
		}

		// Token: 0x02000065 RID: 101
		public class AssembleEmpireQuestBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x060005CD RID: 1485 RVA: 0x00021724 File Offset: 0x0001F924
			public AssembleEmpireQuestBehaviorTypeDefiner()
				: base(1002000)
			{
			}

			// Token: 0x060005CE RID: 1486 RVA: 0x00021731 File Offset: 0x0001F931
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(AssembleEmpireQuestBehavior.AssembleEmpireQuest), 1, null);
			}
		}

		// Token: 0x02000066 RID: 102
		public class AssembleEmpireQuest : StoryModeQuestBase
		{
			// Token: 0x170000D5 RID: 213
			// (get) Token: 0x060005CF RID: 1487 RVA: 0x00021745 File Offset: 0x0001F945
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=ya8eMCpj}Unify the Empire", null);
				}
			}

			// Token: 0x060005D0 RID: 1488 RVA: 0x00021754 File Offset: 0x0001F954
			public AssembleEmpireQuest(Hero questGiver)
				: base("assemble_empire_quest", questGiver, CampaignTime.Never)
			{
				this._assembledEmpire = false;
				this.CacheSettlementCounts();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
				this._numberOfCapturedSettlementsLog = base.AddDiscreteLog(new TextObject("{=3deb2lMd}To restore the Empire you should capture two thirds of settlements with imperial culture.", null), new TextObject("{=Dp6newHS}Conquered Settlements", null), this._ownedByPlayerImperialTowns, MathF.Ceiling((float)this._imperialCultureTowns * 0.66f), null, false);
			}

			// Token: 0x060005D1 RID: 1489 RVA: 0x000217C7 File Offset: 0x0001F9C7
			protected override void SetDialogs()
			{
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=mxKhvbn7}You have decided to unify the Empire.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.CloseDialog();
			}

			// Token: 0x060005D2 RID: 1490 RVA: 0x00021804 File Offset: 0x0001FA04
			protected override void InitializeQuestOnGameLoad()
			{
				this.CacheSettlementCounts();
				this.SetDialogs();
				if (this._numberOfCapturedSettlementsLog == null)
				{
					this._numberOfCapturedSettlementsLog = base.AddDiscreteLog(new TextObject("{=3deb2lMd}To restore the Empire you should capture two thirds of settlements with imperial culture.", null), new TextObject("{=Dp6newHS}Conquered Settlements", null), this._ownedByPlayerImperialTowns, MathF.Ceiling((float)this._imperialCultureTowns * 0.66f), null, false);
				}
			}

			// Token: 0x060005D3 RID: 1491 RVA: 0x00021864 File Offset: 0x0001FA64
			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			}

			// Token: 0x060005D4 RID: 1492 RVA: 0x000218CD File Offset: 0x0001FACD
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x060005D5 RID: 1493 RVA: 0x000218F0 File Offset: 0x0001FAF0
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement.IsTown && settlement.Culture.StringId == "empire")
				{
					if (settlement.OwnerClan.Kingdom == Clan.PlayerClan.Kingdom)
					{
						this._ownedByPlayerImperialTowns++;
					}
					if (oldOwner.Clan.Kingdom == Clan.PlayerClan.Kingdom && newOwner.Clan.Kingdom != Clan.PlayerClan.Kingdom)
					{
						this._ownedByPlayerImperialTowns--;
					}
					this._numberOfCapturedSettlementsLog.UpdateCurrentProgress((int)MathF.Clamp((float)this._ownedByPlayerImperialTowns, 0f, (float)this._imperialCultureTowns));
				}
			}

			// Token: 0x060005D6 RID: 1494 RVA: 0x000219A8 File Offset: 0x0001FBA8
			private void HourlyTick()
			{
				if (this.QuestConditionsHold())
				{
					this.SuccessQuest();
				}
			}

			// Token: 0x060005D7 RID: 1495 RVA: 0x000219B8 File Offset: 0x0001FBB8
			private void OnConspiracyActivated()
			{
				if (!this._assembledEmpire)
				{
					base.CompleteQuestWithFail(new TextObject("{=80NOk1Ee}You could not unify the Empire.", null));
				}
			}

			// Token: 0x060005D8 RID: 1496 RVA: 0x000219D4 File Offset: 0x0001FBD4
			private void CacheSettlementCounts()
			{
				this._imperialCultureTowns = 0;
				this._ownedByPlayerImperialTowns = 0;
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.IsTown && settlement.Culture.StringId == "empire")
					{
						this._imperialCultureTowns++;
						if (settlement.OwnerClan.Kingdom == Clan.PlayerClan.Kingdom)
						{
							this._ownedByPlayerImperialTowns++;
						}
					}
				}
			}

			// Token: 0x060005D9 RID: 1497 RVA: 0x00021A80 File Offset: 0x0001FC80
			private bool QuestConditionsHold()
			{
				return (float)this._ownedByPlayerImperialTowns >= (float)this._imperialCultureTowns * 0.66f;
			}

			// Token: 0x060005DA RID: 1498 RVA: 0x00021A9B File Offset: 0x0001FC9B
			private void SuccessQuest()
			{
				base.AddLog(new TextObject("{=sJeYHMGG}You have unified the Empire.", null), false);
				base.CompleteQuestWithSuccess();
				this._assembledEmpire = true;
				SecondPhase.Instance.ActivateConspiracy();
			}

			// Token: 0x060005DB RID: 1499 RVA: 0x00021AC7 File Offset: 0x0001FCC7
			internal static void AutoGeneratedStaticCollectObjectsAssembleEmpireQuest(object o, List<object> collectedObjects)
			{
				((AssembleEmpireQuestBehavior.AssembleEmpireQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060005DC RID: 1500 RVA: 0x00021AD5 File Offset: 0x0001FCD5
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._numberOfCapturedSettlementsLog);
			}

			// Token: 0x060005DD RID: 1501 RVA: 0x00021AEA File Offset: 0x0001FCEA
			internal static object AutoGeneratedGetMemberValue_numberOfCapturedSettlementsLog(object o)
			{
				return ((AssembleEmpireQuestBehavior.AssembleEmpireQuest)o)._numberOfCapturedSettlementsLog;
			}

			// Token: 0x04000202 RID: 514
			private int _imperialCultureTowns;

			// Token: 0x04000203 RID: 515
			private int _ownedByPlayerImperialTowns;

			// Token: 0x04000204 RID: 516
			private bool _assembledEmpire;

			// Token: 0x04000205 RID: 517
			[SaveableField(1)]
			private JournalLog _numberOfCapturedSettlementsLog;
		}
	}
}
