using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.KillFeed.General
{
	public class MPGeneralKillNotificationItemVM : ViewModel
	{
		public MPGeneralKillNotificationItemVM(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, Action<MPGeneralKillNotificationItemVM> onRemove)
		{
			this._onRemove = onRemove;
			this.IsDamageNotification = false;
			this.InitProperties(affectedAgent, affectorAgent);
			this.InitDeathProperties(affectedAgent, affectorAgent, assistedAgent);
		}

		public virtual void InitProperties(Agent affectedAgent, Agent affectorAgent)
		{
			this.IsItemInitializationOver = false;
			uint num;
			uint num2;
			this.GetAgentColors(affectorAgent, out num, out num2);
			TargetIconType multiplayerAgentType = this.GetMultiplayerAgentType(affectorAgent);
			BannerCode agentBannerCode = this.GetAgentBannerCode(affectorAgent);
			bool? flag;
			if (affectorAgent == null)
			{
				flag = null;
			}
			else
			{
				Team team = affectorAgent.Team;
				flag = ((team != null) ? new bool?(team.IsPlayerAlly) : null);
			}
			bool flag2 = flag ?? false;
			uint num3;
			uint num4;
			this.GetAgentColors(affectedAgent, out num3, out num4);
			TargetIconType multiplayerAgentType2 = this.GetMultiplayerAgentType(affectedAgent);
			BannerCode agentBannerCode2 = this.GetAgentBannerCode(affectedAgent);
			Team team2 = affectedAgent.Team;
			bool flag3 = team2 != null && team2.IsPlayerAlly;
			this.MurdererName = ((affectorAgent != null) ? ((affectorAgent.MissionPeer != null) ? affectorAgent.MissionPeer.DisplayedName : affectorAgent.Name) : "");
			this.MurdererType = multiplayerAgentType.ToString();
			this.IsMurdererBot = affectorAgent != null && !affectorAgent.IsPlayerControlled;
			this.MurdererCompassElement = new MPTeammateCompassTargetVM(multiplayerAgentType, num, num2, agentBannerCode, flag2);
			this.VictimName = ((affectedAgent.MissionPeer != null) ? affectedAgent.MissionPeer.DisplayedName : affectedAgent.Name);
			this.VictimType = multiplayerAgentType2.ToString();
			this.IsVictimBot = !affectedAgent.IsPlayerControlled;
			this.VictimCompassElement = new MPTeammateCompassTargetVM(multiplayerAgentType2, num3, num4, agentBannerCode2, flag3);
			this.IsPlayerDeath = affectedAgent.IsMainAgent;
			if (flag2 && flag3)
			{
				this.Color1 = Color.FromUint(4278190080U);
				this.Color2 = Color.FromUint(uint.MaxValue);
			}
			else if (!flag2 && !flag3)
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
					goto IL_220;
				}
			}
			if (this.IsMurdererBot && affectorAgent != null)
			{
				Formation formation2 = affectorAgent.Formation;
				Agent main2 = Agent.Main;
				if (formation2 == ((main2 != null) ? main2.Formation : null))
				{
					this.IsRelatedToFriendlyTroop = true;
				}
			}
			IL_220:
			this.IsItemInitializationOver = true;
		}

		public void InitDeathProperties(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent)
		{
			this.IsItemInitializationOver = false;
			if (affectorAgent != null && affectorAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", "{=!}" + affectedAgent.Name.ToString(), false);
				this.Message = GameTexts.FindText("str_kill_feed_message", null).ToString();
			}
			else if (affectedAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", ("{=!}" + affectorAgent != null) ? affectorAgent.Name.ToString() : "", false);
				this.Message = GameTexts.FindText("str_death_feed_message", null).ToString();
			}
			else if (assistedAgent != null && assistedAgent.IsMainAgent)
			{
				MBTextManager.SetTextVariable("TROOP_NAME", "{=!}" + affectedAgent.Name.ToString(), false);
				this.Message = GameTexts.FindText("str_assist_feed_message", null).ToString();
			}
			this.IsItemInitializationOver = true;
		}

		protected TargetIconType GetMultiplayerAgentType(Agent agent)
		{
			if (agent == null)
			{
				return -1;
			}
			if (!agent.IsHuman)
			{
				return 0;
			}
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
			if (mpheroClassForCharacter == null)
			{
				Debug.FailedAssert("Hero class is not set for agent: " + agent.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\KillFeed\\General\\MPGeneralKillNotificationItemVM.cs", "GetMultiplayerAgentType", 116);
				return -1;
			}
			return mpheroClassForCharacter.IconType;
		}

		private BannerCode GetAgentBannerCode(Agent agent)
		{
			BannerCode bannerCode = this.DefaultBannerCode;
			if (agent != null)
			{
				MissionPeer missionPeer = agent.MissionPeer;
				MissionPeer missionPeer2 = ((missionPeer != null) ? missionPeer.GetComponent<MissionPeer>() : null);
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
			}
			return bannerCode;
		}

		private void GetAgentColors(Agent agent, out uint color1, out uint color2)
		{
			if (((agent != null) ? agent.Team : null) != null)
			{
				color1 = agent.Team.Color;
				color2 = agent.Team.Color2;
				return;
			}
			color1 = 4284111450U;
			color2 = uint.MaxValue;
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

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

		private readonly Action<MPGeneralKillNotificationItemVM> _onRemove;

		private readonly BannerCode DefaultBannerCode = BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(92));

		private string _murdererName;

		private string _murdererType;

		private string _victimName;

		private string _victimType;

		private MPTeammateCompassTargetVM _murdererCompassElement;

		private MPTeammateCompassTargetVM _victimCompassElement;

		private Color _color1;

		private Color _color2;

		private bool _isPlayerDeath;

		private bool _isItemInitializationOver;

		private bool _isVictimBot;

		private bool _isMurdererBot;

		private bool _isDamageNotification;

		private bool _isDamagedMount;

		private bool _isRelatedToFriendlyTroop;

		private bool _isFriendlyTroopDeath;

		private string _message;
	}
}
