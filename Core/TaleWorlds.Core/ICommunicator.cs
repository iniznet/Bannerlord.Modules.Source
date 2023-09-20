using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200007E RID: 126
	public interface ICommunicator
	{
		// Token: 0x1700028C RID: 652
		// (get) Token: 0x0600078E RID: 1934
		VirtualPlayer VirtualPlayer { get; }

		// Token: 0x0600078F RID: 1935
		void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component);

		// Token: 0x06000790 RID: 1936
		void OnAddComponent(PeerComponent component);

		// Token: 0x06000791 RID: 1937
		void OnRemoveComponent(PeerComponent component);

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000792 RID: 1938
		bool IsNetworkActive { get; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000793 RID: 1939
		bool IsConnectionActive { get; }

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000794 RID: 1940
		bool IsServerPeer { get; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000795 RID: 1941
		// (set) Token: 0x06000796 RID: 1942
		bool IsSynchronized { get; set; }
	}
}
