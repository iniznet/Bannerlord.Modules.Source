using System;
using System.Collections.Generic;
using TaleWorlds.AchievementSystem;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000279 RID: 633
	public class MultiplayerAchievementComponent : MissionLogic
	{
		// Token: 0x060021BE RID: 8638 RVA: 0x0007B00A File Offset: 0x0007920A
		public override void OnBehaviorInitialize()
		{
			this._missionLobbyComponent = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();
			this._multiplayerRoundComponent = Mission.Current.GetMissionBehavior<MultiplayerRoundComponent>();
			this.CacheAndInitializeAchievementVariables();
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x0007B032 File Offset: 0x00079232
		public override void EarlyStart()
		{
			if (this._multiplayerRoundComponent != null)
			{
				this._multiplayerRoundComponent.OnRoundStarted += this.OnRoundStarted;
			}
			if (this._recentBoulderKills == null)
			{
				this._recentBoulderKills = new Queue<MultiplayerAchievementComponent.BoulderKillRecord>();
				return;
			}
			this._recentBoulderKills.Clear();
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x0007B072 File Offset: 0x00079272
		protected override void OnEndMission()
		{
			if (this._multiplayerRoundComponent != null)
			{
				this._multiplayerRoundComponent.OnRoundStarted -= this.OnRoundStarted;
			}
			if (this._recentBoulderKills != null)
			{
				this._recentBoulderKills.Clear();
			}
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x0007B0A8 File Offset: 0x000792A8
		public override void OnMissionTick(float dt)
		{
			if (this._recentBoulderKills != null)
			{
				while (this._recentBoulderKills.Count > 0)
				{
					MultiplayerAchievementComponent.BoulderKillRecord boulderKillRecord = this._recentBoulderKills.Peek();
					if (base.Mission.CurrentTime - boulderKillRecord.time < 4f)
					{
						break;
					}
					this._recentBoulderKills.Dequeue();
				}
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x0007B0FE File Offset: 0x000792FE
		private void OnRoundStarted()
		{
			this._singleRoundKillsWithMeleeOnFoot = 0;
			this._singleRoundKillsWithMeleeMounted = 0;
			this._singleRoundKillsWithRangedOnFoot = 0;
			this._singleRoundKillsWithRangedMounted = 0;
			this._singleRoundKillsWithCouchedLance = 0;
			this._killsWithAStolenHorse = 0;
			this._hasStolenMount = false;
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x0007B134 File Offset: 0x00079334
		public override void OnAgentMount(Agent agent)
		{
			if (agent.IsMine && agent.SpawnEquipment.Horse.IsEmpty)
			{
				this._hasStolenMount = true;
				this._killsWithAStolenHorse = 0;
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x0007B16C File Offset: 0x0007936C
		public override void OnAgentDismount(Agent agent)
		{
			if (agent.IsMine)
			{
				this._hasStolenMount = false;
				this._killsWithAStolenHorse = 0;
			}
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x0007B184 File Offset: 0x00079384
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsMine)
			{
				this._hasStolenMount = false;
				this._killsWithAStolenHorse = 0;
			}
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x0007B19C File Offset: 0x0007939C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent != null && !affectedAgent.IsMount)
			{
				if (agentState == AgentState.Killed)
				{
					if (affectorAgent != null && affectorAgent.IsMine && affectorAgent != affectedAgent && !affectedAgent.IsFriendOf(affectorAgent))
					{
						int weaponClass;
						int num = (weaponClass = blow.WeaponClass);
						bool flag = weaponClass >= 1 && weaponClass <= 11;
						bool isMissile = blow.IsMissile;
						if (num == 18)
						{
							this._recentBoulderKills.Enqueue(new MultiplayerAchievementComponent.BoulderKillRecord(base.Mission.CurrentTime));
							if (this._recentBoulderKills.Count > 1 && this._recentBoulderKills.Count > this._cachedMaxMultiKillsWithSingleMangonelShot)
							{
								this._cachedMaxMultiKillsWithSingleMangonelShot = this._recentBoulderKills.Count;
								this.SetStatInternal("MaxMultiKillsWithSingleMangonelShot", this._cachedMaxMultiKillsWithSingleMangonelShot);
							}
							this._cachedKillsWithBoulder++;
							this.SetStatInternal("KillsWithBoulder", this._cachedKillsWithBoulder);
						}
						if (blow.AttackType == AgentAttackType.Kick && blow.OverrideKillInfo == Agent.KillInfo.Gravity)
						{
							this.SetStatInternal("PushedSomeoneOffLedge", 1);
						}
						if (isMissile && blow.IsHeadShot())
						{
							this._cachedKillsWithRangedHeadShots++;
							this.SetStatInternal("KillsWithRangedHeadshots", this._cachedKillsWithRangedHeadShots);
						}
						if (affectorAgent.IsReleasingChainAttack())
						{
							this._cachedKillsWithChainAttack++;
							this.SetStatInternal("KillsWithChainAttack", this._cachedKillsWithChainAttack);
						}
						if (affectorAgent.HasMount)
						{
							if (affectorAgent.IsDoingPassiveAttack)
							{
								this._singleRoundKillsWithCouchedLance++;
								this._cachedKillsWithCouchedLance++;
								this.SetStatInternal("KillsWithCouchedLance", this._cachedKillsWithCouchedLance);
							}
							if (isMissile)
							{
								this._singleRoundKillsWithRangedMounted++;
								this._cachedKillsWithRangedMounted++;
								this.SetStatInternal("KillsWithRangedMounted", this._cachedKillsWithRangedMounted);
							}
							if (flag)
							{
								this._singleRoundKillsWithMeleeMounted++;
							}
							if (!flag && !isMissile)
							{
								this._cachedKillsWithHorseCharge++;
								this.SetStatInternal("KillsWithHorseCharge", this._cachedKillsWithHorseCharge);
							}
							if (this._hasStolenMount)
							{
								this._killsWithAStolenHorse++;
								if (this._killsWithAStolenHorse > this._cachedKillsWithStolenHorse)
								{
									this._cachedKillsWithStolenHorse = this._killsWithAStolenHorse;
									this.SetStatInternal("KillsWithStolenHorse", this._cachedKillsWithStolenHorse);
								}
							}
						}
						else
						{
							if (isMissile)
							{
								this._singleRoundKillsWithRangedOnFoot++;
							}
							if (flag)
							{
								this._singleRoundKillsWithMeleeOnFoot++;
							}
						}
						if (this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Skirmish && this._singleRoundKillsWithMeleeOnFoot > 0 && this._singleRoundKillsWithMeleeMounted > 0 && this._singleRoundKillsWithRangedOnFoot > 0 && this._singleRoundKillsWithRangedMounted > 0 && this._singleRoundKillsWithCouchedLance > 0)
						{
							this.SetStatInternal("SatisfiedJackOfAllTrades", 1);
						}
					}
					NetworkCommunicator myPeer = GameNetwork.MyPeer;
					MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
					if (missionPeer != null)
					{
						Team team = missionPeer.Team;
						if (this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Captain)
						{
							Agent mainAgent = Mission.Current.MainAgent;
							if (mainAgent != null && affectorAgent != null)
							{
								Formation formation = mainAgent.Formation;
								Formation formation2 = affectorAgent.Formation;
								if (formation != null && formation2 != null && formation2 == formation && team != null && team != ((affectedAgent != null) ? affectedAgent.Team : null))
								{
									this._cachedKillCountCaptain++;
									this.SetStatInternal("KillCountCaptain", this._cachedKillCountCaptain);
								}
							}
						}
					}
				}
				if (affectedAgent.IsMine)
				{
					this._hasStolenMount = false;
					this._killsWithAStolenHorse = 0;
				}
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x0007B4F4 File Offset: 0x000796F4
		private async void CacheAndInitializeAchievementVariables()
		{
			int[] array = await AchievementManager.GetStats(new string[] { "MaxMultiKillsWithSingleMangonelShot", "KillsWithBoulder", "KillsWithChainAttack", "KillsWithRangedHeadshots", "KillsWithRangedMounted", "KillsWithCouchedLance", "KillsWithHorseCharge", "KillCountCaptain", "KillsWithStolenHorse" });
			if (array != null)
			{
				int num = 0;
				int cachedMaxMultiKillsWithSingleMangonelShot = this._cachedMaxMultiKillsWithSingleMangonelShot;
				int[] array2 = array;
				num++;
				this._cachedMaxMultiKillsWithSingleMangonelShot = cachedMaxMultiKillsWithSingleMangonelShot + array2[num];
				int cachedKillsWithBoulder = this._cachedKillsWithBoulder;
				int[] array3 = array;
				num++;
				this._cachedKillsWithBoulder = cachedKillsWithBoulder + array3[num];
				int cachedKillsWithChainAttack = this._cachedKillsWithChainAttack;
				int[] array4 = array;
				num++;
				this._cachedKillsWithChainAttack = cachedKillsWithChainAttack + array4[num];
				int cachedKillsWithRangedHeadShots = this._cachedKillsWithRangedHeadShots;
				int[] array5 = array;
				num++;
				this._cachedKillsWithRangedHeadShots = cachedKillsWithRangedHeadShots + array5[num];
				int cachedKillsWithRangedMounted = this._cachedKillsWithRangedMounted;
				int[] array6 = array;
				num++;
				this._cachedKillsWithRangedMounted = cachedKillsWithRangedMounted + array6[num];
				int cachedKillsWithCouchedLance = this._cachedKillsWithCouchedLance;
				int[] array7 = array;
				num++;
				this._cachedKillsWithCouchedLance = cachedKillsWithCouchedLance + array7[num];
				int cachedKillsWithHorseCharge = this._cachedKillsWithHorseCharge;
				int[] array8 = array;
				num++;
				this._cachedKillsWithHorseCharge = cachedKillsWithHorseCharge + array8[num];
				int cachedKillCountCaptain = this._cachedKillCountCaptain;
				int[] array9 = array;
				num++;
				this._cachedKillCountCaptain = cachedKillCountCaptain + array9[num];
				int cachedKillsWithStolenHorse = this._cachedKillsWithStolenHorse;
				int[] array10 = array;
				num++;
				this._cachedKillsWithStolenHorse = cachedKillsWithStolenHorse + array10[num];
			}
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x0007B52D File Offset: 0x0007972D
		private void SetStatInternal(string statId, int value)
		{
			AchievementManager.SetStat(statId, value);
		}

		// Token: 0x04000C7F RID: 3199
		private const float SingleMangonelShotTimeout = 4f;

		// Token: 0x04000C80 RID: 3200
		private const string MaxMultiKillsWithSingleMangonelShotStatID = "MaxMultiKillsWithSingleMangonelShot";

		// Token: 0x04000C81 RID: 3201
		private const string KillsWithBoulderStatID = "KillsWithBoulder";

		// Token: 0x04000C82 RID: 3202
		private const string KillsWithChainAttackStatID = "KillsWithChainAttack";

		// Token: 0x04000C83 RID: 3203
		private const string KillsWithRangedHeadShotsStatID = "KillsWithRangedHeadshots";

		// Token: 0x04000C84 RID: 3204
		private const string KillsWithRangedMountedStatID = "KillsWithRangedMounted";

		// Token: 0x04000C85 RID: 3205
		private const string KillsWithCouchedLanceStatID = "KillsWithCouchedLance";

		// Token: 0x04000C86 RID: 3206
		private const string KillsWithHorseChargeStatID = "KillsWithHorseCharge";

		// Token: 0x04000C87 RID: 3207
		private const string KillCountCaptainStatID = "KillCountCaptain";

		// Token: 0x04000C88 RID: 3208
		private const string KillsWithStolenHorse = "KillsWithStolenHorse";

		// Token: 0x04000C89 RID: 3209
		private const string SatisfiedJackOfAllTradesStatID = "SatisfiedJackOfAllTrades";

		// Token: 0x04000C8A RID: 3210
		private const string PushedSomeoneOffLedgeStatID = "PushedSomeoneOffLedge";

		// Token: 0x04000C8B RID: 3211
		private int _cachedMaxMultiKillsWithSingleMangonelShot;

		// Token: 0x04000C8C RID: 3212
		private int _cachedKillsWithBoulder;

		// Token: 0x04000C8D RID: 3213
		private int _cachedKillsWithChainAttack;

		// Token: 0x04000C8E RID: 3214
		private int _cachedKillsWithRangedHeadShots;

		// Token: 0x04000C8F RID: 3215
		private int _cachedKillsWithRangedMounted;

		// Token: 0x04000C90 RID: 3216
		private int _cachedKillsWithCouchedLance;

		// Token: 0x04000C91 RID: 3217
		private int _cachedKillsWithHorseCharge;

		// Token: 0x04000C92 RID: 3218
		private int _cachedKillCountCaptain;

		// Token: 0x04000C93 RID: 3219
		private int _cachedKillsWithStolenHorse;

		// Token: 0x04000C94 RID: 3220
		private int _singleRoundKillsWithMeleeOnFoot;

		// Token: 0x04000C95 RID: 3221
		private int _singleRoundKillsWithMeleeMounted;

		// Token: 0x04000C96 RID: 3222
		private int _singleRoundKillsWithRangedOnFoot;

		// Token: 0x04000C97 RID: 3223
		private int _singleRoundKillsWithRangedMounted;

		// Token: 0x04000C98 RID: 3224
		private int _singleRoundKillsWithCouchedLance;

		// Token: 0x04000C99 RID: 3225
		private int _killsWithAStolenHorse;

		// Token: 0x04000C9A RID: 3226
		private bool _hasStolenMount;

		// Token: 0x04000C9B RID: 3227
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000C9C RID: 3228
		private MultiplayerRoundComponent _multiplayerRoundComponent;

		// Token: 0x04000C9D RID: 3229
		private Queue<MultiplayerAchievementComponent.BoulderKillRecord> _recentBoulderKills;

		// Token: 0x02000586 RID: 1414
		private struct BoulderKillRecord
		{
			// Token: 0x06003B08 RID: 15112 RVA: 0x000EDEE3 File Offset: 0x000EC0E3
			public BoulderKillRecord(float time)
			{
				this.time = time;
			}

			// Token: 0x04001D73 RID: 7539
			public readonly float time;
		}
	}
}
