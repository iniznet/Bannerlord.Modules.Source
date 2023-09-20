using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public static class MultiplayerPlayerContextMenuHelper
	{
		public static void AddLobbyViewProfileOptions(MPLobbyPlayerBaseVM player, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			contextMenuOptions.Add(new StringPairItemWithActionVM(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewProfile), new TextObject("{=bjJkW9dO}View Profile", null).ToString(), "ViewProfile", player));
			if (PlatformServices.Instance.IsPlayerProfileCardAvailable(player.ProvidedID))
			{
				MultiplayerPlayerContextMenuHelper.AddPlatformProfileCardOption(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewPlatformProfileCardLobby), player, contextMenuOptions);
			}
		}

		public static void AddMissionViewProfileOptions(MPPlayerVM player, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			if (PlatformServices.Instance.IsPlayerProfileCardAvailable(player.Peer.Peer.Id))
			{
				MultiplayerPlayerContextMenuHelper.AddPlatformProfileCardOption(new Action<object>(MultiplayerPlayerContextMenuHelper.ExecuteViewPlatformProfileCardMission), player, contextMenuOptions);
			}
		}

		private static void AddPlatformProfileCardOption(Action<object> onExecuted, object target, MBBindingList<StringPairItemWithActionVM> contextMenuOptions)
		{
			TextObject empty = TextObject.Empty;
			Debug.FailedAssert("Platform profile is supported but \"Show Profile\" text is not defined!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MultiplayerPlayerContextMenuHelper.cs", "AddPlatformProfileCardOption", 38);
			if (empty != TextObject.Empty)
			{
				contextMenuOptions.Add(new StringPairItemWithActionVM(onExecuted, empty.ToString(), "ViewProfile", target));
			}
		}

		private static void ExecuteViewProfile(object playerObj)
		{
			(playerObj as MPLobbyPlayerBaseVM).ExecuteShowProfile();
		}

		private static void ExecuteViewPlatformProfileCardLobby(object playerObj)
		{
			PlatformServices.Instance.ShowPlayerProfileCard((playerObj as MPLobbyPlayerBaseVM).ProvidedID);
		}

		private static void ExecuteViewPlatformProfileCardMission(object playerObj)
		{
			PlatformServices.Instance.ShowPlayerProfileCard((playerObj as MPPlayerVM).Peer.Peer.Id);
		}
	}
}
