using System;
using System.Collections.Generic;
using TaleWorlds.AchievementSystem;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerAchievementComponent : MissionLogic
	{
		public override void OnBehaviorInitialize()
		{
			this._missionLobbyComponent = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();
			this._multiplayerRoundComponent = Mission.Current.GetMissionBehavior<MultiplayerRoundComponent>();
			this.CacheAndInitializeAchievementVariables();
		}

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

		public override void OnAgentMount(Agent agent)
		{
			if (agent.IsMine && agent.SpawnEquipment.Horse.IsEmpty)
			{
				this._hasStolenMount = true;
				this._killsWithAStolenHorse = 0;
			}
		}

		public override void OnAgentDismount(Agent agent)
		{
			if (agent.IsMine)
			{
				this._hasStolenMount = false;
				this._killsWithAStolenHorse = 0;
			}
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsMine)
			{
				this._hasStolenMount = false;
				this._killsWithAStolenHorse = 0;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent != null && !affectedAgent.IsMount)
			{
				if (agentState == 4)
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
						if (blow.AttackType == 1 && blow.OverrideKillInfo == 21)
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
						if (this._missionLobbyComponent.MissionType == 6 && this._singleRoundKillsWithMeleeOnFoot > 0 && this._singleRoundKillsWithMeleeMounted > 0 && this._singleRoundKillsWithRangedOnFoot > 0 && this._singleRoundKillsWithRangedMounted > 0 && this._singleRoundKillsWithCouchedLance > 0)
						{
							this.SetStatInternal("SatisfiedJackOfAllTrades", 1);
						}
					}
					NetworkCommunicator myPeer = GameNetwork.MyPeer;
					MissionPeer missionPeer = ((myPeer != null) ? PeerExtensions.GetComponent<MissionPeer>(myPeer) : null);
					if (missionPeer != null)
					{
						Team team = missionPeer.Team;
						if (this._missionLobbyComponent.MissionType == 5)
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

		private void SetStatInternal(string statId, int value)
		{
			AchievementManager.SetStat(statId, value);
		}

		private const float SingleMangonelShotTimeout = 4f;

		private const string MaxMultiKillsWithSingleMangonelShotStatID = "MaxMultiKillsWithSingleMangonelShot";

		private const string KillsWithBoulderStatID = "KillsWithBoulder";

		private const string KillsWithChainAttackStatID = "KillsWithChainAttack";

		private const string KillsWithRangedHeadShotsStatID = "KillsWithRangedHeadshots";

		private const string KillsWithRangedMountedStatID = "KillsWithRangedMounted";

		private const string KillsWithCouchedLanceStatID = "KillsWithCouchedLance";

		private const string KillsWithHorseChargeStatID = "KillsWithHorseCharge";

		private const string KillCountCaptainStatID = "KillCountCaptain";

		private const string KillsWithStolenHorse = "KillsWithStolenHorse";

		private const string SatisfiedJackOfAllTradesStatID = "SatisfiedJackOfAllTrades";

		private const string PushedSomeoneOffLedgeStatID = "PushedSomeoneOffLedge";

		private int _cachedMaxMultiKillsWithSingleMangonelShot;

		private int _cachedKillsWithBoulder;

		private int _cachedKillsWithChainAttack;

		private int _cachedKillsWithRangedHeadShots;

		private int _cachedKillsWithRangedMounted;

		private int _cachedKillsWithCouchedLance;

		private int _cachedKillsWithHorseCharge;

		private int _cachedKillCountCaptain;

		private int _cachedKillsWithStolenHorse;

		private int _singleRoundKillsWithMeleeOnFoot;

		private int _singleRoundKillsWithMeleeMounted;

		private int _singleRoundKillsWithRangedOnFoot;

		private int _singleRoundKillsWithRangedMounted;

		private int _singleRoundKillsWithCouchedLance;

		private int _killsWithAStolenHorse;

		private bool _hasStolenMount;

		private MissionLobbyComponent _missionLobbyComponent;

		private MultiplayerRoundComponent _multiplayerRoundComponent;

		private Queue<MultiplayerAchievementComponent.BoulderKillRecord> _recentBoulderKills;

		private struct BoulderKillRecord
		{
			public BoulderKillRecord(float time)
			{
				this.time = time;
			}

			public readonly float time;
		}
	}
}
