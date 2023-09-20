using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Diamond.AccessProvider.Test;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	public class TestFriendListService : IFriendListService
	{
		public TestFriendListService(string userName, PlayerId myPlayerId)
		{
			this._userName = userName;
			this._playerId = myPlayerId;
			this._testUserNames = new Dictionary<PlayerId, string>();
			this._testUserPlayerIds = new Dictionary<string, PlayerId>();
			this._testUserNames.Add(this._playerId, this._userName);
			this._testUserPlayerIds.Add(this._userName, this._playerId);
			for (int i = 1; i <= 12; i++)
			{
				string text = "TestPlayer" + i;
				PlayerId playerIdFromUserName = TestLoginAccessProvider.GetPlayerIdFromUserName(text);
				if (!this._testUserNames.ContainsKey(playerIdFromUserName))
				{
					this._testUserNames.Add(playerIdFromUserName, text);
					this._testUserPlayerIds.Add(text, playerIdFromUserName);
				}
			}
		}

		string IFriendListService.GetServiceCodeName()
		{
			return "Test";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Test", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Test;
		}

		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			if (this._testUserNames.ContainsKey(providedId))
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			if (this._testUserNames.ContainsKey(providedId))
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return true;
			}
		}

		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			List<string> list = new List<string>();
			if (this._userName == "TestPlayer1" || this._userName == "TestPlayer2" || this._userName == "TestPlayer3" || this._userName == "TestPlayer4" || this._userName == "TestPlayer5" || this._userName == "TestPlayer6")
			{
				list.Add("TestPlayer1");
				list.Add("TestPlayer2");
				list.Add("TestPlayer3");
				list.Add("TestPlayer4");
				list.Add("TestPlayer5");
				list.Add("TestPlayer6");
			}
			else if (this._userName == "TestPlayer7" || this._userName == "TestPlayer8" || this._userName == "TestPlayer9" || this._userName == "TestPlayer10" || this._userName == "TestPlayer11" || this._userName == "TestPlayer12")
			{
				list.Add("TestPlayer7");
				list.Add("TestPlayer8");
				list.Add("TestPlayer9");
				list.Add("TestPlayer10");
				list.Add("TestPlayer11");
				list.Add("TestPlayer12");
			}
			else
			{
				list.Add("TestPlayer1");
				list.Add("TestPlayer2");
				list.Add("TestPlayer3");
				list.Add("TestPlayer4");
				list.Add("TestPlayer5");
				list.Add("TestPlayer6");
				list.Add("TestPlayer7");
				list.Add("TestPlayer8");
				list.Add("TestPlayer9");
				list.Add("TestPlayer10");
				list.Add("TestPlayer11");
				list.Add("TestPlayer12");
			}
			foreach (string text in list)
			{
				if (this._userName != text)
				{
					yield return TestLoginAccessProvider.GetPlayerIdFromUserName(text);
				}
			}
			List<string>.Enumerator enumerator = default(List<string>.Enumerator);
			yield break;
			yield break;
		}

		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<PlayerId> OnFriendRemoved;

		public event Action OnFriendListChanged;

		private void Dummy()
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
			}
			if (this.OnFriendRemoved != null)
			{
				this.OnFriendRemoved(default(PlayerId));
			}
			if (this.OnFriendListChanged != null)
			{
				this.OnFriendListChanged();
			}
		}

		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			string text = "-";
			string text2;
			if (this._testUserNames.TryGetValue(providedId, out text2))
			{
				text = text2;
			}
			return Task.FromResult<string>(text);
		}

		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			PlayerId playerId = default(PlayerId);
			PlayerId playerId2;
			if (this._testUserPlayerIds.TryGetValue(name, out playerId2))
			{
				playerId = playerId2;
			}
			return Task.FromResult<PlayerId>(playerId);
		}

		private string _userName;

		private PlayerId _playerId;

		private Dictionary<PlayerId, string> _testUserNames;

		private Dictionary<string, PlayerId> _testUserPlayerIds;
	}
}
