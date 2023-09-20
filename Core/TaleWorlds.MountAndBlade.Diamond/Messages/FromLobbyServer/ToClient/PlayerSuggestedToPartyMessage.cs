using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200004B RID: 75
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerSuggestedToPartyMessage : Message
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00002C6D File Offset: 0x00000E6D
		public PlayerId PlayerId { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00002C75 File Offset: 0x00000E75
		public string PlayerName { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00002C7D File Offset: 0x00000E7D
		public PlayerId SuggestingPlayerId { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600011A RID: 282 RVA: 0x00002C85 File Offset: 0x00000E85
		public string SuggestingPlayerName { get; }

		// Token: 0x0600011B RID: 283 RVA: 0x00002C8D File Offset: 0x00000E8D
		public PlayerSuggestedToPartyMessage(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.SuggestingPlayerId = suggestingPlayerId;
			this.SuggestingPlayerName = suggestingPlayerName;
		}
	}
}
