using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components
{
	// Token: 0x0200005B RID: 91
	public abstract class MusicBaseComponent
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x000205B1 File Offset: 0x0001E7B1
		protected float CurrentVolume
		{
			get
			{
				return this._volume;
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x000205B9 File Offset: 0x0001E7B9
		public virtual void PreInitialize()
		{
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x000205BB File Offset: 0x0001E7BB
		public virtual void Initialize()
		{
			this._volumeInOptions = NativeOptions.GetConfig(2);
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x000205C9 File Offset: 0x0001E7C9
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x000205CB File Offset: 0x0001E7CB
		public virtual void OnActived()
		{
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x000205CD File Offset: 0x0001E7CD
		public virtual void OnDeactivated()
		{
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000205CF File Offset: 0x0001E7CF
		public virtual void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon attackerWeapon)
		{
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000205D1 File Offset: 0x0001E7D1
		public virtual void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000205D3 File Offset: 0x0001E7D3
		public virtual void OnEntityRemoved(GameEntity entity)
		{
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x000205D8 File Offset: 0x0001E7D8
		public virtual void OnMusicVolumeChanged(float newVolume)
		{
			float num = this._volumeInOptions - newVolume;
			this._volume += num;
			this._volume = MBMath.ClampFloat(this._volume, 0f, 1f);
			this._volumeInOptions = newVolume;
		}

		// Token: 0x060003EC RID: 1004
		public abstract bool IsActive();

		// Token: 0x060003ED RID: 1005
		public abstract MusicPriority GetPriority();

		// Token: 0x0400027C RID: 636
		protected MBMusicManagerOld.MusicMood CurrentMood = -1;

		// Token: 0x0400027D RID: 637
		private float _volumeInOptions;

		// Token: 0x0400027E RID: 638
		private float _volume;
	}
}
