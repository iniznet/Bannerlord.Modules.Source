using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000068 RID: 104
	public abstract class GameManagerComponent : IEntityComponent
	{
		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060006EB RID: 1771 RVA: 0x00018356 File Offset: 0x00016556
		// (set) Token: 0x060006EC RID: 1772 RVA: 0x0001835E File Offset: 0x0001655E
		public GameManagerBase GameManager { get; internal set; }

		// Token: 0x060006ED RID: 1773 RVA: 0x00018367 File Offset: 0x00016567
		void IEntityComponent.OnInitialize()
		{
			this.OnInitialize();
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0001836F File Offset: 0x0001656F
		protected virtual void OnInitialize()
		{
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x00018371 File Offset: 0x00016571
		void IEntityComponent.OnFinalize()
		{
			this.OnFinalize();
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x00018379 File Offset: 0x00016579
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0001837B File Offset: 0x0001657B
		protected internal virtual void OnTick()
		{
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0001837D File Offset: 0x0001657D
		protected internal virtual void OnPlayerDisconnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0001837F File Offset: 0x0001657F
		protected internal virtual void OnEarlyPlayerConnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00018381 File Offset: 0x00016581
		protected internal virtual void OnPlayerConnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00018383 File Offset: 0x00016583
		protected internal virtual void OnGameNetworkBegin()
		{
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00018385 File Offset: 0x00016585
		protected internal virtual void OnGameNetworkEnd()
		{
		}
	}
}
