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
	public class MissionTournamentJoustingView : MissionView
	{
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
				BannerVisualExtensions.GetTableauTextureLarge(banner, action);
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
				BannerVisualExtensions.GetTableauTextureLarge(banner2, action2);
			}
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			this.RefreshScoreBoard();
		}

		private void OnVictoryAchieved(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_player_victory", null).ToString(), 8f, true);
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_victory", null).ToString(), 8f, true);
		}

		private void OnPointGanied(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_you_gain_point", null).ToString(), 5f, true);
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_gain_point", null).ToString(), 5f, true);
		}

		private void OnDisqualified(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_opponent_disqualified", null).ToString(), 5f, true);
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_you_disqualified", null).ToString(), 5f, true);
		}

		private void OnUnconscious(Agent affectorAgent, Agent affectedAgent)
		{
			this.ShowMessage(affectedAgent, GameTexts.FindText("str_tournament_joust_you_become_unconscious", null).ToString(), 5f, true);
			this.ShowMessage(affectorAgent, GameTexts.FindText("str_tournament_joust_opponent_become_unconscious", null).ToString(), 5f, true);
		}

		public void ShowMessage(string str, float duration, bool hasPriority = true)
		{
			this._messageUIHandler.ShowMessage(str, duration, hasPriority);
		}

		public void ShowMessage(Agent agent, string str, float duration, bool hasPriority = true)
		{
			if (agent.Character == this._gameSystem.PlayerTroop)
			{
				this.ShowMessage(str, duration, hasPriority);
			}
		}

		public void DeleteMessage(string str)
		{
			this._messageUIHandler.DeleteMessage(str);
		}

		public void DeleteMessage(Agent agent, string str)
		{
			this.DeleteMessage(str);
		}

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

		private MissionScoreUIHandler _scoreUIHandler;

		private MissionMessageUIHandler _messageUIHandler;

		private TournamentJoustingMissionController _tournamentJoustingMissionController;

		private Game _gameSystem;
	}
}
