using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	public class TournamentParticipant
	{
		public int Score { get; private set; }

		public CharacterObject Character { get; private set; }

		public UniqueTroopDescriptor Descriptor { get; private set; }

		public TournamentTeam Team { get; private set; }

		public Equipment MatchEquipment { get; set; }

		public bool IsAssigned { get; set; }

		public bool IsPlayer
		{
			get
			{
				CharacterObject character = this.Character;
				return character != null && character.IsPlayerCharacter;
			}
		}

		public TournamentParticipant(CharacterObject character, UniqueTroopDescriptor descriptor = default(UniqueTroopDescriptor))
		{
			this.Character = character;
			this.Descriptor = (descriptor.IsValid ? descriptor : new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed));
			this.Team = null;
			this.IsAssigned = false;
		}

		public void SetTeam(TournamentTeam team)
		{
			this.Team = team;
		}

		public int AddScore(int score)
		{
			this.Score += score;
			return this.Score;
		}

		public void ResetScore()
		{
			this.Score = 0;
		}
	}
}
