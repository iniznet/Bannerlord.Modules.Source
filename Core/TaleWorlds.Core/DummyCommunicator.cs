using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	// Token: 0x02000053 RID: 83
	public class DummyCommunicator : ICommunicator
	{
		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x00016629 File Offset: 0x00014829
		public VirtualPlayer VirtualPlayer { get; }

		// Token: 0x06000618 RID: 1560 RVA: 0x00016631 File Offset: 0x00014831
		public void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component)
		{
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00016633 File Offset: 0x00014833
		public void OnAddComponent(PeerComponent component)
		{
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00016635 File Offset: 0x00014835
		public void OnRemoveComponent(PeerComponent component)
		{
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x00016637 File Offset: 0x00014837
		public bool IsNetworkActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x0001663A File Offset: 0x0001483A
		public bool IsConnectionActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x0001663D File Offset: 0x0001483D
		public bool IsServerPeer
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x00016640 File Offset: 0x00014840
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x00016643 File Offset: 0x00014843
		public bool IsSynchronized
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00016645 File Offset: 0x00014845
		private DummyCommunicator(int index, string name)
		{
			this.VirtualPlayer = new VirtualPlayer(index, name, PlayerId.Empty, this);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00016660 File Offset: 0x00014860
		public static DummyCommunicator CreateAsServer(int index, string name)
		{
			return new DummyCommunicator(index, name);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00016669 File Offset: 0x00014869
		public static DummyCommunicator CreateAsClient(string name, int index)
		{
			return new DummyCommunicator(index, name);
		}
	}
}
