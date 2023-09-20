using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.General
{
	// Token: 0x020000B2 RID: 178
	public class MPGeneralKillNotificationItemVM : ViewModel
	{
		// Token: 0x060010C9 RID: 4297 RVA: 0x000377FA File Offset: 0x000359FA
		public MPGeneralKillNotificationItemVM(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, Action<MPGeneralKillNotificationItemVM> onRemove)
		{
			this._onRemove = onRemove;
			this.IsDamageNotification = false;
			this.InitProperties(affectedAgent, affectorAgent);
			this.InitDeathProperties(affectedAgent, affectorAgent, assistedAgent);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x00037834 File Offset: 0x00035A34
		public virtual void InitProperties(Agent affectedAgent, Agent affectorAgent)
		{
			this.IsItemInitializationOver = false;
			uint num;
			uint num2;
			this.GetAgentColors(affectorAgent, out num, out num2);
			TargetIconType multiplayerAgentType = this.GetMultiplayerAgentType(affectorAgent);
			BannerCode agentBannerCode = this.GetAgentBannerCode(affectorAgent);
			Team team = affectorAgent.Team;
			bool flag = team != null && team.IsPlayerAlly;
			uint num3;
			uint num4;
			this.GetAgentColors(affectedAgent, out num3, out num4);
			TargetIconType multiplayerAgentType2 = this.GetMultiplayerAgentType(affectedAgent);
			BannerCode agentBannerCode2 = this.GetAgentBannerCode(affectedAgent);
			Team team2 = affectedAgent.Team;
			bool flag2 = team2 != null && team2.IsPlayerAlly;
			this.MurdererName = ((affectorAgent.MissionPeer != null) ? affectorAgent.MissionPeer.DisplayedName : affectorAgent.Name);
			this.MurdererType = multiplayerAgentType.ToString();
			this.IsMurdererBot = !affectorAgent.IsPlayerControlled;
			this.MurdererCompassElement = new MPTeammateCompassTargetVM(multiplayerAgentType, num, num2, agentBannerCode, flag);
			this.VictimName = ((affectedAgent.MissionPeer != null) ? affectedAgent.MissionPeer.DisplayedName : affectedAgent.Name);
			this.VictimType = multiplayerAgentType2.ToString();
			this.IsVictimBot = !affectedAgent.IsPlayerControlled;
			this.VictimCompassElement = new MPTeammateCompassTargetVM(multiplayerAgentType2, num3, num4, agentBannerCode2, flag2);
			this.IsPlayerDeath = affectedAgent.IsMainAgent;
			if (flag && flag2)
			{
				this.Color1 = Color.FromUint(4278190080U);
				this.Color2 = Color.FromUint(uint.MaxValue);
			}
			else if (!flag && !flag2)
			{
				this.Color1 = Color.FromUint(4281545266U);
				this.Color2 = Color.FromUint(uint.MaxValue);
			}
			else
			{
				this.Color1 = Color.FromUint(num);
				this.Color2 = Color.FromUint(num2);
			}
			if (this.IsVictimBot)
			{
				Formation formation = affectedAgent.Formation;
				Agent main = Agent.Main;
				if (formation == ((main != null) ? main.Formation : null))
				{
					this.IsRelatedToFriendlyTroop = true;
					this.IsFriendlyTroopDeath = true;
					goto IL_1DB;
				}
			}
			if (this.IsMurdererBot)
			{
				Formation formation2 = affectorAgent.Formation;
				Agent main2 = Agent.Main;
				if (formation2 == ((main2 != null) ? main2.Formation : null))
				{
					this.IsRelatedToFriendlyTroop = true;
				}
			}
			IL_1DB:
			this.IsItemInitializationOver = true;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00037A24 File Offset: 0x00035C24
		public void InitDeathProperties(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent)
		{
			this.IsItemInitializationOver = false;
			if (affectorAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", "{=!}" + affectedAgent.Name.ToString(), false);
				this.Message = GameTexts.FindText("str_kill_feed_message", null).ToString();
			}
			else if (affectedAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", "{=!}" + affectorAgent.Name.ToString(), false);
				this.Message = GameTexts.FindText("str_death_feed_message", null).ToString();
			}
			else if (assistedAgent != null && assistedAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", "{=!}" + affectedAgent.Name.ToString(), false);
				this.Message = GameTexts.FindText("str_assist_feed_message", null).ToString();
			}
			this.IsItemInitializationOver = true;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00037B04 File Offset: 0x00035D04
		protected TargetIconType GetMultiplayerAgentType(Agent agent)
		{
			if (!agent.IsHuman)
			{
				return TargetIconType.Monster;
			}
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
			if (mpheroClassForCharacter == null)
			{
				Debug.FailedAssert("Hero class is not set for agent: " + agent.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\KillFeed\\General\\MPGeneralKillNotificationItemVM.cs", "GetMultiplayerAgentType", 111);
				return TargetIconType.None;
			}
			return mpheroClassForCharacter.IconType;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00037B54 File Offset: 0x00035D54
		private BannerCode GetAgentBannerCode(Agent agent)
		{
			MissionPeer missionPeer = agent.MissionPeer;
			MissionPeer missionPeer2 = ((missionPeer != null) ? missionPeer.GetComponent<MissionPeer>() : null);
			BannerCode bannerCode;
			if (agent.Team != null && missionPeer2 != null)
			{
				bannerCode = BannerCode.CreateFrom(new Banner(missionPeer2.Peer.BannerCode, agent.Team.Color, agent.Team.Color2));
			}
			else if (agent.Team != null && agent.Formation != null && !string.IsNullOrEmpty(agent.Formation.BannerCode))
			{
				bannerCode = BannerCode.CreateFrom(new Banner(agent.Formation.BannerCode, agent.Team.Color, agent.Team.Color2));
			}
			else if (agent.Team != null)
			{
				bannerCode = BannerCode.CreateFrom(agent.Team.Banner);
			}
			else
			{
				bannerCode = this.DefaultBannerCode;
			}
			return bannerCode;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00037C22 File Offset: 0x00035E22
		private void GetAgentColors(Agent agent, out uint color1, out uint color2)
		{
			if (agent != null && agent.Team != null)
			{
				color1 = agent.Team.Color;
				color2 = agent.Team.Color2;
				return;
			}
			color1 = 4284111450U;
			color2 = uint.MaxValue;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00037C54 File Offset: 0x00035E54
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x060010D0 RID: 4304 RVA: 0x00037C62 File Offset: 0x00035E62
		// (set) Token: 0x060010D1 RID: 4305 RVA: 0x00037C6A File Offset: 0x00035E6A
		[DataSourceProperty]
		public string MurdererName
		{
			get
			{
				return this._murdererName;
			}
			set
			{
				if (value != this._murdererName)
				{
					this._murdererName = value;
					base.OnPropertyChangedWithValue<string>(value, "MurdererName");
				}
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x060010D2 RID: 4306 RVA: 0x00037C8D File Offset: 0x00035E8D
		// (set) Token: 0x060010D3 RID: 4307 RVA: 0x00037C95 File Offset: 0x00035E95
		[DataSourceProperty]
		public string MurdererType
		{
			get
			{
				return this._murdererType;
			}
			set
			{
				if (value != this._murdererType)
				{
					this._murdererType = value;
					base.OnPropertyChangedWithValue<string>(value, "MurdererType");
				}
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00037CB8 File Offset: 0x00035EB8
		// (set) Token: 0x060010D5 RID: 4309 RVA: 0x00037CC0 File Offset: 0x00035EC0
		[DataSourceProperty]
		public string VictimName
		{
			get
			{
				return this._victimName;
			}
			set
			{
				if (value != this._victimName)
				{
					this._victimName = value;
					base.OnPropertyChangedWithValue<string>(value, "VictimName");
				}
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00037CE3 File Offset: 0x00035EE3
		// (set) Token: 0x060010D7 RID: 4311 RVA: 0x00037CEB File Offset: 0x00035EEB
		[DataSourceProperty]
		public string VictimType
		{
			get
			{
				return this._victimType;
			}
			set
			{
				if (value != this._victimType)
				{
					this._victimType = value;
					base.OnPropertyChangedWithValue<string>(value, "VictimType");
				}
			}
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x00037D0E File Offset: 0x00035F0E
		// (set) Token: 0x060010D9 RID: 4313 RVA: 0x00037D16 File Offset: 0x00035F16
		[DataSourceProperty]
		public bool IsDamageNotification
		{
			get
			{
				return this._isDamageNotification;
			}
			set
			{
				if (value != this._isDamageNotification)
				{
					this._isDamageNotification = value;
					base.OnPropertyChangedWithValue(value, "IsDamageNotification");
				}
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x060010DA RID: 4314 RVA: 0x00037D34 File Offset: 0x00035F34
		// (set) Token: 0x060010DB RID: 4315 RVA: 0x00037D3C File Offset: 0x00035F3C
		[DataSourceProperty]
		public bool IsDamagedMount
		{
			get
			{
				return this._isDamagedMount;
			}
			set
			{
				if (value != this._isDamagedMount)
				{
					this._isDamagedMount = value;
					base.OnPropertyChangedWithValue(value, "IsDamagedMount");
				}
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x060010DC RID: 4316 RVA: 0x00037D5A File Offset: 0x00035F5A
		// (set) Token: 0x060010DD RID: 4317 RVA: 0x00037D62 File Offset: 0x00035F62
		[DataSourceProperty]
		public Color Color1
		{
			get
			{
				return this._color1;
			}
			set
			{
				if (value != this._color1)
				{
					this._color1 = value;
					base.OnPropertyChangedWithValue(value, "Color1");
				}
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x060010DE RID: 4318 RVA: 0x00037D85 File Offset: 0x00035F85
		// (set) Token: 0x060010DF RID: 4319 RVA: 0x00037D8D File Offset: 0x00035F8D
		[DataSourceProperty]
		public Color Color2
		{
			get
			{
				return this._color2;
			}
			set
			{
				if (value != this._color2)
				{
					this._color2 = value;
					base.OnPropertyChangedWithValue(value, "Color2");
				}
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x060010E0 RID: 4320 RVA: 0x00037DB0 File Offset: 0x00035FB0
		// (set) Token: 0x060010E1 RID: 4321 RVA: 0x00037DB8 File Offset: 0x00035FB8
		[DataSourceProperty]
		public MPTeammateCompassTargetVM MurdererCompassElement
		{
			get
			{
				return this._murdererCompassElement;
			}
			set
			{
				if (value != this._murdererCompassElement)
				{
					this._murdererCompassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "MurdererCompassElement");
				}
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x00037DD6 File Offset: 0x00035FD6
		// (set) Token: 0x060010E3 RID: 4323 RVA: 0x00037DDE File Offset: 0x00035FDE
		[DataSourceProperty]
		public MPTeammateCompassTargetVM VictimCompassElement
		{
			get
			{
				return this._victimCompassElement;
			}
			set
			{
				if (value != this._victimCompassElement)
				{
					this._victimCompassElement = value;
					base.OnPropertyChangedWithValue<MPTeammateCompassTargetVM>(value, "VictimCompassElement");
				}
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x060010E4 RID: 4324 RVA: 0x00037DFC File Offset: 0x00035FFC
		// (set) Token: 0x060010E5 RID: 4325 RVA: 0x00037E04 File Offset: 0x00036004
		[DataSourceProperty]
		public bool IsPlayerDeath
		{
			get
			{
				return this._isPlayerDeath;
			}
			set
			{
				if (value != this._isPlayerDeath)
				{
					this._isPlayerDeath = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerDeath");
				}
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x060010E6 RID: 4326 RVA: 0x00037E22 File Offset: 0x00036022
		// (set) Token: 0x060010E7 RID: 4327 RVA: 0x00037E2A File Offset: 0x0003602A
		[DataSourceProperty]
		public bool IsItemInitializationOver
		{
			get
			{
				return this._isItemInitializationOver;
			}
			set
			{
				if (value != this._isItemInitializationOver)
				{
					this._isItemInitializationOver = value;
					base.OnPropertyChangedWithValue(value, "IsItemInitializationOver");
				}
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x060010E8 RID: 4328 RVA: 0x00037E48 File Offset: 0x00036048
		// (set) Token: 0x060010E9 RID: 4329 RVA: 0x00037E50 File Offset: 0x00036050
		[DataSourceProperty]
		public bool IsVictimBot
		{
			get
			{
				return this._isVictimBot;
			}
			set
			{
				if (value != this._isVictimBot)
				{
					this._isVictimBot = value;
					base.OnPropertyChangedWithValue(value, "IsVictimBot");
				}
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x060010EA RID: 4330 RVA: 0x00037E6E File Offset: 0x0003606E
		// (set) Token: 0x060010EB RID: 4331 RVA: 0x00037E76 File Offset: 0x00036076
		[DataSourceProperty]
		public bool IsMurdererBot
		{
			get
			{
				return this._isMurdererBot;
			}
			set
			{
				if (value != this._isMurdererBot)
				{
					this._isMurdererBot = value;
					base.OnPropertyChangedWithValue(value, "IsMurdererBot");
				}
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x060010EC RID: 4332 RVA: 0x00037E94 File Offset: 0x00036094
		// (set) Token: 0x060010ED RID: 4333 RVA: 0x00037E9C File Offset: 0x0003609C
		[DataSourceProperty]
		public bool IsRelatedToFriendlyTroop
		{
			get
			{
				return this._isRelatedToFriendlyTroop;
			}
			set
			{
				if (value != this._isRelatedToFriendlyTroop)
				{
					this._isRelatedToFriendlyTroop = value;
					base.OnPropertyChangedWithValue(value, "IsRelatedToFriendlyTroop");
				}
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x060010EE RID: 4334 RVA: 0x00037EBA File Offset: 0x000360BA
		// (set) Token: 0x060010EF RID: 4335 RVA: 0x00037EC2 File Offset: 0x000360C2
		[DataSourceProperty]
		public bool IsFriendlyTroopDeath
		{
			get
			{
				return this._isFriendlyTroopDeath;
			}
			set
			{
				if (value != this._isFriendlyTroopDeath)
				{
					this._isFriendlyTroopDeath = value;
					base.OnPropertyChangedWithValue(value, "IsFriendlyTroopDeath");
				}
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x060010F0 RID: 4336 RVA: 0x00037EE0 File Offset: 0x000360E0
		// (set) Token: 0x060010F1 RID: 4337 RVA: 0x00037EE8 File Offset: 0x000360E8
		[DataSourceProperty]
		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (value != this._message)
				{
					this._message = value;
					base.OnPropertyChangedWithValue<string>(value, "Message");
				}
			}
		}

		// Token: 0x040007F8 RID: 2040
		private readonly Action<MPGeneralKillNotificationItemVM> _onRemove;

		// Token: 0x040007F9 RID: 2041
		private readonly BannerCode DefaultBannerCode = BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(92));

		// Token: 0x040007FA RID: 2042
		private string _murdererName;

		// Token: 0x040007FB RID: 2043
		private string _murdererType;

		// Token: 0x040007FC RID: 2044
		private string _victimName;

		// Token: 0x040007FD RID: 2045
		private string _victimType;

		// Token: 0x040007FE RID: 2046
		private MPTeammateCompassTargetVM _murdererCompassElement;

		// Token: 0x040007FF RID: 2047
		private MPTeammateCompassTargetVM _victimCompassElement;

		// Token: 0x04000800 RID: 2048
		private Color _color1;

		// Token: 0x04000801 RID: 2049
		private Color _color2;

		// Token: 0x04000802 RID: 2050
		private bool _isPlayerDeath;

		// Token: 0x04000803 RID: 2051
		private bool _isItemInitializationOver;

		// Token: 0x04000804 RID: 2052
		private bool _isVictimBot;

		// Token: 0x04000805 RID: 2053
		private bool _isMurdererBot;

		// Token: 0x04000806 RID: 2054
		private bool _isDamageNotification;

		// Token: 0x04000807 RID: 2055
		private bool _isDamagedMount;

		// Token: 0x04000808 RID: 2056
		private bool _isRelatedToFriendlyTroop;

		// Token: 0x04000809 RID: 2057
		private bool _isFriendlyTroopDeath;

		// Token: 0x0400080A RID: 2058
		private string _message;
	}
}
