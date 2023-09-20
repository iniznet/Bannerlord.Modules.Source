using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200025E RID: 606
	public class BattleEndLogic : MissionLogic, IBattleEndLogic
	{
		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060020AB RID: 8363 RVA: 0x00074A2E File Offset: 0x00072C2E
		public bool PlayerVictory
		{
			get
			{
				return (this._isEnemySideRetreating || this._isEnemySideDepleted) && !this._isEnemyDefenderPulledBack;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060020AC RID: 8364 RVA: 0x00074A4B File Offset: 0x00072C4B
		public bool EnemyVictory
		{
			get
			{
				return this._isPlayerSideRetreating || this._isPlayerSideDepleted;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060020AD RID: 8365 RVA: 0x00074A5D File Offset: 0x00072C5D
		// (set) Token: 0x060020AE RID: 8366 RVA: 0x00074A65 File Offset: 0x00072C65
		private bool _notificationsDisabled { get; set; }

		// Token: 0x060020AF RID: 8367 RVA: 0x00074A70 File Offset: 0x00072C70
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

		// Token: 0x060020B0 RID: 8368 RVA: 0x00074AFC File Offset: 0x00072CFC
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

		// Token: 0x060020B1 RID: 8369 RVA: 0x00074E4E File Offset: 0x0007304E
		public void ChangeCanCheckForEndCondition(bool canCheckForEndCondition)
		{
			this._canCheckForEndCondition = canCheckForEndCondition;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00074E58 File Offset: 0x00073058
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

		// Token: 0x060020B3 RID: 8371 RVA: 0x000750E8 File Offset: 0x000732E8
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

		// Token: 0x060020B4 RID: 8372 RVA: 0x0007518B File Offset: 0x0007338B
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._checkRetreatingTimer = new BasicMissionTimer();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x000751B0 File Offset: 0x000733B0
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

		// Token: 0x060020B6 RID: 8374 RVA: 0x00075220 File Offset: 0x00073420
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (base.Mission.Mode != MissionMode.Deployment && this._enemyDefenderPullbackEnabled && this._troopNumberNeededForEnemyDefenderPullBack > 0 && affectedAgent.IsHuman && agentState == AgentState.Routed && affectedAgent.Team != null && affectedAgent.Team.Side == BattleSideEnum.Defender && affectedAgent.Team.Side != base.Mission.PlayerTeam.Side)
			{
				this._troopNumberNeededForEnemyDefenderPullBack--;
				this._isEnemyDefenderPulledBack = this._troopNumberNeededForEnemyDefenderPullBack <= 0;
			}
		}

		// Token: 0x060020B7 RID: 8375 RVA: 0x000752AA File Offset: 0x000734AA
		public void EnableEnemyDefenderPullBack(int neededTroopNumber)
		{
			this._enemyDefenderPullbackEnabled = true;
			this._troopNumberNeededForEnemyDefenderPullBack = neededTroopNumber;
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x060020B8 RID: 8376 RVA: 0x000752BA File Offset: 0x000734BA
		public bool IsEnemySideRetreating
		{
			get
			{
				return this._isEnemySideRetreating;
			}
		}

		// Token: 0x060020B9 RID: 8377 RVA: 0x000752C2 File Offset: 0x000734C2
		public void SetNotificationDisabled(bool value)
		{
			this._notificationsDisabled = value;
		}

		// Token: 0x04000C01 RID: 3073
		private IMissionAgentSpawnLogic _missionAgentSpawnLogic;

		// Token: 0x04000C02 RID: 3074
		private MissionTime _enemySideNotYetRetreatingTime;

		// Token: 0x04000C03 RID: 3075
		private MissionTime _playerSideNotYetRetreatingTime;

		// Token: 0x04000C04 RID: 3076
		private BasicMissionTimer _checkRetreatingTimer;

		// Token: 0x04000C05 RID: 3077
		private bool _isEnemySideRetreating;

		// Token: 0x04000C06 RID: 3078
		private bool _isPlayerSideRetreating;

		// Token: 0x04000C07 RID: 3079
		private bool _isEnemySideDepleted;

		// Token: 0x04000C08 RID: 3080
		private bool _isPlayerSideDepleted;

		// Token: 0x04000C09 RID: 3081
		private bool _isEnemyDefenderPulledBack;

		// Token: 0x04000C0A RID: 3082
		private bool _canCheckForEndCondition = true;

		// Token: 0x04000C0B RID: 3083
		private bool _canCheckForEndConditionSiege;

		// Token: 0x04000C0C RID: 3084
		private bool _enemyDefenderPullbackEnabled;

		// Token: 0x04000C0D RID: 3085
		private int _troopNumberNeededForEnemyDefenderPullBack;

		// Token: 0x04000C0E RID: 3086
		private bool _missionEndedMessageShown;

		// Token: 0x04000C0F RID: 3087
		private bool _victoryReactionsActivated;

		// Token: 0x04000C10 RID: 3088
		private bool _victoryReactionsActivatedForRetreating;

		// Token: 0x04000C11 RID: 3089
		private bool _scoreBoardOpenedOnceOnMissionEnd;

		// Token: 0x0200056A RID: 1386
		public enum ExitResult
		{
			// Token: 0x04001D09 RID: 7433
			False,
			// Token: 0x04001D0A RID: 7434
			NeedsPlayerConfirmation,
			// Token: 0x04001D0B RID: 7435
			SurrenderSiege,
			// Token: 0x04001D0C RID: 7436
			True
		}
	}
}
