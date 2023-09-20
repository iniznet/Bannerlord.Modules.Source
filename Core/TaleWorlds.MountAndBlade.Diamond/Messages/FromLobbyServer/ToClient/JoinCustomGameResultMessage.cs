using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000039 RID: 57
	[Serializable]
	public class JoinCustomGameResultMessage : Message
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000028ED File Offset: 0x00000AED
		// (set) Token: 0x060000CB RID: 203 RVA: 0x000028F5 File Offset: 0x00000AF5
		public JoinGameData JoinGameData { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000CC RID: 204 RVA: 0x000028FE File Offset: 0x00000AFE
		// (set) Token: 0x060000CD RID: 205 RVA: 0x00002906 File Offset: 0x00000B06
		public bool Success { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000290F File Offset: 0x00000B0F
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00002917 File Offset: 0x00000B17
		public CustomGameJoinResponse Response { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00002920 File Offset: 0x00000B20
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00002928 File Offset: 0x00000B28
		public string MatchId { get; private set; }

		// Token: 0x060000D2 RID: 210 RVA: 0x00002931 File Offset: 0x00000B31
		private JoinCustomGameResultMessage(JoinGameData joinGameData, bool success, CustomGameJoinResponse response, string matchId)
		{
			this.JoinGameData = joinGameData;
			this.Success = success;
			this.Response = response;
			this.MatchId = matchId;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00002956 File Offset: 0x00000B56
		public static JoinCustomGameResultMessage CreateSuccess(JoinGameData joinGameData, string matchId)
		{
			return new JoinCustomGameResultMessage(joinGameData, true, CustomGameJoinResponse.Success, matchId);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00002961 File Offset: 0x00000B61
		public static JoinCustomGameResultMessage CreateFailed(CustomGameJoinResponse response)
		{
			return new JoinCustomGameResultMessage(null, false, response, null);
		}
	}
}
