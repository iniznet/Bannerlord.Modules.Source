using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace StoryMode.Quests.SecondPhase
{
	// Token: 0x02000027 RID: 39
	public class WeakenEmpireQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x060001BE RID: 446 RVA: 0x0000A4B5 File Offset: 0x000086B5
		public override void RegisterEvents()
		{
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.OnMainStoryLineSideChosen));
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000A4CE File Offset: 0x000086CE
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000A4D0 File Offset: 0x000086D0
		private void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			if (side == MainStoryLineSide.CreateAntiImperialKingdom || side == MainStoryLineSide.SupportAntiImperialKingdom)
			{
				new WeakenEmpireQuestBehavior.WeakenEmpireQuest(StoryModeHeroes.AntiImperialMentor).StartQuest();
			}
		}

		// Token: 0x0200006A RID: 106
		public class WeakenEmpireQuestBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x060005E8 RID: 1512 RVA: 0x00021B73 File Offset: 0x0001FD73
			public WeakenEmpireQuestBehaviorTypeDefiner()
				: base(1005000)
			{
			}

			// Token: 0x060005E9 RID: 1513 RVA: 0x00021B80 File Offset: 0x0001FD80
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(WeakenEmpireQuestBehavior.WeakenEmpireQuest), 1, null);
			}
		}

		// Token: 0x0200006B RID: 107
		public class WeakenEmpireQuest : StoryModeQuestBase
		{
			// Token: 0x170000D6 RID: 214
			// (get) Token: 0x060005EA RID: 1514 RVA: 0x00021B94 File Offset: 0x0001FD94
			private TextObject _startQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=0wQlpbtL}In order for the Empire to go into its final decline, there should be fewer than {NUMBER} imperial-owned settlements. If this happens, another kingdom can become the dominant power in Calradia.", null);
					textObject.SetTextVariable("NUMBER", 4);
					return textObject;
				}
			}

			// Token: 0x170000D7 RID: 215
			// (get) Token: 0x060005EB RID: 1515 RVA: 0x00021BAE File Offset: 0x0001FDAE
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=iR4QCTxv}Weaken Empire", null);
				}
			}

			// Token: 0x060005EC RID: 1516 RVA: 0x00021BBB File Offset: 0x0001FDBB
			public WeakenEmpireQuest(Hero questGiver)
				: base("weaken_empire_quest", questGiver, CampaignTime.Never)
			{
				this._weakenedEmpire = false;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
				base.AddLog(this._startQuestLog, false);
			}

			// Token: 0x060005ED RID: 1517 RVA: 0x00021BEF File Offset: 0x0001FDEF
			protected override void SetDialogs()
			{
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=VeY3PQFL}You chose to defeat the Empire.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.CloseDialog();
			}

			// Token: 0x060005EE RID: 1518 RVA: 0x00021C2B File Offset: 0x0001FE2B
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			// Token: 0x060005EF RID: 1519 RVA: 0x00021C34 File Offset: 0x0001FE34
			protected override void RegisterEvents()
			{
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			}

			// Token: 0x060005F0 RID: 1520 RVA: 0x00021C86 File Offset: 0x0001FE86
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x060005F1 RID: 1521 RVA: 0x00021CA9 File Offset: 0x0001FEA9
			protected void HourlyTick()
			{
				if (this.QuestConditionsHold())
				{
					this.SuccessComplete();
				}
			}

			// Token: 0x060005F2 RID: 1522 RVA: 0x00021CB9 File Offset: 0x0001FEB9
			private void OnConspiracyActivated()
			{
				if (!this._weakenedEmpire)
				{
					base.CompleteQuestWithFail(new TextObject("{=JVkPkbdg}You could not weaken the Empire.", null));
				}
			}

			// Token: 0x060005F3 RID: 1523 RVA: 0x00021CD4 File Offset: 0x0001FED4
			private bool QuestConditionsHold()
			{
				return StoryModeData.NorthernEmpireKingdom.Fiefs.Count((Town f) => f.IsTown) + StoryModeData.WesternEmpireKingdom.Fiefs.Count((Town f) => f.IsTown) + StoryModeData.SouthernEmpireKingdom.Fiefs.Count((Town f) => f.IsTown) < 4;
			}

			// Token: 0x060005F4 RID: 1524 RVA: 0x00021D70 File Offset: 0x0001FF70
			private void SuccessComplete()
			{
				base.AddLog(new TextObject("{=wO19nK2y}You have weakened the Empire.", null), false);
				base.CompleteQuestWithSuccess();
				this._weakenedEmpire = true;
				SecondPhase.Instance.ActivateConspiracy();
			}

			// Token: 0x060005F5 RID: 1525 RVA: 0x00021D9C File Offset: 0x0001FF9C
			internal static void AutoGeneratedStaticCollectObjectsWeakenEmpireQuest(object o, List<object> collectedObjects)
			{
				((WeakenEmpireQuestBehavior.WeakenEmpireQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060005F6 RID: 1526 RVA: 0x00021DAA File Offset: 0x0001FFAA
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0400020B RID: 523
			private const int EmpireDefeatSettlementCount = 4;

			// Token: 0x0400020C RID: 524
			private bool _weakenedEmpire;
		}
	}
}
