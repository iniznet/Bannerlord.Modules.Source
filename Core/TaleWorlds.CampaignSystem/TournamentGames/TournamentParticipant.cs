using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x02000282 RID: 642
	public class TournamentParticipant
	{
		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060021B7 RID: 8631 RVA: 0x0008F790 File Offset: 0x0008D990
		// (set) Token: 0x060021B8 RID: 8632 RVA: 0x0008F798 File Offset: 0x0008D998
		public int Score { get; private set; }

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x060021B9 RID: 8633 RVA: 0x0008F7A1 File Offset: 0x0008D9A1
		// (set) Token: 0x060021BA RID: 8634 RVA: 0x0008F7A9 File Offset: 0x0008D9A9
		public CharacterObject Character { get; private set; }

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060021BB RID: 8635 RVA: 0x0008F7B2 File Offset: 0x0008D9B2
		// (set) Token: 0x060021BC RID: 8636 RVA: 0x0008F7BA File Offset: 0x0008D9BA
		public UniqueTroopDescriptor Descriptor { get; private set; }

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x060021BD RID: 8637 RVA: 0x0008F7C3 File Offset: 0x0008D9C3
		// (set) Token: 0x060021BE RID: 8638 RVA: 0x0008F7CB File Offset: 0x0008D9CB
		public TournamentTeam Team { get; private set; }

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x060021BF RID: 8639 RVA: 0x0008F7D4 File Offset: 0x0008D9D4
		// (set) Token: 0x060021C0 RID: 8640 RVA: 0x0008F7DC File Offset: 0x0008D9DC
		public Equipment MatchEquipment { get; set; }

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x060021C1 RID: 8641 RVA: 0x0008F7E5 File Offset: 0x0008D9E5
		// (set) Token: 0x060021C2 RID: 8642 RVA: 0x0008F7ED File Offset: 0x0008D9ED
		public bool IsAssigned { get; set; }

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x060021C3 RID: 8643 RVA: 0x0008F7F6 File Offset: 0x0008D9F6
		public bool IsPlayer
		{
			get
			{
				CharacterObject character = this.Character;
				return character != null && character.IsPlayerCharacter;
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x0008F809 File Offset: 0x0008DA09
		public TournamentParticipant(CharacterObject character, UniqueTroopDescriptor descriptor = default(UniqueTroopDescriptor))
		{
			this.Character = character;
			this.Descriptor = (descriptor.IsValid ? descriptor : new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed));
			this.Team = null;
			this.IsAssigned = false;
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x0008F847 File Offset: 0x0008DA47
		public void SetTeam(TournamentTeam team)
		{
			this.Team = team;
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x0008F850 File Offset: 0x0008DA50
		public int AddScore(int score)
		{
			this.Score += score;
			return this.Score;
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x0008F866 File Offset: 0x0008DA66
		public void ResetScore()
		{
			this.Score = 0;
		}
	}
}
