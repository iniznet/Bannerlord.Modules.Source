using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000B7 RID: 183
	public abstract class PeerComponent : IEntityComponent
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x0001EDAC File Offset: 0x0001CFAC
		// (set) Token: 0x06000942 RID: 2370 RVA: 0x0001EDB4 File Offset: 0x0001CFB4
		public VirtualPlayer Peer
		{
			get
			{
				return this._peer;
			}
			set
			{
				this._peer = value;
			}
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0001EDBD File Offset: 0x0001CFBD
		public virtual void Initialize()
		{
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0001EDBF File Offset: 0x0001CFBF
		public string Name
		{
			get
			{
				return this.Peer.UserName;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x0001EDCC File Offset: 0x0001CFCC
		public bool IsMine
		{
			get
			{
				return this.Peer.IsMine;
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0001EDD9 File Offset: 0x0001CFD9
		public T GetComponent<T>() where T : PeerComponent
		{
			return this.Peer.GetComponent<T>();
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0001EDE6 File Offset: 0x0001CFE6
		public virtual void OnInitialize()
		{
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0001EDE8 File Offset: 0x0001CFE8
		public virtual void OnFinalize()
		{
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x0001EDEA File Offset: 0x0001CFEA
		// (set) Token: 0x0600094A RID: 2378 RVA: 0x0001EDF2 File Offset: 0x0001CFF2
		public uint TypeId { get; set; }

		// Token: 0x0400055D RID: 1373
		private VirtualPlayer _peer;
	}
}
