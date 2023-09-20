using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004A RID: 74
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayersAddedToPartyMessage : Message
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00002BF3 File Offset: 0x00000DF3
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00002BFB File Offset: 0x00000DFB
		[TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
		public List<ValueTuple<PlayerId, string, bool>> Players
		{
			[return: TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
			get;
			[param: TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })]
			private set;
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00002C04 File Offset: 0x00000E04
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00002C0C File Offset: 0x00000E0C
		[TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
		public List<ValueTuple<PlayerId, string>> InvitedPlayers
		{
			[return: TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
			get;
			[param: TupleElementNames(new string[] { "PlayerId", "PlayerName" })]
			private set;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00002C15 File Offset: 0x00000E15
		public PlayersAddedToPartyMessage()
		{
			this.Players = new List<ValueTuple<PlayerId, string, bool>>();
			this.InvitedPlayers = new List<ValueTuple<PlayerId, string>>();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00002C33 File Offset: 0x00000E33
		public PlayersAddedToPartyMessage(PlayerId playerId, string playerName, bool isPartyLeader)
			: this()
		{
			this.AddPlayer(playerId, playerName, isPartyLeader);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00002C44 File Offset: 0x00000E44
		public void AddPlayer(PlayerId playerId, string playerName, bool isPartyLeader)
		{
			this.Players.Add(new ValueTuple<PlayerId, string, bool>(playerId, playerName, isPartyLeader));
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00002C59 File Offset: 0x00000E59
		public void AddInvitedPlayer(PlayerId playerId, string playerName)
		{
			this.InvitedPlayers.Add(new ValueTuple<PlayerId, string>(playerId, playerName));
		}
	}
}
