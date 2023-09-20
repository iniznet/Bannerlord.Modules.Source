using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	// Token: 0x02000058 RID: 88
	public class MusicMissionView : MissionView
	{
		// Token: 0x060003D4 RID: 980 RVA: 0x0002046C File Offset: 0x0001E66C
		public MusicMissionView(params MusicBaseComponent[] musicComponents)
		{
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0002047F File Offset: 0x0001E67F
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MusicComponentHolder.Instance.Initialize();
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionConfigurationChange));
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000204B1 File Offset: 0x0001E6B1
		public override void OnRemoveBehavior()
		{
			MusicComponentHolder.Instance.RemoveComponents(this._components);
			base.OnRemoveBehavior();
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000204C9 File Offset: 0x0001E6C9
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionConfigurationChange));
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x000204F1 File Offset: 0x0001E6F1
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			MusicComponentHolder.Instance.Tick(dt);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00020505 File Offset: 0x0001E705
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			base.OnAgentHit(affectedAgent, affectorAgent, ref attackerWeapon, ref blow, ref attackCollisionData);
			MusicComponentHolder.Instance.OnAgentHit(affectedAgent, affectorAgent, blow.InflictedDamage, attackerWeapon);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00020528 File Offset: 0x0001E728
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			MusicComponentHolder.Instance.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00020544 File Offset: 0x0001E744
		public override void OnEntityRemoved(GameEntity entity)
		{
			base.OnEntityRemoved(entity);
			MusicComponentHolder.Instance.OnEntityRemoved(entity);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00020558 File Offset: 0x0001E758
		public void OnNativeOptionConfigurationChange(NativeOptions.NativeOptionsType optionType)
		{
			if (optionType == 2)
			{
				float config = NativeOptions.GetConfig(2);
				MusicComponentHolder.Instance.OnMusicVolumeChanged(config);
			}
		}

		// Token: 0x04000277 RID: 631
		private readonly List<MusicBaseComponent> _components = new List<MusicBaseComponent>();
	}
}
