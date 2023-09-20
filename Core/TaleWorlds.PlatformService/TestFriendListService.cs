using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Diamond.AccessProvider.Test;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000012 RID: 18
	public class TestFriendListService : IFriendListService
	{
		// Token: 0x06000096 RID: 150 RVA: 0x000025F8 File Offset: 0x000007F8
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

		// Token: 0x06000097 RID: 151 RVA: 0x000026AA File Offset: 0x000008AA
		string IFriendListService.GetServiceCodeName()
		{
			return "Test";
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000026B1 File Offset: 0x000008B1
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Test", null);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000026BE File Offset: 0x000008BE
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Test;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000026C1 File Offset: 0x000008C1
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			if (this._testUserNames.ContainsKey(providedId))
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000026DE File Offset: 0x000008DE
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			if (this._testUserNames.ContainsKey(providedId))
			{
				return Task.FromResult<bool>(true);
			}
			return Task.FromResult<bool>(false);
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000026FB File Offset: 0x000008FB
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000026FE File Offset: 0x000008FE
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00002701 File Offset: 0x00000901
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00002704 File Offset: 0x00000904
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00002707 File Offset: 0x00000907
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

		// Token: 0x060000A1 RID: 161 RVA: 0x00002717 File Offset: 0x00000917
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000271A File Offset: 0x0000091A
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060000A3 RID: 163 RVA: 0x00002720 File Offset: 0x00000920
		// (remove) Token: 0x060000A4 RID: 164 RVA: 0x00002758 File Offset: 0x00000958
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060000A5 RID: 165 RVA: 0x00002790 File Offset: 0x00000990
		// (remove) Token: 0x060000A6 RID: 166 RVA: 0x000027C8 File Offset: 0x000009C8
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060000A7 RID: 167 RVA: 0x00002800 File Offset: 0x00000A00
		// (remove) Token: 0x060000A8 RID: 168 RVA: 0x00002838 File Offset: 0x00000A38
		public event Action OnFriendListChanged;

		// Token: 0x060000A9 RID: 169 RVA: 0x00002870 File Offset: 0x00000A70
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

		// Token: 0x060000AA RID: 170 RVA: 0x000028C8 File Offset: 0x00000AC8
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

		// Token: 0x060000AB RID: 171 RVA: 0x000028F4 File Offset: 0x00000AF4
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

		// Token: 0x0400002F RID: 47
		private string _userName;

		// Token: 0x04000030 RID: 48
		private PlayerId _playerId;

		// Token: 0x04000031 RID: 49
		private Dictionary<PlayerId, string> _testUserNames;

		// Token: 0x04000032 RID: 50
		private Dictionary<string, PlayerId> _testUserPlayerIds;
	}
}
