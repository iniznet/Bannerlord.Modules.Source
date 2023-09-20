using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	// Token: 0x0200005C RID: 92
	public abstract class MusicMissionActionComponent : MusicBaseComponent
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x0002062D File Offset: 0x0001E82D
		// (set) Token: 0x060003F0 RID: 1008 RVA: 0x00020635 File Offset: 0x0001E835
		protected MBList<Tuple<MBMusicManagerOld.MusicMood, string>> ActionTracks { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x0002063E File Offset: 0x0001E83E
		// (set) Token: 0x060003F2 RID: 1010 RVA: 0x00020646 File Offset: 0x0001E846
		protected MBList<MBMusicManagerOld.MusicMood> NegativeTracks { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x0002064F File Offset: 0x0001E84F
		// (set) Token: 0x060003F4 RID: 1012 RVA: 0x00020657 File Offset: 0x0001E857
		protected MBList<MBMusicManagerOld.MusicMood> PositiveTracks { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00020660 File Offset: 0x0001E860
		// (set) Token: 0x060003F6 RID: 1014 RVA: 0x00020668 File Offset: 0x0001E868
		protected Dictionary<string, MBMusicManagerOld.MusicMood> BattleWonTracks { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00020671 File Offset: 0x0001E871
		// (set) Token: 0x060003F8 RID: 1016 RVA: 0x00020679 File Offset: 0x0001E879
		protected MBList<MBMusicManagerOld.MusicMood> BattleLostTracks { get; set; }

		// Token: 0x060003F9 RID: 1017 RVA: 0x00020684 File Offset: 0x0001E884
		protected MusicMissionActionComponent()
		{
			this.TrackUpdateInterval = 5f;
			this.ActionTracks = new MBList<Tuple<MBMusicManagerOld.MusicMood, string>>();
			this.NegativeTracks = new MBList<MBMusicManagerOld.MusicMood>();
			this.PositiveTracks = new MBList<MBMusicManagerOld.MusicMood>();
			this.BattleWonTracks = new Dictionary<string, MBMusicManagerOld.MusicMood>();
			this.BattleLostTracks = new MBList<MBMusicManagerOld.MusicMood>();
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x000206E8 File Offset: 0x0001E8E8
		public override void PreInitialize()
		{
			base.PreInitialize();
			float currentTime = Mission.Current.CurrentTime;
			this._trackUpdateTimer = new Timer(currentTime, this.TrackUpdateInterval, true);
			this._intensityUpdateTimer = new Timer(currentTime, 1.5f, true);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0002072C File Offset: 0x0001E92C
		public override void Tick(float dt)
		{
			base.Tick(dt);
			bool flag = false;
			if (this._isFirstTick)
			{
				this.defenderSideAgentCount = Mission.Current.GetMemberCountOfSide(0);
				this.attackerSideAgentCount = Mission.Current.GetMemberCountOfSide(1);
				this._missionAgentSpawnLogic = Mission.Current.GetMissionBehavior<MissionAgentSpawnLogic>();
				if (this.attackerSideAgentCount == 0)
				{
					this.attackerSideAgentCount = 3;
				}
				if (this.defenderSideAgentCount == 0)
				{
					this.defenderSideAgentCount = 3;
				}
				float num = (float)(this.defenderSideAgentCount + this.attackerSideAgentCount) / 1000f;
				this._intensityCoefficient = ((num > 0.2f) ? 1f : ((num > 0.1f) ? 0.6666667f : ((num > 0.05f) ? 0.33333334f : (((double)num > 0.03) ? 0.16666667f : 0.1f))));
				this.TrackUpdateInterval = ((num > 0.2f) ? 5f : ((num > 0.1f) ? 4.5f : ((num > 0.05f) ? 4f : (((double)num > 0.03) ? 3.5f : 3f))));
				this._isFirstTick = false;
				this.AdjustCombatScale();
				flag = true;
			}
			this.HandleMoodChanges(flag);
			this.HandleIntensity();
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00020868 File Offset: 0x0001EA68
		private void HandleMoodChanges(bool forceUpdate = false)
		{
			if (this._allowMoodUpdating && this._trackUpdateTimer.Check(Mission.Current.CurrentTime))
			{
				if (Mission.Current.IsMissionEnding)
				{
					MBMusicManagerOld.MusicMood musicMood = this.HandleEndingTrackSelection();
					if (musicMood != this.CurrentMood)
					{
						this.CurrentMood = musicMood;
						MBMusicManagerOld.SetMood(this.CurrentMood, 0f, false, true);
						this._allowMoodUpdating = false;
						return;
					}
				}
				else
				{
					MBMusicManagerOld.MusicMood musicMood2;
					if (this.HandleFeedbackTrackSelection(out musicMood2))
					{
						this.CurrentMood = musicMood2;
						return;
					}
					this.CurrentMood = this.HandleNormalTrackSelection(forceUpdate);
				}
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x000208F0 File Offset: 0x0001EAF0
		protected virtual MBMusicManagerOld.MusicMood HandleEndingTrackSelection()
		{
			if (Mission.Current.MissionResult == null)
			{
				return this.CurrentMood;
			}
			return this.SelectEndingTrack(Mission.Current.MissionResult.PlayerVictory);
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0002091A File Offset: 0x0001EB1A
		protected virtual bool HandleFeedbackTrackSelection(out MBMusicManagerOld.MusicMood feedbackTrack)
		{
			feedbackTrack = -1;
			return false;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00020920 File Offset: 0x0001EB20
		protected virtual MBMusicManagerOld.MusicMood HandleNormalTrackSelection(bool forceUpdate = false)
		{
			int num = MBRandom.RandomInt(100);
			if (!forceUpdate && num <= 80)
			{
				return this.CurrentMood;
			}
			return this.SelectNewActionTrack();
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0002094C File Offset: 0x0001EB4C
		private void AdjustCombatScale()
		{
			int num = 0;
			foreach (Team team in Mission.Current.Teams)
			{
				num += team.ActiveAgents.Count;
			}
			float num2 = MBMath.ClampFloat((float)num / 200f, 0.05f, 0.95f);
			this._isCombatScaleMedium = num2 < MBRandom.RandomFloat;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x000209D4 File Offset: 0x0001EBD4
		protected MBMusicManagerOld.MusicMood SelectNewActionTrack()
		{
			MBMusicManagerOld.MusicMood musicMood = this.CurrentMood;
			MBList<MBMusicManagerOld.MusicMood> mblist = null;
			Mission mission = Mission.Current;
			if (((mission != null) ? mission.PlayerTeam : null) != null && mission.PlayerEnemyTeam != null)
			{
				float count = (float)mission.PlayerTeam.ActiveAgents.Count;
				int count2 = mission.PlayerEnemyTeam.ActiveAgents.Count;
				float num = count * 1f / (float)count2;
				bool flag = num > 3f;
				bool flag2 = num < 0.33333334f;
				if (flag && this.PositiveTracks.Count > 0)
				{
					mblist = this.PositiveTracks;
				}
				else if (flag2 && this.NegativeTracks.Count > 0)
				{
					mblist = this.NegativeTracks;
				}
			}
			if (mblist != null)
			{
				MBMusicManagerOld.MusicMood randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<MBMusicManagerOld.MusicMood>(mblist, (MBMusicManagerOld.MusicMood t) => t != this.CurrentMood);
				if (randomElementWithPredicate != null)
				{
					musicMood = randomElementWithPredicate;
				}
			}
			else
			{
				Tuple<MBMusicManagerOld.MusicMood, string> tuple = Extensions.GetRandomElementWithPredicate<Tuple<MBMusicManagerOld.MusicMood, string>>(this.ActionTracks, (Tuple<MBMusicManagerOld.MusicMood, string> at) => at.Item2 == (this._isCombatScaleMedium ? "medium" : "small") && at.Item1 != this.CurrentMood) ?? Extensions.GetRandomElementWithPredicate<Tuple<MBMusicManagerOld.MusicMood, string>>(this.ActionTracks, (Tuple<MBMusicManagerOld.MusicMood, string> at) => at.Item1 != this.CurrentMood);
				if (tuple != null)
				{
					musicMood = tuple.Item1;
				}
			}
			return musicMood;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00020ADC File Offset: 0x0001ECDC
		protected virtual MBMusicManagerOld.MusicMood SelectEndingTrack(bool victory)
		{
			if (!victory)
			{
				if (this.BattleLostTracks.Count != 0)
				{
					return Extensions.GetRandomElement<MBMusicManagerOld.MusicMood>(this.BattleLostTracks);
				}
				return this.CurrentMood;
			}
			else
			{
				if (this.BattleWonTracks.Count == 0)
				{
					return this.CurrentMood;
				}
				if (MBRandom.RandomInt(2) == 0)
				{
					return 11;
				}
				BasicCharacterObject playerTroop = Game.Current.PlayerTroop;
				if (playerTroop != null)
				{
					TextObject cultureName = playerTroop.Culture.Name;
					if (this.BattleWonTracks.ContainsKey(cultureName.ToString()))
					{
						return this.BattleWonTracks.FirstOrDefault((KeyValuePair<string, MBMusicManagerOld.MusicMood> x) => cultureName.ToString().Contains(x.Key)).Value;
					}
				}
				return 11;
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00020B8C File Offset: 0x0001ED8C
		private void HandleIntensity()
		{
			if (this.IsNextMoodChangeInstant)
			{
				if (this.CurrentMood != -1)
				{
					MBMusicManagerOld.SetMood(this.CurrentMood, MBMusicManagerOld.GetCurrentIntensity(), false, this.IsNextMoodChangeInstant);
				}
				else
				{
					MBMusicManagerOld.StopMusic(true);
				}
				this.IsNextMoodChangeInstant = false;
				this._intensityUpdateTimer.Reset(Mission.Current.CurrentTime);
				return;
			}
			if (this._allowMoodUpdating && this._intensityUpdateTimer.Check(Mission.Current.CurrentTime))
			{
				this.ComputeAndCacheAgentCountsAround();
				this._nextIntensity = this.CalculateIntensityTotal();
				this.ResetAccumulatedVariables();
				this._nextIntensity = MathF.Max(this._nextIntensity, 1E-05f);
				if (this.CurrentMood != -1)
				{
					this._nextIntensity += MBMusicManagerOld.GetCurrentIntensity();
					if (this._nextIntensity > 1f)
					{
						this._nextIntensity = 1f;
					}
					MBMusicManagerOld.SetMood(this.CurrentMood, this._nextIntensity, false, false);
					return;
				}
				MBMusicManagerOld.StopMusic(false);
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00020C84 File Offset: 0x0001EE84
		private float CalculateIntensityTotal()
		{
			float num = this.CalculateIntensityFromDamageToPlayer();
			float num2 = this.CalculateIntensityFromDamageFromPlayer();
			float num3 = this.CalculateIntensityFromNPCDeaths();
			float num4 = this.CalculateIntensityFromDamageToAlliedNPCs();
			float num5 = this.CalculateIntensityFromEnemiesAround();
			float num6 = this.CalculateIntensityFromEnemiesInDuelRange();
			float num7 = this.CalculateIntensityFromAlliesAround();
			float num8 = this.CalculateIntensityMisc();
			return MBMath.ClampFloat(num + num2 + num3 + num4 + num5 + num6 + num7 + num8, 0f, 1f);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00020CEC File Offset: 0x0001EEEC
		protected virtual float CalculateIntensityFromDamageToPlayer()
		{
			float num = 0f;
			if (Agent.Main != null && this._damageTakenByPlayerSinceLastUpdate > 1E-05f)
			{
				float num2 = (float)Agent.Main.Character.MaxHitPoints();
				num = this.CalculateIntensityValue(this._damageTakenByPlayerSinceLastUpdate, num2);
			}
			return 1.5f * num;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00020D3C File Offset: 0x0001EF3C
		protected virtual float CalculateIntensityFromDamageFromPlayer()
		{
			float num = (float)(this.attackerSideAgentCount * 100);
			float num2 = 0f;
			if (Mission.Current.PlayerTeam != null && Agent.Main != null && this._damageDealtByPlayerSinceLastUpdate > 1E-05f)
			{
				if (Mission.Current.PlayerTeam.IsAttacker)
				{
					num = (float)this.defenderSideAgentCount * Agent.Main.HealthLimit;
				}
				else
				{
					num = (float)this.attackerSideAgentCount * Agent.Main.HealthLimit;
				}
				num2 = this.CalculateIntensityValue(this._damageDealtByPlayerSinceLastUpdate, num);
			}
			return 1.5f * num2;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00020DC8 File Offset: 0x0001EFC8
		protected virtual float CalculateIntensityFromDamageToAlliedNPCs()
		{
			float num = 0f;
			float num2 = (float)(this.defenderSideAgentCount * 100);
			if (Mission.Current.PlayerTeam != null && Agent.Main != null)
			{
				if (Mission.Current.PlayerTeam.IsAttacker)
				{
					num2 = (float)this.attackerSideAgentCount * Agent.Main.HealthLimit;
				}
				else
				{
					num2 = (float)this.defenderSideAgentCount * Agent.Main.HealthLimit;
				}
				if (this._damageTakenByAlliedNPCsSinceLastUpdate > 0f)
				{
					num = this.CalculateIntensityValue(this._damageTakenByAlliedNPCsSinceLastUpdate, num2);
				}
			}
			return num;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00020E50 File Offset: 0x0001F050
		protected virtual float CalculateIntensityFromNPCDeaths()
		{
			float num = 0f;
			int num2 = this.defenderSideAgentCount + this.attackerSideAgentCount;
			if (this._npcDeathsSinceLastUpdate > 0)
			{
				num = this.CalculateIntensityValue((float)this._npcDeathsSinceLastUpdate, (float)num2);
			}
			return num;
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x00020E8B File Offset: 0x0001F08B
		private bool HasRetreatComponent
		{
			get
			{
				return Mission.Current.HasMissionBehavior<AgentHumanAILogic>();
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00020E98 File Offset: 0x0001F098
		private void ComputeAndCacheAgentCountsAround()
		{
			if (Agent.Main == null)
			{
				return;
			}
			float num = MathF.Max(MathF.Max(50f, 6f), 25f);
			bool flag = !this.HasRetreatComponent;
			this._enemyCountAround = 0;
			this._closeEnemyCountAround = 0;
			this._allyCountAround = 0;
			Vec3 position = Agent.Main.Position;
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, position.AsVec2, num, false);
			while (proximityMapSearchStruct.LastFoundAgent != null)
			{
				Agent lastFoundAgent = proximityMapSearchStruct.LastFoundAgent;
				if (lastFoundAgent.IsHuman && lastFoundAgent != Agent.Main && (flag || (lastFoundAgent.IsAIControlled && !lastFoundAgent.IsRetreating())))
				{
					float num2 = position.DistanceSquared(lastFoundAgent.Position);
					if (lastFoundAgent.IsEnemyOf(Agent.Main))
					{
						if (num2 <= 2500f)
						{
							this._enemyCountAround++;
							if (num2 <= 36f)
							{
								this._closeEnemyCountAround++;
							}
						}
					}
					else if (num2 <= 625f)
					{
						this._allyCountAround++;
					}
				}
				AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00020FBC File Offset: 0x0001F1BC
		protected virtual float CalculateIntensityFromEnemiesAround()
		{
			float num = 0f;
			int num2 = this.attackerSideAgentCount;
			if (Mission.Current.PlayerTeam != null)
			{
				if (Mission.Current.PlayerTeam.IsAttacker)
				{
					num2 = this.defenderSideAgentCount;
				}
				else
				{
					num2 = this.attackerSideAgentCount;
				}
			}
			int enemyCountAround = this._enemyCountAround;
			if (enemyCountAround != 0)
			{
				num = this.CalculateIntensityValue((float)enemyCountAround, (float)num2);
			}
			return num;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0002101C File Offset: 0x0001F21C
		protected virtual float CalculateIntensityFromEnemiesInDuelRange()
		{
			float num = 0f;
			int num2 = this.attackerSideAgentCount;
			if (Mission.Current.PlayerTeam != null && Mission.Current.PlayerTeam.IsAttacker)
			{
				num2 = this.defenderSideAgentCount;
			}
			int closeEnemyCountAround = this._closeEnemyCountAround;
			if (closeEnemyCountAround != 0)
			{
				num = this.CalculateIntensityValue((float)closeEnemyCountAround, (float)num2);
			}
			return num;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00021070 File Offset: 0x0001F270
		protected virtual float CalculateIntensityFromAlliesAround()
		{
			float num = 0f;
			int num2 = this.defenderSideAgentCount;
			if (Mission.Current.PlayerTeam != null && Mission.Current.PlayerTeam.IsAttacker)
			{
				num2 = this.attackerSideAgentCount;
			}
			int allyCountAround = this._allyCountAround;
			if (allyCountAround != 0)
			{
				num = this.CalculateIntensityValue((float)allyCountAround, (float)num2);
			}
			return num;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x000210C4 File Offset: 0x0001F2C4
		protected virtual float CalculateIntensityMisc()
		{
			return 0f;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000210CB File Offset: 0x0001F2CB
		private void ResetAccumulatedVariables()
		{
			this._damageDealtByPlayerSinceLastUpdate = 0f;
			this._damageTakenByPlayerSinceLastUpdate = 0f;
			this._damageTakenByAlliedNPCsSinceLastUpdate = 0f;
			this._npcDeathsSinceLastUpdate = 0;
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x000210F5 File Offset: 0x0001F2F5
		private float CalculateIntensityValue(float numerator, float denominator)
		{
			return numerator / denominator * (0.09527f * this._intensityCoefficient);
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00021108 File Offset: 0x0001F308
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, damage, attackerWeapon);
			if (affectorAgent == Agent.Main)
			{
				this._damageDealtByPlayerSinceLastUpdate += (float)damage;
				return;
			}
			if (affectedAgent == Agent.Main)
			{
				this._damageTakenByPlayerSinceLastUpdate += (float)damage;
				return;
			}
			if (Agent.Main != null && affectedAgent.Team == Agent.Main.Team && affectedAgent.Position.DistanceSquared(Agent.Main.Position) <= 3600f)
			{
				this._damageTakenByAlliedNPCsSinceLastUpdate += (float)damage;
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00021198 File Offset: 0x0001F398
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (Agent.Main != null && Agent.Main.Position.DistanceSquared(affectedAgent.Position) <= 10000f)
			{
				this._npcDeathsSinceLastUpdate++;
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000211E4 File Offset: 0x0001F3E4
		public override MusicPriority GetPriority()
		{
			return MusicPriority.MissionHigh;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x000211E7 File Offset: 0x0001F3E7
		public override bool IsActive()
		{
			return true;
		}

		// Token: 0x0400027F RID: 639
		private const float KillRangeCheck = 100f;

		// Token: 0x04000280 RID: 640
		private const float HitRangeCheck = 60f;

		// Token: 0x04000281 RID: 641
		private const float EnemiesRangeCheck = 50f;

		// Token: 0x04000282 RID: 642
		private const float AlliesRangeCheck = 25f;

		// Token: 0x04000283 RID: 643
		private const float DuelRangeCheck = 6f;

		// Token: 0x04000284 RID: 644
		private const float IntensityUpdateInterval = 1.5f;

		// Token: 0x04000285 RID: 645
		protected const float IntensityZeroAlternative = 1E-05f;

		// Token: 0x04000286 RID: 646
		protected float TrackUpdateInterval;

		// Token: 0x0400028C RID: 652
		private Timer _trackUpdateTimer;

		// Token: 0x0400028D RID: 653
		private Timer _intensityUpdateTimer;

		// Token: 0x0400028E RID: 654
		private bool _allowMoodUpdating = true;

		// Token: 0x0400028F RID: 655
		protected bool IsNextMoodChangeInstant;

		// Token: 0x04000290 RID: 656
		private float _damageDealtByPlayerSinceLastUpdate;

		// Token: 0x04000291 RID: 657
		private float _damageTakenByPlayerSinceLastUpdate;

		// Token: 0x04000292 RID: 658
		private float _damageTakenByAlliedNPCsSinceLastUpdate;

		// Token: 0x04000293 RID: 659
		private int _npcDeathsSinceLastUpdate;

		// Token: 0x04000294 RID: 660
		private float _nextIntensity;

		// Token: 0x04000295 RID: 661
		private int _enemyCountAround;

		// Token: 0x04000296 RID: 662
		private int _closeEnemyCountAround;

		// Token: 0x04000297 RID: 663
		private int _allyCountAround;

		// Token: 0x04000298 RID: 664
		protected int attackerSideAgentCount;

		// Token: 0x04000299 RID: 665
		protected int defenderSideAgentCount;

		// Token: 0x0400029A RID: 666
		private float _intensityCoefficient;

		// Token: 0x0400029B RID: 667
		private bool _isCombatScaleMedium;

		// Token: 0x0400029C RID: 668
		private bool _isFirstTick = true;

		// Token: 0x0400029D RID: 669
		private MissionAgentSpawnLogic _missionAgentSpawnLogic;
	}
}
