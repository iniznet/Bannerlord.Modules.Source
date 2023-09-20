using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000371 RID: 881
	public abstract class UsableMissionObjectComponent
	{
		// Token: 0x06003033 RID: 12339 RVA: 0x000C668C File Offset: 0x000C488C
		protected internal virtual void OnAdded(Scene scene)
		{
		}

		// Token: 0x06003034 RID: 12340 RVA: 0x000C668E File Offset: 0x000C488E
		protected internal virtual void OnRemoved()
		{
		}

		// Token: 0x06003035 RID: 12341 RVA: 0x000C6690 File Offset: 0x000C4890
		protected internal virtual void OnFocusGain(Agent userAgent)
		{
		}

		// Token: 0x06003036 RID: 12342 RVA: 0x000C6692 File Offset: 0x000C4892
		protected internal virtual void OnFocusLose(Agent userAgent)
		{
		}

		// Token: 0x06003037 RID: 12343 RVA: 0x000C6694 File Offset: 0x000C4894
		public virtual bool IsOnTickRequired()
		{
			return false;
		}

		// Token: 0x06003038 RID: 12344 RVA: 0x000C6697 File Offset: 0x000C4897
		protected internal virtual void OnTick(float dt)
		{
		}

		// Token: 0x06003039 RID: 12345 RVA: 0x000C6699 File Offset: 0x000C4899
		protected internal virtual void OnEditorTick(float dt)
		{
		}

		// Token: 0x0600303A RID: 12346 RVA: 0x000C669B File Offset: 0x000C489B
		protected internal virtual void OnEditorValidate()
		{
		}

		// Token: 0x0600303B RID: 12347 RVA: 0x000C669D File Offset: 0x000C489D
		protected internal virtual void OnUse(Agent userAgent)
		{
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x000C669F File Offset: 0x000C489F
		protected internal virtual void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
		}

		// Token: 0x0600303D RID: 12349 RVA: 0x000C66A1 File Offset: 0x000C48A1
		protected internal virtual void OnMissionReset()
		{
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x000C66A3 File Offset: 0x000C48A3
		protected internal virtual bool ReadFromNetwork()
		{
			return true;
		}

		// Token: 0x0600303F RID: 12351 RVA: 0x000C66A6 File Offset: 0x000C48A6
		protected internal virtual void WriteToNetwork()
		{
		}
	}
}
