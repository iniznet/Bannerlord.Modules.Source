using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	public class GameMenuOption
	{
		public GameMenu.MenuAndOptionType Type { get; private set; }

		public GameMenuOption.LeaveType OptionLeaveType { get; set; }

		public GameMenuOption.IssueQuestFlags OptionQuestData { get; set; }

		public string IdString { get; private set; }

		public TextObject Text { get; private set; }

		public TextObject Text2 { get; private set; }

		public TextObject Tooltip { get; private set; }

		public bool IsLeave { get; private set; }

		public bool IsRepeatable { get; private set; }

		public bool IsEnabled { get; private set; }

		public object RelatedObject { get; private set; }

		internal GameMenuOption()
		{
			this.Text = TextObject.Empty;
			this.Tooltip = TextObject.Empty;
			this.IsEnabled = true;
		}

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

		public void SetEnable(bool isEnable)
		{
			this.IsEnabled = isEnable;
		}

		public static GameMenuOption.IssueQuestFlags[] IssueQuestFlagsValues = (GameMenuOption.IssueQuestFlags[])Enum.GetValues(typeof(GameMenuOption.IssueQuestFlags));

		public GameMenuOption.OnConditionDelegate OnCondition;

		public GameMenuOption.OnConsequenceDelegate OnConsequence;

		public delegate bool OnConditionDelegate(MenuCallbackArgs args);

		public delegate void OnConsequenceDelegate(MenuCallbackArgs args);

		public enum LeaveType
		{
			Default,
			Mission,
			Submenu,
			BribeAndEscape,
			Escape,
			Craft,
			ForceToGiveGoods,
			ForceToGiveTroops,
			Bribe,
			LeaveTroopsAndFlee,
			OrderTroopsToAttack,
			Raid,
			HostileAction,
			Recruit,
			Trade,
			Wait,
			Leave,
			Continue,
			Manage,
			TroopSelection,
			WaitQuest,
			Surrender,
			Conversation,
			DefendAction,
			Devastate,
			Pillage,
			ShowMercy,
			Leaderboard,
			OpenStash,
			ManageGarrison,
			StagePrisonBreak,
			ManagePrisoners,
			Ransom,
			PracticeFight,
			BesiegeTown,
			SneakIn,
			LeadAssault,
			DonateTroops,
			DonatePrisoners,
			SiegeAmbush
		}

		[Flags]
		public enum IssueQuestFlags
		{
			None = 0,
			AvailableIssue = 1,
			ActiveIssue = 2,
			ActiveStoryQuest = 4,
			TrackedIssue = 8,
			TrackedStoryQuest = 16
		}
	}
}
