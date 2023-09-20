using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General
{
	public class SPGeneralKillNotificationItemVM : ViewModel
	{
		private Color friendlyColor
		{
			get
			{
				return new Color(0.54296875f, 0.77734375f, 0.421875f, 1f);
			}
		}

		private Color enemyColor
		{
			get
			{
				return new Color(0.953125f, 0.48828125f, 0.42578125f, 1f);
			}
		}

		public SPGeneralKillNotificationItemVM(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot, Action<SPGeneralKillNotificationItemVM> onRemove)
		{
			this._affectedAgent = affectedAgent;
			this._affectorAgent = affectorAgent;
			this._assistedAgent = assistedAgent;
			this._onRemove = onRemove;
			this._showNames = BannerlordConfig.ReportCasualtiesType == 0;
			this.InitProperties(this._affectedAgent, this._affectorAgent, this._assistedAgent, isHeadshot);
		}

		private void InitProperties(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(64, "InitProperties");
			if (!this._showNames)
			{
				if (affectorAgent == null)
				{
					goto IL_66;
				}
				BasicCharacterObject character = affectorAgent.Character;
				bool? flag = ((character != null) ? new bool?(character.IsHero) : null);
				bool flag2 = true;
				if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
				{
					goto IL_66;
				}
			}
			mbstringBuilder.Append<string>(affectorAgent.Name);
			IL_66:
			mbstringBuilder.Append('\0');
			mbstringBuilder.Append<string>(SPGeneralKillNotificationItemVM.GetAgentType(affectorAgent));
			mbstringBuilder.Append('\0');
			if (!this._showNames)
			{
				BasicCharacterObject character2 = affectedAgent.Character;
				if (character2 == null || !character2.IsHero)
				{
					goto IL_B0;
				}
			}
			mbstringBuilder.Append<string>(affectedAgent.Name);
			IL_B0:
			mbstringBuilder.Append('\0');
			mbstringBuilder.Append<string>(SPGeneralKillNotificationItemVM.GetAgentType(affectedAgent));
			mbstringBuilder.Append('\0');
			mbstringBuilder.Append((affectedAgent.State == AgentState.Unconscious) ? 1 : 0);
			mbstringBuilder.Append('\0');
			mbstringBuilder.Append(isHeadshot ? 1 : 0);
			mbstringBuilder.Append('\0');
			Team team = affectedAgent.Team;
			Color color;
			if (team != null && team.IsValid)
			{
				if (affectedAgent.Team.IsPlayerAlly)
				{
					color = this.enemyColor;
				}
				else
				{
					color = this.friendlyColor;
				}
			}
			else
			{
				color = Color.FromUint(4284111450U);
			}
			mbstringBuilder.Append(color.ToUnsignedInteger());
			this.Message = mbstringBuilder.ToStringAndRelease();
		}

		private static string GetAgentType(Agent agent)
		{
			if (((agent != null) ? agent.Character : null) == null)
			{
				return "None";
			}
			switch (agent.Character.DefaultFormationGroup)
			{
			case 0:
				return "Infantry_Light";
			case 1:
				return "Archer_Light";
			case 2:
				return "Cavalry_Light";
			case 3:
				return "HorseArcher_Light";
			case 4:
			case 5:
				return "Infantry_Heavy";
			case 6:
				return "Cavalry_Light";
			case 7:
				return "Cavalry_Heavy";
			case 8:
			case 9:
			case 10:
				return "Infantry_Heavy";
			default:
				return "None";
			}
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
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

		private const char _seperator = '\0';

		private readonly Agent _affectedAgent;

		private readonly Agent _affectorAgent;

		private readonly Agent _assistedAgent;

		private readonly Action<SPGeneralKillNotificationItemVM> _onRemove;

		private bool _showNames;

		private string _message;
	}
}
