using System;
using TaleWorlds.ScreenSystem;

namespace SandBox.View
{
	// Token: 0x02000009 RID: 9
	public abstract class SandboxView
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00003A14 File Offset: 0x00001C14
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00003A1C File Offset: 0x00001C1C
		public bool IsFinalized { get; protected set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00003A25 File Offset: 0x00001C25
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00003A2D File Offset: 0x00001C2D
		public ScreenLayer Layer { get; protected set; }

		// Token: 0x0600002B RID: 43 RVA: 0x00003A36 File Offset: 0x00001C36
		protected internal virtual void OnActivate()
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003A38 File Offset: 0x00001C38
		protected internal virtual void OnDeactivate()
		{
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003A3A File Offset: 0x00001C3A
		protected internal virtual void OnInitialize()
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003A3C File Offset: 0x00001C3C
		protected internal virtual void OnFinalize()
		{
			this.IsFinalized = true;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003A45 File Offset: 0x00001C45
		protected internal virtual void OnFrameTick(float dt)
		{
		}
	}
}
