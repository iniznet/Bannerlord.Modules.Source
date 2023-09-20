using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000EC RID: 236
	public abstract class AgentComponent
	{
		// Token: 0x06000B1F RID: 2847 RVA: 0x000159AE File Offset: 0x00013BAE
		protected AgentComponent(Agent agent)
		{
			this.Agent = agent;
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x000159BD File Offset: 0x00013BBD
		public virtual void Initialize()
		{
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x000159BF File Offset: 0x00013BBF
		public virtual void OnTickAsAI(float dt)
		{
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x000159C1 File Offset: 0x00013BC1
		public virtual float GetMoraleAddition()
		{
			return 0f;
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x000159C8 File Offset: 0x00013BC8
		public virtual float GetMoraleDecreaseConstant()
		{
			return 1f;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x000159CF File Offset: 0x00013BCF
		public virtual void OnItemPickup(SpawnedItemEntity item)
		{
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x000159D1 File Offset: 0x00013BD1
		public virtual void OnWeaponDrop(MissionWeapon droppedWeapon)
		{
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x000159D3 File Offset: 0x00013BD3
		public virtual void OnStopUsingGameObject()
		{
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x000159D5 File Offset: 0x00013BD5
		public virtual void OnWeaponHPChanged(ItemObject item, int hitPoints)
		{
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x000159D7 File Offset: 0x00013BD7
		public virtual void OnRetreating()
		{
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x000159D9 File Offset: 0x00013BD9
		public virtual void OnMount(Agent mount)
		{
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x000159DB File Offset: 0x00013BDB
		public virtual void OnDismount(Agent mount)
		{
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x000159DD File Offset: 0x00013BDD
		public virtual void OnHit(Agent affectorAgent, int damage, in MissionWeapon affectorWeapon)
		{
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x000159DF File Offset: 0x00013BDF
		public virtual void OnDisciplineChanged()
		{
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x000159E1 File Offset: 0x00013BE1
		public virtual void OnAgentRemoved()
		{
		}

		// Token: 0x04000289 RID: 649
		protected readonly Agent Agent;
	}
}
