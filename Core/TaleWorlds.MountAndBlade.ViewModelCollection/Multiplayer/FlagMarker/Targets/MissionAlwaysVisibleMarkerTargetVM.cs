using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	// Token: 0x020000BD RID: 189
	public class MissionAlwaysVisibleMarkerTargetVM : MissionMarkerTargetVM
	{
		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x0003BF9A File Offset: 0x0003A19A
		// (set) Token: 0x06001230 RID: 4656 RVA: 0x0003BFA2 File Offset: 0x0003A1A2
		public MissionPeer TargetPeer { get; private set; }

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x0003BFAB File Offset: 0x0003A1AB
		public override Vec3 WorldPosition
		{
			get
			{
				return this._position;
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06001232 RID: 4658 RVA: 0x0003BFB3 File Offset: 0x0003A1B3
		protected override float HeightOffset
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x0003BFBA File Offset: 0x0003A1BA
		public MissionAlwaysVisibleMarkerTargetVM(MissionPeer peer, Vec3 position, Action<MissionAlwaysVisibleMarkerTargetVM> onRemove)
			: base(MissionMarkerType.Peer)
		{
			this.TargetPeer = peer;
			this._position = position;
			this._onRemove = onRemove;
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0003BFD8 File Offset: 0x0003A1D8
		public void ExecuteRemove()
		{
			Action<MissionAlwaysVisibleMarkerTargetVM> onRemove = this._onRemove;
			if (onRemove == null)
			{
				return;
			}
			onRemove(this);
		}

		// Token: 0x040008B0 RID: 2224
		private Vec3 _position;

		// Token: 0x040008B1 RID: 2225
		private Action<MissionAlwaysVisibleMarkerTargetVM> _onRemove;
	}
}
