using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000045 RID: 69
	public static class MultiplayerPlayerContextMenuHelper
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x00018A20 File Offset: 0x00016C20
		public static void AddLobbyViewProfileOptions(MPLobbyPlayerBaseVM player, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			contextMenuOptions.Add(new StringPairItemWithActionVM(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewProfile), new TextObject("{=bjJkW9dO}View Profile", null).ToString(), "ViewProfile", player));
			if (PlatformServices.Instance.IsPlayerProfileCardAvailable(player.ProvidedID))
			{
				MultiplayerPlayerContextMenuHelper.AddPlatformProfileCardOption(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewPlatformProfileCardLobby), player, contextMenuOptions);
			}
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00018A7F File Offset: 0x00016C7F
		public static void AddMissionViewProfileOptions(MPPlayerVM player, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			if (PlatformServices.Instance.IsPlayerProfileCardAvailable(player.Peer.Peer.Id))
			{
				MultiplayerPlayerContextMenuHelper.AddPlatformProfileCardOption(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewPlatformProfileCardMission), player, contextMenuOptions);
			}
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00018AB0 File Offset: 0x00016CB0
		private static void AddPlatformProfileCardOption(Action<object> onExecuted, object target, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			TextObject empty = TextObject.Empty;
			Debug.FailedAssert("Platform profile is supported but \"Show Profile\" text is not defined!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MultiplayerPlayerContextMenuHelper.cs", "AddPlatformProfileCardOption", 38);
			if (empty != TextObject.Empty)
			{
				contextMenuOptions.Add(new StringPairItemWithActionVM(onExecuted, empty.ToString(), "ViewProfile", target));
			}
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00018AF9 File Offset: 0x00016CF9
		private static void ExecuteViewProfile(object playerObj)
		{
			(playerObj as MPLobbyPlayerBaseVM).ExecuteShowProfile();
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x00018B06 File Offset: 0x00016D06
		private static void ExecuteViewPlatformProfileCardLobby(object playerObj)
		{
			PlatformServices.Instance.ShowPlayerProfileCard((playerObj as MPLobbyPlayerBaseVM).ProvidedID);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00018B1D File Offset: 0x00016D1D
		private static void ExecuteViewPlatformProfileCardMission(object playerObj)
		{
			PlatformServices.Instance.ShowPlayerProfileCard((playerObj as MPPlayerVM).Peer.Peer.Id);
		}
	}
}
