using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	public class TournamentParticipantVM : ViewModel
	{
		public TournamentParticipant Participant { get; private set; }

		public TournamentParticipantVM()
		{
			this._visual = new ImageIdentifierVM(0);
			this._character = new CharacterViewModel(3);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.IsInitialized)
			{
				this.Refresh(this.Participant, this.TeamColor);
			}
		}

		public void Refresh(TournamentParticipant participant, Color teamColor)
		{
			this.Participant = participant;
			this.TeamColor = teamColor;
			this.State = ((participant == null) ? 0 : ((participant.Character == CharacterObject.PlayerCharacter) ? 2 : 1));
			this.IsInitialized = true;
			this._latestParticipant = participant;
			if (participant != null)
			{
				this.Name = participant.Character.Name.ToString();
				CharacterCode characterCode = SandBoxUIHelper.GetCharacterCode(participant.Character, false);
				this.Character = new CharacterViewModel(3);
				this.Character.FillFrom(participant.Character, -1);
				this.Visual = new ImageIdentifierVM(characterCode);
				this.IsValid = true;
				this.IsMainHero = participant.Character.IsPlayerCharacter;
			}
		}

		public void ExecuteOpenEncyclopedia()
		{
			TournamentParticipant participant = this.Participant;
			if (((participant != null) ? participant.Character : null) != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Participant.Character.EncyclopediaLink);
			}
		}

		public void Refresh()
		{
			base.OnPropertyChanged("Name");
			base.OnPropertyChanged("Visual");
			base.OnPropertyChanged("Score");
			base.OnPropertyChanged("State");
			base.OnPropertyChanged("TeamColor");
			base.OnPropertyChanged("IsDead");
			TournamentParticipant latestParticipant = this._latestParticipant;
			this.IsMainHero = latestParticipant != null && latestParticipant.Character.IsPlayerCharacter;
		}

		[DataSourceProperty]
		public bool IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
			set
			{
				if (value != this._isInitialized)
				{
					this._isInitialized = value;
					base.OnPropertyChangedWithValue(value, "IsInitialized");
				}
			}
		}

		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChangedWithValue(value, "IsDead");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		[DataSourceProperty]
		public Color TeamColor
		{
			get
			{
				return this._teamColor;
			}
			set
			{
				if (value != this._teamColor)
				{
					this._teamColor = value;
					base.OnPropertyChangedWithValue(value, "TeamColor");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public int State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (value != this._state)
				{
					this._state = value;
					base.OnPropertyChangedWithValue(value, "State");
				}
			}
		}

		[DataSourceProperty]
		public bool IsQualifiedForNextRound
		{
			get
			{
				return this._isQualifiedForNextRound;
			}
			set
			{
				if (value != this._isQualifiedForNextRound)
				{
					this._isQualifiedForNextRound = value;
					base.OnPropertyChangedWithValue(value, "IsQualifiedForNextRound");
				}
			}
		}

		[DataSourceProperty]
		public string Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue<string>(value, "Score");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public CharacterViewModel Character
		{
			get
			{
				return this._character;
			}
			set
			{
				if (value != this._character)
				{
					this._character = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "Character");
				}
			}
		}

		private TournamentParticipant _latestParticipant;

		private bool _isInitialized;

		private bool _isValid;

		private string _name = "";

		private string _score = "-";

		private bool _isQualifiedForNextRound;

		private int _state = -1;

		private ImageIdentifierVM _visual;

		private Color _teamColor;

		private bool _isDead;

		private bool _isMainHero;

		private CharacterViewModel _character;

		public enum TournamentPlayerState
		{
			EmptyPlayer,
			GenericPlayer,
			MainPlayer
		}
	}
}
