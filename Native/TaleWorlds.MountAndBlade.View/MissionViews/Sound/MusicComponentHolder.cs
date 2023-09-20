using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	public class MusicComponentHolder
	{
		public static MusicComponentHolder Instance
		{
			get
			{
				return MusicComponentHolder.instance;
			}
		}

		private MusicComponentHolder()
		{
			this._musicComponents = new List<MusicBaseComponent>();
		}

		private protected MusicBaseComponent PreviousMusic { protected get; private set; }

		private protected MusicBaseComponent CurrentMusic { protected get; private set; }

		public void AddComponents(List<MusicBaseComponent> musicComponents)
		{
			foreach (MusicBaseComponent musicBaseComponent in musicComponents)
			{
				this._musicComponents.Add(musicBaseComponent);
				musicBaseComponent.PreInitialize();
			}
		}

		public void RemoveComponents(List<MusicBaseComponent> musicComponents)
		{
			this._musicComponents.RemoveAll((MusicBaseComponent x) => musicComponents.Contains(x));
		}

		public void Initialize()
		{
			foreach (MusicBaseComponent musicBaseComponent in this._musicComponents)
			{
				musicBaseComponent.Initialize();
			}
		}

		public void Tick(float dt)
		{
			this.CurrentMusic = (from x in this._musicComponents
				where x.IsActive()
				orderby (int)x.GetPriority() descending
				select x).FirstOrDefault<MusicBaseComponent>();
			if (this.PreviousMusic != null && this.PreviousMusic != this.CurrentMusic)
			{
				this.PreviousMusic.OnDeactivated();
			}
			if (this.CurrentMusic != null)
			{
				if (this.CurrentMusic != this.PreviousMusic)
				{
					this.CurrentMusic.OnActived();
				}
				this.CurrentMusic.Tick(dt);
			}
			this.PreviousMusic = this.CurrentMusic;
		}

		public void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnAgentHit(affectedAgent, affectorAgent, damage, attackerWeapon);
		}

		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		}

		public void OnEntityRemoved(GameEntity entity)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnEntityRemoved(entity);
		}

		public void OnMusicVolumeChanged(float newVolume)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnMusicVolumeChanged(newVolume);
		}

		private List<MusicBaseComponent> _musicComponents;

		private static MusicComponentHolder instance = new MusicComponentHolder();
	}
}
