using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000305 RID: 773
	public class MissionPeer : PeerComponent
	{
		// Token: 0x14000081 RID: 129
		// (add) Token: 0x060029A0 RID: 10656 RVA: 0x000A16B0 File Offset: 0x0009F8B0
		// (remove) Token: 0x060029A1 RID: 10657 RVA: 0x000A16E4 File Offset: 0x0009F8E4
		public static event MissionPeer.OnUpdateEquipmentSetIndexEventDelegate OnEquipmentIndexRefreshed;

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x060029A2 RID: 10658 RVA: 0x000A1718 File Offset: 0x0009F918
		// (remove) Token: 0x060029A3 RID: 10659 RVA: 0x000A174C File Offset: 0x0009F94C
		public static event MissionPeer.OnPerkUpdateEventDelegate OnPerkSelectionUpdated;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x060029A4 RID: 10660 RVA: 0x000A1780 File Offset: 0x0009F980
		// (remove) Token: 0x060029A5 RID: 10661 RVA: 0x000A17B4 File Offset: 0x0009F9B4
		public static event MissionPeer.OnTeamChangedDelegate OnPreTeamChanged;

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x060029A6 RID: 10662 RVA: 0x000A17E8 File Offset: 0x0009F9E8
		// (remove) Token: 0x060029A7 RID: 10663 RVA: 0x000A181C File Offset: 0x0009FA1C
		public static event MissionPeer.OnTeamChangedDelegate OnTeamChanged;

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x060029A8 RID: 10664 RVA: 0x000A1850 File Offset: 0x0009FA50
		// (remove) Token: 0x060029A9 RID: 10665 RVA: 0x000A1888 File Offset: 0x0009FA88
		private event MissionPeer.OnCultureChangedDelegate OnCultureChanged;

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x060029AA RID: 10666 RVA: 0x000A18C0 File Offset: 0x0009FAC0
		// (remove) Token: 0x060029AB RID: 10667 RVA: 0x000A18F4 File Offset: 0x0009FAF4
		public static event MissionPeer.OnPlayerKilledDelegate OnPlayerKilled;

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x060029AC RID: 10668 RVA: 0x000A1927 File Offset: 0x0009FB27
		// (set) Token: 0x060029AD RID: 10669 RVA: 0x000A192F File Offset: 0x0009FB2F
		public DateTime JoinTime { get; internal set; }

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x060029AE RID: 10670 RVA: 0x000A1938 File Offset: 0x0009FB38
		// (set) Token: 0x060029AF RID: 10671 RVA: 0x000A1940 File Offset: 0x0009FB40
		public bool EquipmentUpdatingExpired { get; set; }

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x060029B0 RID: 10672 RVA: 0x000A1949 File Offset: 0x0009FB49
		// (set) Token: 0x060029B1 RID: 10673 RVA: 0x000A1951 File Offset: 0x0009FB51
		public bool TeamInitialPerkInfoReady { get; private set; }

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x060029B2 RID: 10674 RVA: 0x000A195A File Offset: 0x0009FB5A
		// (set) Token: 0x060029B3 RID: 10675 RVA: 0x000A1962 File Offset: 0x0009FB62
		public bool HasSpawnedAgentVisuals { get; set; }

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x060029B4 RID: 10676 RVA: 0x000A196B File Offset: 0x0009FB6B
		// (set) Token: 0x060029B5 RID: 10677 RVA: 0x000A1973 File Offset: 0x0009FB73
		public int SelectedTroopIndex
		{
			get
			{
				return this._selectedTroopIndex;
			}
			set
			{
				if (this._selectedTroopIndex != value)
				{
					this._selectedTroopIndex = value;
					this.ResetSelectedPerks();
					MissionPeer.OnUpdateEquipmentSetIndexEventDelegate onEquipmentIndexRefreshed = MissionPeer.OnEquipmentIndexRefreshed;
					if (onEquipmentIndexRefreshed == null)
					{
						return;
					}
					onEquipmentIndexRefreshed(this, value);
				}
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x060029B6 RID: 10678 RVA: 0x000A199C File Offset: 0x0009FB9C
		// (set) Token: 0x060029B7 RID: 10679 RVA: 0x000A19A4 File Offset: 0x0009FBA4
		public int NextSelectedTroopIndex { get; set; }

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x060029B8 RID: 10680 RVA: 0x000A19AD File Offset: 0x0009FBAD
		public MissionRepresentativeBase Representative
		{
			get
			{
				if (this._representative == null)
				{
					this._representative = base.Peer.GetComponent<MissionRepresentativeBase>();
				}
				return this._representative;
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x060029B9 RID: 10681 RVA: 0x000A19CE File Offset: 0x0009FBCE
		public MBReadOnlyList<int[]> Perks
		{
			get
			{
				return this._perks;
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x060029BA RID: 10682 RVA: 0x000A19D8 File Offset: 0x0009FBD8
		public string DisplayedName
		{
			get
			{
				if (GameNetwork.IsDedicatedServer)
				{
					return base.Name;
				}
				if (NetworkMain.GameClient.HasUserGeneratedContentPrivilege && (NetworkMain.GameClient.IsKnownPlayer(base.Peer.Id) || !BannerlordConfig.EnableGenericNames))
				{
					VirtualPlayer peer = base.Peer;
					return ((peer != null) ? peer.UserName : null) ?? "";
				}
				if (this.Culture == null || MultiplayerClassDivisions.GetMPHeroClassForPeer(this, false) == null)
				{
					return new TextObject("{=RN6zHak0}Player", null).ToString();
				}
				return MultiplayerClassDivisions.GetMPHeroClassForPeer(this, false).TroopName.ToString();
			}
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x060029BB RID: 10683 RVA: 0x000A1A6C File Offset: 0x0009FC6C
		public MBReadOnlyList<MPPerkObject> SelectedPerks
		{
			get
			{
				if (this.SelectedTroopIndex < 0 || this.Team == null || this.Team.Side == BattleSideEnum.None)
				{
					return new MBList<MPPerkObject>();
				}
				if (this._selectedPerks.Item2 == null || this.SelectedTroopIndex != this._selectedPerks.Item1 || this._selectedPerks.Item2.Count < 3)
				{
					MBList<MPPerkObject> mblist = new MBList<MPPerkObject>();
					List<List<IReadOnlyPerkObject>> availablePerksForPeer = MultiplayerClassDivisions.GetAvailablePerksForPeer(this);
					if (availablePerksForPeer.Count != 3)
					{
						return mblist;
					}
					for (int i = 0; i < 3; i++)
					{
						int num = this._perks[this.SelectedTroopIndex][i];
						if (availablePerksForPeer[i].Count > 0)
						{
							mblist.Add(availablePerksForPeer[i][(num >= 0 && num < availablePerksForPeer[i].Count) ? num : 0].Clone(this));
						}
					}
					this._selectedPerks = new ValueTuple<int, MBList<MPPerkObject>>(this.SelectedTroopIndex, mblist);
				}
				return this._selectedPerks.Item2;
			}
		}

		// Token: 0x060029BC RID: 10684 RVA: 0x000A1B6C File Offset: 0x0009FD6C
		public MissionPeer()
		{
			this.SpawnTimer = new Timer(Mission.Current.CurrentTime, 3f, false);
			this._selectedPerks = new ValueTuple<int, MBList<MPPerkObject>>(0, null);
			this._perks = new MBList<int[]>();
			for (int i = 0; i < 16; i++)
			{
				int[] array = new int[3];
				this._perks.Add(array);
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x060029BD RID: 10685 RVA: 0x000A1BFB File Offset: 0x0009FDFB
		// (set) Token: 0x060029BE RID: 10686 RVA: 0x000A1C03 File Offset: 0x0009FE03
		public Timer SpawnTimer { get; internal set; }

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x060029BF RID: 10687 RVA: 0x000A1C0C File Offset: 0x0009FE0C
		// (set) Token: 0x060029C0 RID: 10688 RVA: 0x000A1C14 File Offset: 0x0009FE14
		public bool HasSpawnTimerExpired { get; set; }

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x060029C1 RID: 10689 RVA: 0x000A1C1D File Offset: 0x0009FE1D
		// (set) Token: 0x060029C2 RID: 10690 RVA: 0x000A1C25 File Offset: 0x0009FE25
		public BasicCultureObject VotedForBan { get; private set; }

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x060029C3 RID: 10691 RVA: 0x000A1C2E File Offset: 0x0009FE2E
		// (set) Token: 0x060029C4 RID: 10692 RVA: 0x000A1C36 File Offset: 0x0009FE36
		public BasicCultureObject VotedForSelection { get; private set; }

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x060029C5 RID: 10693 RVA: 0x000A1C3F File Offset: 0x0009FE3F
		// (set) Token: 0x060029C6 RID: 10694 RVA: 0x000A1C47 File Offset: 0x0009FE47
		public bool WantsToSpawnAsBot { get; set; }

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x060029C7 RID: 10695 RVA: 0x000A1C50 File Offset: 0x0009FE50
		// (set) Token: 0x060029C8 RID: 10696 RVA: 0x000A1C58 File Offset: 0x0009FE58
		public int SpawnCountThisRound { get; set; }

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x060029C9 RID: 10697 RVA: 0x000A1C61 File Offset: 0x0009FE61
		// (set) Token: 0x060029CA RID: 10698 RVA: 0x000A1C69 File Offset: 0x0009FE69
		public int RequestedKickPollCount { get; private set; }

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x060029CB RID: 10699 RVA: 0x000A1C72 File Offset: 0x0009FE72
		// (set) Token: 0x060029CC RID: 10700 RVA: 0x000A1C7A File Offset: 0x0009FE7A
		public int KillCount
		{
			get
			{
				return this._killCount;
			}
			internal set
			{
				this._killCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x060029CD RID: 10701 RVA: 0x000A1C92 File Offset: 0x0009FE92
		// (set) Token: 0x060029CE RID: 10702 RVA: 0x000A1C9A File Offset: 0x0009FE9A
		public int AssistCount
		{
			get
			{
				return this._assistCount;
			}
			internal set
			{
				this._assistCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x060029CF RID: 10703 RVA: 0x000A1CB2 File Offset: 0x0009FEB2
		// (set) Token: 0x060029D0 RID: 10704 RVA: 0x000A1CBA File Offset: 0x0009FEBA
		public int DeathCount
		{
			get
			{
				return this._deathCount;
			}
			internal set
			{
				this._deathCount = MBMath.ClampInt(value, -1000, 100000);
			}
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x060029D1 RID: 10705 RVA: 0x000A1CD2 File Offset: 0x0009FED2
		// (set) Token: 0x060029D2 RID: 10706 RVA: 0x000A1CDA File Offset: 0x0009FEDA
		public int Score
		{
			get
			{
				return this._score;
			}
			internal set
			{
				this._score = MBMath.ClampInt(value, -1000000, 1000000);
			}
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x060029D3 RID: 10707 RVA: 0x000A1CF2 File Offset: 0x0009FEF2
		// (set) Token: 0x060029D4 RID: 10708 RVA: 0x000A1CFA File Offset: 0x0009FEFA
		public int BotsUnderControlAlive
		{
			get
			{
				return this._botsUnderControlAlive;
			}
			set
			{
				if (this._botsUnderControlAlive != value)
				{
					this._botsUnderControlAlive = value;
					MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this);
					if (perkHandler == null)
					{
						return;
					}
					perkHandler.OnEvent(MPPerkCondition.PerkEventFlags.AliveBotCountChange);
				}
			}
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x060029D5 RID: 10709 RVA: 0x000A1D1E File Offset: 0x0009FF1E
		// (set) Token: 0x060029D6 RID: 10710 RVA: 0x000A1D26 File Offset: 0x0009FF26
		public int BotsUnderControlTotal { get; internal set; }

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x060029D7 RID: 10711 RVA: 0x000A1D2F File Offset: 0x0009FF2F
		public bool IsControlledAgentActive
		{
			get
			{
				return this.ControlledAgent != null && this.ControlledAgent.IsActive();
			}
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x060029D8 RID: 10712 RVA: 0x000A1D46 File Offset: 0x0009FF46
		// (set) Token: 0x060029D9 RID: 10713 RVA: 0x000A1D54 File Offset: 0x0009FF54
		public Agent ControlledAgent
		{
			get
			{
				return this.GetNetworkPeer().ControlledAgent;
			}
			set
			{
				NetworkCommunicator networkPeer = this.GetNetworkPeer();
				if (networkPeer.ControlledAgent != value)
				{
					this.ResetSelectedPerks();
					Agent controlledAgent = networkPeer.ControlledAgent;
					networkPeer.ControlledAgent = value;
					if (controlledAgent != null && controlledAgent.MissionPeer == this && controlledAgent.IsActive())
					{
						controlledAgent.MissionPeer = null;
					}
					if (networkPeer.ControlledAgent != null && networkPeer.ControlledAgent.MissionPeer != this)
					{
						networkPeer.ControlledAgent.MissionPeer = this;
					}
					MissionRepresentativeBase component = networkPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
					if (component != null)
					{
						component.SetAgent(value);
					}
					if (value != null)
					{
						MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(this);
						if (perkHandler == null)
						{
							return;
						}
						perkHandler.OnEvent(value, MPPerkCondition.PerkEventFlags.PeerControlledAgentChange);
					}
				}
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x060029DA RID: 10714 RVA: 0x000A1DEE File Offset: 0x0009FFEE
		// (set) Token: 0x060029DB RID: 10715 RVA: 0x000A1DF6 File Offset: 0x0009FFF6
		public Agent FollowedAgent
		{
			get
			{
				return this._followedAgent;
			}
			set
			{
				if (this._followedAgent != value)
				{
					this._followedAgent = value;
					if (GameNetwork.IsClient)
					{
						GameNetwork.BeginModuleEventAsClient();
						GameNetwork.WriteMessage(new SetFollowedAgent(this._followedAgent));
						GameNetwork.EndModuleEventAsClient();
					}
				}
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x060029DC RID: 10716 RVA: 0x000A1E29 File Offset: 0x000A0029
		// (set) Token: 0x060029DD RID: 10717 RVA: 0x000A1E34 File Offset: 0x000A0034
		public Team Team
		{
			get
			{
				return this._team;
			}
			set
			{
				if (this._team != value)
				{
					if (MissionPeer.OnPreTeamChanged != null)
					{
						MissionPeer.OnPreTeamChanged(this.GetNetworkPeer(), this._team, value);
					}
					Team team = this._team;
					this._team = value;
					string text = "Set the team to: ";
					Team team2 = this._team;
					Debug.Print(text + (((team2 != null) ? team2.Side.ToString() : null) ?? "null") + ", for peer: " + base.Name, 0, Debug.DebugColor.White, 17592186044416UL);
					this._controlledFormation = null;
					if (this._team != null)
					{
						if (GameNetwork.IsServer)
						{
							MBAPI.IMBPeer.SetTeam(base.Peer.Index, this._team.MBTeam.Index);
							GameNetwork.BeginBroadcastModuleEvent();
							GameNetwork.WriteMessage(new SetPeerTeam(this.GetNetworkPeer(), this._team));
							GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
						}
						if (MissionPeer.OnTeamChanged != null)
						{
							MissionPeer.OnTeamChanged(this.GetNetworkPeer(), team, this._team);
							return;
						}
					}
					else if (GameNetwork.IsServer)
					{
						MBAPI.IMBPeer.SetTeam(base.Peer.Index, -1);
					}
				}
			}
		}

		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x060029DE RID: 10718 RVA: 0x000A1F62 File Offset: 0x000A0162
		// (set) Token: 0x060029DF RID: 10719 RVA: 0x000A1F6C File Offset: 0x000A016C
		public BasicCultureObject Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				BasicCultureObject culture = this._culture;
				this._culture = value;
				if (GameNetwork.IsServerOrRecorder)
				{
					this.TeamInitialPerkInfoReady = base.Peer.IsMine;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new ChangeCulture(this, this._culture));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				if (this.OnCultureChanged != null)
				{
					this.OnCultureChanged(this._culture);
				}
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x060029E0 RID: 10720 RVA: 0x000A1FD6 File Offset: 0x000A01D6
		// (set) Token: 0x060029E1 RID: 10721 RVA: 0x000A1FDE File Offset: 0x000A01DE
		public Formation ControlledFormation
		{
			get
			{
				return this._controlledFormation;
			}
			set
			{
				if (this._controlledFormation != value)
				{
					this._controlledFormation = value;
				}
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x060029E2 RID: 10722 RVA: 0x000A1FF0 File Offset: 0x000A01F0
		public bool IsAgentAliveForChatting
		{
			get
			{
				MissionPeer component = base.GetComponent<MissionPeer>();
				return component != null && (this.IsControlledAgentActive || component.HasSpawnedAgentVisuals);
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x060029E3 RID: 10723 RVA: 0x000A2019 File Offset: 0x000A0219
		// (set) Token: 0x060029E4 RID: 10724 RVA: 0x000A2021 File Offset: 0x000A0221
		public bool IsMutedFromPlatform { get; private set; }

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x060029E5 RID: 10725 RVA: 0x000A202A File Offset: 0x000A022A
		// (set) Token: 0x060029E6 RID: 10726 RVA: 0x000A2032 File Offset: 0x000A0232
		public bool IsMuted { get; private set; }

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x060029E7 RID: 10727 RVA: 0x000A203B File Offset: 0x000A023B
		public bool IsMutedFromGameOrPlatform
		{
			get
			{
				return this.IsMutedFromPlatform || this.IsMuted;
			}
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000A204D File Offset: 0x000A024D
		public void SetMutedFromPlatform(bool isMuted)
		{
			this.IsMutedFromPlatform = isMuted;
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x000A2056 File Offset: 0x000A0256
		public void SetMuted(bool isMuted)
		{
			this.IsMuted = isMuted;
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x000A205F File Offset: 0x000A025F
		public void ResetRequestedKickPollCount()
		{
			this.RequestedKickPollCount = 0;
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x000A2068 File Offset: 0x000A0268
		public void IncrementRequestedKickPollCount()
		{
			int requestedKickPollCount = this.RequestedKickPollCount;
			this.RequestedKickPollCount = requestedKickPollCount + 1;
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x000A2085 File Offset: 0x000A0285
		public int GetSelectedPerkIndexWithPerkListIndex(int troopIndex, int perkListIndex)
		{
			return this._perks[troopIndex][perkListIndex];
		}

		// Token: 0x060029ED RID: 10733 RVA: 0x000A2098 File Offset: 0x000A0298
		public bool SelectPerk(int perkListIndex, int perkIndex, int enforcedSelectedTroopIndex = -1)
		{
			if (this.SelectedTroopIndex >= 0 && enforcedSelectedTroopIndex >= 0 && this.SelectedTroopIndex != enforcedSelectedTroopIndex)
			{
				Debug.Print("SelectedTroopIndex < 0 || enforcedSelectedTroopIndex < 0 || SelectedTroopIndex == enforcedSelectedTroopIndex", 0, Debug.DebugColor.White, 17179869184UL);
				Debug.Print(string.Format("SelectedTroopIndex: {0} enforcedSelectedTroopIndex: {1}", this.SelectedTroopIndex, enforcedSelectedTroopIndex), 0, Debug.DebugColor.White, 17179869184UL);
			}
			int num = ((enforcedSelectedTroopIndex >= 0) ? enforcedSelectedTroopIndex : this.SelectedTroopIndex);
			if (perkIndex != this._perks[num][perkListIndex])
			{
				this._perks[num][perkListIndex] = perkIndex;
				if (this.GetNetworkPeer().IsMine)
				{
					List<MultiplayerClassDivisions.MPHeroClass> list = MultiplayerClassDivisions.GetMPHeroClasses(this.Culture).ToList<MultiplayerClassDivisions.MPHeroClass>();
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						if (num == i)
						{
							MultiplayerClassDivisions.MPHeroClass mpheroClass = list[i];
							List<MPPerkSelectionManager.MPPerkSelection> list2 = new List<MPPerkSelectionManager.MPPerkSelection>();
							for (int j = 0; j < 3; j++)
							{
								list2.Add(new MPPerkSelectionManager.MPPerkSelection(this._perks[i][j], j));
							}
							MPPerkSelectionManager.Instance.SetSelectionsForHeroClassTemporarily(mpheroClass, list2);
							break;
						}
					}
				}
				if (num == this.SelectedTroopIndex)
				{
					this.ResetSelectedPerks();
				}
				MissionPeer.OnPerkUpdateEventDelegate onPerkSelectionUpdated = MissionPeer.OnPerkSelectionUpdated;
				if (onPerkSelectionUpdated != null)
				{
					onPerkSelectionUpdated(this);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060029EE RID: 10734 RVA: 0x000A21D4 File Offset: 0x000A03D4
		public void HandleVoteChange(CultureVoteTypes voteType, BasicCultureObject culture)
		{
			if (voteType != CultureVoteTypes.Ban)
			{
				if (voteType == CultureVoteTypes.Select)
				{
					this.VotedForSelection = culture;
				}
			}
			else
			{
				this.VotedForBan = culture;
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CultureVoteServer(this.GetNetworkPeer(), voteType, (voteType == CultureVoteTypes.Ban) ? this.VotedForBan : this.VotedForSelection));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		// Token: 0x060029EF RID: 10735 RVA: 0x000A2230 File Offset: 0x000A0430
		public override void OnFinalize()
		{
			base.OnFinalize();
			if (base.IsMine)
			{
				MPPerkSelectionManager.Instance.TryToApplyAndSavePendingChanges();
			}
			this.ResetKillRegistry();
			if (this.HasSpawnedAgentVisuals && Mission.Current != null)
			{
				MultiplayerMissionAgentVisualSpawnComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
				if (missionBehavior != null)
				{
					missionBehavior.RemoveAgentVisuals(this, false);
				}
				this.OnCultureChanged -= this.CultureChanged;
			}
		}

		// Token: 0x060029F0 RID: 10736 RVA: 0x000A2292 File Offset: 0x000A0492
		public override void OnInitialize()
		{
			base.OnInitialize();
			this.OnCultureChanged += this.CultureChanged;
		}

		// Token: 0x060029F1 RID: 10737 RVA: 0x000A22AC File Offset: 0x000A04AC
		public int GetAmountOfAgentVisualsForPeer()
		{
			return this._visuals.Count((PeerVisualsHolder v) => v != null);
		}

		// Token: 0x060029F2 RID: 10738 RVA: 0x000A22D8 File Offset: 0x000A04D8
		public PeerVisualsHolder GetVisuals(int visualIndex)
		{
			if (this._visuals.Count <= 0)
			{
				return null;
			}
			return this._visuals[visualIndex];
		}

		// Token: 0x060029F3 RID: 10739 RVA: 0x000A22F8 File Offset: 0x000A04F8
		public void ClearVisuals(int visualIndex)
		{
			if (visualIndex < this._visuals.Count && this._visuals[visualIndex] != null)
			{
				if (!GameNetwork.IsDedicatedServer)
				{
					MBAgentVisuals visuals = this._visuals[visualIndex].AgentVisuals.GetVisuals();
					visuals.ClearVisualComponents(true);
					visuals.ClearAllWeaponMeshes();
					visuals.Reset();
					if (this._visuals[visualIndex].MountAgentVisuals != null)
					{
						MBAgentVisuals visuals2 = this._visuals[visualIndex].MountAgentVisuals.GetVisuals();
						visuals2.ClearVisualComponents(true);
						visuals2.ClearAllWeaponMeshes();
						visuals2.Reset();
					}
				}
				this._visuals[visualIndex] = null;
			}
		}

		// Token: 0x060029F4 RID: 10740 RVA: 0x000A239C File Offset: 0x000A059C
		public void ClearAllVisuals(bool freeResources = false)
		{
			if (this._visuals != null)
			{
				for (int i = this._visuals.Count - 1; i >= 0; i--)
				{
					if (this._visuals[i] != null)
					{
						this.ClearVisuals(i);
					}
				}
				if (freeResources)
				{
					this._visuals = null;
				}
			}
		}

		// Token: 0x060029F5 RID: 10741 RVA: 0x000A23E8 File Offset: 0x000A05E8
		public void OnVisualsSpawned(PeerVisualsHolder visualsHolder, int visualIndex)
		{
			if (visualIndex >= this._visuals.Count)
			{
				int num = visualIndex - this._visuals.Count;
				for (int i = 0; i < num + 1; i++)
				{
					this._visuals.Add(null);
				}
			}
			this._visuals[visualIndex] = visualsHolder;
		}

		// Token: 0x060029F6 RID: 10742 RVA: 0x000A2438 File Offset: 0x000A0638
		public IEnumerable<IAgentVisual> GetAllAgentVisualsForPeer()
		{
			int count = this.GetAmountOfAgentVisualsForPeer();
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				yield return this.GetVisuals(i).AgentVisuals;
				num = i;
			}
			yield break;
		}

		// Token: 0x060029F7 RID: 10743 RVA: 0x000A2448 File Offset: 0x000A0648
		public IAgentVisual GetAgentVisualForPeer(int visualsIndex)
		{
			IAgentVisual agentVisual;
			return this.GetAgentVisualForPeer(visualsIndex, out agentVisual);
		}

		// Token: 0x060029F8 RID: 10744 RVA: 0x000A2460 File Offset: 0x000A0660
		public IAgentVisual GetAgentVisualForPeer(int visualsIndex, out IAgentVisual mountAgentVisuals)
		{
			PeerVisualsHolder visuals = this.GetVisuals(visualsIndex);
			mountAgentVisuals = ((visuals != null) ? visuals.MountAgentVisuals : null);
			if (visuals == null)
			{
				return null;
			}
			return visuals.AgentVisuals;
		}

		// Token: 0x060029F9 RID: 10745 RVA: 0x000A2490 File Offset: 0x000A0690
		public void TickInactivityStatus()
		{
			NetworkCommunicator networkPeer = this.GetNetworkPeer();
			if (!networkPeer.IsMine)
			{
				if (this.ControlledAgent != null && this.ControlledAgent.IsActive())
				{
					if (this._lastActiveTime == MissionTime.Zero)
					{
						this._lastActiveTime = MissionTime.Now;
						this._previousActivityStatus = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
						this._inactiveWarningGiven = false;
						return;
					}
					ValueTuple<Agent.MovementControlFlag, Vec2, Vec3> valueTuple = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
					if (this._previousActivityStatus.Item1 != valueTuple.Item1 || this._previousActivityStatus.Item2.DistanceSquared(valueTuple.Item2) > 1E-05f || this._previousActivityStatus.Item3.DistanceSquared(valueTuple.Item3) > 1E-05f)
					{
						this._lastActiveTime = MissionTime.Now;
						this._previousActivityStatus = valueTuple;
						this._inactiveWarningGiven = false;
					}
					if (this._lastActiveTime.ElapsedSeconds > 180f)
					{
						DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
						disconnectInfo.Type = DisconnectType.Inactivity;
						networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
						GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
						return;
					}
					if (this._lastActiveTime.ElapsedSeconds > 120f && !this._inactiveWarningGiven)
					{
						MultiplayerGameNotificationsComponent missionBehavior = Mission.Current.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
						if (missionBehavior != null)
						{
							missionBehavior.PlayerIsInactive(this.GetNetworkPeer());
						}
						this._inactiveWarningGiven = true;
						return;
					}
				}
				else
				{
					this._lastActiveTime = MissionTime.Now;
					this._inactiveWarningGiven = false;
				}
			}
		}

		// Token: 0x060029FA RID: 10746 RVA: 0x000A2644 File Offset: 0x000A0844
		public void OnKillAnotherPeer(MissionPeer victimPeer)
		{
			if (victimPeer != null)
			{
				if (!this._numberOfTimesPeerKilledPerPeer.ContainsKey(victimPeer))
				{
					this._numberOfTimesPeerKilledPerPeer.Add(victimPeer, 1);
				}
				else
				{
					Dictionary<MissionPeer, int> numberOfTimesPeerKilledPerPeer = this._numberOfTimesPeerKilledPerPeer;
					int num = numberOfTimesPeerKilledPerPeer[victimPeer];
					numberOfTimesPeerKilledPerPeer[victimPeer] = num + 1;
				}
				MissionPeer.OnPlayerKilledDelegate onPlayerKilled = MissionPeer.OnPlayerKilled;
				if (onPlayerKilled == null)
				{
					return;
				}
				onPlayerKilled(this, victimPeer);
			}
		}

		// Token: 0x060029FB RID: 10747 RVA: 0x000A269C File Offset: 0x000A089C
		public void OverrideCultureWithTeamCulture()
		{
			MultiplayerOptions.OptionType optionType = ((this.Team.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1 : MultiplayerOptions.OptionType.CultureTeam2);
			this.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(optionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x000A26D5 File Offset: 0x000A08D5
		public int GetNumberOfTimesPeerKilledPeer(MissionPeer killedPeer)
		{
			if (this._numberOfTimesPeerKilledPerPeer.ContainsKey(killedPeer))
			{
				return this._numberOfTimesPeerKilledPerPeer[killedPeer];
			}
			return 0;
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x000A26F3 File Offset: 0x000A08F3
		public void ResetKillRegistry()
		{
			this._numberOfTimesPeerKilledPerPeer.Clear();
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x000A2700 File Offset: 0x000A0900
		private void ResetSelectedPerks()
		{
			if (this._selectedPerks.Item2 != null)
			{
				foreach (MPPerkObject mpperkObject in this._selectedPerks.Item2)
				{
					mpperkObject.Reset();
				}
			}
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x000A2764 File Offset: 0x000A0964
		private void CultureChanged(BasicCultureObject newCulture)
		{
			List<MultiplayerClassDivisions.MPHeroClass> list = MultiplayerClassDivisions.GetMPHeroClasses(newCulture).ToList<MultiplayerClassDivisions.MPHeroClass>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				MultiplayerClassDivisions.MPHeroClass mpheroClass = list[i];
				List<MPPerkSelectionManager.MPPerkSelection> selectionsForHeroClass = MPPerkSelectionManager.Instance.GetSelectionsForHeroClass(mpheroClass);
				if (selectionsForHeroClass != null)
				{
					int count2 = selectionsForHeroClass.Count;
					for (int j = 0; j < count2; j++)
					{
						MPPerkSelectionManager.MPPerkSelection mpperkSelection = selectionsForHeroClass[j];
						this._perks[i][mpperkSelection.ListIndex] = mpperkSelection.Index;
					}
				}
				else
				{
					for (int k = 0; k < 3; k++)
					{
						this._perks[i][k] = 0;
					}
				}
			}
			if (base.IsMine && GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TeamInitialPerkInfoMessage(this._perks[this.SelectedTroopIndex]));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x000A2844 File Offset: 0x000A0A44
		public void OnTeamInitialPerkInfoReceived(int[] perks)
		{
			for (int i = 0; i < 3; i++)
			{
				this.SelectPerk(i, perks[i], -1);
			}
			this.TeamInitialPerkInfoReady = true;
		}

		// Token: 0x04000FDB RID: 4059
		public const int NumberOfPerkLists = 3;

		// Token: 0x04000FDC RID: 4060
		public const int MaxNumberOfTroopTypesPerCulture = 16;

		// Token: 0x04000FDD RID: 4061
		private const float InactivityKickInSeconds = 180f;

		// Token: 0x04000FDE RID: 4062
		private const float InactivityWarnInSeconds = 120f;

		// Token: 0x04000FDF RID: 4063
		public const int MinKDACount = -1000;

		// Token: 0x04000FE0 RID: 4064
		public const int MaxKDACount = 100000;

		// Token: 0x04000FE1 RID: 4065
		public const int MinScore = -1000000;

		// Token: 0x04000FE2 RID: 4066
		public const int MaxScore = 1000000;

		// Token: 0x04000FE3 RID: 4067
		public const int MinSpawnTimer = 3;

		// Token: 0x04000FE4 RID: 4068
		public int CaptainBeingDetachedThreshold = 125;

		// Token: 0x04000FEB RID: 4075
		private List<PeerVisualsHolder> _visuals = new List<PeerVisualsHolder>();

		// Token: 0x04000FEC RID: 4076
		private Dictionary<MissionPeer, int> _numberOfTimesPeerKilledPerPeer = new Dictionary<MissionPeer, int>();

		// Token: 0x04000FED RID: 4077
		private MissionTime _lastActiveTime = MissionTime.Zero;

		// Token: 0x04000FEE RID: 4078
		private ValueTuple<Agent.MovementControlFlag, Vec2, Vec3> _previousActivityStatus;

		// Token: 0x04000FEF RID: 4079
		private bool _inactiveWarningGiven;

		// Token: 0x04000FF4 RID: 4084
		private int _selectedTroopIndex;

		// Token: 0x04000FF6 RID: 4086
		private Agent _followedAgent;

		// Token: 0x04000FF7 RID: 4087
		private Team _team;

		// Token: 0x04000FF8 RID: 4088
		private BasicCultureObject _culture;

		// Token: 0x04000FF9 RID: 4089
		private Formation _controlledFormation;

		// Token: 0x04000FFA RID: 4090
		private MissionRepresentativeBase _representative;

		// Token: 0x04000FFB RID: 4091
		private readonly MBList<int[]> _perks;

		// Token: 0x04000FFC RID: 4092
		private int _killCount;

		// Token: 0x04000FFD RID: 4093
		private int _assistCount;

		// Token: 0x04000FFE RID: 4094
		private int _deathCount;

		// Token: 0x04000FFF RID: 4095
		private int _score;

		// Token: 0x04001000 RID: 4096
		private ValueTuple<int, MBList<MPPerkObject>> _selectedPerks;

		// Token: 0x04001008 RID: 4104
		private int _botsUnderControlAlive;

		// Token: 0x02000614 RID: 1556
		// (Invoke) Token: 0x06003D42 RID: 15682
		public delegate void OnUpdateEquipmentSetIndexEventDelegate(MissionPeer lobbyPeer, int equipmentSetIndex);

		// Token: 0x02000615 RID: 1557
		// (Invoke) Token: 0x06003D46 RID: 15686
		public delegate void OnPerkUpdateEventDelegate(MissionPeer peer);

		// Token: 0x02000616 RID: 1558
		// (Invoke) Token: 0x06003D4A RID: 15690
		public delegate void OnTeamChangedDelegate(NetworkCommunicator peer, Team previousTeam, Team newTeam);

		// Token: 0x02000617 RID: 1559
		// (Invoke) Token: 0x06003D4E RID: 15694
		public delegate void OnCultureChangedDelegate(BasicCultureObject newCulture);

		// Token: 0x02000618 RID: 1560
		// (Invoke) Token: 0x06003D52 RID: 15698
		public delegate void OnPlayerKilledDelegate(MissionPeer killerPeer, MissionPeer killedPeer);
	}
}
