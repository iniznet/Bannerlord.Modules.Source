using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General
{
	// Token: 0x020000E8 RID: 232
	public class SPGeneralKillNotificationItemVM : ViewModel
	{
		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x060014E9 RID: 5353 RVA: 0x00044115 File Offset: 0x00042315
		private Color friendlyColor
		{
			get
			{
				return new Color(0.54296875f, 0.77734375f, 0.421875f, 1f);
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x00044130 File Offset: 0x00042330
		private Color enemyColor
		{
			get
			{
				return new Color(0.953125f, 0.48828125f, 0.42578125f, 1f);
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0004414C File Offset: 0x0004234C
		public SPGeneralKillNotificationItemVM(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot, Action<SPGeneralKillNotificationItemVM> onRemove)
		{
			this._affectedAgent = affectedAgent;
			this._affectorAgent = affectorAgent;
			this._assistedAgent = assistedAgent;
			this._onRemove = onRemove;
			this._showNames = BannerlordConfig.ReportCasualtiesType == 0;
			this.InitProperties(this._affectedAgent, this._affectorAgent, this._assistedAgent, isHeadshot);
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x000441A4 File Offset: 0x000423A4
		private void InitProperties(Agent affectedAgent, Agent affectorAgent, Agent assistedAgent, bool isHeadshot)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(64, "InitProperties");
			if (!this._showNames)
			{
				BasicCharacterObject character = affectorAgent.Character;
				if (character == null || !character.IsHero)
				{
					goto IL_40;
				}
			}
			mbstringBuilder.Append<string>(affectorAgent.Name);
			IL_40:
			mbstringBuilder.Append('\0');
			mbstringBuilder.Append<string>(SPGeneralKillNotificationItemVM.GetAgentType(affectorAgent));
			mbstringBuilder.Append('\0');
			if (!this._showNames)
			{
				BasicCharacterObject character2 = affectedAgent.Character;
				if (character2 == null || !character2.IsHero)
				{
					goto IL_8A;
				}
			}
			mbstringBuilder.Append<string>(affectedAgent.Name);
			IL_8A:
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

		// Token: 0x060014ED RID: 5357 RVA: 0x000442EC File Offset: 0x000424EC
		private static string GetAgentType(Agent agent)
		{
			if (agent.Character == null)
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

		// Token: 0x060014EE RID: 5358 RVA: 0x0004437C File Offset: 0x0004257C
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x060014EF RID: 5359 RVA: 0x0004438A File Offset: 0x0004258A
		// (set) Token: 0x060014F0 RID: 5360 RVA: 0x00044392 File Offset: 0x00042592
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

		// Token: 0x040009FB RID: 2555
		private const char _seperator = '\0';

		// Token: 0x040009FC RID: 2556
		private readonly Agent _affectedAgent;

		// Token: 0x040009FD RID: 2557
		private readonly Agent _affectorAgent;

		// Token: 0x040009FE RID: 2558
		private readonly Agent _assistedAgent;

		// Token: 0x040009FF RID: 2559
		private readonly Action<SPGeneralKillNotificationItemVM> _onRemove;

		// Token: 0x04000A00 RID: 2560
		private bool _showNames;

		// Token: 0x04000A01 RID: 2561
		private string _message;
	}
}
