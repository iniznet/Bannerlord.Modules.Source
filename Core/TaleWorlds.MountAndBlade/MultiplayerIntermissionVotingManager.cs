using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerIntermissionVotingManager
	{
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

		public List<IntermissionVoteItem> MapVoteItems { get; private set; }

		public List<IntermissionVoteItem> CultureVoteItems { get; private set; }

		public event MultiplayerIntermissionVotingManager.MapItemAddedDelegate OnMapItemAdded;

		public event MultiplayerIntermissionVotingManager.CultureItemAddedDelegate OnCultureItemAdded;

		public event MultiplayerIntermissionVotingManager.MapItemVoteCountChangedDelegate OnMapItemVoteCountChanged;

		public event MultiplayerIntermissionVotingManager.CultureItemVoteCountChangedDelegate OnCultureItemVoteCountChanged;

		public MultiplayerIntermissionVotingManager()
		{
			this.MapVoteItems = new List<IntermissionVoteItem>();
			this.CultureVoteItems = new List<IntermissionVoteItem>();
			this._votesOfPlayers = new Dictionary<PlayerId, List<string>>();
			this.IsMapVoteEnabled = true;
			this.IsCultureVoteEnabled = true;
		}

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

		public void ClearItems()
		{
			this.MapVoteItems.Clear();
			this.CultureVoteItems.Clear();
			this._votesOfPlayers.Clear();
		}

		public bool IsCultureItem(string itemID)
		{
			return this.CultureVoteItems.ContainsItem(itemID);
		}

		public bool IsMapItem(string itemID)
		{
			return this.MapVoteItems.ContainsItem(itemID);
		}

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

		public bool IsPeerVotedForItem(NetworkCommunicator peer, string itemID)
		{
			return this._votesOfPlayers.ContainsKey(peer.VirtualPlayer.Id) && this._votesOfPlayers[peer.VirtualPlayer.Id].Contains(itemID);
		}

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

		public const int MaxAllowedMapCount = 100;

		private static MultiplayerIntermissionVotingManager _instance;

		public bool IsMapVoteEnabled;

		public bool IsCultureVoteEnabled;

		private readonly Dictionary<PlayerId, List<string>> _votesOfPlayers;

		public MultiplayerIntermissionState CurrentVoteState;

		public delegate void MapItemAddedDelegate(string mapId);

		public delegate void CultureItemAddedDelegate(string cultureId);

		public delegate void MapItemVoteCountChangedDelegate(int mapItemIndex, int voteCount);

		public delegate void CultureItemVoteCountChangedDelegate(int cultureItemIndex, int voteCount);
	}
}
