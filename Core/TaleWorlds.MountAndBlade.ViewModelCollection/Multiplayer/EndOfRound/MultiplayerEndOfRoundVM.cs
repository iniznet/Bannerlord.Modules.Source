using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.EndOfRound
{
	// Token: 0x020000C6 RID: 198
	public class MultiplayerEndOfRoundVM : ViewModel
	{
		// Token: 0x0600128C RID: 4748 RVA: 0x0003CE60 File Offset: 0x0003B060
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

		// Token: 0x0600128D RID: 4749 RVA: 0x0003CF83 File Offset: 0x0003B183
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._multiplayerRoundComponent != null)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0003CF9C File Offset: 0x0003B19C
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

		// Token: 0x0600128F RID: 4751 RVA: 0x0003D26C File Offset: 0x0003B46C
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

		// Token: 0x06001290 RID: 4752 RVA: 0x0003D3D4 File Offset: 0x0003B5D4
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

		// Token: 0x06001291 RID: 4753 RVA: 0x0003D4D1 File Offset: 0x0003B6D1
		private void OnIsShownChanged()
		{
			if (!this.IsShown)
			{
				this.HasAttackerMVP = false;
				this.HasDefenderMVP = false;
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x0003D4E9 File Offset: 0x0003B6E9
		// (set) Token: 0x06001293 RID: 4755 RVA: 0x0003D4F1 File Offset: 0x0003B6F1
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

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x0003D515 File Offset: 0x0003B715
		// (set) Token: 0x06001295 RID: 4757 RVA: 0x0003D51D File Offset: 0x0003B71D
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

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001296 RID: 4758 RVA: 0x0003D53B File Offset: 0x0003B73B
		// (set) Token: 0x06001297 RID: 4759 RVA: 0x0003D543 File Offset: 0x0003B743
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

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001298 RID: 4760 RVA: 0x0003D561 File Offset: 0x0003B761
		// (set) Token: 0x06001299 RID: 4761 RVA: 0x0003D569 File Offset: 0x0003B769
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

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x0600129A RID: 4762 RVA: 0x0003D58C File Offset: 0x0003B78C
		// (set) Token: 0x0600129B RID: 4763 RVA: 0x0003D594 File Offset: 0x0003B794
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

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x0600129C RID: 4764 RVA: 0x0003D5B7 File Offset: 0x0003B7B7
		// (set) Token: 0x0600129D RID: 4765 RVA: 0x0003D5BF File Offset: 0x0003B7BF
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

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x0600129E RID: 4766 RVA: 0x0003D5E2 File Offset: 0x0003B7E2
		// (set) Token: 0x0600129F RID: 4767 RVA: 0x0003D5EA File Offset: 0x0003B7EA
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

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x060012A0 RID: 4768 RVA: 0x0003D608 File Offset: 0x0003B808
		// (set) Token: 0x060012A1 RID: 4769 RVA: 0x0003D610 File Offset: 0x0003B810
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

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x060012A2 RID: 4770 RVA: 0x0003D62E File Offset: 0x0003B82E
		// (set) Token: 0x060012A3 RID: 4771 RVA: 0x0003D636 File Offset: 0x0003B836
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

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x0003D654 File Offset: 0x0003B854
		// (set) Token: 0x060012A5 RID: 4773 RVA: 0x0003D65C File Offset: 0x0003B85C
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

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x060012A6 RID: 4774 RVA: 0x0003D67A File Offset: 0x0003B87A
		// (set) Token: 0x060012A7 RID: 4775 RVA: 0x0003D682 File Offset: 0x0003B882
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

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x060012A8 RID: 4776 RVA: 0x0003D6A0 File Offset: 0x0003B8A0
		// (set) Token: 0x060012A9 RID: 4777 RVA: 0x0003D6A8 File Offset: 0x0003B8A8
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

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x0003D6CB File Offset: 0x0003B8CB
		// (set) Token: 0x060012AB RID: 4779 RVA: 0x0003D6D3 File Offset: 0x0003B8D3
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

		// Token: 0x040008DB RID: 2267
		private readonly MissionScoreboardComponent _scoreboardComponent;

		// Token: 0x040008DC RID: 2268
		private readonly MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x040008DD RID: 2269
		private readonly IRoundComponent _multiplayerRoundComponent;

		// Token: 0x040008DE RID: 2270
		private readonly string _victoryText;

		// Token: 0x040008DF RID: 2271
		private readonly string _defeatText;

		// Token: 0x040008E0 RID: 2272
		private readonly TextObject _roundEndReasonAllyTeamSideDepletedTextObject;

		// Token: 0x040008E1 RID: 2273
		private readonly TextObject _roundEndReasonEnemyTeamSideDepletedTextObject;

		// Token: 0x040008E2 RID: 2274
		private readonly TextObject _roundEndReasonAllyTeamRoundTimeEndedTextObject;

		// Token: 0x040008E3 RID: 2275
		private readonly TextObject _roundEndReasonEnemyTeamRoundTimeEndedTextObject;

		// Token: 0x040008E4 RID: 2276
		private readonly TextObject _roundEndReasonAllyTeamGameModeSpecificEndedTextObject;

		// Token: 0x040008E5 RID: 2277
		private readonly TextObject _roundEndReasonEnemyTeamGameModeSpecificEndedTextObject;

		// Token: 0x040008E6 RID: 2278
		private readonly TextObject _roundEndReasonRoundTimeEndedWithDrawTextObject;

		// Token: 0x040008E7 RID: 2279
		private bool _isShown;

		// Token: 0x040008E8 RID: 2280
		private bool _hasAttackerMVP;

		// Token: 0x040008E9 RID: 2281
		private bool _hasDefenderMVP;

		// Token: 0x040008EA RID: 2282
		private string _title;

		// Token: 0x040008EB RID: 2283
		private string _description;

		// Token: 0x040008EC RID: 2284
		private string _cultureId;

		// Token: 0x040008ED RID: 2285
		private bool _isRoundWinner;

		// Token: 0x040008EE RID: 2286
		private MultiplayerEndOfRoundSideVM _attackerSide;

		// Token: 0x040008EF RID: 2287
		private MultiplayerEndOfRoundSideVM _defenderSide;

		// Token: 0x040008F0 RID: 2288
		private MPPlayerVM _attackerMVP;

		// Token: 0x040008F1 RID: 2289
		private MPPlayerVM _defenderMVP;

		// Token: 0x040008F2 RID: 2290
		private string _attackerMVPTitleText;

		// Token: 0x040008F3 RID: 2291
		private string _defenderMVPTitleText;
	}
}
