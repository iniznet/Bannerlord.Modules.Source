using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x02000012 RID: 18
	public class MissionAudienceHandler : MissionView
	{
		// Token: 0x06000062 RID: 98 RVA: 0x0000488D File Offset: 0x00002A8D
		public MissionAudienceHandler(float density)
		{
			this._density = density;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000048AC File Offset: 0x00002AAC
		public override void EarlyStart()
		{
			this._allOneShotSoundEventsAreDisabled = true;
			this._audienceMidPoints = base.Mission.Scene.FindEntitiesWithTag("audience_mid_point").ToList<GameEntity>();
			this._arenaSoundEntity = base.Mission.Scene.FindEntityWithTag("arena_sound");
			this._audienceList = new List<KeyValuePair<GameEntity, float>>();
			if (this._audienceMidPoints.Count > 0)
			{
				this.OnInit();
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000491C File Offset: 0x00002B1C
		public void OnInit()
		{
			this._minChance = MathF.Max(this._density - 0.5f, 0f);
			this._maxChance = this._density;
			this.GetAudienceEntities();
			this.SpawnAudienceAgents();
			this._lastOneShotSoundEventStarted = MissionTime.Zero;
			this._allOneShotSoundEventsAreDisabled = false;
			this._ambientSoundEvent = SoundManager.CreateEvent("event:/mission/ambient/detail/arena/arena", base.Mission.Scene);
			this._ambientSoundEvent.Play();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004996 File Offset: 0x00002B96
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectorAgent != null && affectorAgent.IsHuman && affectedAgent.IsHuman)
			{
				this.Cheer(false);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000049B4 File Offset: 0x00002BB4
		private void Cheer(bool onEnd = false)
		{
			if (!this._allOneShotSoundEventsAreDisabled)
			{
				string text = null;
				if (onEnd)
				{
					text = "event:/mission/ambient/detail/arena/cheer_big";
					this._allOneShotSoundEventsAreDisabled = true;
				}
				else if (this._lastOneShotSoundEventStarted.ElapsedSeconds > 4f && this._lastOneShotSoundEventStarted.ElapsedSeconds < 10f)
				{
					text = "event:/mission/ambient/detail/arena/cheer_medium";
				}
				else if (this._lastOneShotSoundEventStarted.ElapsedSeconds > 10f)
				{
					text = "event:/mission/ambient/detail/arena/cheer_small";
				}
				if (text != null)
				{
					Vec3 vec = ((this._arenaSoundEntity != null) ? this._arenaSoundEntity.GlobalPosition : (this._audienceMidPoints.Any<GameEntity>() ? Extensions.GetRandomElement<GameEntity>(this._audienceMidPoints).GlobalPosition : Vec3.Zero));
					SoundManager.StartOneShotEvent(text, ref vec);
					this._lastOneShotSoundEventStarted = MissionTime.Now;
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004A7C File Offset: 0x00002C7C
		private void GetAudienceEntities()
		{
			this._maxDist = 0f;
			this._minDist = float.MaxValue;
			this._maxHeight = 0f;
			this._minHeight = float.MaxValue;
			foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("audience"))
			{
				float distanceSquareToArena = this.GetDistanceSquareToArena(gameEntity);
				this._maxDist = ((distanceSquareToArena > this._maxDist) ? distanceSquareToArena : this._maxDist);
				this._minDist = ((distanceSquareToArena < this._minDist) ? distanceSquareToArena : this._minDist);
				float z = gameEntity.GetFrame().origin.z;
				this._maxHeight = ((z > this._maxHeight) ? z : this._maxHeight);
				this._minHeight = ((z < this._minHeight) ? z : this._minHeight);
				this._audienceList.Add(new KeyValuePair<GameEntity, float>(gameEntity, distanceSquareToArena));
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004B98 File Offset: 0x00002D98
		private float GetDistanceSquareToArena(GameEntity audienceEntity)
		{
			float num = float.MaxValue;
			foreach (GameEntity gameEntity in this._audienceMidPoints)
			{
				float num2 = gameEntity.GlobalPosition.DistanceSquared(audienceEntity.GlobalPosition);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004C04 File Offset: 0x00002E04
		private CharacterObject GetRandomAudienceCharacterToSpawn()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			CharacterObject characterObject = MBRandom.ChooseWeighted<CharacterObject>(new List<ValueTuple<CharacterObject, float>>
			{
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Townswoman, 0.2f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Townsman, 0.2f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Armorer, 0.1f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Merchant, 0.1f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Musician, 0.1f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Weaponsmith, 0.1f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.RansomBroker, 0.1f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.Barber, 0.05f),
				new ValueTuple<CharacterObject, float>(currentSettlement.Culture.FemaleDancer, 0.05f)
			});
			if (characterObject == null)
			{
				characterObject = ((MBRandom.RandomFloat < 0.65f) ? currentSettlement.Culture.Townsman : currentSettlement.Culture.Townswoman);
			}
			return characterObject;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004D40 File Offset: 0x00002F40
		private void SpawnAudienceAgents()
		{
			for (int i = this._audienceList.Count - 1; i >= 0; i--)
			{
				KeyValuePair<GameEntity, float> keyValuePair = this._audienceList[i];
				float num = this._minChance + (1f - (keyValuePair.Value - this._minDist) / (this._maxDist - this._minDist)) * (this._maxChance - this._minChance);
				float num2 = this._minChance + (1f - MathF.Pow((keyValuePair.Key.GetFrame().origin.z - this._minHeight) / (this._maxHeight - this._minHeight), 2f)) * (this._maxChance - this._minChance);
				float num3 = num * 0.4f + num2 * 0.6f;
				if (MBRandom.RandomFloat < num3)
				{
					MatrixFrame globalFrame = keyValuePair.Key.GetGlobalFrame();
					CharacterObject randomAudienceCharacterToSpawn = this.GetRandomAudienceCharacterToSpawn();
					AgentBuildData agentBuildData = new AgentBuildData(randomAudienceCharacterToSpawn).InitialPosition(ref globalFrame.origin);
					Vec2 vec = new Vec2(-globalFrame.rotation.f.AsVec2.x, -globalFrame.rotation.f.AsVec2.y);
					AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).TroopOrigin(new SimpleAgentOrigin(randomAudienceCharacterToSpawn, -1, null, default(UniqueTroopDescriptor))).Team(Team.Invalid)
						.CanSpawnOutsideOfMissionBoundary(true);
					Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
					MBAnimation.PrefetchAnimationClip(agent.ActionSet, this._spectatorAction);
					agent.SetActionChannel(0, this._spectatorAction, true, 0UL, 0f, MBRandom.RandomFloatRanged(0.75f, 1f), -0.2f, 0.4f, MBRandom.RandomFloatRanged(0.01f, 1f), false, -0.2f, 0, true);
					agent.Controller = 0;
					agent.ToggleInvulnerable();
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004F19 File Offset: 0x00003119
		public override void OnMissionTick(float dt)
		{
			if (this._audienceMidPoints == null)
			{
				return;
			}
			if (base.Mission.MissionEnded)
			{
				this.Cheer(true);
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004F38 File Offset: 0x00003138
		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			if (oldMissionMode == 2 && Mission.Current.Mode == null && Agent.Main != null && Agent.Main.IsActive())
			{
				this.Cheer(true);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004F64 File Offset: 0x00003164
		public override void OnMissionScreenFinalize()
		{
			SoundEvent ambientSoundEvent = this._ambientSoundEvent;
			if (ambientSoundEvent == null)
			{
				return;
			}
			ambientSoundEvent.Release();
		}

		// Token: 0x0400002A RID: 42
		private const int GapBetweenCheerSmallInSeconds = 10;

		// Token: 0x0400002B RID: 43
		private const int GapBetweenCheerMedium = 4;

		// Token: 0x0400002C RID: 44
		private float _minChance;

		// Token: 0x0400002D RID: 45
		private float _maxChance;

		// Token: 0x0400002E RID: 46
		private float _minDist;

		// Token: 0x0400002F RID: 47
		private float _maxDist;

		// Token: 0x04000030 RID: 48
		private float _minHeight;

		// Token: 0x04000031 RID: 49
		private float _maxHeight;

		// Token: 0x04000032 RID: 50
		private List<GameEntity> _audienceMidPoints;

		// Token: 0x04000033 RID: 51
		private List<KeyValuePair<GameEntity, float>> _audienceList;

		// Token: 0x04000034 RID: 52
		private readonly float _density;

		// Token: 0x04000035 RID: 53
		private GameEntity _arenaSoundEntity;

		// Token: 0x04000036 RID: 54
		private SoundEvent _ambientSoundEvent;

		// Token: 0x04000037 RID: 55
		private MissionTime _lastOneShotSoundEventStarted;

		// Token: 0x04000038 RID: 56
		private bool _allOneShotSoundEventsAreDisabled;

		// Token: 0x04000039 RID: 57
		private ActionIndexCache _spectatorAction = ActionIndexCache.Create("act_arena_spectator");
	}
}
