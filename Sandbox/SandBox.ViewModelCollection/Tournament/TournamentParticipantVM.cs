using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	// Token: 0x02000009 RID: 9
	public class TournamentParticipantVM : ViewModel
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000054BA File Offset: 0x000036BA
		// (set) Token: 0x0600005A RID: 90 RVA: 0x000054C2 File Offset: 0x000036C2
		public TournamentParticipant Participant { get; private set; }

		// Token: 0x0600005B RID: 91 RVA: 0x000054CB File Offset: 0x000036CB
		public TournamentParticipantVM()
		{
			this._visual = new ImageIdentifierVM(0);
			this._character = new CharacterViewModel(3);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005508 File Offset: 0x00003708
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.IsInitialized)
			{
				this.Refresh(this.Participant, this.TeamColor);
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000552C File Offset: 0x0000372C
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

		// Token: 0x0600005E RID: 94 RVA: 0x000055DA File Offset: 0x000037DA
		public void ExecuteOpenEncyclopedia()
		{
			TournamentParticipant participant = this.Participant;
			if (((participant != null) ? participant.Character : null) != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Participant.Character.EncyclopediaLink);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005610 File Offset: 0x00003810
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000567C File Offset: 0x0000387C
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00005684 File Offset: 0x00003884
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000056A2 File Offset: 0x000038A2
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000056AA File Offset: 0x000038AA
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000056C8 File Offset: 0x000038C8
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000056D0 File Offset: 0x000038D0
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000056EE File Offset: 0x000038EE
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000056F6 File Offset: 0x000038F6
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

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00005714 File Offset: 0x00003914
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000571C File Offset: 0x0000391C
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

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600006A RID: 106 RVA: 0x0000573F File Offset: 0x0000393F
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00005747 File Offset: 0x00003947
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

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00005765 File Offset: 0x00003965
		// (set) Token: 0x0600006D RID: 109 RVA: 0x0000576D File Offset: 0x0000396D
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

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000578B File Offset: 0x0000398B
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00005793 File Offset: 0x00003993
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

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000057B1 File Offset: 0x000039B1
		// (set) Token: 0x06000071 RID: 113 RVA: 0x000057B9 File Offset: 0x000039B9
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000072 RID: 114 RVA: 0x000057DC File Offset: 0x000039DC
		// (set) Token: 0x06000073 RID: 115 RVA: 0x000057E4 File Offset: 0x000039E4
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

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00005807 File Offset: 0x00003A07
		// (set) Token: 0x06000075 RID: 117 RVA: 0x0000580F File Offset: 0x00003A0F
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

		// Token: 0x0400001B RID: 27
		private TournamentParticipant _latestParticipant;

		// Token: 0x0400001D RID: 29
		private bool _isInitialized;

		// Token: 0x0400001E RID: 30
		private bool _isValid;

		// Token: 0x0400001F RID: 31
		private string _name = "";

		// Token: 0x04000020 RID: 32
		private string _score = "-";

		// Token: 0x04000021 RID: 33
		private bool _isQualifiedForNextRound;

		// Token: 0x04000022 RID: 34
		private int _state = -1;

		// Token: 0x04000023 RID: 35
		private ImageIdentifierVM _visual;

		// Token: 0x04000024 RID: 36
		private Color _teamColor;

		// Token: 0x04000025 RID: 37
		private bool _isDead;

		// Token: 0x04000026 RID: 38
		private bool _isMainHero;

		// Token: 0x04000027 RID: 39
		private CharacterViewModel _character;

		// Token: 0x0200004D RID: 77
		public enum TournamentPlayerState
		{
			// Token: 0x04000281 RID: 641
			EmptyPlayer,
			// Token: 0x04000282 RID: 642
			GenericPlayer,
			// Token: 0x04000283 RID: 643
			MainPlayer
		}
	}
}
