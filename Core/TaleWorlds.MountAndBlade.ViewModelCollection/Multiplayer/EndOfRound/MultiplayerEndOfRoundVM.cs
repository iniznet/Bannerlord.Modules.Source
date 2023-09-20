using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.EndOfRound
{
	public class MultiplayerEndOfRoundVM : ViewModel
	{
		public MultiplayerEndOfRoundVM(MissionScoreboardComponent scoreboardComponent, MissionLobbyComponent missionLobbyComponent, IRoundComponent multiplayerRoundComponent)
		{
			this._scoreboardComponent = scoreboardComponent;
			this._multiplayerRoundComponent = multiplayerRoundComponent;
			this._missionLobbyComponent = missionLobbyComponent;
			this._victoryText = new TextObject("{=RCuCoVgd}ROUND WON", null).ToString();
			this._defeatText = new TextObject("{=Dbkx4v90}ROUND LOST", null).ToString();
			this._roundEndReasonAllyTeamSideDepletedTextObject = new TextObject("{=9M4G8DDd}Your team was wiped out", null);
			this._roundEndReasonEnemyTeamSideDepletedTextObject = new TextObject("{=jPXglGWT}Enemy team was wiped out", null);
			this._roundEndReasonAllyTeamRoundTimeEndedTextObject = new TextObject("{=x1HZy70i}Your team had the upper hand at timeout", null);
			this._roundEndReasonEnemyTeamRoundTimeEndedTextObject = new TextObject("{=Dc3fFblo}Enemy team had the upper hand at timeout", null);
			this._roundEndReasonRoundTimeEndedWithDrawTextObject = new TextObject("{=i3dJSlD0}No team had the upper hand at timeout", null);
			if (this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Battle || this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Captain || this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Skirmish)
			{
				this._roundEndReasonAllyTeamGameModeSpecificEndedTextObject = new TextObject("{=xxuzZJ3G}Your team ran out of morale", null);
				this._roundEndReasonEnemyTeamGameModeSpecificEndedTextObject = new TextObject("{=c6c9eYrD}Enemy team ran out of morale", null);
			}
			else
			{
				this._roundEndReasonAllyTeamGameModeSpecificEndedTextObject = TextObject.Empty;
				this._roundEndReasonEnemyTeamGameModeSpecificEndedTextObject = TextObject.Empty;
			}
			this.AttackerSide = new MultiplayerEndOfRoundSideVM();
			this.DefenderSide = new MultiplayerEndOfRoundSideVM();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._multiplayerRoundComponent != null)
			{
				this.Refresh();
			}
		}

		public void Refresh()
		{
			BattleSideEnum allyBattleSide = BattleSideEnum.None;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (missionPeer != null && missionPeer.Team != null)
			{
				allyBattleSide = missionPeer.Team.Side;
			}
			BattleSideEnum battleSideEnum = ((allyBattleSide == BattleSideEnum.Attacker) ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject object2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = this._scoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == BattleSideEnum.Attacker);
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide2 = this._scoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == BattleSideEnum.Defender);
			bool flag = this._multiplayerRoundComponent.RoundWinner == BattleSideEnum.Attacker;
			bool flag2 = this._multiplayerRoundComponent.RoundWinner == BattleSideEnum.Defender;
			Team team = missionPeer.Team;
			if (team != null && team.Side == BattleSideEnum.Attacker)
			{
				this.AttackerMVPTitleText = this.GetMVPTitleText(@object);
				this.DefenderMVPTitleText = this.GetMVPTitleText(object2);
				this.AttackerSide.SetData(@object, missionScoreboardSide.SideScore, flag, false);
				this.DefenderSide.SetData(object2, missionScoreboardSide2.SideScore, flag2, @object == object2);
			}
			else
			{
				this.DefenderMVPTitleText = this.GetMVPTitleText(@object);
				this.AttackerMVPTitleText = this.GetMVPTitleText(object2);
				this.DefenderSide.SetData(@object, missionScoreboardSide.SideScore, flag, @object == object2);
				this.AttackerSide.SetData(object2, missionScoreboardSide2.SideScore, flag2, false);
			}
			if (this._scoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == allyBattleSide) != null && this._multiplayerRoundComponent != null)
			{
				bool flag3 = false;
				if (this._multiplayerRoundComponent.RoundWinner == allyBattleSide)
				{
					this.IsRoundWinner = true;
					this.Title = this._victoryText;
				}
				else if (this._multiplayerRoundComponent.RoundWinner == battleSideEnum)
				{
					this.IsRoundWinner = false;
					this.Title = this._defeatText;
				}
				else
				{
					flag3 = true;
				}
				RoundEndReason roundEndReason = this._multiplayerRoundComponent.RoundEndReason;
				if (roundEndReason == RoundEndReason.SideDepleted)
				{
					this.Description = (this.IsRoundWinner ? this._roundEndReasonEnemyTeamSideDepletedTextObject.ToString() : this._roundEndReasonAllyTeamSideDepletedTextObject.ToString());
					return;
				}
				if (roundEndReason == RoundEndReason.GameModeSpecificEnded)
				{
					this.Description = (this.IsRoundWinner ? this._roundEndReasonEnemyTeamGameModeSpecificEndedTextObject.ToString() : this._roundEndReasonAllyTeamGameModeSpecificEndedTextObject.ToString());
					return;
				}
				if (roundEndReason == RoundEndReason.RoundTimeEnded)
				{
					this.Description = (this.IsRoundWinner ? this._roundEndReasonAllyTeamRoundTimeEndedTextObject.ToString() : (flag3 ? this._roundEndReasonRoundTimeEndedWithDrawTextObject.ToString() : this._roundEndReasonEnemyTeamRoundTimeEndedTextObject.ToString()));
				}
			}
		}

		public void OnMVPSelected(MissionPeer mvpPeer)
		{
			BasicCharacterObject @object = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
			@object.UpdatePlayerCharacterBodyProperties(mvpPeer.Peer.BodyProperties, mvpPeer.Peer.Race, mvpPeer.Peer.IsFemale);
			@object.Age = mvpPeer.Peer.BodyProperties.Age;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			Team team = mvpPeer.Team;
			BattleSideEnum? battleSideEnum = ((team != null) ? new BattleSideEnum?(team.Side) : null);
			Team team2 = missionPeer.Team;
			BattleSideEnum? battleSideEnum2 = ((team2 != null) ? new BattleSideEnum?(team2.Side) : null);
			if ((battleSideEnum.GetValueOrDefault() == battleSideEnum2.GetValueOrDefault()) & (battleSideEnum != null == (battleSideEnum2 != null)))
			{
				this.AttackerMVP = new MPPlayerVM(mvpPeer);
				this.AttackerMVP.RefreshDivision(false);
				this.AttackerMVP.RefreshPreview(@object, mvpPeer.Peer.BodyProperties.DynamicProperties, mvpPeer.Peer.IsFemale);
				this.HasAttackerMVP = true;
				return;
			}
			this.DefenderMVP = new MPPlayerVM(mvpPeer);
			this.DefenderMVP.RefreshDivision(false);
			this.DefenderMVP.RefreshPreview(@object, mvpPeer.Peer.BodyProperties.DynamicProperties, mvpPeer.Peer.IsFemale);
			this.HasDefenderMVP = true;
		}

		private string GetMVPTitleText(BasicCultureObject culture)
		{
			if (culture.StringId == "vlandia")
			{
				return new TextObject("{=3VosbFR0}Vlandian Champion", null).ToString();
			}
			if (culture.StringId == "sturgia")
			{
				return new TextObject("{=AGUXiN8u}Voivode", null).ToString();
			}
			if (culture.StringId == "khuzait")
			{
				return new TextObject("{=F2h2cT4q}Khan's Chosen", null).ToString();
			}
			if (culture.StringId == "battania")
			{
				return new TextObject("{=eWPN3HmE}Hero of Battania", null).ToString();
			}
			if (culture.StringId == "aserai")
			{
				return new TextObject("{=5zNfxZ7B}War Prince", null).ToString();
			}
			if (culture.StringId == "empire")
			{
				return new TextObject("{=wwbIcqsq}Conqueror", null).ToString();
			}
			Debug.FailedAssert("Invalid Culture ID for MVP Title", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\EndOfRound\\MultiplayerEndOfRoundVM.cs", "GetMVPTitleText", 210);
			return string.Empty;
		}

		private void OnIsShownChanged()
		{
			if (!this.IsShown)
			{
				this.HasAttackerMVP = false;
				this.HasDefenderMVP = false;
			}
		}

		[DataSourceProperty]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChangedWithValue(value, "IsShown");
					this.OnIsShownChanged();
				}
			}
		}

		[DataSourceProperty]
		public bool HasAttackerMVP
		{
			get
			{
				return this._hasAttackerMVP;
			}
			set
			{
				if (value != this._hasAttackerMVP)
				{
					this._hasAttackerMVP = value;
					base.OnPropertyChangedWithValue(value, "HasAttackerMVP");
				}
			}
		}

		[DataSourceProperty]
		public bool HasDefenderMVP
		{
			get
			{
				return this._hasDefenderMVP;
			}
			set
			{
				if (value != this._hasDefenderMVP)
				{
					this._hasDefenderMVP = value;
					base.OnPropertyChangedWithValue(value, "HasDefenderMVP");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRoundWinner
		{
			get
			{
				return this._isRoundWinner;
			}
			set
			{
				if (value != this._isRoundWinner)
				{
					this._isRoundWinner = value;
					base.OnPropertyChangedWithValue(value, "IsRoundWinner");
				}
			}
		}

		[DataSourceProperty]
		public MultiplayerEndOfRoundSideVM AttackerSide
		{
			get
			{
				return this._attackerSide;
			}
			set
			{
				if (value != this._attackerSide)
				{
					this._attackerSide = value;
					base.OnPropertyChangedWithValue<MultiplayerEndOfRoundSideVM>(value, "AttackerSide");
				}
			}
		}

		[DataSourceProperty]
		public MultiplayerEndOfRoundSideVM DefenderSide
		{
			get
			{
				return this._defenderSide;
			}
			set
			{
				if (value != this._defenderSide)
				{
					this._defenderSide = value;
					base.OnPropertyChangedWithValue<MultiplayerEndOfRoundSideVM>(value, "DefenderSide");
				}
			}
		}

		[DataSourceProperty]
		public MPPlayerVM AttackerMVP
		{
			get
			{
				return this._attackerMVP;
			}
			set
			{
				if (value != this._attackerMVP)
				{
					this._attackerMVP = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "AttackerMVP");
				}
			}
		}

		[DataSourceProperty]
		public MPPlayerVM DefenderMVP
		{
			get
			{
				return this._defenderMVP;
			}
			set
			{
				if (value != this._defenderMVP)
				{
					this._defenderMVP = value;
					base.OnPropertyChangedWithValue<MPPlayerVM>(value, "DefenderMVP");
				}
			}
		}

		[DataSourceProperty]
		public string AttackerMVPTitleText
		{
			get
			{
				return this._attackerMVPTitleText;
			}
			set
			{
				if (value != this._attackerMVPTitleText)
				{
					this._attackerMVPTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerMVPTitleText");
				}
			}
		}

		[DataSourceProperty]
		public string DefenderMVPTitleText
		{
			get
			{
				return this._defenderMVPTitleText;
			}
			set
			{
				if (value != this._defenderMVPTitleText)
				{
					this._defenderMVPTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderMVPTitleText");
				}
			}
		}

		private readonly MissionScoreboardComponent _scoreboardComponent;

		private readonly MissionLobbyComponent _missionLobbyComponent;

		private readonly IRoundComponent _multiplayerRoundComponent;

		private readonly string _victoryText;

		private readonly string _defeatText;

		private readonly TextObject _roundEndReasonAllyTeamSideDepletedTextObject;

		private readonly TextObject _roundEndReasonEnemyTeamSideDepletedTextObject;

		private readonly TextObject _roundEndReasonAllyTeamRoundTimeEndedTextObject;

		private readonly TextObject _roundEndReasonEnemyTeamRoundTimeEndedTextObject;

		private readonly TextObject _roundEndReasonAllyTeamGameModeSpecificEndedTextObject;

		private readonly TextObject _roundEndReasonEnemyTeamGameModeSpecificEndedTextObject;

		private readonly TextObject _roundEndReasonRoundTimeEndedWithDrawTextObject;

		private bool _isShown;

		private bool _hasAttackerMVP;

		private bool _hasDefenderMVP;

		private string _title;

		private string _description;

		private string _cultureId;

		private bool _isRoundWinner;

		private MultiplayerEndOfRoundSideVM _attackerSide;

		private MultiplayerEndOfRoundSideVM _defenderSide;

		private MPPlayerVM _attackerMVP;

		private MPPlayerVM _defenderMVP;

		private string _attackerMVPTitleText;

		private string _defenderMVPTitleText;
	}
}
