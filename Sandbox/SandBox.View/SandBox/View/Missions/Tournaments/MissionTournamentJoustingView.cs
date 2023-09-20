using System;
using SandBox.Tournaments.AgentControllers;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace SandBox.View.Missions.Tournaments
{
	// Token: 0x02000022 RID: 34
	public class MissionTournamentJoustingView : MissionView
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x0000BEE0 File Offset: 0x0000A0E0
		public override void AfterStart()
		{
			base.AfterStart();
			this._gameSystem = Game.Current;
			this._messageUIHandler = base.Mission.GetMissionBehavior<MissionMessageUIHandler>();
			this._scoreUIHandler = base.Mission.GetMissionBehavior<MissionScoreUIHandler>();
			this._tournamentJoustingMissionController = base.Mission.GetMissionBehavior<TournamentJoustingMissionController>();
			this._tournamentJoustingMissionController.VictoryAchieved += this.OnVictoryAchieved;
			this._tournamentJoustingMissionController.PointGanied += this.OnPointGanied;
			this._tournamentJoustingMissionController.Disqualified += this.OnDisqualified;
			this._tournamentJoustingMissionController.Unconscious += this.OnUnconscious;
			this._tournamentJoustingMissionController.AgentStateChanged += this.OnAgentStateChanged;
			int num = 0;
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsHuman)
				{
					this._scoreUIHandler.SetName(agent.Name.ToString(), num);
					num++;
				}
			}
			this.SetJoustingBanners();
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000C014 File Offset: 0x0000A214
		private void RefreshScoreBoard()
		{
			int num = 0;
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.IsHuman)
				{
					JoustingAgentController controller = agent.GetController<JoustingAgentController>();
					this._scoreUIHandler.SaveScore(controller.Score, num);
					num++;
				}
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000C08C File Offset: 0x0000A28C
		private void SetJoustingBanners()
		{
			GameEntity banner0 = base.Mission.Scene.FindEntityWithTag("banner_0");
			GameEntity banner1 = base.Mission.Scene.FindEntityWithTag("banner_1");
			Banner banner = Banner.CreateOneColoredEmptyBanner(6);
			Banner banner2 = Banner.CreateOneColoredEmptyBanner(8);
			if (banner0 != null)
			{
				Action<Texture> action = delegate(Texture tex)
				{
					Material material = Mesh.GetFromResource("banner_test").GetMaterial().CreateCopy();
					if (Campaign.Current.GameMode == 1)
					{
						material.SetTexture(1, tex);
					}
					banner0.SetMaterialForAllMeshes(material);
				};
				banner.GetTableauTextureLarge(action);
			}
			if (banner1 != null)
			{
				Action<Texture> action2 = delegate(Texture tex)
				{
					Material material2 = Mesh.GetFromResource("banner_test").GetMaterial().CreateCopy();
					if (Campaign.Current.GameMode == 1)
					{
						material2.SetTexture(1, tex);
					}
					banner1.SetMaterialForAllMeshes(material2);
				};
				banner2.GetTableauTextureLarge(action2);
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000C12B File Offset: 0x0000A32B
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			this.RefreshScoreBoard();
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000C133 File Offset: 0x0000A333
		private void OnVictoryAchieved(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_player_victory", null).ToString(), 8f, true);
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_victory", null).ToString(), 8f, true);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000C16F File Offset: 0x0000A36F
		private void OnPointGanied(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_you_gain_point", null).ToString(), 5f, true);
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_gain_point", null).ToString(), 5f, true);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000C1AB File Offset: 0x0000A3AB
		private void OnDisqualified(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_disqualified", null).ToString(), 5f, true);
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_you_disqualified", null).ToString(), 5f, true);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000C1E7 File Offset: 0x0000A3E7
		private void OnUnconscious(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_you_become_unconscious", null).ToString(), 5f, true);
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_opponent_become_unconscious", null).ToString(), 5f, true);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000C223 File Offset: 0x0000A423
		public void ShowMessage(string str, float duration, bool hasPriority = true)
		{
			this._messageUIHandler.ShowMessage(str, duration, hasPriority);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000C233 File Offset: 0x0000A433
		public void ShowMessage(Agent agent, string str, float duration, bool hasPriority = true)
		{
			if (agent.Character == this._gameSystem.PlayerTroop)
			{
				this.ShowMessage(str, duration, hasPriority);
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000C252 File Offset: 0x0000A452
		public void DeleteMessage(string str)
		{
			this._messageUIHandler.DeleteMessage(str);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000C260 File Offset: 0x0000A460
		public void DeleteMessage(Agent agent, string str)
		{
			this.DeleteMessage(str);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000C26C File Offset: 0x0000A46C
		private void OnAgentStateChanged(Agent agent, JoustingAgentController.JoustingAgentState state)
		{
			string text;
			switch (state)
			{
			case JoustingAgentController.JoustingAgentState.GoingToBackStart:
				text = "";
				break;
			case JoustingAgentController.JoustingAgentState.GoToStartPosition:
				text = "str_tournament_joust_go_to_starting_position";
				break;
			case JoustingAgentController.JoustingAgentState.WaitInStartPosition:
				text = "str_tournament_joust_wait_in_starting_position";
				break;
			case JoustingAgentController.JoustingAgentState.WaitingOpponent:
				text = "str_tournament_joust_wait_opponent_to_go_starting_position";
				break;
			case JoustingAgentController.JoustingAgentState.Ready:
				text = "str_tournament_joust_go";
				break;
			case JoustingAgentController.JoustingAgentState.StartRiding:
				text = "";
				break;
			case JoustingAgentController.JoustingAgentState.Riding:
				text = "";
				break;
			case JoustingAgentController.JoustingAgentState.RidingAtWrongSide:
				text = "str_tournament_joust_wrong_side";
				break;
			case JoustingAgentController.JoustingAgentState.SwordDuel:
				text = "";
				break;
			default:
				throw new ArgumentOutOfRangeException("value");
			}
			if (text == "")
			{
				this.ShowMessage(agent, "", 15f, true);
			}
			else
			{
				this.ShowMessage(agent, GameTexts.FindText(text, null).ToString(), float.PositiveInfinity, true);
			}
			if (state == JoustingAgentController.JoustingAgentState.SwordDuel)
			{
				this.ShowMessage(agent, GameTexts.FindText("str_tournament_joust_duel_on_foot", null).ToString(), 8f, true);
			}
		}

		// Token: 0x0400007C RID: 124
		private MissionScoreUIHandler _scoreUIHandler;

		// Token: 0x0400007D RID: 125
		private MissionMessageUIHandler _messageUIHandler;

		// Token: 0x0400007E RID: 126
		private TournamentJoustingMissionController _tournamentJoustingMissionController;

		// Token: 0x0400007F RID: 127
		private Game _gameSystem;
	}
}
