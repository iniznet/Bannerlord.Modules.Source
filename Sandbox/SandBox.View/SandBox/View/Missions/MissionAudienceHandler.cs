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
	public class MissionAudienceHandler : MissionView
	{
		public MissionAudienceHandler(float density)
		{
			this._density = density;
		}

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

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectorAgent != null && affectorAgent.IsHuman && affectedAgent.IsHuman)
			{
				this.Cheer(false);
			}
		}

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

		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			if (oldMissionMode == 2 && Mission.Current.Mode == null && Agent.Main != null && Agent.Main.IsActive())
			{
				this.Cheer(true);
			}
		}

		public override void OnMissionScreenFinalize()
		{
			SoundEvent ambientSoundEvent = this._ambientSoundEvent;
			if (ambientSoundEvent == null)
			{
				return;
			}
			ambientSoundEvent.Release();
		}

		private const int GapBetweenCheerSmallInSeconds = 10;

		private const int GapBetweenCheerMedium = 4;

		private float _minChance;

		private float _maxChance;

		private float _minDist;

		private float _maxDist;

		private float _minHeight;

		private float _maxHeight;

		private List<GameEntity> _audienceMidPoints;

		private List<KeyValuePair<GameEntity, float>> _audienceList;

		private readonly float _density;

		private GameEntity _arenaSoundEntity;

		private SoundEvent _ambientSoundEvent;

		private MissionTime _lastOneShotSoundEventStarted;

		private bool _allOneShotSoundEventsAreDisabled;

		private ActionIndexCache _spectatorAction = ActionIndexCache.Create("act_arena_spectator");
	}
}
