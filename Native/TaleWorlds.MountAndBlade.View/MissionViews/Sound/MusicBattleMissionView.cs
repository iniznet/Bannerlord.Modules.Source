using System;
using System.Collections.Generic;
using System.Linq;
using psai.net;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	// Token: 0x02000057 RID: 87
	public class MusicBattleMissionView : MissionView, IMusicHandler
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x0001FB6C File Offset: 0x0001DD6C
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x0001FB6F File Offset: 0x0001DD6F
		private BattleSideEnum PlayerSide
		{
			get
			{
				Team playerTeam = Mission.Current.PlayerTeam;
				if (playerTeam == null)
				{
					return -1;
				}
				return playerTeam.Side;
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001FB86 File Offset: 0x0001DD86
		public MusicBattleMissionView(bool isSiegeBattle)
		{
			this._isSiegeBattle = isSiegeBattle;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001FB95 File Offset: 0x0001DD95
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = Mission.Current.GetMissionBehavior<MissionAgentSpawnLogic>();
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.ActivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001FBCC File Offset: 0x0001DDCC
		public override void OnMissionScreenFinalize()
		{
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
			base.Mission.PlayerTeam.PlayerOrderController.OnOrderIssued -= new OnOrderIssuedDelegate(this.PlayerOrderControllerOnOrderIssued);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0001FC03 File Offset: 0x0001DE03
		public override void AfterStart()
		{
			this._nextPossibleTimeToIncreaseIntensityForChargeOrder = MissionTime.Now;
			base.Mission.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.PlayerOrderControllerOnOrderIssued);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001FC34 File Offset: 0x0001DE34
		private void PlayerOrderControllerOnOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, object[] parameters)
		{
			if ((orderType == 4 || orderType == 5) && this._nextPossibleTimeToIncreaseIntensityForChargeOrder.IsPast)
			{
				float currentIntensity = PsaiCore.Instance.GetCurrentIntensity();
				float num = currentIntensity * MusicParameters.PlayerChargeEffectMultiplierOnIntensity - currentIntensity;
				MBMusicManager.Current.ChangeCurrentThemeIntensity(num);
				this._nextPossibleTimeToIncreaseIntensityForChargeOrder = MissionTime.Now + MissionTime.Seconds(60f);
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001FC90 File Offset: 0x0001DE90
		private void CheckIntensityFall()
		{
			PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
			if (psaiInfo.effectiveThemeId >= 0)
			{
				if (float.IsNaN(psaiInfo.currentIntensity))
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity);
					return;
				}
				if (psaiInfo.currentIntensity < MusicParameters.MinIntensity)
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity - psaiInfo.currentIntensity);
				}
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0001FCF8 File Offset: 0x0001DEF8
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._battleState != MusicBattleMissionView.BattleState.Starting)
			{
				bool flag = affectedAgent.IsMine || (affectedAgent.RiderAgent != null && affectedAgent.RiderAgent.IsMine);
				Team team = affectedAgent.Team;
				BattleSideEnum battleSideEnum = ((team != null) ? team.Side : (-1));
				bool flag2;
				if (!flag)
				{
					if (battleSideEnum != -1)
					{
						Team playerTeam = Mission.Current.PlayerTeam;
						flag2 = ((playerTeam != null) ? playerTeam.Side : (-1)) == battleSideEnum;
					}
					else
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (!this._isSiegeBattle && affectedAgent.IsHuman && battleSideEnum != -1 && this._battleState == MusicBattleMissionView.BattleState.Started && this._startingTroopCounts.Sum() >= MusicParameters.SmallBattleTreshold && MissionTime.Now.ToSeconds > (double)MusicParameters.BattleTurnsOneSideCooldown && this._missionAgentSpawnLogic.NumberOfRemainingTroops == 0)
				{
					int[] array = new int[]
					{
						this._missionAgentSpawnLogic.NumberOfActiveDefenderTroops,
						this._missionAgentSpawnLogic.NumberOfActiveAttackerTroops
					};
					array[battleSideEnum]--;
					MusicTheme musicTheme = -1;
					if (array[0] > 0 && array[1] > 0)
					{
						float num = (float)array[0] / (float)array[1];
						if (num < this._startingBattleRatio * MusicParameters.BattleRatioTresholdOnIntensity)
						{
							musicTheme = MBMusicManager.Current.GetBattleTurnsOneSideTheme(base.Mission.MusicCulture.GetCultureCode(), this.PlayerSide > 0, this._isPaganBattle);
						}
						else if (num > this._startingBattleRatio / MusicParameters.BattleRatioTresholdOnIntensity)
						{
							musicTheme = MBMusicManager.Current.GetBattleTurnsOneSideTheme(base.Mission.MusicCulture.GetCultureCode(), this.PlayerSide == 0, this._isPaganBattle);
						}
					}
					if (musicTheme != -1)
					{
						MBMusicManager.Current.StartTheme(musicTheme, PsaiCore.Instance.GetCurrentIntensity(), false);
						this._battleState = MusicBattleMissionView.BattleState.TurnedOneSide;
					}
				}
				if ((affectedAgent.IsHuman && affectedAgent.State != 2) || flag)
				{
					float num2 = (flag3 ? MusicParameters.FriendlyTroopDeadEffectOnIntensity : MusicParameters.EnemyTroopDeadEffectOnIntensity);
					if (flag)
					{
						num2 *= MusicParameters.PlayerTroopDeadEffectMultiplierOnIntensity;
					}
					MBMusicManager.Current.ChangeCurrentThemeIntensity(num2);
				}
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0001FF08 File Offset: 0x0001E108
		private void CheckForStarting()
		{
			if (this._startingTroopCounts == null)
			{
				this._startingTroopCounts = new int[]
				{
					this._missionAgentSpawnLogic.GetTotalNumberOfTroopsForSide(0),
					this._missionAgentSpawnLogic.GetTotalNumberOfTroopsForSide(1)
				};
				this._startingBattleRatio = (float)this._startingTroopCounts[0] / (float)this._startingTroopCounts[1];
			}
			Agent main = Agent.Main;
			Vec2 vec = ((main != null) ? main.Position.AsVec2 : Vec2.Invalid);
			Team playerTeam = Mission.Current.PlayerTeam;
			bool flag;
			if (playerTeam == null)
			{
				flag = false;
			}
			else
			{
				flag = playerTeam.FormationsIncludingEmpty.Any((Formation f) => f.CountOfUnits > 0);
			}
			bool flag2 = flag;
			float num = float.MaxValue;
			if (flag2 || vec.IsValid)
			{
				foreach (Formation formation in Mission.Current.PlayerEnemyTeam.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						float num2 = float.MaxValue;
						if (!flag2 && vec.IsValid)
						{
							num2 = vec.DistanceSquared(formation.CurrentPosition);
						}
						else if (flag2)
						{
							foreach (Formation formation2 in Mission.Current.PlayerTeam.FormationsIncludingEmpty)
							{
								if (formation2.CountOfUnits > 0)
								{
									float num3 = formation2.CurrentPosition.DistanceSquared(formation.CurrentPosition);
									if (num2 > num3)
									{
										num2 = num3;
									}
								}
							}
						}
						if (num > num2)
						{
							num = num2;
						}
					}
				}
			}
			int num4 = this._startingTroopCounts.Sum();
			bool flag3 = false;
			if (num4 < MusicParameters.SmallBattleTreshold)
			{
				if (num < MusicParameters.SmallBattleDistanceTreshold * MusicParameters.SmallBattleDistanceTreshold)
				{
					flag3 = true;
				}
			}
			else if (num4 < MusicParameters.MediumBattleTreshold)
			{
				if (num < MusicParameters.MediumBattleDistanceTreshold * MusicParameters.MediumBattleDistanceTreshold)
				{
					flag3 = true;
				}
			}
			else if (num4 < MusicParameters.LargeBattleTreshold)
			{
				if (num < MusicParameters.LargeBattleDistanceTreshold * MusicParameters.LargeBattleDistanceTreshold)
				{
					flag3 = true;
				}
			}
			else if (num < MusicParameters.MaxBattleDistanceTreshold * MusicParameters.MaxBattleDistanceTreshold)
			{
				flag3 = true;
			}
			if (flag3)
			{
				float num5 = (float)num4 / 1000f;
				float num6 = MusicParameters.DefaultStartIntensity + num5 * MusicParameters.BattleSizeEffectOnStartIntensity + (MBRandom.RandomFloat - 0.5f) * (MusicParameters.RandomEffectMultiplierOnStartIntensity * 2f);
				MusicTheme musicTheme = (this._isSiegeBattle ? MBMusicManager.Current.GetSiegeTheme(base.Mission.MusicCulture.GetCultureCode()) : MBMusicManager.Current.GetBattleTheme(base.Mission.MusicCulture.GetCultureCode(), num4, ref this._isPaganBattle));
				MBMusicManager.Current.StartTheme(musicTheme, num6, false);
				this._battleState = MusicBattleMissionView.BattleState.Started;
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x000201D8 File Offset: 0x0001E3D8
		private void CheckForEnding()
		{
			if (Mission.Current.IsMissionEnding)
			{
				if (Mission.Current.MissionResult != null)
				{
					MusicTheme battleEndTheme = MBMusicManager.Current.GetBattleEndTheme(base.Mission.MusicCulture.GetCultureCode(), Mission.Current.MissionResult.PlayerVictory);
					MBMusicManager.Current.StartTheme(battleEndTheme, PsaiCore.Instance.GetPsaiInfo().currentIntensity, true);
					this._battleState = MusicBattleMissionView.BattleState.Ending;
					return;
				}
				MBMusicManager.Current.StartTheme(26, PsaiCore.Instance.GetPsaiInfo().currentIntensity, true);
				this._battleState = MusicBattleMissionView.BattleState.Ending;
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00020278 File Offset: 0x0001E478
		void IMusicHandler.OnUpdated(float dt)
		{
			if (this._battleState == MusicBattleMissionView.BattleState.Starting)
			{
				if (base.Mission.MusicCulture == null && Mission.Current.GetMissionBehavior<DeploymentHandler>() == null && this._missionAgentSpawnLogic.IsDeploymentOver)
				{
					KeyValuePair<BasicCultureObject, int> keyValuePair = new KeyValuePair<BasicCultureObject, int>(null, -1);
					Dictionary<BasicCultureObject, int> dictionary = new Dictionary<BasicCultureObject, int>();
					foreach (Team team in base.Mission.Teams)
					{
						foreach (Agent agent in team.ActiveAgents)
						{
							BasicCultureObject culture = agent.Character.Culture;
							if (culture != null && culture.IsMainCulture)
							{
								if (!dictionary.ContainsKey(agent.Character.Culture))
								{
									dictionary.Add(agent.Character.Culture, 0);
								}
								Dictionary<BasicCultureObject, int> dictionary2 = dictionary;
								BasicCultureObject culture2 = agent.Character.Culture;
								int num = dictionary2[culture2];
								dictionary2[culture2] = num + 1;
								if (dictionary[agent.Character.Culture] > keyValuePair.Value)
								{
									keyValuePair = new KeyValuePair<BasicCultureObject, int>(agent.Character.Culture, dictionary[agent.Character.Culture]);
								}
							}
						}
					}
					if (keyValuePair.Key != null)
					{
						base.Mission.MusicCulture = keyValuePair.Key;
					}
					else
					{
						base.Mission.MusicCulture = Game.Current.PlayerTroop.Culture;
					}
				}
				if (base.Mission.MusicCulture != null)
				{
					this.CheckForStarting();
				}
			}
			if (this._battleState == MusicBattleMissionView.BattleState.Started || this._battleState == MusicBattleMissionView.BattleState.TurnedOneSide)
			{
				this.CheckForEnding();
			}
			this.CheckIntensityFall();
		}

		// Token: 0x0400026F RID: 623
		private const float ChargeOrderIntensityIncreaseCooldownInSeconds = 60f;

		// Token: 0x04000270 RID: 624
		private MusicBattleMissionView.BattleState _battleState;

		// Token: 0x04000271 RID: 625
		private MissionAgentSpawnLogic _missionAgentSpawnLogic;

		// Token: 0x04000272 RID: 626
		private int[] _startingTroopCounts;

		// Token: 0x04000273 RID: 627
		private float _startingBattleRatio;

		// Token: 0x04000274 RID: 628
		private bool _isSiegeBattle;

		// Token: 0x04000275 RID: 629
		private bool _isPaganBattle;

		// Token: 0x04000276 RID: 630
		private MissionTime _nextPossibleTimeToIncreaseIntensityForChargeOrder;

		// Token: 0x020000C2 RID: 194
		private enum BattleState
		{
			// Token: 0x04000378 RID: 888
			Starting,
			// Token: 0x04000379 RID: 889
			Started,
			// Token: 0x0400037A RID: 890
			TurnedOneSide,
			// Token: 0x0400037B RID: 891
			Ending
		}
	}
}
