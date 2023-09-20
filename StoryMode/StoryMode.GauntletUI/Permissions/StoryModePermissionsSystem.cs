using System;
using SandBox.View.Map;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace StoryMode.GauntletUI.Permissions
{
	public class StoryModePermissionsSystem
	{
		private StoryModePermissionsSystem()
		{
			this.RegisterEvents();
		}

		public static void OnInitialize()
		{
			if (StoryModePermissionsSystem.Current == null)
			{
				StoryModePermissionsSystem.Current = new StoryModePermissionsSystem();
			}
		}

		internal static void OnUnload()
		{
			if (StoryModePermissionsSystem.Current != null)
			{
				StoryModePermissionsSystem.Current.UnregisterEvents();
				StoryModePermissionsSystem.Current = null;
			}
		}

		private void OnClanScreenPermission(MapNavigationHandler.ClanScreenPermissionEvent obj)
		{
			StoryModeManager storyModeManager = StoryModeManager.Current;
			if (storyModeManager != null && storyModeManager.MainStoryLine.IsPlayerInteractionRestricted)
			{
				obj.IsClanScreenAvailable(false, new TextObject("{=75nwCTEn}Clan Screen is disabled during Tutorial.", null));
			}
		}

		private void OnSettlementOverlayTalkPermission(SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent obj)
		{
			bool flag = StoryModeManager.Current != null;
			TutorialPhase instance = TutorialPhase.Instance;
			bool flag2 = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
			StoryModeManager storyModeManager = StoryModeManager.Current;
			bool flag3;
			if (storyModeManager == null)
			{
				flag3 = false;
			}
			else
			{
				MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
				bool? flag4 = ((mainStoryLine != null) ? new bool?(mainStoryLine.TutorialPhase.IsCompleted) : null);
				bool flag5 = true;
				flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
			}
			bool flag6 = flag3;
			if (flag && !flag2 && !flag6)
			{
				obj.IsTalkAvailable(false, new TextObject("{=UjERCi2F}This feature is disabled.", null));
			}
		}

		private void OnSettlementOverlayQuickTalkPermission(SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent obj)
		{
			bool flag = StoryModeManager.Current != null;
			TutorialPhase instance = TutorialPhase.Instance;
			bool flag2 = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.Finalized;
			StoryModeManager storyModeManager = StoryModeManager.Current;
			bool flag3;
			if (storyModeManager == null)
			{
				flag3 = false;
			}
			else
			{
				MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
				bool? flag4 = ((mainStoryLine != null) ? new bool?(mainStoryLine.TutorialPhase.IsCompleted) : null);
				bool flag5 = true;
				flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
			}
			bool flag6 = flag3;
			if (flag && !flag2 && !flag6)
			{
				obj.IsTalkAvailable(false, new TextObject("{=UjERCi2F}This feature is disabled.", null));
			}
		}

		private void OnSettlementOverlayLeaveMemberPermission(SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent obj)
		{
			bool flag = StoryModeManager.Current != null;
			TutorialPhase instance = TutorialPhase.Instance;
			bool flag2 = instance != null && instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
			StoryModeManager storyModeManager = StoryModeManager.Current;
			bool flag3;
			if (storyModeManager == null)
			{
				flag3 = false;
			}
			else
			{
				MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
				bool? flag4 = ((mainStoryLine != null) ? new bool?(mainStoryLine.TutorialPhase.IsCompleted) : null);
				bool flag5 = true;
				flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
			}
			bool flag6 = flag3;
			if (flag && !flag2 && !flag6)
			{
				obj.IsLeaveAvailable(false, new TextObject("{=UjERCi2F}This feature is disabled.", null));
			}
		}

		private void RegisterEvents()
		{
			Game.Current.EventManager.RegisterEvent<MapNavigationHandler.ClanScreenPermissionEvent>(new Action<MapNavigationHandler.ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
		}

		internal void UnregisterEvents()
		{
			Game.Current.EventManager.UnregisterEvent<MapNavigationHandler.ClanScreenPermissionEvent>(new Action<MapNavigationHandler.ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
		}

		private static StoryModePermissionsSystem Current;
	}
}
