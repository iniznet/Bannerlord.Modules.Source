using System;
using System.Collections.Generic;
using StoryMode.GameComponents.CampaignBehaviors;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace StoryMode
{
	// Token: 0x0200000A RID: 10
	public class MainStoryLine
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000220C File Offset: 0x0000040C
		public bool IsPlayerInteractionRestricted
		{
			get
			{
				return !this.TutorialPhase.IsCompleted && !this.IsOnImperialQuestLine && !this.IsOnAntiImperialQuestLine;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002230 File Offset: 0x00000430
		public bool IsOnImperialQuestLine
		{
			get
			{
				return this.MainStoryLineSide == MainStoryLineSide.CreateImperialKingdom || this.MainStoryLineSide == MainStoryLineSide.SupportImperialKingdom;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002246 File Offset: 0x00000446
		public bool IsOnAntiImperialQuestLine
		{
			get
			{
				return this.MainStoryLineSide == MainStoryLineSide.CreateAntiImperialKingdom || this.MainStoryLineSide == MainStoryLineSide.SupportAntiImperialKingdom;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000020 RID: 32 RVA: 0x0000225C File Offset: 0x0000045C
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002264 File Offset: 0x00000464
		[SaveableProperty(2)]
		public TutorialPhase TutorialPhase { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000226D File Offset: 0x0000046D
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002275 File Offset: 0x00000475
		[SaveableProperty(3)]
		public FirstPhase FirstPhase { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000227E File Offset: 0x0000047E
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002286 File Offset: 0x00000486
		[SaveableProperty(4)]
		public SecondPhase SecondPhase { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000228F File Offset: 0x0000048F
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002297 File Offset: 0x00000497
		[SaveableProperty(5)]
		public ThirdPhase ThirdPhase { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000022A0 File Offset: 0x000004A0
		// (set) Token: 0x06000029 RID: 41 RVA: 0x000022A8 File Offset: 0x000004A8
		[SaveableProperty(8)]
		public Kingdom PlayerSupportedKingdom { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002A RID: 42 RVA: 0x000022B1 File Offset: 0x000004B1
		public bool IsCompleted
		{
			get
			{
				return StoryModeManager.Current.MainStoryLine.ThirdPhase != null && StoryModeManager.Current.MainStoryLine.ThirdPhase.IsCompleted;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000022DA File Offset: 0x000004DA
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000022E2 File Offset: 0x000004E2
		public ItemObject DragonBanner { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000022EB File Offset: 0x000004EB
		public bool IsFirstPhaseCompleted
		{
			get
			{
				return this.SecondPhase != null;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000022F6 File Offset: 0x000004F6
		public bool IsSecondPhaseCompleted
		{
			get
			{
				return this.ThirdPhase != null;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002301 File Offset: 0x00000501
		public MainStoryLine()
		{
			this.MainStoryLineSide = MainStoryLineSide.None;
			this.TutorialPhase = new TutorialPhase();
			this._tutorialScores = new Dictionary<string, float>();
			this.FamilyRescued = false;
			this.BusyHideouts = new List<Hideout>();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002338 File Offset: 0x00000538
		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this.BusyHideouts = new List<Hideout>();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002345 File Offset: 0x00000545
		public void OnSessionLaunched()
		{
			this.DragonBanner = Campaign.Current.ObjectManager.GetObject<ItemObject>("dragon_banner");
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002361 File Offset: 0x00000561
		public void SetTutorialScores(Dictionary<string, float> scores)
		{
			this._tutorialScores = new Dictionary<string, float>(scores);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000236F File Offset: 0x0000056F
		public Dictionary<string, float> GetTutorialScores()
		{
			return new Dictionary<string, float>(this._tutorialScores);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000237C File Offset: 0x0000057C
		public void SetStoryLineSide(MainStoryLineSide side)
		{
			this.MainStoryLineSide = side;
			this.PlayerSupportedKingdom = Clan.PlayerClan.Kingdom;
			StoryModeEvents.Instance.OnMainStoryLineSideChosen(this.MainStoryLineSide);
			DisableHeroAction.Apply(StoryModeHeroes.ImperialMentor);
			DisableHeroAction.Apply(StoryModeHeroes.AntiImperialMentor);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000023B9 File Offset: 0x000005B9
		public void SetMentorSettlements(Settlement imperialMentorSettlement, Settlement antiImperialMentorSettlement)
		{
			this.ImperialMentorSettlement = imperialMentorSettlement;
			this.AntiImperialMentorSettlement = antiImperialMentorSettlement;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000023CC File Offset: 0x000005CC
		public void CompleteTutorialPhase(bool isSkipped)
		{
			this.TutorialPhase.CompleteTutorial(isSkipped);
			this.FirstPhase = new FirstPhase();
			StoryModeEvents.Instance.OnStoryModeTutorialEnded();
			StoryModeManager.Current.MainStoryLine.FirstPhase.CollectBannerPiece();
			Campaign.Current.CampaignBehaviorManager.RemoveBehavior<TutorialPhaseCampaignBehavior>();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000241D File Offset: 0x0000061D
		public void CompleteFirstPhase()
		{
			this.SecondPhase = new SecondPhase();
			Campaign.Current.CampaignBehaviorManager.RemoveBehavior<FirstPhaseCampaignBehavior>();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002439 File Offset: 0x00000639
		public void CompleteSecondPhase()
		{
			this.ThirdPhase = new ThirdPhase();
			StoryModeEvents.Instance.OnConspiracyActivated();
			Campaign.Current.CampaignBehaviorManager.RemoveBehavior<SecondPhaseCampaignBehavior>();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000245F File Offset: 0x0000065F
		internal static void AutoGeneratedStaticCollectObjectsMainStoryLine(object o, List<object> collectedObjects)
		{
			((MainStoryLine)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002470 File Offset: 0x00000670
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this.ImperialMentorSettlement);
			collectedObjects.Add(this.AntiImperialMentorSettlement);
			collectedObjects.Add(this._tutorialScores);
			collectedObjects.Add(this.TutorialPhase);
			collectedObjects.Add(this.FirstPhase);
			collectedObjects.Add(this.SecondPhase);
			collectedObjects.Add(this.ThirdPhase);
			collectedObjects.Add(this.PlayerSupportedKingdom);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000024DD File Offset: 0x000006DD
		internal static object AutoGeneratedGetMemberValueTutorialPhase(object o)
		{
			return ((MainStoryLine)o).TutorialPhase;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000024EA File Offset: 0x000006EA
		internal static object AutoGeneratedGetMemberValueFirstPhase(object o)
		{
			return ((MainStoryLine)o).FirstPhase;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000024F7 File Offset: 0x000006F7
		internal static object AutoGeneratedGetMemberValueSecondPhase(object o)
		{
			return ((MainStoryLine)o).SecondPhase;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002504 File Offset: 0x00000704
		internal static object AutoGeneratedGetMemberValueThirdPhase(object o)
		{
			return ((MainStoryLine)o).ThirdPhase;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002511 File Offset: 0x00000711
		internal static object AutoGeneratedGetMemberValuePlayerSupportedKingdom(object o)
		{
			return ((MainStoryLine)o).PlayerSupportedKingdom;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000251E File Offset: 0x0000071E
		internal static object AutoGeneratedGetMemberValueMainStoryLineSide(object o)
		{
			return ((MainStoryLine)o).MainStoryLineSide;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002530 File Offset: 0x00000730
		internal static object AutoGeneratedGetMemberValueImperialMentorSettlement(object o)
		{
			return ((MainStoryLine)o).ImperialMentorSettlement;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000253D File Offset: 0x0000073D
		internal static object AutoGeneratedGetMemberValueAntiImperialMentorSettlement(object o)
		{
			return ((MainStoryLine)o).AntiImperialMentorSettlement;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000254A File Offset: 0x0000074A
		internal static object AutoGeneratedGetMemberValueFamilyRescued(object o)
		{
			return ((MainStoryLine)o).FamilyRescued;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000255C File Offset: 0x0000075C
		internal static object AutoGeneratedGetMemberValue_tutorialScores(object o)
		{
			return ((MainStoryLine)o)._tutorialScores;
		}

		// Token: 0x0400000C RID: 12
		public const int MainStoryLineDialogOptionPriority = 150;

		// Token: 0x0400000D RID: 13
		public const string DragonBannerItemStringId = "dragon_banner";

		// Token: 0x0400000E RID: 14
		public const string DragonBannerPart1ItemStringId = "dragon_banner_center";

		// Token: 0x0400000F RID: 15
		public const string DragonBannerPart2ItemStringId = "dragon_banner_dragonhead";

		// Token: 0x04000010 RID: 16
		public const string DragonBannerPart3ItemStringId = "dragon_banner_handle";

		// Token: 0x04000011 RID: 17
		[SaveableField(1)]
		public MainStoryLineSide MainStoryLineSide;

		// Token: 0x04000016 RID: 22
		[SaveableField(6)]
		public Settlement ImperialMentorSettlement;

		// Token: 0x04000017 RID: 23
		[SaveableField(7)]
		public Settlement AntiImperialMentorSettlement;

		// Token: 0x04000019 RID: 25
		[SaveableField(9)]
		private Dictionary<string, float> _tutorialScores;

		// Token: 0x0400001A RID: 26
		[SaveableField(10)]
		public bool FamilyRescued;

		// Token: 0x0400001B RID: 27
		public List<Hideout> BusyHideouts;
	}
}
