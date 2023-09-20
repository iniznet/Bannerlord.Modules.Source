using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000030 RID: 48
	public class CampaignEntityComponent : IEntityComponent
	{
		// Token: 0x06000349 RID: 841 RVA: 0x000193AC File Offset: 0x000175AC
		void IEntityComponent.OnInitialize()
		{
			this.OnInitialize();
		}

		// Token: 0x0600034A RID: 842 RVA: 0x000193B4 File Offset: 0x000175B4
		void IEntityComponent.OnFinalize()
		{
			this.OnFinalize();
		}

		// Token: 0x0600034B RID: 843 RVA: 0x000193BC File Offset: 0x000175BC
		protected virtual void OnInitialize()
		{
		}

		// Token: 0x0600034C RID: 844 RVA: 0x000193BE File Offset: 0x000175BE
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x0600034D RID: 845 RVA: 0x000193C0 File Offset: 0x000175C0
		public virtual void OnTick(float realDt, float dt)
		{
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000193C2 File Offset: 0x000175C2
		public virtual void OnLoadSavedGame()
		{
		}
	}
}
