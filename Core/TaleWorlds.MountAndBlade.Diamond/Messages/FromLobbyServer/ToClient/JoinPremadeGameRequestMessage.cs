using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200003B RID: 59
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameRequestMessage : Message
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000298C File Offset: 0x00000B8C
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00002994 File Offset: 0x00000B94
		public Guid ChallengerPartyId { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000DA RID: 218 RVA: 0x0000299D File Offset: 0x00000B9D
		// (set) Token: 0x060000DB RID: 219 RVA: 0x000029A5 File Offset: 0x00000BA5
		public string ClanName { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000029AE File Offset: 0x00000BAE
		// (set) Token: 0x060000DD RID: 221 RVA: 0x000029B6 File Offset: 0x00000BB6
		public string Sigil { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000029BF File Offset: 0x00000BBF
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000029C7 File Offset: 0x00000BC7
		public PlayerId[] ChallengerPlayers { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000029D0 File Offset: 0x00000BD0
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x000029D8 File Offset: 0x00000BD8
		public PlayerId ChallengerPartyLeaderId { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000029E1 File Offset: 0x00000BE1
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x000029E9 File Offset: 0x00000BE9
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x060000E4 RID: 228 RVA: 0x000029F2 File Offset: 0x00000BF2
		protected JoinPremadeGameRequestMessage(Guid challengerPartyId, string clanName, string sigil, PlayerId[] challengerPlayers, PlayerId challengerPartyLeaderId, PremadeGameType premadeGameType)
		{
			this.ChallengerPartyId = challengerPartyId;
			this.ClanName = clanName;
			this.Sigil = sigil;
			this.ChallengerPlayers = challengerPlayers;
			this.ChallengerPartyLeaderId = challengerPartyLeaderId;
			this.PremadeGameType = premadeGameType;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00002A27 File Offset: 0x00000C27
		public static JoinPremadeGameRequestMessage CreateClanGameRequest(Guid challengerPartyId, string clanName, string sigil, PlayerId[] challengerPlayers)
		{
			return new JoinPremadeGameRequestMessage(challengerPartyId, clanName, sigil, challengerPlayers, PlayerId.Empty, PremadeGameType.Clan);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00002A38 File Offset: 0x00000C38
		public static JoinPremadeGameRequestMessage CreatePracticeGameRequest(Guid challengerPartyId, PlayerId leaderId, PlayerId[] challengerPlayers)
		{
			return new JoinPremadeGameRequestMessage(challengerPartyId, null, null, challengerPlayers, leaderId, PremadeGameType.Practice);
		}
	}
}
