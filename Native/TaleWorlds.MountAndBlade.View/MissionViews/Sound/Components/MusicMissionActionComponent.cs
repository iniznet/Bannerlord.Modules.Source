using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	public abstract class MusicMissionActionComponent : MusicBaseComponent
	{
		protected MBList<Tuple<MBMusicManagerOld.MusicMood, string>> ActionTracks { get; set; }

		protected MBList<MBMusicManagerOld.MusicMood> NegativeTracks { get; set; }

		protected MBList<MBMusicManagerOld.MusicMood> PositiveTracks { get; set; }

		protected Dictionary<string, MBMusicManagerOld.MusicMood> BattleWonTracks { get; set; }

		protected MBList<MBMusicManagerOld.MusicMood> BattleLostTracks { get; set; }

		protected MusicMissionActionComponent()
		{
			this.TrackUpdateInterval = 5f;
			this.ActionTracks = new MBList<Tuple<MBMusicManagerOld.MusicMood, string>>();
			this.NegativeTracks = new MBList<MBMusicManagerOld.MusicMood>();
			this.PositiveTracks = new MBList<MBMusicManagerOld.MusicMood>();
			this.BattleWonTracks = new Dictionary<string, MBMusicManagerOld.MusicMood>();
			this.BattleLostTracks = new MBList<MBMusicManagerOld.MusicMood>();
		}

		public override void PreInitialize()
		{
			base.PreInitialize();
			float currentTime = Mission.Current.CurrentTime;
			this._trackUpdateTimer = new Timer(currentTime, this.TrackUpdateInterval, true);
			this._intensityUpdateTimer = new Timer(currentTime, 1.5f, true);
		}

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

		protected virtual MBMusicManagerOld.MusicMood HandleEndingTrackSelection()
		{
			if (Mission.Current.MissionResult == null)
			{
				return this.CurrentMood;
			}
			return this.SelectEndingTrack(Mission.Current.MissionResult.PlayerVictory);
		}

		protected virtual bool HandleFeedbackTrackSelection(out MBMusicManagerOld.MusicMood feedbackTrack)
		{
			feedbackTrack = -1;
			return false;
		}

		protected virtual MBMusicManagerOld.MusicMood HandleNormalTrackSelection(bool forceUpdate = false)
		{
			int num = MBRandom.RandomInt(100);
			if (!forceUpdate && num <= 80)
			{
				return this.CurrentMood;
			}
			return this.SelectNewActionTrack();
		}

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

		private bool HasRetreatComponent
		{
			get
			{
				return Mission.Current.HasMissionBehavior<AgentHumanAILogic>();
			}
		}

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

		protected virtual float CalculateIntensityMisc()
		{
			return 0f;
		}

		private void ResetAccumulatedVariables()
		{
			this._damageDealtByPlayerSinceLastUpdate = 0f;
			this._damageTakenByPlayerSinceLastUpdate = 0f;
			this._damageTakenByAlliedNPCsSinceLastUpdate = 0f;
			this._npcDeathsSinceLastUpdate = 0;
		}

		private float CalculateIntensityValue(float numerator, float denominator)
		{
			return numerator / denominator * (0.09527f * this._intensityCoefficient);
		}

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

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (Agent.Main != null && Agent.Main.Position.DistanceSquared(affectedAgent.Position) <= 10000f)
			{
				this._npcDeathsSinceLastUpdate++;
			}
		}

		public override MusicPriority GetPriority()
		{
			return MusicPriority.MissionHigh;
		}

		public override bool IsActive()
		{
			return true;
		}

		private const float KillRangeCheck = 100f;

		private const float HitRangeCheck = 60f;

		private const float EnemiesRangeCheck = 50f;

		private const float AlliesRangeCheck = 25f;

		private const float DuelRangeCheck = 6f;

		private const float IntensityUpdateInterval = 1.5f;

		protected const float IntensityZeroAlternative = 1E-05f;

		protected float TrackUpdateInterval;

		private Timer _trackUpdateTimer;

		private Timer _intensityUpdateTimer;

		private bool _allowMoodUpdating = true;

		protected bool IsNextMoodChangeInstant;

		private float _damageDealtByPlayerSinceLastUpdate;

		private float _damageTakenByPlayerSinceLastUpdate;

		private float _damageTakenByAlliedNPCsSinceLastUpdate;

		private int _npcDeathsSinceLastUpdate;

		private float _nextIntensity;

		private int _enemyCountAround;

		private int _closeEnemyCountAround;

		private int _allyCountAround;

		protected int attackerSideAgentCount;

		protected int defenderSideAgentCount;

		private float _intensityCoefficient;

		private bool _isCombatScaleMedium;

		private bool _isFirstTick = true;

		private MissionAgentSpawnLogic _missionAgentSpawnLogic;
	}
}
