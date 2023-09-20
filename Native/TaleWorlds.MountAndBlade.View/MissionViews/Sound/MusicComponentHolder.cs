using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	// Token: 0x02000056 RID: 86
	public class MusicComponentHolder
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x0001F930 File Offset: 0x0001DB30
		public static MusicComponentHolder Instance
		{
			get
			{
				return MusicComponentHolder.instance;
			}
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0001F937 File Offset: 0x0001DB37
		private MusicComponentHolder()
		{
			this._musicComponents = new List<MusicBaseComponent>();
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060003BB RID: 955 RVA: 0x0001F94A File Offset: 0x0001DB4A
		// (set) Token: 0x060003BC RID: 956 RVA: 0x0001F952 File Offset: 0x0001DB52
		private protected MusicBaseComponent PreviousMusic { protected get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060003BD RID: 957 RVA: 0x0001F95B File Offset: 0x0001DB5B
		// (set) Token: 0x060003BE RID: 958 RVA: 0x0001F963 File Offset: 0x0001DB63
		private protected MusicBaseComponent CurrentMusic { protected get; private set; }

		// Token: 0x060003BF RID: 959 RVA: 0x0001F96C File Offset: 0x0001DB6C
		public void AddComponents(List<MusicBaseComponent> musicComponents)
		{
			foreach (MusicBaseComponent musicBaseComponent in musicComponents)
			{
				this._musicComponents.Add(musicBaseComponent);
				musicBaseComponent.PreInitialize();
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001F9C8 File Offset: 0x0001DBC8
		public void RemoveComponents(List<MusicBaseComponent> musicComponents)
		{
			this._musicComponents.RemoveAll((MusicBaseComponent x) => musicComponents.Contains(x));
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001F9FC File Offset: 0x0001DBFC
		public void Initialize()
		{
			foreach (MusicBaseComponent musicBaseComponent in this._musicComponents)
			{
				musicBaseComponent.Initialize();
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0001FA4C File Offset: 0x0001DC4C
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

		// Token: 0x060003C3 RID: 963 RVA: 0x0001FB0C File Offset: 0x0001DD0C
		public void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnAgentHit(affectedAgent, affectorAgent, damage, attackerWeapon);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001FB23 File Offset: 0x0001DD23
		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0001FB3A File Offset: 0x0001DD3A
		public void OnEntityRemoved(GameEntity entity)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnEntityRemoved(entity);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001FB4D File Offset: 0x0001DD4D
		public void OnMusicVolumeChanged(float newVolume)
		{
			MusicBaseComponent currentMusic = this.CurrentMusic;
			if (currentMusic == null)
			{
				return;
			}
			currentMusic.OnMusicVolumeChanged(newVolume);
		}

		// Token: 0x0400026B RID: 619
		private List<MusicBaseComponent> _musicComponents;

		// Token: 0x0400026C RID: 620
		private static MusicComponentHolder instance = new MusicComponentHolder();
	}
}
