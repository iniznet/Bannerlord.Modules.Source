using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace TaleWorlds.MountAndBlade
{
	public class BattleEndLogic : MissionLogic, IBattleEndLogic
	{
		public bool PlayerVictory
		{
			get
			{
				return (this._isEnemySideRetreating || this._isEnemySideDepleted) && !this._isEnemyDefenderPulledBack;
			}
		}

		public bool EnemyVictory
		{
			get
			{
				return this._isPlayerSideRetreating || this._isPlayerSideDepleted;
			}
		}

		private bool _notificationsDisabled { get; set; }

		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this._isEnemySideDepleted && this._isEnemyDefenderPulledBack)
			{
				missionResult = MissionResult.CreateDefenderPushedBack();
				flag = true;
			}
			else if (this._isEnemySideRetreating || this._isEnemySideDepleted)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, this._isEnemySideRetreating);
				flag = true;
			}
			else if (this._isPlayerSideRetreating || this._isPlayerSideDepleted)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			if (flag)
			{
				this._missionAgentSpawnLogic.StopSpawner(BattleSideEnum.Attacker);
				this._missionAgentSpawnLogic.StopSpawner(BattleSideEnum.Defender);
			}
			return flag;
		}

		public override void OnMissionTick(float dt)
		{
			if (base.Mission.IsMissionEnding)
			{
				if (this._notificationsDisabled)
				{
					this._scoreBoardOpenedOnceOnMissionEnd = true;
				}
				if (this._missionEndedMessageShown && !this._scoreBoardOpenedOnceOnMissionEnd)
				{
					if (this._checkRetreatingTimer.ElapsedTime > 7f)
					{
						this.CheckIsEnemySideRetreatingOrOneSideDepleted();
						this._checkRetreatingTimer.Reset();
						if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerDefeated)
						{
							GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4)));
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost_press_tab_to_view_results", null), 0, null, "");
						}
						else if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerVictory)
						{
							if (this._isEnemySideDepleted)
							{
								GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4)));
								MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won_press_tab_to_view_results", null), 0, null, "");
							}
						}
						else
						{
							GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4)));
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished_press_tab_to_view_results", null), 0, null, "");
						}
					}
				}
				else if (this._checkRetreatingTimer.ElapsedTime > 3f && !this._scoreBoardOpenedOnceOnMissionEnd)
				{
					if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerDefeated)
					{
						if (this._isPlayerSideDepleted)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost", null), 0, null, "");
						}
						else if (this._isPlayerSideRetreating)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_friendlies_are_fleeing_you_lost", null), 0, null, "");
						}
					}
					else if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerVictory)
					{
						if (this._isEnemySideDepleted)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won", null), 0, null, "");
						}
						else if (this._isEnemySideRetreating)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_enemies_are_fleeing_you_won", null), 0, null, "");
						}
					}
					else
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished", null), 0, null, "");
					}
					this._missionEndedMessageShown = true;
					this._checkRetreatingTimer.Reset();
				}
				if (!this._victoryReactionsActivated)
				{
					AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
					if (missionBehavior != null)
					{
						this.CheckIsEnemySideRetreatingOrOneSideDepleted();
						if (this._isEnemySideDepleted)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(base.Mission.PlayerTeam.Side);
							this._victoryReactionsActivated = true;
							return;
						}
						if (this._isPlayerSideDepleted)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(base.Mission.PlayerEnemyTeam.Side);
							this._victoryReactionsActivated = true;
							return;
						}
						if (this._isEnemySideRetreating && !this._victoryReactionsActivatedForRetreating)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnRetreat(base.Mission.PlayerTeam.Side);
							this._victoryReactionsActivatedForRetreating = true;
							return;
						}
						if (this._isPlayerSideRetreating && !this._victoryReactionsActivatedForRetreating)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnRetreat(base.Mission.PlayerEnemyTeam.Side);
							this._victoryReactionsActivatedForRetreating = true;
							return;
						}
					}
				}
			}
			else if (this._checkRetreatingTimer.ElapsedTime > 1f)
			{
				this.CheckIsEnemySideRetreatingOrOneSideDepleted();
				this._checkRetreatingTimer.Reset();
			}
		}

		public void ChangeCanCheckForEndCondition(bool canCheckForEndCondition)
		{
			this._canCheckForEndCondition = canCheckForEndCondition;
		}

		private void CheckIsEnemySideRetreatingOrOneSideDepleted()
		{
			if (!this._canCheckForEndConditionSiege)
			{
				this._canCheckForEndConditionSiege = base.Mission.GetMissionBehavior<BattleDeploymentHandler>() == null;
				return;
			}
			if (this._canCheckForEndCondition)
			{
				BattleSideEnum side = base.Mission.PlayerTeam.Side;
				BattleSideEnum oppositeSide = side.GetOppositeSide();
				this._isPlayerSideDepleted = this._missionAgentSpawnLogic.IsSideDepleted(side);
				this._isEnemySideDepleted = this._missionAgentSpawnLogic.IsSideDepleted(oppositeSide);
				if (this._isEnemySideDepleted || this._isPlayerSideDepleted)
				{
					return;
				}
				if (base.Mission.GetMissionBehavior<HideoutPhasedMissionController>() != null)
				{
					return;
				}
				float num = this._missionAgentSpawnLogic.GetReinforcementInterval() + 3f;
				if (base.Mission.MainAgent != null && base.Mission.MainAgent.IsPlayerControlled && base.Mission.MainAgent.IsActive())
				{
					this._playerSideNotYetRetreatingTime = MissionTime.Now;
				}
				else
				{
					bool flag = true;
					foreach (Team team in base.Mission.Teams)
					{
						if (team.IsFriendOf(base.Mission.PlayerTeam))
						{
							using (List<Agent>.Enumerator enumerator2 = team.ActiveAgents.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									if (!enumerator2.Current.IsRunningAway)
									{
										flag = false;
										break;
									}
								}
							}
						}
					}
					if (!flag)
					{
						this._playerSideNotYetRetreatingTime = MissionTime.Now;
					}
				}
				if (this._playerSideNotYetRetreatingTime.ElapsedSeconds > num)
				{
					this._isPlayerSideRetreating = true;
				}
				if (oppositeSide != BattleSideEnum.Defender || !this._enemyDefenderPullbackEnabled)
				{
					float num2 = this._missionAgentSpawnLogic.GetReinforcementInterval() + 3f;
					bool flag2 = true;
					foreach (Team team2 in base.Mission.Teams)
					{
						if (team2.IsEnemyOf(base.Mission.PlayerTeam))
						{
							using (List<Agent>.Enumerator enumerator2 = team2.ActiveAgents.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									if (!enumerator2.Current.IsRunningAway)
									{
										flag2 = false;
										break;
									}
								}
							}
						}
					}
					if (!flag2)
					{
						this._enemySideNotYetRetreatingTime = MissionTime.Now;
					}
					if (this._enemySideNotYetRetreatingTime.ElapsedSeconds > num2)
					{
						this._isEnemySideRetreating = true;
					}
				}
			}
		}

		public BattleEndLogic.ExitResult TryExit()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				return BattleEndLogic.ExitResult.False;
			}
			Agent mainAgent = base.Mission.MainAgent;
			if ((mainAgent != null && mainAgent.IsActive() && base.Mission.IsPlayerCloseToAnEnemy(5f)) || (!base.Mission.MissionEnded && (this.PlayerVictory || this.EnemyVictory)))
			{
				return BattleEndLogic.ExitResult.False;
			}
			if (base.Mission.MissionEnded || this._isEnemySideRetreating)
			{
				base.Mission.EndMission();
				return BattleEndLogic.ExitResult.True;
			}
			if (Mission.Current.IsSiegeBattle && base.Mission.PlayerTeam.IsDefender)
			{
				return BattleEndLogic.ExitResult.SurrenderSiege;
			}
			return BattleEndLogic.ExitResult.NeedsPlayerConfirmation;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._checkRetreatingTimer = new BasicMissionTimer();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
		}

		protected override void OnEndMission()
		{
			if (this._isEnemySideRetreating)
			{
				foreach (Agent agent in base.Mission.PlayerEnemyTeam.ActiveAgents)
				{
					IAgentOriginBase origin = agent.Origin;
					if (origin != null)
					{
						origin.SetRouted();
					}
				}
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (this._enemyDefenderPullbackEnabled && this._troopNumberNeededForEnemyDefenderPullBack > 0 && affectedAgent.IsHuman && agentState == AgentState.Routed && affectedAgent.Team != null && affectedAgent.Team.Side == BattleSideEnum.Defender && affectedAgent.Team.Side != base.Mission.PlayerTeam.Side)
			{
				this._troopNumberNeededForEnemyDefenderPullBack--;
				this._isEnemyDefenderPulledBack = this._troopNumberNeededForEnemyDefenderPullBack <= 0;
			}
		}

		public void EnableEnemyDefenderPullBack(int neededTroopNumber)
		{
			this._enemyDefenderPullbackEnabled = true;
			this._troopNumberNeededForEnemyDefenderPullBack = neededTroopNumber;
		}

		public bool IsEnemySideRetreating
		{
			get
			{
				return this._isEnemySideRetreating;
			}
		}

		public void SetNotificationDisabled(bool value)
		{
			this._notificationsDisabled = value;
		}

		private IMissionAgentSpawnLogic _missionAgentSpawnLogic;

		private MissionTime _enemySideNotYetRetreatingTime;

		private MissionTime _playerSideNotYetRetreatingTime;

		private BasicMissionTimer _checkRetreatingTimer;

		private bool _isEnemySideRetreating;

		private bool _isPlayerSideRetreating;

		private bool _isEnemySideDepleted;

		private bool _isPlayerSideDepleted;

		private bool _isEnemyDefenderPulledBack;

		private bool _canCheckForEndCondition = true;

		private bool _canCheckForEndConditionSiege;

		private bool _enemyDefenderPullbackEnabled;

		private int _troopNumberNeededForEnemyDefenderPullBack;

		private bool _missionEndedMessageShown;

		private bool _victoryReactionsActivated;

		private bool _victoryReactionsActivatedForRetreating;

		private bool _scoreBoardOpenedOnceOnMissionEnd;

		public enum ExitResult
		{
			False,
			NeedsPlayerConfirmation,
			SurrenderSiege,
			True
		}
	}
}
