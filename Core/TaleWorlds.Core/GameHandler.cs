using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000064 RID: 100
	public abstract class GameHandler : IEntityComponent
	{
		// Token: 0x060006B3 RID: 1715 RVA: 0x00017EA9 File Offset: 0x000160A9
		void IEntityComponent.OnInitialize()
		{
			this.OnInitialize();
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00017EB1 File Offset: 0x000160B1
		void IEntityComponent.OnFinalize()
		{
			this.OnFinalize();
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00017EB9 File Offset: 0x000160B9
		protected virtual void OnInitialize()
		{
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00017EBB File Offset: 0x000160BB
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00017EBD File Offset: 0x000160BD
		protected internal virtual void OnTick(float dt)
		{
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00017EBF File Offset: 0x000160BF
		protected internal virtual void OnGameStart()
		{
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00017EC1 File Offset: 0x000160C1
		protected internal virtual void OnGameEnd()
		{
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x00017EC3 File Offset: 0x000160C3
		protected internal virtual void OnGameNetworkBegin()
		{
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00017EC5 File Offset: 0x000160C5
		protected internal virtual void OnGameNetworkEnd()
		{
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00017EC7 File Offset: 0x000160C7
		protected internal virtual void OnEarlyPlayerConnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00017EC9 File Offset: 0x000160C9
		protected internal virtual void OnPlayerConnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00017ECB File Offset: 0x000160CB
		protected internal virtual void OnPlayerDisconnect(VirtualPlayer peer)
		{
		}

		// Token: 0x060006BF RID: 1727
		public abstract void OnBeforeSave();

		// Token: 0x060006C0 RID: 1728
		public abstract void OnAfterSave();
	}
}
