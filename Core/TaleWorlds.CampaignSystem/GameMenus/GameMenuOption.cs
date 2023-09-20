using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E7 RID: 231
	public class GameMenuOption
	{
		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060013F0 RID: 5104 RVA: 0x000584E9 File Offset: 0x000566E9
		// (set) Token: 0x060013F1 RID: 5105 RVA: 0x000584F1 File Offset: 0x000566F1
		public GameMenu.MenuAndOptionType Type { get; private set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060013F2 RID: 5106 RVA: 0x000584FA File Offset: 0x000566FA
		// (set) Token: 0x060013F3 RID: 5107 RVA: 0x00058502 File Offset: 0x00056702
		public GameMenuOption.LeaveType OptionLeaveType { get; set; }

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060013F4 RID: 5108 RVA: 0x0005850B File Offset: 0x0005670B
		// (set) Token: 0x060013F5 RID: 5109 RVA: 0x00058513 File Offset: 0x00056713
		public GameMenuOption.IssueQuestFlags OptionQuestData { get; set; }

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0005851C File Offset: 0x0005671C
		// (set) Token: 0x060013F7 RID: 5111 RVA: 0x00058524 File Offset: 0x00056724
		public string IdString { get; private set; }

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0005852D File Offset: 0x0005672D
		// (set) Token: 0x060013F9 RID: 5113 RVA: 0x00058535 File Offset: 0x00056735
		public TextObject Text { get; private set; }

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x0005853E File Offset: 0x0005673E
		// (set) Token: 0x060013FB RID: 5115 RVA: 0x00058546 File Offset: 0x00056746
		public TextObject Text2 { get; private set; }

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0005854F File Offset: 0x0005674F
		// (set) Token: 0x060013FD RID: 5117 RVA: 0x00058557 File Offset: 0x00056757
		public TextObject Tooltip { get; private set; }

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x00058560 File Offset: 0x00056760
		// (set) Token: 0x060013FF RID: 5119 RVA: 0x00058568 File Offset: 0x00056768
		public bool IsLeave { get; private set; }

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06001400 RID: 5120 RVA: 0x00058571 File Offset: 0x00056771
		// (set) Token: 0x06001401 RID: 5121 RVA: 0x00058579 File Offset: 0x00056779
		public bool IsRepeatable { get; private set; }

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001402 RID: 5122 RVA: 0x00058582 File Offset: 0x00056782
		// (set) Token: 0x06001403 RID: 5123 RVA: 0x0005858A File Offset: 0x0005678A
		public bool IsEnabled { get; private set; }

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x00058593 File Offset: 0x00056793
		// (set) Token: 0x06001405 RID: 5125 RVA: 0x0005859B File Offset: 0x0005679B
		public object RelatedObject { get; private set; }

		// Token: 0x06001406 RID: 5126 RVA: 0x000585A4 File Offset: 0x000567A4
		internal GameMenuOption()
		{
			this.Text = TextObject.Empty;
			this.Tooltip = TextObject.Empty;
			this.IsEnabled = true;
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x000585CC File Offset: 0x000567CC
		public GameMenuOption(GameMenu.MenuAndOptionType type, string idString, TextObject text, TextObject text2, GameMenuOption.OnConditionDelegate condition, GameMenuOption.OnConsequenceDelegate consequence, bool isLeave = false, bool isRepeatable = false, object relatedObject = null)
		{
			this.Type = type;
			this.IdString = idString;
			this.Text = text;
			this.Text2 = text2;
			this.OnCondition = condition;
			this.OnConsequence = consequence;
			this.Tooltip = TextObject.Empty;
			this.IsRepeatable = isRepeatable;
			this.IsEnabled = true;
			this.IsLeave = isLeave;
			this.RelatedObject = relatedObject;
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x00058638 File Offset: 0x00056838
		public bool GetConditionsHold(Game game, MenuContext menuContext)
		{
			if (this.OnCondition != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.Text);
				bool flag = this.OnCondition(menuCallbackArgs);
				this.IsEnabled = menuCallbackArgs.IsEnabled;
				this.Tooltip = menuCallbackArgs.Tooltip;
				this.OptionQuestData = menuCallbackArgs.OptionQuestData;
				this.OptionLeaveType = menuCallbackArgs.optionLeaveType;
				return flag;
			}
			return true;
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00058698 File Offset: 0x00056898
		public void RunConsequence(MenuContext menuContext)
		{
			if (this.OnConsequence != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.Text);
				this.OnConsequence(menuCallbackArgs);
			}
			menuContext.OnConsequence(this);
			if (Campaign.Current != null)
			{
				CampaignEventDispatcher.Instance.OnGameMenuOptionSelected(this);
			}
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x000586DF File Offset: 0x000568DF
		public void SetEnable(bool isEnable)
		{
			this.IsEnabled = isEnable;
		}

		// Token: 0x040006F8 RID: 1784
		public static GameMenuOption.IssueQuestFlags[] IssueQuestFlagsValues = (GameMenuOption.IssueQuestFlags[])Enum.GetValues(typeof(GameMenuOption.IssueQuestFlags));

		// Token: 0x04000700 RID: 1792
		public GameMenuOption.OnConditionDelegate OnCondition;

		// Token: 0x04000701 RID: 1793
		public GameMenuOption.OnConsequenceDelegate OnConsequence;

		// Token: 0x020004F2 RID: 1266
		// (Invoke) Token: 0x060041E4 RID: 16868
		public delegate bool OnConditionDelegate(MenuCallbackArgs args);

		// Token: 0x020004F3 RID: 1267
		// (Invoke) Token: 0x060041E8 RID: 16872
		public delegate void OnConsequenceDelegate(MenuCallbackArgs args);

		// Token: 0x020004F4 RID: 1268
		public enum LeaveType
		{
			// Token: 0x0400153A RID: 5434
			Default,
			// Token: 0x0400153B RID: 5435
			Mission,
			// Token: 0x0400153C RID: 5436
			Submenu,
			// Token: 0x0400153D RID: 5437
			BribeAndEscape,
			// Token: 0x0400153E RID: 5438
			Escape,
			// Token: 0x0400153F RID: 5439
			Craft,
			// Token: 0x04001540 RID: 5440
			ForceToGiveGoods,
			// Token: 0x04001541 RID: 5441
			ForceToGiveTroops,
			// Token: 0x04001542 RID: 5442
			Bribe,
			// Token: 0x04001543 RID: 5443
			LeaveTroopsAndFlee,
			// Token: 0x04001544 RID: 5444
			OrderTroopsToAttack,
			// Token: 0x04001545 RID: 5445
			Raid,
			// Token: 0x04001546 RID: 5446
			HostileAction,
			// Token: 0x04001547 RID: 5447
			Recruit,
			// Token: 0x04001548 RID: 5448
			Trade,
			// Token: 0x04001549 RID: 5449
			Wait,
			// Token: 0x0400154A RID: 5450
			Leave,
			// Token: 0x0400154B RID: 5451
			Continue,
			// Token: 0x0400154C RID: 5452
			Manage,
			// Token: 0x0400154D RID: 5453
			TroopSelection,
			// Token: 0x0400154E RID: 5454
			WaitQuest,
			// Token: 0x0400154F RID: 5455
			Surrender,
			// Token: 0x04001550 RID: 5456
			Conversation,
			// Token: 0x04001551 RID: 5457
			DefendAction,
			// Token: 0x04001552 RID: 5458
			Devastate,
			// Token: 0x04001553 RID: 5459
			Pillage,
			// Token: 0x04001554 RID: 5460
			ShowMercy,
			// Token: 0x04001555 RID: 5461
			Leaderboard,
			// Token: 0x04001556 RID: 5462
			OpenStash,
			// Token: 0x04001557 RID: 5463
			ManageGarrison,
			// Token: 0x04001558 RID: 5464
			StagePrisonBreak,
			// Token: 0x04001559 RID: 5465
			ManagePrisoners,
			// Token: 0x0400155A RID: 5466
			Ransom,
			// Token: 0x0400155B RID: 5467
			PracticeFight,
			// Token: 0x0400155C RID: 5468
			BesiegeTown,
			// Token: 0x0400155D RID: 5469
			SneakIn,
			// Token: 0x0400155E RID: 5470
			LeadAssault,
			// Token: 0x0400155F RID: 5471
			DonateTroops,
			// Token: 0x04001560 RID: 5472
			DonatePrisoners,
			// Token: 0x04001561 RID: 5473
			SiegeAmbush
		}

		// Token: 0x020004F5 RID: 1269
		[Flags]
		public enum IssueQuestFlags
		{
			// Token: 0x04001563 RID: 5475
			None = 0,
			// Token: 0x04001564 RID: 5476
			AvailableIssue = 1,
			// Token: 0x04001565 RID: 5477
			ActiveIssue = 2,
			// Token: 0x04001566 RID: 5478
			ActiveStoryQuest = 4,
			// Token: 0x04001567 RID: 5479
			TrackedIssue = 8,
			// Token: 0x04001568 RID: 5480
			TrackedStoryQuest = 16
		}
	}
}
