using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	// Token: 0x0200005D RID: 93
	public class MusicMissionBattleComponent : MusicMissionActionComponent
	{
		// Token: 0x06000418 RID: 1048 RVA: 0x00021241 File Offset: 0x0001F441
		public MusicMissionBattleComponent()
		{
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00021266 File Offset: 0x0001F466
		public MusicMissionBattleComponent(BasicCultureObject playerFactionCulture)
		{
			this._playerFactionCulture = playerFactionCulture;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00021294 File Offset: 0x0001F494
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

		// Token: 0x0600041B RID: 1051 RVA: 0x00021374 File Offset: 0x0001F574
		private bool CheckIfBattleHasStarted()
		{
			return Mission.GetUnderAttackTypeOfAgents(Mission.Current.Agents, 3f) > 0;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00021390 File Offset: 0x0001F590
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

		// Token: 0x0600041D RID: 1053 RVA: 0x00021449 File Offset: 0x0001F649
		private MBMusicManagerOld.MusicMood BattleFinalizingTrackSelection(BattleSideEnum side)
		{
			if (side == Mission.Current.PlayerTeam.Side)
			{
				return 9;
			}
			return 11;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00021464 File Offset: 0x0001F664
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

		// Token: 0x0600041F RID: 1055 RVA: 0x000214EC File Offset: 0x0001F6EC
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

		// Token: 0x06000420 RID: 1056 RVA: 0x00021558 File Offset: 0x0001F758
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

		// Token: 0x06000421 RID: 1057 RVA: 0x000215E0 File Offset: 0x0001F7E0
		private void AdjustCooldownByCombatSize()
		{
			this._initialCooldown = ((this._scale > 0.2f) ? 25 : ((this._scale > 0.1f) ? 23 : 20));
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0002160C File Offset: 0x0001F80C
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

		// Token: 0x06000423 RID: 1059 RVA: 0x00021668 File Offset: 0x0001F868
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, damage, attackerWeapon);
			if (affectedAgent == Agent.Main && damage > 50)
			{
				this.PlayShockEvent(false);
			}
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0002168C File Offset: 0x0001F88C
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

		// Token: 0x06000425 RID: 1061 RVA: 0x00021729 File Offset: 0x0001F929
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

		// Token: 0x0400029E RID: 670
		private MusicMissionBattleComponent.BattleProcess _battleProcess;

		// Token: 0x0400029F RID: 671
		private int _initialCooldown;

		// Token: 0x040002A0 RID: 672
		private BasicCultureObject _playerFactionCulture;

		// Token: 0x040002A1 RID: 673
		private bool _normalTrackFlag;

		// Token: 0x040002A2 RID: 674
		private bool _normalTrackFirstTick = true;

		// Token: 0x040002A3 RID: 675
		private float _scale;

		// Token: 0x040002A4 RID: 676
		private bool _hasMissionStarted;

		// Token: 0x040002A5 RID: 677
		private readonly MBList<Formation> _retreatedEnemyFormations = new MBList<Formation>();

		// Token: 0x040002A6 RID: 678
		private readonly MBList<Formation> _retreatedPlayerFormations = new MBList<Formation>();

		// Token: 0x020000C5 RID: 197
		private enum BattleProcess
		{
			// Token: 0x04000380 RID: 896
			Initialized,
			// Token: 0x04000381 RID: 897
			Started,
			// Token: 0x04000382 RID: 898
			Ongoing,
			// Token: 0x04000383 RID: 899
			SideAdvantage,
			// Token: 0x04000384 RID: 900
			Ended
		}
	}
}
