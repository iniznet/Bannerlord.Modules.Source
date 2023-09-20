using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayersAddedToPartyMessage : Message
	{
		[TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
		[JsonProperty]
		public List<ValueTuple<PlayerId, string, bool>> Players
		{
			[return: TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
			get;
			[param: TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
			private set;
		}

		[TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
		[JsonProperty]
		public List<ValueTuple<PlayerId, string>> InvitedPlayers
		{
			[return: TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
			get;
			[param: TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
			private set;
		}

		public PlayersAddedToPartyMessage()
		{
			this.Players = new List<ValueTuple<PlayerId, string, bool>>();
			this.InvitedPlayers = new List<ValueTuple<PlayerId, string>>();
		}

		public PlayersAddedToPartyMessage(PlayerId playerId, string playerName, bool isPartyLeader)
			: this()
		{
			this.AddPlayer(playerId, playerName, isPartyLeader);
		}

		public void AddPlayer(PlayerId playerId, string playerName, bool isPartyLeader)
		{
			this.Players.Add(new ValueTuple<PlayerId, string, bool>(playerId, playerName, isPartyLeader));
		}

		public void AddInvitedPlayer(PlayerId playerId, string playerName)
		{
			this.InvitedPlayers.Add(new ValueTuple<PlayerId, string>(playerId, playerName));
		}
	}
}
