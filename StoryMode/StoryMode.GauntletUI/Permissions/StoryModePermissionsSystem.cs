using System;
using SandBox.View.Map;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace StoryMode.GauntletUI.Permissions
{
	// Token: 0x0200003F RID: 63
	public class StoryModePermissionsSystem
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x00006B7F File Offset: 0x00004D7F
		private StoryModePermissionsSystem()
		{
			this.RegisterEvents();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00006B8D File Offset: 0x00004D8D
		public static void OnInitialize()
		{
			if (StoryModePermissionsSystem.Current == null)
			{
				StoryModePermissionsSystem.Current = new StoryModePermissionsSystem();
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00006BA0 File Offset: 0x00004DA0
		internal static void OnUnload()
		{
			if (StoryModePermissionsSystem.Current != null)
			{
				StoryModePermissionsSystem.Current.UnregisterEvents();
				StoryModePermissionsSystem.Current = null;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00006BB9 File Offset: 0x00004DB9
		private void OnClanScreenPermission(MapNavigationHandler.ClanScreenPermissionEvent obj)
		{
			StoryModeManager storyModeManager = StoryModeManager.Current;
			if (storyModeManager != null && storyModeManager.MainStoryLine.IsPlayerInteractionRestricted)
			{
				obj.IsClanScreenAvailable(false, new TextObject("{=75nwCTEn}Clan Screen is disabled during Tutorial.", null));
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00006BEC File Offset: 0x00004DEC
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

		// Token: 0x060001BC RID: 444 RVA: 0x00006C80 File Offset: 0x00004E80
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

		// Token: 0x060001BD RID: 445 RVA: 0x00006D14 File Offset: 0x00004F14
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

		// Token: 0x060001BE RID: 446 RVA: 0x00006DA8 File Offset: 0x00004FA8
		private void RegisterEvents()
		{
			Game.Current.EventManager.RegisterEvent<MapNavigationHandler.ClanScreenPermissionEvent>(new Action<MapNavigationHandler.ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00006E24 File Offset: 0x00005024
		internal void UnregisterEvents()
		{
			Game.Current.EventManager.UnregisterEvent<MapNavigationHandler.ClanScreenPermissionEvent>(new Action<MapNavigationHandler.ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementMenuOverlayVM.SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
		}

		// Token: 0x0400006C RID: 108
		private static StoryModePermissionsSystem Current;
	}
}
