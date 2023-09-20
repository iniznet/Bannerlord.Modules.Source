using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000B8 RID: 184
	public abstract class PlayerGameState : GameState
	{
		// Token: 0x1700031C RID: 796
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x0001EE03 File Offset: 0x0001D003
		// (set) Token: 0x0600094D RID: 2381 RVA: 0x0001EE0B File Offset: 0x0001D00B
		public VirtualPlayer Peer
		{
			get
			{
				return this._peer;
			}
			private set
			{
				this._peer = value;
			}
		}

		// Token: 0x0400055F RID: 1375
		private VirtualPlayer _peer;
	}
}
