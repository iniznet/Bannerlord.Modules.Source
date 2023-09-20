using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	public abstract class MusicBaseComponent
	{
		protected float CurrentVolume
		{
			get
			{
				return this._volume;
			}
		}

		public virtual void PreInitialize()
		{
		}

		public virtual void Initialize()
		{
			this._volumeInOptions = NativeOptions.GetConfig(2);
		}

		public virtual void Tick(float dt)
		{
		}

		public virtual void OnActived()
		{
		}

		public virtual void OnDeactivated()
		{
		}

		public virtual void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
		}

		public virtual void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
		}

		public virtual void OnEntityRemoved(GameEntity entity)
		{
		}

		public virtual void OnMusicVolumeChanged(float newVolume)
		{
			float num = this._volumeInOptions - newVolume;
			this._volume += num;
			this._volume = MBMath.ClampFloat(this._volume, 0f, 1f);
			this._volumeInOptions = newVolume;
		}

		public abstract bool IsActive();

		public abstract MusicPriority GetPriority();

		protected MBMusicManagerOld.MusicMood CurrentMood = -1;

		private float _volumeInOptions;

		private float _volume;
	}
}
