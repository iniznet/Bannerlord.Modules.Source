using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x02000284 RID: 644
	public class TournamentTeam
	{
		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x060021D1 RID: 8657 RVA: 0x0008F97F File Offset: 0x0008DB7F
		// (set) Token: 0x060021D2 RID: 8658 RVA: 0x0008F987 File Offset: 0x0008DB87
		public int TeamSize { get; private set; }

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x060021D3 RID: 8659 RVA: 0x0008F990 File Offset: 0x0008DB90
		// (set) Token: 0x060021D4 RID: 8660 RVA: 0x0008F998 File Offset: 0x0008DB98
		public uint TeamColor { get; private set; }

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x060021D5 RID: 8661 RVA: 0x0008F9A1 File Offset: 0x0008DBA1
		// (set) Token: 0x060021D6 RID: 8662 RVA: 0x0008F9A9 File Offset: 0x0008DBA9
		public Banner TeamBanner { get; private set; }

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x060021D7 RID: 8663 RVA: 0x0008F9B2 File Offset: 0x0008DBB2
		// (set) Token: 0x060021D8 RID: 8664 RVA: 0x0008F9BA File Offset: 0x0008DBBA
		public bool IsPlayerTeam { get; private set; }

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x060021D9 RID: 8665 RVA: 0x0008F9C3 File Offset: 0x0008DBC3
		public IEnumerable<TournamentParticipant> Participants
		{
			get
			{
				return this._participants.AsEnumerable<TournamentParticipant>();
			}
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x060021DA RID: 8666 RVA: 0x0008F9D0 File Offset: 0x0008DBD0
		public int Score
		{
			get
			{
				int num = 0;
				foreach (TournamentParticipant tournamentParticipant in this._participants)
				{
					num += tournamentParticipant.Score;
				}
				return num;
			}
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x0008FA28 File Offset: 0x0008DC28
		public TournamentTeam(int teamSize, uint teamColor, Banner teamBanner)
		{
			this.TeamColor = teamColor;
			this.TeamBanner = teamBanner;
			this.TeamSize = teamSize;
			this._participants = new List<TournamentParticipant>();
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x0008FA50 File Offset: 0x0008DC50
		public bool IsParticipantRequired()
		{
			return this._participants.Count < this.TeamSize;
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x0008FA65 File Offset: 0x0008DC65
		public void AddParticipant(TournamentParticipant participant)
		{
			participant.IsAssigned = true;
			this._participants.Add(participant);
			participant.SetTeam(this);
			if (participant.IsPlayer)
			{
				this.IsPlayerTeam = true;
			}
		}

		// Token: 0x04000A85 RID: 2693
		private List<TournamentParticipant> _participants;
	}
}
