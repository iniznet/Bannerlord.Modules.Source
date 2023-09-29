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
	public class AssembleEmpireQuestBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			StoryModeEvents.OnMainStoryLineSideChosenEvent.AddNonSerializedListener(this, new Action<MainStoryLineSide>(this.OnMainStoryLineSideChosen));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnMainStoryLineSideChosen(MainStoryLineSide side)
		{
			if (side == MainStoryLineSide.CreateImperialKingdom || side == MainStoryLineSide.SupportImperialKingdom)
			{
				new AssembleEmpireQuestBehavior.AssembleEmpireQuest(StoryModeHeroes.ImperialMentor).StartQuest();
			}
		}

		public class AssembleEmpireQuestBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public AssembleEmpireQuestBehaviorTypeDefiner()
				: base(1002000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(AssembleEmpireQuestBehavior.AssembleEmpireQuest), 1, null);
			}
		}

		public class AssembleEmpireQuest : StoryModeQuestBase
		{
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=ya8eMCpj}Unify the Empire", null);
				}
			}

			private TextObject _questCanceledLogText
			{
				get
				{
					return new TextObject("{=tVlZTOst}You have chosen a different path.", null);
				}
			}

			public AssembleEmpireQuest(Hero questGiver)
				: base("assemble_empire_quest", questGiver, CampaignTime.Never)
			{
				this._assembledEmpire = false;
				this.CacheSettlementCounts();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
				this._numberOfCapturedSettlementsLog = base.AddDiscreteLog(new TextObject("{=3deb2lMd}To restore the Empire you should capture two thirds of settlements with imperial culture.", null), new TextObject("{=Dp6newHS}Conquered Settlements", null), this._ownedByPlayerImperialTowns, MathF.Ceiling((float)this._imperialCultureTowns * 0.66f), null, false);
			}

			protected override void SetDialogs()
			{
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=mxKhvbn7}You have decided to unify the Empire.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.CloseDialog();
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.CacheSettlementCounts();
				this.SetDialogs();
				if (this._numberOfCapturedSettlementsLog == null)
				{
					this._numberOfCapturedSettlementsLog = base.AddDiscreteLog(new TextObject("{=3deb2lMd}To restore the Empire you should capture two thirds of settlements with imperial culture.", null), new TextObject("{=Dp6newHS}Conquered Settlements", null), this._ownedByPlayerImperialTowns, MathF.Ceiling((float)this._imperialCultureTowns * 0.66f), null, false);
				}
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				StoryModeEvents.OnConspiracyActivatedEvent.AddNonSerializedListener(this, new Action(this.OnConspiracyActivated));
				CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (clan == Clan.PlayerClan && oldKingdom == StoryModeManager.Current.MainStoryLine.PlayerSupportedKingdom)
				{
					base.CompleteQuestWithCancel(this._questCanceledLogText);
					StoryModeManager.Current.MainStoryLine.CancelSecondAndThirdPhase();
				}
			}

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

			protected override void HourlyTick()
			{
				if (this.QuestConditionsHold())
				{
					this.SuccessQuest();
				}
			}

			private void OnConspiracyActivated()
			{
				if (!this._assembledEmpire)
				{
					base.CompleteQuestWithFail(new TextObject("{=80NOk1Ee}You could not unify the Empire.", null));
				}
			}

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

			private bool QuestConditionsHold()
			{
				return (float)this._ownedByPlayerImperialTowns >= (float)this._imperialCultureTowns * 0.66f;
			}

			private void SuccessQuest()
			{
				base.AddLog(new TextObject("{=sJeYHMGG}You have unified the Empire.", null), false);
				base.CompleteQuestWithSuccess();
				this._assembledEmpire = true;
				SecondPhase.Instance.ActivateConspiracy();
			}

			internal static void AutoGeneratedStaticCollectObjectsAssembleEmpireQuest(object o, List<object> collectedObjects)
			{
				((AssembleEmpireQuestBehavior.AssembleEmpireQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._numberOfCapturedSettlementsLog);
			}

			internal static object AutoGeneratedGetMemberValue_numberOfCapturedSettlementsLog(object o)
			{
				return ((AssembleEmpireQuestBehavior.AssembleEmpireQuest)o)._numberOfCapturedSettlementsLog;
			}

			private int _imperialCultureTowns;

			private int _ownedByPlayerImperialTowns;

			private bool _assembledEmpire;

			[SaveableField(1)]
			private JournalLog _numberOfCapturedSettlementsLog;
		}
	}
}
