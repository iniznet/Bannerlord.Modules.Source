﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Tournaments.MissionLogics
{
	public class TournamentFightMissionController : MissionLogic, ITournamentGameBehavior
	{
		public TournamentFightMissionController(CultureObject culture)
		{
			this._match = null;
			this._culture = culture;
			this._cheerStarted = false;
			this._currentTournamentAgents = new List<Agent>();
			this._currentTournamentMountAgents = new List<Agent>();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.Mission.CanAgentRout_AdditionalCondition += this.CanAgentRout;
		}

		public override void AfterStart()
		{
			TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_fight"));
			this._spawnPoints = new List<GameEntity>();
			for (int i = 0; i < 4; i++)
			{
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_arena_" + (i + 1));
				if (gameEntity != null)
				{
					this._spawnPoints.Add(gameEntity);
				}
			}
			if (this._spawnPoints.Count < 4)
			{
				this._spawnPoints = base.Mission.Scene.FindEntitiesWithTag("sp_arena").ToList<GameEntity>();
			}
		}

		public void PrepareForMatch()
		{
			List<Equipment> teamWeaponEquipmentList = this.GetTeamWeaponEquipmentList(this._match.Teams.First<TournamentTeam>().Participants.Count<TournamentParticipant>());
			foreach (TournamentTeam tournamentTeam in this._match.Teams)
			{
				int num = 0;
				foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
				{
					tournamentParticipant.MatchEquipment = teamWeaponEquipmentList[num].Clone(false);
					this.AddRandomClothes(this._culture, tournamentParticipant);
					num++;
				}
			}
		}

		public void StartMatch(TournamentMatch match, bool isLastRound)
		{
			this._cheerStarted = false;
			this._match = match;
			this._isLastRound = isLastRound;
			this.PrepareForMatch();
			base.Mission.SetMissionMode(2, true);
			List<Team> list = new List<Team>();
			int count = this._spawnPoints.Count;
			int num = 0;
			foreach (TournamentTeam tournamentTeam in this._match.Teams)
			{
				BattleSideEnum battleSideEnum = (tournamentTeam.IsPlayerTeam ? 0 : 1);
				Team team = base.Mission.Teams.Add(battleSideEnum, tournamentTeam.TeamColor, uint.MaxValue, tournamentTeam.TeamBanner, true, false, true);
				GameEntity gameEntity = this._spawnPoints[num % count];
				foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
				{
					if (tournamentParticipant.Character.IsPlayerCharacter)
					{
						this.SpawnTournamentParticipant(gameEntity, tournamentParticipant, team);
						break;
					}
				}
				foreach (TournamentParticipant tournamentParticipant2 in tournamentTeam.Participants)
				{
					if (!tournamentParticipant2.Character.IsPlayerCharacter)
					{
						this.SpawnTournamentParticipant(gameEntity, tournamentParticipant2, team);
					}
				}
				num++;
				list.Add(team);
			}
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					list[i].SetIsEnemyOf(list[j], true);
				}
			}
			this._aliveParticipants = this._match.Participants.ToList<TournamentParticipant>();
			this._aliveTeams = this._match.Teams.ToList<TournamentTeam>();
		}

		protected override void OnEndMission()
		{
			base.Mission.CanAgentRout_AdditionalCondition -= this.CanAgentRout;
		}

		private void SpawnTournamentParticipant(GameEntity spawnPoint, TournamentParticipant participant, Team team)
		{
			MatrixFrame globalFrame = spawnPoint.GetGlobalFrame();
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			this.SpawnAgentWithRandomItems(participant, team, globalFrame);
		}

		private List<Equipment> GetTeamWeaponEquipmentList(int teamSize)
		{
			List<Equipment> list = new List<Equipment>();
			CultureObject culture = PlayerEncounter.EncounterSettlement.Culture;
			MBReadOnlyList<CharacterObject> mbreadOnlyList = ((teamSize == 4) ? culture.TournamentTeamTemplatesForFourParticipant : ((teamSize == 2) ? culture.TournamentTeamTemplatesForTwoParticipant : culture.TournamentTeamTemplatesForOneParticipant));
			CharacterObject characterObject;
			if (mbreadOnlyList.Count > 0)
			{
				characterObject = mbreadOnlyList[MBRandom.RandomInt(mbreadOnlyList.Count)];
			}
			else
			{
				characterObject = ((teamSize == 4) ? this._defaultWeaponTemplatesIdTeamSizeFour : ((teamSize == 2) ? this._defaultWeaponTemplatesIdTeamSizeTwo : this._defaultWeaponTemplatesIdTeamSizeOne));
			}
			foreach (Equipment equipment in characterObject.BattleEquipments)
			{
				Equipment equipment2 = new Equipment();
				equipment2.FillFrom(equipment, true);
				list.Add(equipment2);
			}
			return list;
		}

		public void SkipMatch(TournamentMatch match)
		{
			this._match = match;
			this.PrepareForMatch();
			this.Simulate();
		}

		public bool IsMatchEnded()
		{
			if (this._isSimulated || this._match == null)
			{
				return true;
			}
			if ((this._endTimer != null && this._endTimer.ElapsedTime > 6f) || this._forceEndMatch)
			{
				this._forceEndMatch = false;
				this._endTimer = null;
				return true;
			}
			if (this._cheerTimer != null && !this._cheerStarted && this._cheerTimer.ElapsedTime > 1f)
			{
				this.OnMatchResultsReady();
				this._cheerTimer = null;
				this._cheerStarted = true;
				AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
				foreach (Agent agent in this._currentTournamentAgents)
				{
					if (agent.IsAIControlled)
					{
						missionBehavior.SetTimersOfVictoryReactionsOnTournamentVictoryForAgent(agent, 1f, 3f);
					}
				}
				return false;
			}
			if (this._endTimer == null && !this.CheckIfIsThereAnyEnemies())
			{
				this._endTimer = new BasicMissionTimer();
				if (!this._cheerStarted)
				{
					this._cheerTimer = new BasicMissionTimer();
				}
			}
			return false;
		}

		public void OnMatchResultsReady()
		{
			if (!this._match.IsPlayerParticipating())
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=UBd0dEPp}Match is over", null), 0, null, "");
				return;
			}
			if (this._match.IsPlayerWinner())
			{
				if (this._isLastRound)
				{
					if (this._match.QualificationMode == null)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=Jn0k20c3}Round is over, you survived the final round of the tournament.", null), 0, null, "");
						return;
					}
					MBInformationManager.AddQuickInformation(new TextObject("{=wOqOQuJl}Round is over, your team survived the final round of the tournament.", null), 0, null, "");
					return;
				}
				else
				{
					if (this._match.QualificationMode == null)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=uytwdSVH}Round is over, you are qualified for the next stage of the tournament.", null), 0, null, "");
						return;
					}
					MBInformationManager.AddQuickInformation(new TextObject("{=fkOYvnVG}Round is over, your team is qualified for the next stage of the tournament.", null), 0, null, "");
					return;
				}
			}
			else
			{
				if (this._match.QualificationMode == null)
				{
					MBInformationManager.AddQuickInformation(new TextObject("{=lcVauEKV}Round is over, you are disqualified from the tournament.", null), 0, null, "");
					return;
				}
				MBInformationManager.AddQuickInformation(new TextObject("{=MLyBN51z}Round is over, your team is disqualified from the tournament.", null), 0, null, "");
				return;
			}
		}

		public void OnMatchEnded()
		{
			SandBoxHelpers.MissionHelper.FadeOutAgents(this._currentTournamentAgents.Where((Agent x) => x.IsActive()), true, false);
			SandBoxHelpers.MissionHelper.FadeOutAgents(this._currentTournamentMountAgents.Where((Agent x) => x.IsActive()), true, false);
			base.Mission.ClearCorpses(false);
			base.Mission.Teams.Clear();
			base.Mission.RemoveSpawnedItemsAndMissiles();
			this._match = null;
			this._endTimer = null;
			this._cheerTimer = null;
			this._isSimulated = false;
			this._currentTournamentAgents.Clear();
			this._currentTournamentMountAgents.Clear();
		}

		private void SpawnAgentWithRandomItems(TournamentParticipant participant, Team team, MatrixFrame frame)
		{
			frame.Strafe((float)MBRandom.RandomInt(-2, 2) * 1f);
			frame.Advance((float)MBRandom.RandomInt(0, 2) * 1f);
			CharacterObject character = participant.Character;
			AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(character, -1, null, participant.Descriptor)).Team(team).InitialPosition(ref frame.origin);
			Vec2 vec = frame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).Equipment(participant.MatchEquipment).ClothingColor1(team.Color)
				.Banner(team.Banner)
				.Controller(character.IsPlayerCharacter ? 2 : 1);
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			if (character.IsPlayerCharacter)
			{
				agent.Health = (float)character.HeroObject.HitPoints;
				base.Mission.PlayerTeam = team;
			}
			else
			{
				agent.SetWatchState(2);
			}
			agent.WieldInitialWeapons(2, 0);
			this._currentTournamentAgents.Add(agent);
			if (agent.HasMount)
			{
				this._currentTournamentMountAgents.Add(agent.MountAgent);
			}
		}

		private void AddRandomClothes(CultureObject culture, TournamentParticipant participant)
		{
			Equipment participantArmor = Campaign.Current.Models.TournamentModel.GetParticipantArmor(participant.Character);
			for (int i = 5; i < 10; i++)
			{
				EquipmentElement equipmentFromSlot = participantArmor.GetEquipmentFromSlot(i);
				if (equipmentFromSlot.Item != null)
				{
					participant.MatchEquipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
				}
			}
		}

		private bool CheckIfTeamIsDead(TournamentTeam affectedParticipantTeam)
		{
			bool flag = true;
			using (List<TournamentParticipant>.Enumerator enumerator = this._aliveParticipants.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Team == affectedParticipantTeam)
					{
						flag = false;
						break;
					}
				}
			}
			return flag;
		}

		private void AddScoreToRemainingTeams()
		{
			foreach (TournamentTeam tournamentTeam in this._aliveTeams)
			{
				foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
				{
					tournamentParticipant.AddScore(1);
				}
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (!this.IsMatchEnded() && affectorAgent != null && affectedAgent != affectorAgent && affectedAgent.IsHuman && affectorAgent.IsHuman)
			{
				TournamentParticipant participant = this._match.GetParticipant(affectedAgent.Origin.UniqueSeed);
				this._aliveParticipants.Remove(participant);
				this._currentTournamentAgents.Remove(affectedAgent);
				if (this.CheckIfTeamIsDead(participant.Team))
				{
					this._aliveTeams.Remove(participant.Team);
					this.AddScoreToRemainingTeams();
				}
			}
		}

		public bool CanAgentRout(Agent agent)
		{
			return false;
		}

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent == null)
			{
				return;
			}
			if (affectorAgent.IsMount && affectorAgent.RiderAgent != null)
			{
				affectorAgent = affectorAgent.RiderAgent;
			}
			if (affectorAgent.Character == null || affectedAgent.Character == null)
			{
				return;
			}
			float num = (float)blow.InflictedDamage;
			if (num > affectedAgent.HealthLimit)
			{
				num = affectedAgent.HealthLimit;
			}
			float num2 = num / affectedAgent.HealthLimit;
			this.EnemyHitReward(affectedAgent, affectorAgent, blow.MovementSpeedDamageModifier, shotDifficulty, attackerWeapon, blow.AttackType, 0.5f * num2, num);
		}

		private void EnemyHitReward(Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, WeaponComponentData lastAttackerWeapon, AgentAttackType attackType, float hitpointRatio, float damageAmount)
		{
			CharacterObject characterObject = (CharacterObject)affectedAgent.Character;
			CharacterObject characterObject2 = (CharacterObject)affectorAgent.Character;
			if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
			{
				bool flag = affectorAgent.MountAgent != null && attackType == 3;
				SkillLevelingManager.OnCombatHit(characterObject2, characterObject, null, null, lastSpeedBonus, lastShotDifficulty, lastAttackerWeapon, hitpointRatio, 2, affectorAgent.MountAgent != null, affectorAgent.Team == affectedAgent.Team, false, damageAmount, affectedAgent.Health < 1f, false, flag);
			}
		}

		public bool CheckIfIsThereAnyEnemies()
		{
			Team team = null;
			foreach (Agent agent in this._currentTournamentAgents)
			{
				if (agent.IsHuman && agent.IsActive() && agent.Team != null)
				{
					if (team == null)
					{
						team = agent.Team;
					}
					else if (team != agent.Team)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void Simulate()
		{
			this._isSimulated = false;
			if (this._currentTournamentAgents.Count == 0)
			{
				this._aliveParticipants = this._match.Participants.ToList<TournamentParticipant>();
				this._aliveTeams = this._match.Teams.ToList<TournamentTeam>();
			}
			TournamentParticipant tournamentParticipant = this._aliveParticipants.FirstOrDefault((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
			if (tournamentParticipant != null)
			{
				TournamentTeam team = tournamentParticipant.Team;
				foreach (TournamentParticipant tournamentParticipant2 in team.Participants)
				{
					tournamentParticipant2.ResetScore();
					this._aliveParticipants.Remove(tournamentParticipant2);
				}
				this._aliveTeams.Remove(team);
				this.AddScoreToRemainingTeams();
			}
			Dictionary<TournamentParticipant, Tuple<float, float>> dictionary = new Dictionary<TournamentParticipant, Tuple<float, float>>();
			foreach (TournamentParticipant tournamentParticipant3 in this._aliveParticipants)
			{
				float num;
				float num2;
				tournamentParticipant3.Character.GetSimulationAttackPower(ref num, ref num2, tournamentParticipant3.MatchEquipment);
				dictionary.Add(tournamentParticipant3, new Tuple<float, float>(num, num2));
			}
			int num3 = 0;
			while (this._aliveParticipants.Count > 1 && this._aliveTeams.Count > 1)
			{
				num3++;
				num3 %= this._aliveParticipants.Count;
				TournamentParticipant tournamentParticipant4 = this._aliveParticipants[num3];
				int num4;
				TournamentParticipant tournamentParticipant5;
				do
				{
					num4 = MBRandom.RandomInt(this._aliveParticipants.Count);
					tournamentParticipant5 = this._aliveParticipants[num4];
				}
				while (tournamentParticipant4 == tournamentParticipant5 || tournamentParticipant4.Team == tournamentParticipant5.Team);
				if (dictionary[tournamentParticipant5].Item2 - dictionary[tournamentParticipant4].Item1 > 0f)
				{
					dictionary[tournamentParticipant5] = new Tuple<float, float>(dictionary[tournamentParticipant5].Item1, dictionary[tournamentParticipant5].Item2 - dictionary[tournamentParticipant4].Item1);
				}
				else
				{
					dictionary.Remove(tournamentParticipant5);
					this._aliveParticipants.Remove(tournamentParticipant5);
					if (this.CheckIfTeamIsDead(tournamentParticipant5.Team))
					{
						this._aliveTeams.Remove(tournamentParticipant5.Team);
						this.AddScoreToRemainingTeams();
					}
					if (num4 < num3)
					{
						num3--;
					}
				}
			}
			this._isSimulated = true;
		}

		private bool IsThereAnyPlayerAgent()
		{
			if (base.Mission.MainAgent != null && base.Mission.MainAgent.IsActive())
			{
				return true;
			}
			return this._currentTournamentAgents.Any((Agent agent) => agent.IsPlayerControlled);
		}

		private void SkipMatch()
		{
			Mission.Current.GetMissionBehavior<TournamentBehavior>().SkipMatch(false);
		}

		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			InquiryData inquiryData = null;
			canPlayerLeave = true;
			if (this._match != null)
			{
				if (this._match.IsPlayerParticipating())
				{
					MBTextManager.SetTextVariable("SETTLEMENT_NAME", Hero.MainHero.CurrentSettlement.EncyclopediaLinkWithName, false);
					if (this.IsThereAnyPlayerAgent())
					{
						if (base.Mission.IsPlayerCloseToAnEnemy(5f))
						{
							canPlayerLeave = false;
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat", null), 0, null, "");
						}
						else if (this.CheckIfIsThereAnyEnemies())
						{
							inquiryData = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_forfeit_game", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.SkipMatch), null, "", 0f, null, null, null);
						}
						else
						{
							this._forceEndMatch = true;
							canPlayerLeave = false;
						}
					}
					else if (this.CheckIfIsThereAnyEnemies())
					{
						inquiryData = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_skip", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.SkipMatch), null, "", 0f, null, null, null);
					}
					else
					{
						this._forceEndMatch = true;
						canPlayerLeave = false;
					}
				}
				else if (this.CheckIfIsThereAnyEnemies())
				{
					inquiryData = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_skip", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.SkipMatch), null, "", 0f, null, null, null);
				}
				else
				{
					this._forceEndMatch = true;
					canPlayerLeave = false;
				}
			}
			return inquiryData;
		}

		private readonly CharacterObject _defaultWeaponTemplatesIdTeamSizeOne = MBObjectManager.Instance.GetObject<CharacterObject>("tournament_template_empire_one_participant_set_v1");

		private readonly CharacterObject _defaultWeaponTemplatesIdTeamSizeTwo = MBObjectManager.Instance.GetObject<CharacterObject>("tournament_template_empire_two_participant_set_v1");

		private readonly CharacterObject _defaultWeaponTemplatesIdTeamSizeFour = MBObjectManager.Instance.GetObject<CharacterObject>("tournament_template_empire_four_participant_set_v1");

		private TournamentMatch _match;

		private bool _isLastRound;

		private BasicMissionTimer _endTimer;

		private BasicMissionTimer _cheerTimer;

		private List<GameEntity> _spawnPoints;

		private bool _isSimulated;

		private bool _forceEndMatch;

		private bool _cheerStarted;

		private CultureObject _culture;

		private List<TournamentParticipant> _aliveParticipants;

		private List<TournamentTeam> _aliveTeams;

		private List<Agent> _currentTournamentAgents;

		private List<Agent> _currentTournamentMountAgents;

		private const float XpShareForKill = 0.5f;

		private const float XpShareForDamage = 0.5f;
	}
}
