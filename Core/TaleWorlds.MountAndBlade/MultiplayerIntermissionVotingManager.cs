using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030B RID: 779
	public class MultiplayerIntermissionVotingManager
	{
		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06002A1A RID: 10778 RVA: 0x000A2E07 File Offset: 0x000A1007
		public static MultiplayerIntermissionVotingManager Instance
		{
			get
			{
				MultiplayerIntermissionVotingManager multiplayerIntermissionVotingManager;
				if ((multiplayerIntermissionVotingManager = MultiplayerIntermissionVotingManager._instance) == null)
				{
					multiplayerIntermissionVotingManager = (MultiplayerIntermissionVotingManager._instance = new MultiplayerIntermissionVotingManager());
				}
				return multiplayerIntermissionVotingManager;
			}
		}

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06002A1B RID: 10779 RVA: 0x000A2E1D File Offset: 0x000A101D
		// (set) Token: 0x06002A1C RID: 10780 RVA: 0x000A2E25 File Offset: 0x000A1025
		public List<IntermissionVoteItem> MapVoteItems { get; private set; }

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06002A1D RID: 10781 RVA: 0x000A2E2E File Offset: 0x000A102E
		// (set) Token: 0x06002A1E RID: 10782 RVA: 0x000A2E36 File Offset: 0x000A1036
		public List<IntermissionVoteItem> CultureVoteItems { get; private set; }

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x06002A1F RID: 10783 RVA: 0x000A2E40 File Offset: 0x000A1040
		// (remove) Token: 0x06002A20 RID: 10784 RVA: 0x000A2E78 File Offset: 0x000A1078
		public event MultiplayerIntermissionVotingManager.MapItemAddedDelegate OnMapItemAdded;

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x06002A21 RID: 10785 RVA: 0x000A2EB0 File Offset: 0x000A10B0
		// (remove) Token: 0x06002A22 RID: 10786 RVA: 0x000A2EE8 File Offset: 0x000A10E8
		public event MultiplayerIntermissionVotingManager.CultureItemAddedDelegate OnCultureItemAdded;

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x06002A23 RID: 10787 RVA: 0x000A2F20 File Offset: 0x000A1120
		// (remove) Token: 0x06002A24 RID: 10788 RVA: 0x000A2F58 File Offset: 0x000A1158
		public event MultiplayerIntermissionVotingManager.MapItemVoteCountChangedDelegate OnMapItemVoteCountChanged;

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x06002A25 RID: 10789 RVA: 0x000A2F90 File Offset: 0x000A1190
		// (remove) Token: 0x06002A26 RID: 10790 RVA: 0x000A2FC8 File Offset: 0x000A11C8
		public event MultiplayerIntermissionVotingManager.CultureItemVoteCountChangedDelegate OnCultureItemVoteCountChanged;

		// Token: 0x06002A27 RID: 10791 RVA: 0x000A2FFD File Offset: 0x000A11FD
		public MultiplayerIntermissionVotingManager()
		{
			this.MapVoteItems = new List<IntermissionVoteItem>();
			this.CultureVoteItems = new List<IntermissionVoteItem>();
			this._votesOfPlayers = new Dictionary<PlayerId, List<string>>();
			this.IsMapVoteEnabled = true;
			this.IsCultureVoteEnabled = true;
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x000A3034 File Offset: 0x000A1234
		public void AddMapItem(string mapID)
		{
			if (!this.MapVoteItems.ContainsItem(mapID))
			{
				IntermissionVoteItem intermissionVoteItem = this.MapVoteItems.Add(mapID);
				MultiplayerIntermissionVotingManager.MapItemAddedDelegate onMapItemAdded = this.OnMapItemAdded;
				if (onMapItemAdded != null)
				{
					onMapItemAdded(intermissionVoteItem.Id);
				}
				this.SortVotesAndPickBest();
			}
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000A307C File Offset: 0x000A127C
		public void AddCultureItem(string cultureID)
		{
			if (!this.CultureVoteItems.ContainsItem(cultureID))
			{
				IntermissionVoteItem intermissionVoteItem = this.CultureVoteItems.Add(cultureID);
				MultiplayerIntermissionVotingManager.CultureItemAddedDelegate onCultureItemAdded = this.OnCultureItemAdded;
				if (onCultureItemAdded != null)
				{
					onCultureItemAdded(intermissionVoteItem.Id);
				}
				this.SortVotesAndPickBest();
			}
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x000A30C4 File Offset: 0x000A12C4
		public void AddVote(PlayerId voterID, string itemID, int voteCount)
		{
			if (this.MapVoteItems.ContainsItem(itemID))
			{
				IntermissionVoteItem item = this.MapVoteItems.GetItem(itemID);
				item.IncreaseVoteCount(voteCount);
				MultiplayerIntermissionVotingManager.MapItemVoteCountChangedDelegate onMapItemVoteCountChanged = this.OnMapItemVoteCountChanged;
				if (onMapItemVoteCountChanged != null)
				{
					onMapItemVoteCountChanged(item.Index, item.VoteCount);
				}
			}
			else if (this.CultureVoteItems.ContainsItem(itemID))
			{
				IntermissionVoteItem item2 = this.CultureVoteItems.GetItem(itemID);
				item2.IncreaseVoteCount(voteCount);
				MultiplayerIntermissionVotingManager.CultureItemVoteCountChangedDelegate onCultureItemVoteCountChanged = this.OnCultureItemVoteCountChanged;
				if (onCultureItemVoteCountChanged != null)
				{
					onCultureItemVoteCountChanged(item2.Index, item2.VoteCount);
				}
			}
			else
			{
				Debug.FailedAssert("Item with ID does not exist.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\MultiplayerIntermissionVotingManager.cs", "AddVote", 83);
			}
			if (!this._votesOfPlayers.ContainsKey(voterID))
			{
				this._votesOfPlayers.Add(voterID, new List<string>());
			}
			if (voteCount == 1)
			{
				this._votesOfPlayers[voterID].Add(itemID);
			}
			else if (voteCount == -1)
			{
				this._votesOfPlayers[voterID].Remove(itemID);
			}
			this.SortVotesAndPickBest();
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x000A31BD File Offset: 0x000A13BD
		public void SetVotesOfMap(int mapItemIndex, int voteCount)
		{
			this.MapVoteItems[mapItemIndex].SetVoteCount(voteCount);
			MultiplayerIntermissionVotingManager.MapItemVoteCountChangedDelegate onMapItemVoteCountChanged = this.OnMapItemVoteCountChanged;
			if (onMapItemVoteCountChanged == null)
			{
				return;
			}
			onMapItemVoteCountChanged(mapItemIndex, voteCount);
		}

		// Token: 0x06002A2C RID: 10796 RVA: 0x000A31E3 File Offset: 0x000A13E3
		public void SetVotesOfCulture(int cultureItemIndex, int voteCount)
		{
			this.CultureVoteItems[cultureItemIndex].SetVoteCount(voteCount);
			MultiplayerIntermissionVotingManager.CultureItemVoteCountChangedDelegate onCultureItemVoteCountChanged = this.OnCultureItemVoteCountChanged;
			if (onCultureItemVoteCountChanged == null)
			{
				return;
			}
			onCultureItemVoteCountChanged(cultureItemIndex, voteCount);
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x000A320C File Offset: 0x000A140C
		public void ClearVotes()
		{
			foreach (IntermissionVoteItem intermissionVoteItem in this.MapVoteItems)
			{
				intermissionVoteItem.SetVoteCount(0);
				MultiplayerIntermissionVotingManager.MapItemVoteCountChangedDelegate onMapItemVoteCountChanged = this.OnMapItemVoteCountChanged;
				if (onMapItemVoteCountChanged != null)
				{
					onMapItemVoteCountChanged(intermissionVoteItem.Index, intermissionVoteItem.VoteCount);
				}
			}
			foreach (IntermissionVoteItem intermissionVoteItem2 in this.CultureVoteItems)
			{
				intermissionVoteItem2.SetVoteCount(0);
				MultiplayerIntermissionVotingManager.CultureItemVoteCountChangedDelegate onCultureItemVoteCountChanged = this.OnCultureItemVoteCountChanged;
				if (onCultureItemVoteCountChanged != null)
				{
					onCultureItemVoteCountChanged(intermissionVoteItem2.Index, intermissionVoteItem2.VoteCount);
				}
			}
			this._votesOfPlayers.Clear();
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x000A32E8 File Offset: 0x000A14E8
		public void ClearItems()
		{
			this.MapVoteItems.Clear();
			this.CultureVoteItems.Clear();
			this._votesOfPlayers.Clear();
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x000A330B File Offset: 0x000A150B
		public bool IsCultureItem(string itemID)
		{
			return this.CultureVoteItems.ContainsItem(itemID);
		}

		// Token: 0x06002A30 RID: 10800 RVA: 0x000A3319 File Offset: 0x000A1519
		public bool IsMapItem(string itemID)
		{
			return this.MapVoteItems.ContainsItem(itemID);
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x000A3328 File Offset: 0x000A1528
		public void HandlePlayerDisconnect(PlayerId playerID)
		{
			if (this._votesOfPlayers.ContainsKey(playerID))
			{
				foreach (string text in this._votesOfPlayers[playerID].ToList<string>())
				{
					this.AddVote(playerID, text, -1);
				}
				this._votesOfPlayers.Remove(playerID);
			}
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x000A33A4 File Offset: 0x000A15A4
		public bool IsPeerVotedForItem(NetworkCommunicator peer, string itemID)
		{
			return this._votesOfPlayers.ContainsKey(peer.VirtualPlayer.Id) && this._votesOfPlayers[peer.VirtualPlayer.Id].Contains(itemID);
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x000A33DC File Offset: 0x000A15DC
		public void SortVotesAndPickBest()
		{
			if (GameNetwork.IsServer)
			{
				List<IntermissionVoteItem> list = this.MapVoteItems.ToList<IntermissionVoteItem>();
				if (list.Count > 1)
				{
					list.Sort((IntermissionVoteItem m1, IntermissionVoteItem m2) => -m1.VoteCount.CompareTo(m2.VoteCount));
					string id = list[0].Id;
					MultiplayerOptions.OptionType.Map.SetValue(id, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
				List<IntermissionVoteItem> list2 = this.CultureVoteItems.ToList<IntermissionVoteItem>();
				if (list2.Count > 2)
				{
					list2.Sort((IntermissionVoteItem c1, IntermissionVoteItem c2) => -c1.VoteCount.CompareTo(c2.VoteCount));
					string id2 = list2[0].Id;
					string text = list2[1].Id;
					if (list2[0].VoteCount > 2 * list2[1].VoteCount)
					{
						text = list2[0].Id;
					}
					MultiplayerOptions.OptionType.CultureTeam1.SetValue(id2, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					MultiplayerOptions.OptionType.CultureTeam2.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		// Token: 0x04001012 RID: 4114
		public const int MaxAllowedMapCount = 100;

		// Token: 0x04001013 RID: 4115
		private static MultiplayerIntermissionVotingManager _instance;

		// Token: 0x04001014 RID: 4116
		public bool IsMapVoteEnabled;

		// Token: 0x04001015 RID: 4117
		public bool IsCultureVoteEnabled;

		// Token: 0x04001018 RID: 4120
		private readonly Dictionary<PlayerId, List<string>> _votesOfPlayers;

		// Token: 0x04001019 RID: 4121
		public MultiplayerIntermissionState CurrentVoteState;

		// Token: 0x02000621 RID: 1569
		// (Invoke) Token: 0x06003DAB RID: 15787
		public delegate void MapItemAddedDelegate(string mapId);

		// Token: 0x02000622 RID: 1570
		// (Invoke) Token: 0x06003DAF RID: 15791
		public delegate void CultureItemAddedDelegate(string cultureId);

		// Token: 0x02000623 RID: 1571
		// (Invoke) Token: 0x06003DB3 RID: 15795
		public delegate void MapItemVoteCountChangedDelegate(int mapItemIndex, int voteCount);

		// Token: 0x02000624 RID: 1572
		// (Invoke) Token: 0x06003DB7 RID: 15799
		public delegate void CultureItemVoteCountChangedDelegate(int cultureItemIndex, int voteCount);
	}
}
