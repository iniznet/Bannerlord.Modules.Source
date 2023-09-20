using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	public class MusicMissionView : MissionView
	{
		public MusicMissionView(params MusicBaseComponent[] musicComponents)
		{
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MusicComponentHolder.Instance.Initialize();
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionConfigurationChange));
		}

		public override void OnRemoveBehavior()
		{
			MusicComponentHolder.Instance.RemoveComponents(this._components);
			base.OnRemoveBehavior();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionConfigurationChange));
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			MusicComponentHolder.Instance.Tick(dt);
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, ref attackerWeapon, ref blow, ref attackCollisionData);
			MusicComponentHolder.Instance.OnAgentHit(affectedAgent, affectorAgent, blow.InflictedDamage, attackerWeapon);
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			MusicComponentHolder.Instance.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		}

		public override void OnEntityRemoved(GameEntity entity)
		{
			base.OnEntityRemoved(entity);
			MusicComponentHolder.Instance.OnEntityRemoved(entity);
		}

		public void OnNativeOptionConfigurationChange(NativeOptions.NativeOptionsType optionType)
		{
			if (optionType == 2)
			{
				float config = NativeOptions.GetConfig(2);
				MusicComponentHolder.Instance.OnMusicVolumeChanged(config);
			}
		}

		private readonly List<MusicBaseComponent> _components = new List<MusicBaseComponent>();
	}
}
