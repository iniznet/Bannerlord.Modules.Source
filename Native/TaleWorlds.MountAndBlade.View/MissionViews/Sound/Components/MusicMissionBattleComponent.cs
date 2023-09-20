using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	public class MusicMissionBattleComponent : MusicMissionActionComponent
	{
		public MusicMissionBattleComponent()
		{
		}

		public MusicMissionBattleComponent(BasicCultureObject playerFactionCulture)
		{
			this._playerFactionCulture = playerFactionCulture;
		}

		public override void PreInitialize()
		{
			base.PreInitialize();
			base.ActionTracks.Add(Tuple.Create<MBMusicManagerOld.MusicMood, string>(12, "small"));
			base.ActionTracks.Add(Tuple.Create<MBMusicManagerOld.MusicMood, string>(10, "medium"));
			base.NegativeTracks.Add(9);
			base.PositiveTracks.Add(11);
			base.BattleLostTracks.Add(26);
			base.BattleWonTracks.Add("battania", 17);
			base.BattleWonTracks.Add("sturgia", 19);
			base.BattleWonTracks.Add("khuzait", 21);
			base.BattleWonTracks.Add("empire", 23);
			base.BattleWonTracks.Add("vlandia", 25);
			base.BattleWonTracks.Add("aserai", 4);
			this._battleProcess = MusicMissionBattleComponent.BattleProcess.Initialized;
			MBMusicManagerOld.StopMusic(true);
		}

		private bool CheckIfBattleHasStarted()
		{
			return Mission.GetUnderAttackTypeOfAgents(Mission.Current.Agents, 3f) > 0;
		}

		protected override MBMusicManagerOld.MusicMood HandleNormalTrackSelection(bool forceUpdate = false)
		{
			if (this._battleProcess == MusicMissionBattleComponent.BattleProcess.Initialized)
			{
				this._scale = (float)(this.defenderSideAgentCount + this.attackerSideAgentCount) / 1000f;
				if (!this._hasMissionStarted)
				{
					this.AdjustCooldownByCombatSize();
					this._hasMissionStarted = this.CheckIfBattleHasStarted();
					if (this._hasMissionStarted || MissionTime.Now.ToSeconds > (double)this._initialCooldown)
					{
						this._battleProcess = MusicMissionBattleComponent.BattleProcess.Started;
					}
				}
			}
			if (this._battleProcess == MusicMissionBattleComponent.BattleProcess.Started)
			{
				this._battleProcess = MusicMissionBattleComponent.BattleProcess.Ongoing;
				return this.BattleNormalTrackSelection();
			}
			if (this._battleProcess == MusicMissionBattleComponent.BattleProcess.Ongoing)
			{
				return this.BattleNormalTrackSelection();
			}
			if (this._battleProcess == MusicMissionBattleComponent.BattleProcess.SideAdvantage)
			{
				if (Mission.Current.IsMissionEnding)
				{
					this._battleProcess = MusicMissionBattleComponent.BattleProcess.Ended;
				}
				return this.BattleNormalTrackSelection();
			}
			return -1;
		}

		private MBMusicManagerOld.MusicMood BattleFinalizingTrackSelection(BattleSideEnum side)
		{
			if (side == Mission.Current.PlayerTeam.Side)
			{
				return 9;
			}
			return 11;
		}

		private MBMusicManagerOld.MusicMood BattleNormalTrackSelection()
		{
			if (this._normalTrackFirstTick && MBRandom.RandomFloat > 0.3f)
			{
				this._normalTrackFlag = true;
			}
			this._normalTrackFirstTick = false;
			if (this._playerFactionCulture != null && this._normalTrackFlag)
			{
				string stringId = this._playerFactionCulture.StringId;
				if (stringId.Equals("khuzait") || stringId.Equals("sturgia") || stringId.Equals("aserai"))
				{
					return 27;
				}
			}
			if (this._scale > 0.15f)
			{
				return 10;
			}
			return 12;
		}

		protected unsafe override bool HandleFeedbackTrackSelection(out MBMusicManagerOld.MusicMood feedbackTrack)
		{
			Mission mission = Mission.Current;
			Func<MBList<Formation>, MBList<Formation>, bool> func = delegate(MBList<Formation> local, MBList<Formation> global)
			{
				IEnumerable<Formation> enumerable = global.Where(delegate(Formation f)
				{
					if (f.CountOfUnits > 0)
					{
						MovementOrder movementOrder = *f.GetReadonlyMovementOrderReference();
						return movementOrder.OrderType == 10;
					}
					return false;
				}).Except(local);
				if (enumerable.Any<Formation>())
				{
					this._retreatedEnemyFormations.AddRange(enumerable);
					return true;
				}
				return false;
			};
			bool flag = func(this._retreatedEnemyFormations, mission.PlayerEnemyTeam.FormationsIncludingEmpty);
			if (func(this._retreatedPlayerFormations, mission.PlayerTeam.FormationsIncludingEmpty))
			{
				this.PlayShockEvent(false);
			}
			else if (flag)
			{
				this.PlayShockEvent(true);
			}
			return base.HandleFeedbackTrackSelection(out feedbackTrack);
		}

		protected override MBMusicManagerOld.MusicMood SelectEndingTrack(bool victory)
		{
			if (!victory)
			{
				return 26;
			}
			if (this._playerFactionCulture != null)
			{
				string stringId = this._playerFactionCulture.StringId;
				if (stringId.Equals("battania"))
				{
					return 17;
				}
				if (stringId.Equals("sturgia"))
				{
					return 19;
				}
				if (stringId.Equals("khuzait"))
				{
					return 21;
				}
				if (stringId.Equals("empire"))
				{
					return 23;
				}
				if (stringId.Equals("vlandia"))
				{
					return 25;
				}
				if (stringId.Equals("aserai"))
				{
					return 4;
				}
			}
			return 11;
		}

		private void AdjustCooldownByCombatSize()
		{
			this._initialCooldown = ((this._scale > 0.2f) ? 25 : ((this._scale > 0.1f) ? 23 : 20));
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			bool flag = affectedAgent == Agent.Main || affectedAgent == Mission.Current.PlayerTeam.Leader;
			bool flag2 = affectedAgent == Mission.Current.PlayerEnemyTeam.Leader;
			if (flag)
			{
				this.PlayShockEvent(false);
				return;
			}
			if (flag2)
			{
				this.PlayShockEvent(true);
			}
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, damage, attackerWeapon);
			if (affectedAgent == Agent.Main && damage > 50)
			{
				this.PlayShockEvent(false);
			}
		}

		public override void OnEntityRemoved(GameEntity entity)
		{
			base.OnEntityRemoved(entity);
			IEnumerable<ScriptComponentBehavior> scriptComponents = entity.GetScriptComponents();
			if (scriptComponents.Any((ScriptComponentBehavior sc) => sc is CastleGate))
			{
				this.PlayShockEvent(Mission.Current.PlayerTeam.Side == 1);
				return;
			}
			if (scriptComponents.Any((ScriptComponentBehavior sc) => sc is BatteringRam || sc is SiegeTower))
			{
				this.PlayShockEvent(Mission.Current.PlayerTeam.Side != 1);
			}
		}

		private void PlayShockEvent(bool isPositive)
		{
			if (this.CurrentMood != 15 && this.CurrentMood != 14)
			{
				if (isPositive)
				{
					this.CurrentMood = 14;
				}
				else
				{
					this.CurrentMood = 15;
				}
				MBMusicManagerOld.SetMood(this.CurrentMood, 0f, false, true);
			}
		}

		private MusicMissionBattleComponent.BattleProcess _battleProcess;

		private int _initialCooldown;

		private BasicCultureObject _playerFactionCulture;

		private bool _normalTrackFlag;

		private bool _normalTrackFirstTick = true;

		private float _scale;

		private bool _hasMissionStarted;

		private readonly MBList<Formation> _retreatedEnemyFormations = new MBList<Formation>();

		private readonly MBList<Formation> _retreatedPlayerFormations = new MBList<Formation>();

		private enum BattleProcess
		{
			Initialized,
			Started,
			Ongoing,
			SideAdvantage,
			Ended
		}
	}
}
